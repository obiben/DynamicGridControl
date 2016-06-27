// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ScrollableControl.cs" company="APE-Engineering GmbH">
//  This source code is provided under the CodeProject OpenLicence V1.0 
// </copyright>
// <summary>
//   Simple control that provides an easy interface for scrolling.
//   See the SetScrollEvent and OnScroll methods.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace APE.WPF.Controls.DynamicGrid
{
	using System.Windows;
	using System.Windows.Controls;
	using System.Windows.Controls.Primitives;
	using System.Windows.Media;

	/// <summary>
	/// Simple control that provides an easy interface for scrolling.
	/// See the SetScrollEvent and OnScroll methods.
	/// </summary>
	public abstract class ScrollableControl : Control, IScrollInfo
	{
		#region Private fields

		/// <summary>
		/// Backing field for the item size.
		/// </summary>
		private Size itemSize;

		#endregion

		#region Construction / Desctruction

		/// <summary>
		/// Initializes this instance
		/// </summary>
		protected ScrollableControl()
		{
			this.itemSize = new Size(1, 1);
		}

		#endregion

		#region Public properties

		/// <summary>
		/// Gets or sets a value whether the control can scroll vertically.
		/// </summary>
		public bool CanVerticallyScroll { get; set; }

		/// <summary>
		/// Gets or sets a value whether the control can scroll horizontally.
		/// </summary>
		public bool CanHorizontallyScroll { get; set; }

		/// <summary>
		/// Gets the extent's with.
		/// </summary>
		public double ExtentWidth { get; private set; }

		/// <summary>
		/// Gets the extent's height.
		/// </summary>
		public double ExtentHeight { get; private set; }

		/// <summary>
		/// Gets the viewport's width.
		/// </summary>
		public double ViewportWidth { get; private set; }

		/// <summary>
		/// Gets the viewport's height.
		/// </summary>
		public double ViewportHeight { get; private set; }

		/// <summary>
		/// Gets the horizontal scroll offset.
		/// </summary>
		public double HorizontalOffset { get; private set; }

		/// <summary>
		/// Gets the vertical scroll offset.
		/// </summary>
		public double VerticalOffset { get; private set; }

		/// <summary>
		/// Gets or sets the scroll owner.
		/// </summary>
		public ScrollViewer ScrollOwner { get; set; }

		#endregion

		#region Public methods

		/// <summary>
		/// Scrolls a line up.
		/// </summary>
		public void LineUp()
		{
			this.SetVerticalOffset(this.VerticalOffset - this.itemSize.Height);
		}

		/// <summary>
		/// Scrolls a line down.
		/// </summary>
		public void LineDown()
		{
			this.SetVerticalOffset(this.VerticalOffset + this.itemSize.Height);
		}

		/// <summary>
		/// Scrolls a line to the left.
		/// </summary>
		public void LineLeft()
		{
			this.SetHorizontalOffset(this.HorizontalOffset - this.itemSize.Width);
		}

		/// <summary>
		/// Scrolls a line to the right.
		/// </summary>
		public void LineRight()
		{
			this.SetHorizontalOffset(this.HorizontalOffset + this.itemSize.Width);
		}

		/// <summary>
		/// Scrolls a page up.
		/// </summary>
		public void PageUp()
		{
			this.SetVerticalOffset(this.VerticalOffset - this.ViewportHeight);
		}

		/// <summary>
		/// Scrolls a page down.
		/// </summary>
		public void PageDown()
		{
			this.SetVerticalOffset(this.VerticalOffset + this.ViewportHeight);
		}

		/// <summary>
		/// Scrolls a page to the left.
		/// </summary>
		public void PageLeft()
		{
			this.SetHorizontalOffset(this.HorizontalOffset - this.ViewportWidth);
		}

		/// <summary>
		/// Scrolls a page to the right.
		/// </summary>
		public void PageRight()
		{
			this.SetHorizontalOffset(this.HorizontalOffset + this.ViewportWidth);
		}

		/// <summary>
		/// Scrolls 3 units up.
		/// </summary>
		public void MouseWheelUp()
		{
			this.SetVerticalOffset(this.VerticalOffset - (this.itemSize.Height * 3));
		}

		/// <summary>
		/// Scrolls three units down.
		/// </summary>
		public void MouseWheelDown()
		{
			this.SetVerticalOffset(this.VerticalOffset + (this.itemSize.Height * 3));
		}

		/// <summary>
		/// Scrolls three units to the left.
		/// </summary>
		public void MouseWheelLeft()
		{
			this.SetHorizontalOffset(this.HorizontalOffset - (this.itemSize.Width * 3));
		}

		/// <summary>
		/// Scrolls three units to the right.
		/// </summary>
		public void MouseWheelRight()
		{
			this.SetHorizontalOffset(this.HorizontalOffset - (this.itemSize.Width * 3));
		}

		/// <summary>
		/// Sets the horizontal scroll offset.
		/// </summary>
		public void SetHorizontalOffset(double offset)
		{
			// Validate offset
			if (offset < 0 || this.ExtentWidth <= this.ViewportWidth) 
				offset = 0;
			else if (offset > this.ExtentWidth - this.ViewportWidth) 
				offset = this.ExtentWidth - this.ViewportWidth;

			// Apply offset
			this.HorizontalOffset = offset;
			this.UpdateScrollOwner();
			this.OnScroll();
		}

		/// <summary>
		/// Sets the vertical scroll offset.
		/// </summary>
		public void SetVerticalOffset(double offset)
		{
			// Validate offset
			if (offset < 0 || this.ExtentHeight <= this.ViewportHeight) 
				offset = 0;
			else if (offset > this.ExtentHeight - this.ViewportHeight) 
				offset = this.ExtentHeight - this.ViewportHeight;

			// Apply offset
			this.VerticalOffset = offset;
			this.UpdateScrollOwner();
			this.OnScroll();
		}

		/// <summary>
		/// Ensures that an item is visible.
		/// </summary>
		public Rect MakeVisible(Visual visual, Rect rectangle)
		{
			return rectangle;
		}

		#endregion

		/// <summary>
		/// Is invoked when the control's size changed.
		/// </summary>
		protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
		{
			base.OnRenderSizeChanged(sizeInfo);

			if (sizeInfo.WidthChanged)
				this.SetHorizontalOffset(this.HorizontalOffset);

			if (sizeInfo.HeightChanged)
				this.SetVerticalOffset(this.VerticalOffset);

			this.ViewportWidth = this.ActualWidth;
			this.ViewportHeight = this.ActualHeight;
			
			this.UpdateScrollOwner();
			this.OnScroll();
		}

		/// <summary>
		/// Method that provides a way to set scroll information for derived controls.
		/// </summary>
		/// <param name="itemWidth">A single item's width.</param>
		/// <param name="itemHeight">A single item's height.</param>
		/// <param name="extentWidth">Full extent's width.</param>
		/// <param name="extentHeight">Full extent's height.</param>
		protected void SetScrollExtent(double itemWidth, double itemHeight, double extentWidth, double extentHeight)
		{
			this.itemSize = new Size(itemWidth, itemHeight);
			
			this.ExtentWidth = extentWidth;
			this.ExtentHeight = extentHeight;
			this.ViewportWidth = this.ActualWidth;
			this.ViewportHeight = this.ActualHeight;

			this.UpdateScrollOwner();
			this.OnScroll();
		}

		/// <summary>
		/// Override that enables derived controls to be informed about scrolling events.
		/// </summary>
		protected virtual void OnScroll()
		{
		}

		/// <summary>
		/// Updates the scroll owner.
		/// </summary>
		private void UpdateScrollOwner()
		{
			this.CanHorizontallyScroll = this.ViewportWidth < this.ExtentWidth;
			this.CanVerticallyScroll = this.ViewportHeight < this.ExtentHeight;

			if (this.ScrollOwner != null)
				this.ScrollOwner.InvalidateScrollInfo();
		}
	}
}
