// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DynamicGridHeader.cs" company="APE-Engineering GmbH">
//  This source code is provided under the CodeProject OpenLicence V1.0 
// </copyright>
// <summary>
//   Represents a header within the <see cref="DynamicGridControl" />.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace APE.WPF.Controls.DynamicGrid
{
	/// <summary>
	/// Represents a header within the <see cref="DynamicGridControl"/>.
	/// </summary>
	public class DynamicGridHeader : DynamicGridItem
	{
		/// <summary>
		/// Initializes this instance.
		/// </summary>
		public DynamicGridHeader(int row, int column, object content)
			: base(row, column, content)
		{
		}
	}
}
