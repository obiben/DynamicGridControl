// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DynamicGridRow.cs" company="APE-Engineering GmbH">
//  This source code is provided under the CodeProject OpenLicence V1.0 
// </copyright>
// <summary>
//   Row definition for a <see cref="DynamicGridControl" />
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace APE.WPF.Controls.DynamicGrid
{
	/// <summary>
	/// Row definition for a <see cref="DynamicGridControl"/>
	/// </summary>
	public class DynamicGridRow : DynamicGridItem
	{
		/// <summary>
		/// Initializes this instance.
		/// </summary>
		public DynamicGridRow(int row)
			: base(row, 0, null)
		{
			this.IsOdd = row % 2 == 1;
		}

		/// <summary>
		/// Gets a value whether this column's index is odd.
		/// </summary>
		public bool IsOdd { get; private set; }
	}
}
