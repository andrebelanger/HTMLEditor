using System;
using System.Windows;

namespace HtmlEditor
{
    /// <summary>
    /// Interaction logic for InsertTableWindow.xaml
    /// </summary>
    public partial class InsertTableWindow : Window
    {
        private HtmlEditorWindow _editorWindow;
        private const int MIN_SIZE = 1;
        private const int MAX_SIZE = 99;
        public InsertTableWindow(HtmlEditorWindow editorWindow)
        {
            InitializeComponent();
            _editorWindow = editorWindow;
            error.Visibility = Visibility.Hidden;
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
                int rows = Convert.ToInt32(RowsBox.Text);
                int columns = Convert.ToInt32(ColumnsBox.Text);
                if (rows < MIN_SIZE || rows > MAX_SIZE || columns < MIN_SIZE || columns > MAX_SIZE)
                {
                    throw new FormatException("Number provided was out of the specified range");
                }
                _editorWindow.InsertTable(rows, columns);
                error.Visibility = Visibility.Hidden;
            }
            catch (Exception)
            {
                error.Visibility = Visibility.Visible;
            }

        }

        private void CancelPressed(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
