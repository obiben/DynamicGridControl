// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GridHelpers.cs" company="APE-Engineering GmbH">
//  This source code is provided under the CodeProject OpenLicence V1.0 
// </copyright>
// <summary>
//   XAML helpers to add a dynamic amount or rows / columns to a Grid.
//   Inspired by Rachel Lim http://rachel53461.wordpress.com
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace APE.WPF.Controls.DynamicGrid
{
	using System.Windows;
	using System.Windows.Controls;

	/// <summary>
	/// XAML helpers to add a dynamic amount or rows / columns to a Grid.
	/// Inspired by Rachel Lim http://rachel53461.wordpress.com/
	/// </summary>
	public class GridHelpers
	{
		#region Attached property definitions

		public static readonly DependencyProperty RowCountProperty = DependencyProperty.RegisterAttached("RowCount", typeof(int), typeof(GridHelpers), new PropertyMetadata(-1, RowCountChangedHandler));
		
		public static readonly DependencyProperty ColumnCountProperty = DependencyProperty.RegisterAttached("ColumnCount", typeof(int), typeof(GridHelpers), new PropertyMetadata(-1, ColumnCountChangedHandler));

		#endregion

		#region Attached property accessors

		/// <summary>
		/// Gets the row count.
		/// </summary>
		public static int GetRowCount(DependencyObject obj)
		{
			return (int)obj.GetValue(RowCountProperty);
		}

		/// <summary>
		/// Sets the row count.
		/// </summary>
		public static void SetRowCount(DependencyObject obj, int value)
		{
			obj.SetValue(RowCountProperty, value);
		}

		/// <summary>
		/// Gets the column count.
		/// </summary>
		public static int GetColumnCount(DependencyObject obj)
		{
			return (int)obj.GetValue(ColumnCountProperty);
		}

		/// <summary>
		/// Sets the column count.
		/// </summary>
		public static void SetColumnCount(DependencyObject obj, int value)
		{
			obj.SetValue(ColumnCountProperty, value);
		}

		#endregion

		#region Attached propertychanged handlers

		/// <summary>
		/// Method that is invoked when the RowCount changed.
		/// </summary>
		public static void RowCountChangedHandler(DependencyObject obj, DependencyPropertyChangedEventArgs e)
		{
			if (!(obj is Grid) || (int)e.NewValue < 0)
				return;

			var grid = (Grid)obj;
			grid.RowDefinitions.Clear();

			for (int i = 0; i < (int)e.NewValue; i++) 
				grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
		}

		/// <summary>
		/// Method that is invoked when the ColumnCount changed.
		/// </summary>
		public static void ColumnCountChangedHandler(DependencyObject obj, DependencyPropertyChangedEventArgs e)
		{
			if (!(obj is Grid) || (int)e.NewValue < 0)
				return;

			var grid = (Grid)obj;
			grid.ColumnDefinitions.Clear();

			for (int i = 0; i < (int)e.NewValue; i++) 
				grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
		}

		#endregion
	}
}
