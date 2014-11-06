using System;
using System.Collections.Generic;
using System.Windows;

namespace HtmlEditor
{
    /// <summary>
    /// Interaction logic for InsertListWindow.xaml
    /// </summary>
    public partial class InsertListWindow : InsertWindow
    {
        private const int MinSize = 1;
        private const int MaxSize = 99;

        public InsertListWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Call the InsertList() function from HtmlEditorWindow
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void InsertPressed(object sender, RoutedEventArgs e)
        {
            try
            {
                var columns = Convert.ToInt32(ItemCountBox.Text);

                if (columns < MinSize || columns > MaxSize)
                {
                    throw new FormatException(string.Format("Number provided was out of the allowed range ({0}-{1})", MinSize, MaxSize));
                }

                var table = new List<string>();

                table.Add(OrderedBox.IsChecked == true ? "<ol>" : "<ul>");

                for (var i = 0; i < columns; i++)
                {
                    table.Add("<li></li>");
                }

                table.Add(OrderedBox.IsChecked == true ? "</ol>" : "</ul>");

                Text = table;

	            Close();
            }
            catch (Exception ex)
            {
	            Error.Text = ex.Message;
                Error.Visibility = Visibility.Visible;
            }
        }

        private void CancelPressed(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
