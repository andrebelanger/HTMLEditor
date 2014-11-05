using System;
using System.Windows;

namespace HtmlEditor
{
    /// <summary>
    /// Interaction logic for SpacingWindow.xaml
    /// </summary>
    public partial class SpacingWindow : Window
    {
        private readonly HtmlEditorWindow _editorWindow;
        public SpacingWindow(HtmlEditorWindow editorWindow, int spaces)
        {
            _editorWindow = editorWindow;
	        Owner = editorWindow;

            InitializeComponent();

	        SpacesBox.Text = spaces.ToString();
        }

        private void ApplyPressed(object sender, RoutedEventArgs e)
        {
	        try
	        {
		        _editorWindow.UpdateTabSpacing(Convert.ToInt32(SpacesBox.Text));
		        Close();
	        }
	        catch (Exception ex)
	        {
		        Error.Text = ex.Message;
				Error.Visibility = Visibility.Visible;
	        }
        }
    }
}
