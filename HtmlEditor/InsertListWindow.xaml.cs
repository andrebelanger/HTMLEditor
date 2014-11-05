using System;
using System.Windows;

namespace HtmlEditor
{
    /// <summary>
    /// Interaction logic for InsertListWindow.xaml
    /// </summary>
    public partial class InsertListWindow : Window
    {
        private readonly HtmlEditorWindow _editorWindow;
        private const int MinSize = 1;
        private const int MaxSize = 99;

        public InsertListWindow(HtmlEditorWindow editorWindow)
        {
	        Owner = editorWindow;
            _editorWindow = editorWindow;

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

                if (OrderedBox.IsChecked == true)
                {
                    _editorWindow.InsertOrderedList(columns);
                }
                else
                {
                    _editorWindow.InsertUnorderedList(columns);
                }

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
