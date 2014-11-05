using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using Microsoft.Win32;

namespace HtmlEditor
{
	/// <summary>
	/// Interaction logic for InsertImageWindow.xaml
	/// </summary>
	public partial class InsertImageWindow : Window
	{
		private readonly HtmlEditorWindow _editorWindow;

		public InsertImageWindow(HtmlEditorWindow editorWindow)
		{
			Owner = editorWindow;
			_editorWindow = editorWindow;

			InitializeComponent();
		}

		/// <summary>
		/// Call the InsertImg() function from HtmlEditorWindow
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void InsertPressed(object sender, RoutedEventArgs e)
		{
			_editorWindow.InsertImage(ImgPath.Text);
			Close();
		}

		private void CancelPressed(object sender, RoutedEventArgs e)
		{
			Close();
		}

		private void BrowseBtn_Click(object sender, RoutedEventArgs e)
		{
			var fbd = new OpenFileDialog()
			{
				Multiselect = false,
				Title = "Select Image File",
				Filter = "Image Files|*.png;*.jpg;*.tiff;*.gif;*.bmp;*.ico|All Files|*.*"
			};

			if (fbd.ShowDialog() == true)
			{
				ImgPath.Text = fbd.FileName;
			}
		}

	}

	public sealed class ImageConverter : IValueConverter
	{
		public object Convert(object value, Type targetType,
							  object parameter, CultureInfo culture)
		{
			try
			{
				return new BitmapImage(new Uri((string)value));
			}
			catch
			{
				return new BitmapImage();
			}
		}

		public object ConvertBack(object value, Type targetType,
								  object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
