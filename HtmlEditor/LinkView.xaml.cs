using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace HtmlEditor
{
	/// <summary>
	/// Interaction logic for LinkView.xaml
	/// </summary>
	public partial class LinkView : Window
	{
		public ObservableCollection<Tuple<string, int>> Links { get; private set; }

		private readonly ObservableCollection<string> _linkSource;

		public LinkView(ObservableCollection<string> links)
		{
			Links = new ObservableCollection<Tuple<string, int>>();

			_linkSource = links;
			_linkSource.CollectionChanged += _linkSource_CollectionChanged;

			InitializeComponent();
		}

		public void UpdateLinks()
		{
			var linkExp = ViewType.SelectedIndex == 0
				? // By appearance
				_linkSource.Select(l => Tuple.Create(l, 1))
				: // By dest
				_linkSource.GroupBy(l => l)
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
			UpdateLinks();
		}

		private void ViewType_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			UpdateLinks();
		}
	}
}
