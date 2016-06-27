// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SampleGridItem.cs" company="APE-Engineering GmbH">
//  This source code is provided under the CodeProject OpenLicence V1.0 
// </copyright>
// <summary>
//   Item for the <see cref="DynamicGridSampleControl" />.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace APE.WPF.Controls.DynamicGrid
{
	using System;

	/// <summary>
	/// Item for the <see cref="DynamicGridSampleControl"/>.
	/// </summary>
	public class SampleGridItem
	{
		/// <summary>
		/// Gets or sets a product
		/// </summary>
		public string ProductName { get; set; }
		
		/// <summary>
		/// Gets or sets the production date
		/// </summary>
		public DateTime ProductionDate { get; set; }

		/// <summary>
		/// Gets or sets the production count
		/// </summary>
		public int ProductionCount { get; set; } 
	}
}
