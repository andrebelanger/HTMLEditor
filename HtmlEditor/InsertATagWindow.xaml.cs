using System;
using System.Collections.Generic;
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
        /// Call the InsertList() function from HtmlEditorWindow
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
