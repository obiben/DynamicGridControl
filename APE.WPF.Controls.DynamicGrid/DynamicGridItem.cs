// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DynamicGridItem.cs" company="APE-Engineering GmbH">
//  This source code is provided under the CodeProject OpenLicence V1.0 
// </copyright>
// <summary>
//   Abstract base class for all display cells within a <see cref="DynamicGridControl" />.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace APE.WPF.Controls.DynamicGrid
{
	using System.ComponentModel;

	/// <summary>
	/// Abstract base class for all display cells within a <see cref="DynamicGridControl"/>.
	/// </summary>
	public abstract class DynamicGridItem : INotifyPropertyChanged
	{
		/// <summary>
		/// Backing field for the cell's content.
		/// </summary>
		private object content;

		/// <summary>
		/// Initializes this item.
		/// </summary>
		protected DynamicGridItem(int row, int column, object content)
		{
			this.Row = row;
			this.Column = column;
			this.Content = content;
		}

		/// <summary>
		/// Is raised when a property changed.
		/// </summary>
		public event PropertyChangedEventHandler PropertyChanged;

		/// <summary>
		/// Gets or sets the cell's row.
		/// </summary>
		public int Row { get; set; }

		/// <summary>
		/// Gets or sets the cell's column.
		/// </summary>
		public int Column { get; set; }

		/// <summary>
		/// Gets or sets the cell's content.
		/// </summary>
		public object Content
		{
			get
			{
				return this.content;
			}

			set
			{
				this.content = value;
				this.OnPropertyChanged("Content");
			}
		}

		/// <summary>
		/// Raises a PropertyChangedEvent.
		/// </summary>
		protected virtual void OnPropertyChanged(string propertyName)
		{
			var handler = this.PropertyChanged;
			if (handler != null)
			{
				handler(this, new PropertyChangedEventArgs(propertyName));
			}
		}
	}
}
