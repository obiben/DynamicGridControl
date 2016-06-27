// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DynamicGridDataItem.cs" company="APE-Engineering GmbH">
//  This source code is provided under the CodeProject OpenLicence V1.0 
// </copyright>
// <summary>
//   Represents a data item within the <see cref="DynamicGridControl" />
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace APE.WPF.Controls.DynamicGrid
{
	/// <summary>
	/// Represents a data item within the <see cref="DynamicGridControl"/>
	/// </summary>
	public class DynamicGridDataItem : DynamicGridItem
	{
		/// <summary>
		/// Initializes this instance.
		/// </summary>
		public DynamicGridDataItem(int row, int column, object content)
			: base(row, column, content)
		{
		}
	}
}
