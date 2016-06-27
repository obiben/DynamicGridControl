// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DynamicGridControl.cs" company="APE-Engineering GmbH">
//  This source code is provided under the CodeProject OpenLicence V1.0 
// </copyright>
// <summary>
//   Base class for a virtualizing two dimensional Excel-Like grid control.
//   The input for this control (DataSource) is a one-dimensional array.
//   See the Sample-Folder for usage information.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace APE.WPF.Controls.DynamicGrid
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Threading;

    /// <summary>
    /// Base class for a virtualizing two dimensional Excel-Like grid control.
    /// The input for this control (DataSource) is a one-dimensional array.
    /// See the Sample-Folder for usage information.
    /// </summary>
    public abstract class DynamicGridControl<TDataSource, TRow, TCol> : ScrollableControl
	{
		#region DependencyProperty definitions
		
		public static readonly DependencyProperty WaitLayerTemplateProperty;

		public static readonly DependencyProperty DataItemTemplateProperty;

		public static readonly DependencyProperty DataItemTemplateSelectorProperty;

		public static readonly DependencyProperty HeaderTemplateProperty;

		public static readonly DependencyProperty HeaderTemplateSelectorProperty;

		public static readonly DependencyProperty DataSourceProperty;

		public static readonly DependencyProperty RowCountProperty;

		public static readonly DependencyProperty ColumnCountProperty;

		public static readonly DependencyProperty ItemWidthProperty;

		public static readonly DependencyProperty ItemHeightProperty;

		public static readonly DependencyProperty DataItemsProperty;

		public static readonly DependencyProperty GridRowsProperty;

		public static readonly DependencyProperty GridColumnsProperty;

		public static readonly DependencyProperty GridHeadersProperty;

		public static readonly DependencyProperty IsBusyProperty;

		private static readonly DependencyPropertyKey RowCountKey;

		private static readonly DependencyPropertyKey ColumnCountKey;

		private static readonly DependencyPropertyKey DataItemsKey;

		private static readonly DependencyPropertyKey GridRowsKey;

		private static readonly DependencyPropertyKey GridColumnsKey;

		private static readonly DependencyPropertyKey GridHeadersKey;

		private static readonly DependencyPropertyKey IsBusyKey;

		#endregion

		#region Private readonly fields

		/// <summary>
		/// Backing field for the function that selects the row part of the input array.
		/// </summary>
		private readonly Func<TDataSource, TRow> rowSelector;

		/// <summary>
		/// Backing field for the function that selects the column part of the input array.
		/// </summary>
		private readonly Func<TDataSource, TCol> columnSelector;

		/// <summary>
		/// Backing field for the function that aggregates multiple values.
		/// </summary>
		private readonly Func<IEnumerable<TDataSource>, object> aggregator;

		/// <summary>
		/// Limiter for the scroll event.
		/// </summary>
		private readonly EventLimiter<object> scrollEventLimiter;

		#endregion

		#region Private fields

		/// <summary>
		/// Cache for column headers.
		/// </summary>
		private List<TCol> colHeaderCache;

		/// <summary>
		/// Cache for row headers.
		/// </summary>
		private List<TRow> rowHeaderCache;

		/// <summary>
		/// Cache for all data items.
		/// </summary>
		private object[,] itemSourceCache;

		/// <summary>
		/// Number of currently displayed rows.
		/// </summary>
		private int displayedRows;

		/// <summary>
		/// Number of currently displayed columns.
		/// </summary>
		private int displayedCols;

		/// <summary>
		/// Current vertical scroll offset.
		/// </summary>
		private int displayedRowScrollOffset = -1;

		/// <summary>
		/// Current horizontal scroll offset.
		/// </summary>
		private int displayedColScrollOffset = -1;

		/// <summary>
		/// Value whether the DataSource must be updated again.
		/// </summary>
		private bool updateDataSourceAgain;

		#endregion

		#region Construction / Destruction

		/// <summary>
		/// Initializes all static members.
		/// </summary>
		static DynamicGridControl()
		{
			// Initialize read/write properties
			DataItemTemplateProperty = DependencyProperty.Register("DataItemTemplate", typeof(DataTemplate), typeof(DynamicGridControl<TDataSource, TRow, TCol>), new PropertyMetadata(null));
			DataItemTemplateSelectorProperty = DependencyProperty.Register("DataItemTemplateSelector", typeof(DataTemplateSelector), typeof(DynamicGridControl<TDataSource, TRow, TCol>), new PropertyMetadata(null));
			HeaderTemplateProperty = DependencyProperty.Register("HeaderTemplate", typeof(DataTemplate), typeof(DynamicGridControl<TDataSource, TRow, TCol>), new PropertyMetadata(null));
			HeaderTemplateSelectorProperty = DependencyProperty.Register("HeaderTemplateSelector", typeof(DataTemplateSelector), typeof(DynamicGridControl<TDataSource, TRow, TCol>), new PropertyMetadata(null));
			WaitLayerTemplateProperty = DependencyProperty.Register("WaitLayerTemplate", typeof(ControlTemplate), typeof(DynamicGridControl<TDataSource, TRow, TCol>), new PropertyMetadata(null));
			DataSourceProperty = DependencyProperty.Register("DataSource", typeof(IEnumerable<TDataSource>), typeof(DynamicGridControl<TDataSource, TRow, TCol>), new PropertyMetadata(null, DynamicGridControl<TDataSource, TRow, TCol>.DataSourceChanged));
			ItemWidthProperty = DependencyProperty.Register("ItemWidth", typeof(double), typeof(DynamicGridControl<TDataSource, TRow, TCol>), new PropertyMetadata(130.0, ItemSizeChangedHandler));
			ItemHeightProperty = DependencyProperty.Register("ItemHeight", typeof(double), typeof(DynamicGridControl<TDataSource, TRow, TCol>), new PropertyMetadata(35.0, ItemSizeChangedHandler));

			// Initialize readonly property keys
			ColumnCountKey = DependencyProperty.RegisterReadOnly("ColumnCount", typeof(int), typeof(DynamicGridControl<TDataSource, TRow, TCol>), new PropertyMetadata(0));
			RowCountKey = DependencyProperty.RegisterReadOnly("RowCount", typeof(int), typeof(DynamicGridControl<TDataSource, TRow, TCol>), new PropertyMetadata(0));
			DataItemsKey = DependencyProperty.RegisterReadOnly("DataItems", typeof(IEnumerable<DynamicGridDataItem>), typeof(DynamicGridControl<TDataSource, TRow, TCol>), new PropertyMetadata(null));
			GridRowsKey = DependencyProperty.RegisterReadOnly("GridRows", typeof(IEnumerable<DynamicGridRow>), typeof(DynamicGridControl<TDataSource, TRow, TCol>), new PropertyMetadata(null));
			GridColumnsKey = DependencyProperty.RegisterReadOnly("GridColumns", typeof(IEnumerable<DynamicGridColumn>), typeof(DynamicGridControl<TDataSource, TRow, TCol>), new PropertyMetadata(null));
			GridHeadersKey = DependencyProperty.RegisterReadOnly("GridHeaders", typeof(IEnumerable<DynamicGridHeader>), typeof(DynamicGridControl<TDataSource, TRow, TCol>), new PropertyMetadata(null));
			IsBusyKey = DependencyProperty.RegisterReadOnly("IsBusy", typeof(bool), typeof(DynamicGridControl<TDataSource, TRow, TCol>), new PropertyMetadata(false));

			// Initialize readonly properties
			ColumnCountProperty = ColumnCountKey.DependencyProperty;
			RowCountProperty = RowCountKey.DependencyProperty;
			DataItemsProperty = DataItemsKey.DependencyProperty;
			GridRowsProperty = GridRowsKey.DependencyProperty;
			GridColumnsProperty = GridColumnsKey.DependencyProperty;
			GridHeadersProperty = GridHeadersKey.DependencyProperty;
			IsBusyProperty = IsBusyKey.DependencyProperty;
		}

		/// <summary>
		/// Initializes this instance.
		/// </summary>
		/// <param name="rowSelector">Function that selects the row part of a data item</param>
		/// <param name="columnSelector">Function that selects the column part of a data item</param>
		/// <param name="aggregator">Function that aggregates multiple data items</param>
		protected DynamicGridControl(
									 Func<TDataSource, TRow> rowSelector, 
									 Func<TDataSource, TCol> columnSelector, 
									 Func<IEnumerable<TDataSource>, object> aggregator)
		{
			if (aggregator == null) throw new ArgumentNullException("aggregator");
			if (rowSelector == null) throw new ArgumentNullException("rowSelector");
			if (columnSelector == null) throw new ArgumentNullException("columnSelector");

			this.aggregator = aggregator;
			this.rowSelector = rowSelector;
			this.columnSelector = columnSelector;

			// Scroll events are only processed every 150ms
			this.scrollEventLimiter = new EventLimiter<object>(this.Dispatcher, DispatcherPriority.Background, 150, arg => this.UpdateDisplayData());
		}

		#endregion

		#region Public fields

		/// <summary>
		/// Gets or sets a function that compares two row-headers. Used to sort the rows.
		/// </summary>
		public Func<TRow, TRow, int> RowComparer { get; set; } 
		
		/// <summary>
		/// Gets or sets a function that compares two column-headers. Used to sort the columns.
		/// </summary>
		public Func<TCol, TCol, int> ColumnComparer { get; set; }

		#endregion

		#region DependencyProperty accessors

		/// <summary>
		/// Gets or set the WaitLayerTemplate
		/// </summary>
		public ControlTemplate WaitLayerTemplate
		{
			get
			{
				return (ControlTemplate)this.GetValue(WaitLayerTemplateProperty);
			}

			set
			{
				this.SetValue(WaitLayerTemplateProperty, value);
			}
		}

		/// <summary>
		/// Gets or set the DataItemTemplate
		/// </summary>
		public DataTemplate DataItemTemplate
		{
			get
			{
				return (DataTemplate)this.GetValue(DataItemTemplateProperty);
			}

			set
			{
				this.SetValue(DataItemTemplateProperty, value);
			}
		}

		/// <summary>
		/// Gets or sets the DataItemSelector
		/// </summary>
		public DataTemplateSelector DataItemTemplateSelector
		{
			get
			{
				return (DataTemplateSelector)this.GetValue(DataItemTemplateSelectorProperty);
			}

			set
			{
				this.SetValue(DataItemTemplateSelectorProperty, value);
			}
		}

		/// <summary>
		/// Gets or set the HeaderTemplate
		/// </summary>
		public DataTemplate HeaderTemplate
		{
			get
			{
				return (DataTemplate)this.GetValue(HeaderTemplateProperty);
			}

			set
			{
				this.SetValue(HeaderTemplateProperty, value);
			}
		}

		/// <summary>
		/// Gets or sets the HeaderSelector
		/// </summary>
		public DataTemplateSelector HeaderTemplateSelector
		{
			get
			{
				return (DataTemplateSelector)this.GetValue(HeaderTemplateSelectorProperty);
			}

			set
			{
				this.SetValue(HeaderTemplateSelectorProperty, value);
			}
		}

		/// <summary>
		/// Gets the data items.
		/// </summary>
		public IEnumerable<DynamicGridDataItem> DataItems
		{
			get
			{
				return (IEnumerable<DynamicGridDataItem>)this.GetValue(DataItemsProperty);
			}

			private set
			{
				this.SetValue(DataItemsKey, value);
			}
		}

		/// <summary>
		/// Gets the row definitions.
		/// </summary>
		public IEnumerable<DynamicGridRow> GridRows
		{
			get
			{
				return (IEnumerable<DynamicGridRow>)this.GetValue(GridRowsProperty);
			}

			private set
			{
				this.SetValue(GridRowsKey, value);
			}
		}

		/// <summary>
		/// Gets the column definitions.
		/// </summary>
		public IEnumerable<DynamicGridColumn> GridColumns
		{
			get
			{
				return (IEnumerable<DynamicGridColumn>)this.GetValue(GridColumnsProperty);
			}

			private set
			{
				this.SetValue(GridColumnsKey, value);
			}
		}

		/// <summary>
		/// Gets the grid headers.
		/// </summary>
		public IEnumerable<DynamicGridHeader> GridHeaders
		{
			get
			{
				return (IEnumerable<DynamicGridHeader>)this.GetValue(GridHeadersProperty);
			}

			private set
			{
				this.SetValue(GridHeadersKey, value);
			}
		}

		/// <summary>
		/// Gets or sets the data source. If the source is an ObservableCollection, add a handler to reload the cache on changes.
		/// </summary>
		public IEnumerable<TDataSource> DataSource
		{
			get { return (IEnumerable<TDataSource>)this.GetValue(DataSourceProperty); }
			set {
                this.SetValue(DataSourceProperty, value);
                if (value is ObservableCollection<TDataSource>)
                {
                    (value as ObservableCollection<TDataSource>).CollectionChanged += DataSourceChanged;
                }
            }
		}

		/// <summary>
		/// Gets the number of visible rows.
		/// </summary>
		public int RowCount
		{
			get
			{
				return (int)this.GetValue(RowCountProperty);
			}

			private set
			{
				this.SetValue(RowCountKey, value);
			}
		}

		/// <summary>
		/// Gets the number of visible columns.
		/// </summary>
		public int ColumnCount
		{
			get
			{
				return (int)this.GetValue(ColumnCountProperty);
			}

			private set
			{
				this.SetValue(ColumnCountKey, value);
			}
		}

		/// <summary>
		/// Gets or sets the width of a column.
		/// </summary>
		public double ItemWidth
		{
			get
			{
				return (double)this.GetValue(ItemWidthProperty);
			}

			set
			{
				this.SetValue(ItemWidthProperty, value);
			}
		}

		/// <summary>
		/// Gets or sets the height of a row.
		/// </summary>
		public double ItemHeight
		{
			get
			{
				return (double)this.GetValue(ItemHeightProperty);
			}

			set
			{
				this.SetValue(ItemHeightProperty, value);
			}
		}

		/// <summary>
		/// Gets a value whether the control is busy (performing a long running operation).
		/// </summary>
		public bool IsBusy
		{
			get
			{
				return (bool)this.GetValue(IsBusyProperty);
			}

			set
			{
				this.SetValue(IsBusyKey, value);
			}
		}

		#endregion

		#region Overridden protected methods

		/// <summary>
		/// This override is invoked when a scroll event occurs.
		/// </summary>
		protected override void OnScroll()
		{
			this.scrollEventLimiter.Trigger(null);
		}

		/// <summary>
		/// This override is invoked when the control's size changed.
		/// </summary>
		protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
		{
			base.OnRenderSizeChanged(sizeInfo);
			this.UpdateDisplayData();
		}

		#endregion

		#region Static callbacks

		/// <summary>
		/// This method is invoked when the data source changed.
		/// </summary>
		private static void DataSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var grid = (DynamicGridControl<TDataSource, TRow, TCol>)d;
			grid.DataSourceChanged();
		}

		/// <summary>
		/// This method is invoked when the ItemWidth / ItemHeight changed.
		/// </summary>
		private static void ItemSizeChangedHandler(DependencyObject d, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
		{
			var grid = (DynamicGridControl<TDataSource, TRow, TCol>)d;

			var rows = grid.rowHeaderCache == null ? 0 : grid.rowHeaderCache.Count;
			var cols = grid.colHeaderCache == null ? 0 : grid.colHeaderCache.Count;

			grid.SetScrollExtent(grid.ItemWidth, grid.ItemHeight, grid.ItemWidth * (cols + 1), grid.ItemHeight * (rows + 1)); /* +1 because of the headers */
			grid.UpdateDisplayData();
		}

        #endregion

        #region Private methods

        /// <summary>
        /// This method updates the caches when the DataSource changed.
        /// </summary>
        private void DataSourceChanged(object sender, NotifyCollectionChangedEventArgs args)
        {
            DataSourceChanged();
        }

        /// <summary>
        /// This method updates the caches when the DataSource changed.
        /// </summary>
        private async void DataSourceChanged()
		{

			/* Attention. Since this method is async and may be invoked multiple times while performing asynchronous operations,
			 * we need to make sure that our method body is not run multiple times. Btw: Locking is not needed, because since this method
			 * is triggered by an DependencyProperty, we are always invoked on UI thread. */
			if (this.IsBusy)
			{
				this.updateDataSourceAgain = true;
				return;
			}

			this.IsBusy = true;

			bool updateAgain;

			do
			{
				// Calculate distinct row/column headers (very CPU intensive!)
				var dataSource = this.DataSource;

				if (dataSource == null || !dataSource.Any())
				{
					this.GridColumns = null;
					this.GridHeaders = null;
					this.DataItems = null;
					this.GridRows = null;
					this.itemSourceCache = null;
					this.colHeaderCache = null;
					this.rowHeaderCache = null;
					this.SetScrollExtent(this.ItemWidth, this.ItemHeight, 0, 0);
					
					/* it's ok to break here without checking the updateAgain field, because no
					 * asynchronous action has been invoked until now (in this method). If the data source changed,
					 * the request is still in this dispatcher's message queue. */
					break;
				}

				var items = dataSource.ToList();
				await Task.Run(() => this.UpdateHeaderCache(items)).ConfigureAwait(true);
				
				// If the data source changed while we updated the header cache, stop here and run again (see Attention comment above)
				if (this.updateDataSourceAgain)
				{
					updateAgain = true;
					this.updateDataSourceAgain = false;
					continue;
				}

				// Generate row / column header dictionaries for faster access
				int counter = 0;
				var rowHeaderDict = this.rowHeaderCache.ToDictionary(key => key, value => counter++);
				counter = 0;
				var columnHeaderDict = this.colHeaderCache.ToDictionary(key => key, value => counter++);

				// Build the the cache (asynchronously to keep the UI responsive)
				var itemCache =
					await Task.Run(() => this.BuildItemSourceCache(items, rowHeaderDict, columnHeaderDict)).ConfigureAwait(true);
				this.itemSourceCache = itemCache;

				// Set scroll info
				this.SetScrollExtent(
					this.ItemWidth,
					this.ItemHeight,
					this.ItemWidth * (this.colHeaderCache.Count + 1),
					this.ItemHeight * (this.rowHeaderCache.Count + 1)); /* +1 because of the headers */

				// Should we run again? See the "Attention" comment.
				updateAgain = this.updateDataSourceAgain;
				this.updateDataSourceAgain = false;
			}
			while (updateAgain);

			this.IsBusy = false;

			// Finally, updates
			this.UpdateDisplayData(true);
		}

		/// <summary>
		/// This method updates the display data (scrolling / initialization).
		/// </summary>
		/// <param name="forceUpdate">force the update, even if the algorithm thinks it is not necessary</param>
		private void UpdateDisplayData(bool forceUpdate = false)
		{
			// If the data source is empty, reset the display
			if (this.IsBusy || this.DataSource == null || this.itemSourceCache == null || !this.DataSource.Any())
			{
				this.GridColumns = null;
				this.GridHeaders = null;
				this.DataItems = null;
				this.GridRows = null;
				return;
			}

			// Calculate visible columns / rows
			var viewPortWidth = this.ActualWidth;
			var viewPortHeight = this.ActualHeight;
			var visibleRowCount = (int)(viewPortHeight / this.ItemHeight);
			var visibleColumnCount = (int)(viewPortWidth / this.ItemWidth);
			this.ColumnCount = visibleColumnCount + 1;
			this.RowCount = visibleRowCount + 1;

			// Calculate current scroll offsets
			var rowScrollOffset = (int)(this.VerticalOffset / this.ItemHeight);
			var colScrollOffset = (int)(this.HorizontalOffset / this.ItemWidth);
			if (rowScrollOffset > this.rowHeaderCache.Count - visibleRowCount) rowScrollOffset = Math.Max(this.rowHeaderCache.Count - visibleRowCount, 0);
			if (colScrollOffset > this.colHeaderCache.Count - visibleColumnCount) colScrollOffset = Math.Max(this.colHeaderCache.Count - visibleColumnCount, 0);

			// Get visible row / column headers
			var visibleRows = this.rowHeaderCache.Skip(rowScrollOffset).Take(visibleRowCount).ToList();
			var visibleColumns = this.colHeaderCache.Skip(colScrollOffset).Take(visibleColumnCount).ToList();
			visibleRowCount = visibleRows.Count();
			visibleColumnCount = visibleColumns.Count();

			// Local variables used for display data generation
			List<DynamicGridDataItem> dataItems;
			List<DynamicGridRow> gridRows;
			List<DynamicGridColumn> gridColumns;
			List<DynamicGridHeader> gridHeaders;
			Action<int, object> setRowHeader;
			Action<int, object> setColHeader;

			// Decide if display data needs to be regenerated, can be recycled, or can be omitted at all (massive performance improvement)
			var regenerateDataItems = this.DataItems == null || this.displayedRows != visibleRowCount || this.displayedCols != visibleColumnCount;
			
			// If data items are arleady present and the scroll offsets didn't change, the data is already up to date
			if (!regenerateDataItems && rowScrollOffset == this.displayedRowScrollOffset
			    && colScrollOffset == this.displayedColScrollOffset && !forceUpdate) return;

			if (regenerateDataItems)
			{
				// Display data is created, headers must be added
				dataItems = new List<DynamicGridDataItem>();
				gridRows = new List<DynamicGridRow>();
				gridColumns = new List<DynamicGridColumn>();
				gridHeaders = new List<DynamicGridHeader>();
				setRowHeader = (row, content) => gridHeaders.Add(new DynamicGridHeader(row, 0, content));
				setColHeader = (col, content) => gridHeaders.Add(new DynamicGridHeader(0, col, content));
				gridHeaders.Add(new DynamicGridHeader(0, 0, null));
			}
			else
			{
				// Display data already exists, headers are already present
				gridRows = (List<DynamicGridRow>)this.GridRows;
				dataItems = (List<DynamicGridDataItem>)this.DataItems;
				gridColumns = (List<DynamicGridColumn>)this.GridColumns;
				gridHeaders = (List<DynamicGridHeader>)this.GridHeaders;
				setRowHeader = (row, content) => gridHeaders.First(item => item.Row == row && item.Column == 0).Content = content;
				setColHeader = (col, content) => gridHeaders.First(item => item.Row == 0 && item.Column == col).Content = content;
			}

			if (regenerateDataItems)
			{
				// Regenerate the display data items
				for (int y = rowScrollOffset; y < rowScrollOffset + visibleRowCount; y++)
					for (int x = colScrollOffset; x < colScrollOffset + visibleColumnCount; x++)
						dataItems.Add(new DynamicGridDataItem(y - rowScrollOffset + 1, x - colScrollOffset + 1, this.itemSourceCache[x, y]));

				// Generate row separators
				for (int y = 1; y < visibleRowCount + 1; y++) 
					gridRows.Add(new DynamicGridRow(y));

				// Generate column separators
				for (int x = 1; x < visibleColumnCount + 1; x++) 
					gridColumns.Add(new DynamicGridColumn(x));
			}
			else
			{
				// Recycle the display data
				foreach (var item in dataItems)
					item.Content = this.itemSourceCache[item.Column + colScrollOffset - 1, item.Row + rowScrollOffset - 1];
			}

			// Generate row headers
			var rowIndex = 0;
			foreach (var row in visibleRows)
				setRowHeader(++rowIndex, row);

			// Generate column headers
			var colIndex = 0;
			foreach (var col in visibleColumns)
				setColHeader(++colIndex, col);

			// If the display data is regenerated, make it accessible to the view
			if (regenerateDataItems)
			{
				this.DataItems = dataItems;
				this.GridRows = gridRows;
				this.GridColumns = gridColumns;
				this.GridHeaders = gridHeaders;
			}

			/* Save display configuration to be able to figure out if updating
			 * the display data (while resizing, etc) is necessary */
			this.displayedRows = visibleRowCount;
			this.displayedCols = visibleColumnCount;
			this.displayedRowScrollOffset = rowScrollOffset;
			this.displayedColScrollOffset = colScrollOffset;
		}

		/* Helper functions */

		/// <summary>
		/// Builds the item source cache.
		/// </summary>
		private object[,] BuildItemSourceCache(IEnumerable<TDataSource> items, Dictionary<TRow, int> rowHeaderDict, Dictionary<TCol, int> columnHeaderDict)
		{
			Debug.WriteLine("START");

			var itemSourceCache = new object[this.colHeaderCache.Count, this.rowHeaderCache.Count];

			// Store indices where aggregation needs to be executed
			var aggregationNeededIndices = new List<Tuple<int, int>>();

			// Store all items in the two-dimensional cache
			foreach (var item in items)
			{
				// Get the current item's row / column headers
				var rowHeader = rowHeaderDict[this.rowSelector(item)];
				var colHeader = columnHeaderDict[this.columnSelector(item)];

				/* fill the cache. if a position is already used, generate a list of items
				 * and execute the aggregate function within the next section */
				if (itemSourceCache[colHeader, rowHeader] == null)
				{
					// position is empty. easy.
					itemSourceCache[colHeader, rowHeader] = item;
				}
				else
				{
					// Have a look whats already in the cache (list or single item)
					var cachedObject = itemSourceCache[colHeader, rowHeader];
					var valueList = cachedObject as List<TDataSource>;

					if (valueList == null)
					{
						// Generate a new list of items
						valueList = new List<TDataSource> { (TDataSource)cachedObject };
						itemSourceCache[colHeader, rowHeader] = valueList;
						aggregationNeededIndices.Add(new Tuple<int, int>(colHeader, rowHeader));
					}

					valueList.Add(item);
				}
			}

			// Execute the aggregation function if needed
			foreach (var entry in aggregationNeededIndices)
			{
				itemSourceCache[entry.Item1, entry.Item2] =
					this.aggregator((IEnumerable<TDataSource>)itemSourceCache[entry.Item1, entry.Item2]);
			}

			Debug.WriteLine("STOP");

			return itemSourceCache;
		}

		/// <summary>
		/// Updates the header cache.
		/// </summary>
		private List<TDataSource> UpdateHeaderCache(List<TDataSource> items)
		{
			Action updateRowCache = () =>
				{
					var rows = items.Select(dataItem => this.rowSelector(dataItem)).Distinct();
					if (this.RowComparer != null) rows = rows.OrderBy(x => x, new DelegateComparer<TRow>(this.RowComparer));
					this.rowHeaderCache = rows.ToList();
				};

			Action updateColCache = () =>
				{
					var cols = items.Select(dataItem => this.columnSelector(dataItem)).Distinct();
					if (this.ColumnComparer != null) cols = cols.OrderBy(x => x, new DelegateComparer<TCol>(this.ColumnComparer));
					this.colHeaderCache = cols.ToList();
				};

			Parallel.Invoke(updateRowCache, updateColCache);
			return items;
		}

		#endregion
	}
}
