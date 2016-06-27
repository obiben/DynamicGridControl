// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DynamicGridColumn.cs" company="APE-Engineering GmbH">
//  This source code is provided under the CodeProject OpenLicence V1.0 
// </copyright>
// <summary>
//   Column definition for a <see cref="DynamicGridControl" />.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace APE.WPF.Controls.DynamicGrid
{
	/// <summary>
	/// Column definition for a <see cref="DynamicGridControl"/>.
	/// </summary>
	public class DynamicGridColumn : DynamicGridItem
	{
		/// <summary>
		/// Initializes this instance.
		/// </summary>
		public DynamicGridColumn(int column)
			: base(0, column, null)
		{
			this.IsOdd = column % 2 == 1;
		}

		/// <summary>
		/// Gets a value whether this column's index is odd.
		/// </summary>
		public bool IsOdd { get; private set; }
	}
}
