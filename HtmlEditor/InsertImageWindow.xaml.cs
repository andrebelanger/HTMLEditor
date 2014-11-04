using System.Windows;

namespace HtmlEditor
{
    /// <summary>
    /// Interaction logic for InsertImageWindow.xaml
    /// </summary>
    public partial class InsertImageWindow : Window
    {
        public InsertImageWindow()
        {
            InitializeComponent();
        }

        private HtmlEditorWindow _editorWindow;
        public InsertImageWindow(HtmlEditorWindow editorWindow)
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
            _editorWindow.InsertImage(urlBox.Text);
            this.Close();
        }

        private void CancelPressed(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
