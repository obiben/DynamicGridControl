// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DelegateComparer.cs" company="APE-Engineering GmbH">
//  This source code is provided under the CodeProject OpenLicence V1.0 
// </copyright>
// <summary>
//   Implementation of an <see cref="IComparer" /> that uses a delegate to compare two instances.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace APE.WPF.Controls.DynamicGrid
{
	using System;
	using System.Collections.Generic;

	/// <summary>
	/// Implementation of an <see cref="IComparer"/> that uses a delegate to compare two instances.
	/// </summary>
	/// <typeparam name="T">Type of items to compare</typeparam>
	public class DelegateComparer<T> : IComparer<T>
	{
		/// <summary>
		/// Backing field for the compare delegate
		/// </summary>
		private readonly Func<T, T, int> compareDelegate;

		/// <summary>
		/// Initializes this instance.
		/// </summary>
		/// <param name="compareDelegate">Delegate that compares two items.</param>
		public DelegateComparer(Func<T, T, int> compareDelegate)
		{
			if (compareDelegate == null) throw new ArgumentNullException("compareDelegate");
			this.compareDelegate = compareDelegate;
		}

		/// <summary>
		/// Compare implementation.
		/// </summary>
		public int Compare(T x, T y)
		{
			return this.compareDelegate(x, y);
		}
	}
}
