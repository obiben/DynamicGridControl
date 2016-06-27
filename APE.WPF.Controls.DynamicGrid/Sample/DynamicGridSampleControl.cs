// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DynamicGridSampleControl.cs" company="APE-Engineering GmbH">
//  This source code is provided under the CodeProject OpenLicence V1.0 
// </copyright>
// <summary>
//   Control to show the usage of the DynamicGridControl.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace APE.WPF.Controls.DynamicGrid
{
	using System;
	using System.Linq;
	using System.Windows;

	/// <summary>
	/// Control to show the usage of the DynamicGridControl.
	/// </summary>
	public class DynamicGridSampleControl : DynamicGridControl<SampleGridItem, string, DateTime>
	{
		/// <summary>
		/// Initializes static members of the <see cref="DynamicGridSampleControl"/> class.
		/// </summary>
		static DynamicGridSampleControl()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(DynamicGridSampleControl), new FrameworkPropertyMetadata(typeof(DynamicGridSampleControl)));
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="DynamicGridSampleControl"/> class.
		/// </summary>
		public DynamicGridSampleControl()
			: base(
					// Row selector: Lambda expression to select the row identifier of a data item
					item => item.ProductName,

					// Column selector: Lambda expression to select the column identifier of a data item
					item => item.ProductionDate,

					// Aggregator: Is invoked when a cell contains multiple data items and returns an aggregated item
					items => new SampleGridItem {
						ProductName = items.First().ProductName,
						ProductionDate = items.First().ProductionDate,
						ProductionCount = items.Sum(item => item.ProductionCount)
					})
		{
			// ColumnComparer: If this property is set, the columns are sorted (see also RowComparer)
			this.ColumnComparer = (a, b) => a.CompareTo(b);
		}
	}
}
