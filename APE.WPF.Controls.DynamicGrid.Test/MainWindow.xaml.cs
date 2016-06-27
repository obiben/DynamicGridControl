namespace APE.WPF.Controls.DynamicGrid
{
	using System;
	using System.Collections.Generic;
	using System.Windows;

	public partial class MainWindow : Window
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="MainWindow"/> class.
		/// </summary>
		public MainWindow()
		{
			var random = new Random();
			var dataItems = new List<SampleGridItem>();

			for (int x = 0; x < 1000; x++)
			{
				for (int y = 0; y < 1000; y++)
				{
					dataItems.Add(
						new SampleGridItem()
							{
								ProductionDate = new DateTime(random.Next(2010, 2014), random.Next(1, 12), random.Next(1, 27)),
								ProductName = string.Format("Product {0:x4}", y),
								ProductionCount = random.Next(0, 2) * random.Next(0, 50)
							});
				}
			}

			this.DataContext = dataItems;

			this.InitializeComponent();
		}
	}
}
