using System;
using System.Windows;

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
