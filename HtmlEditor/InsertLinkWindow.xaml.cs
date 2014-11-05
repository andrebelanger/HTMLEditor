using System.Windows;

namespace HtmlEditor
{
    /// <summary>
    /// Interaction logic for InsertLinkWindow.xaml
    /// </summary>
    public partial class InsertLinkWindow : Window
    {
        private readonly HtmlEditorWindow _editorWindow;
        public InsertLinkWindow(HtmlEditorWindow editorWindow)
        {
            _editorWindow = editorWindow;
	        Owner = editorWindow;

            InitializeComponent();
        }

        /// <summary>
        /// Call the InsertLink() function from HtmlEditorWindow
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void InsertPressed(object sender, RoutedEventArgs e)
        {
            _editorWindow.InsertLink(HrefBox.Text, LinkTextBox.Text);
            Close();
        }

        private void CancelPressed(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
