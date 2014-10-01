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
    /// Interaction logic for SpacingWindow.xaml
    /// </summary>
    public partial class SpacingWindow : Window
    {
        private HtmlEditorWindow _editorWindow;
        public SpacingWindow(HtmlEditorWindow editorWindow, int spaces)
        {
            InitializeComponent();
            _editorWindow = editorWindow;
            SpacesBox.Text = Convert.ToString(spaces);
        }

        private void ApplyPressed(object sender, RoutedEventArgs e)
        {
            _editorWindow.UpdateTabSpacing(Convert.ToInt32(SpacesBox.Text));
        }
    }
}
