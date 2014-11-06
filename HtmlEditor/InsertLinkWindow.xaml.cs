using System.Collections.Generic;
using System.Windows;

namespace HtmlEditor
{
    /// <summary>
    /// Interaction logic for InsertLinkWindow.xaml
    /// </summary>
    public partial class InsertLinkWindow : InsertWindow
    {
        public InsertLinkWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Call the InsertLink() function from HtmlEditorWindow
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void InsertPressed(object sender, RoutedEventArgs e)
        {
            Text = new List<string> { "<a href=\"" + HrefBox.Text + "\">" + LinkTextBox.Text + "</a>" };
            Close();
        }

        private void CancelPressed(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
