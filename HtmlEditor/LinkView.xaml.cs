using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace HtmlEditor
{
	/// <summary>
	/// Interaction logic for LinkView.xaml
	/// </summary>
	public partial class LinkView : Window
	{
		public ObservableCollection<Tuple<string, int>> Links { get; private set; }

		private readonly Buffer buff;

		public LinkView(Buffer b)
		{
			Links = new ObservableCollection<Tuple<string, int>>();

			buff = b;
			buff.Links.CollectionChanged += _linkSource_CollectionChanged;

			InitializeComponent();
		}

		public void UpdateLinks()
		{
			var linkExp = ViewType.SelectedIndex == 0
				? // By appearance
				buff.Links.Select(l => Tuple.Create(l, 1))
				: // By dest
				buff.Links.GroupBy(l => l)
					.Select(g => Tuple.Create(g.Key, g.Count()))
					.OrderBy(t => t.Item1);

			Links.Clear();
			foreach (var link in linkExp)
				Links.Add(link);
		}

		void _linkSource_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
			UpdateLinks();
		}

		private void Refresh_Click(object sender, RoutedEventArgs e)
		{
			buff.RefreshLinks();
		}

		private void ViewType_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			UpdateLinks();
		}
	}
}
