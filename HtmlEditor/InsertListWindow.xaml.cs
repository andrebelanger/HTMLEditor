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
    /// Interaction logic for InsertListWindow.xaml
    /// </summary>
    public partial class InsertListWindow : Window
    {
        private HtmlEditorWindow _editorWindow;
        public InsertListWindow(HtmlEditorWindow editorWindow)
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
            if(OrderedBox.IsChecked == true)
                _editorWindow.InsertOrderedList(Convert.ToInt32(ColumnsBox.Text));
            else
                _editorWindow.InsertUnorderedList(Convert.ToInt32(ColumnsBox.Text));
            
        }

        private void CancelPressed(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
