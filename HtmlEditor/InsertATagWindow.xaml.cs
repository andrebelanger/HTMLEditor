using System.Windows;

namespace HtmlEditor
{
    /// <summary>
    /// Interaction logic for InsertATagWindow.xaml
    /// </summary>
    public partial class InsertATagWindow : Window
    {
        private HtmlEditorWindow _editorWindow;
        public InsertATagWindow(HtmlEditorWindow editorWindow)
        {
            InitializeComponent();
            _editorWindow = editorWindow;
        }

        /// <summary>
        /// Call the InsertATag() function from HtmlEditorWindow
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void InsertPressed(object sender, RoutedEventArgs e)
        {
            _editorWindow.InsertATag(hrefBox.Text);
            this.Close();
        }

        private void CancelPressed(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
