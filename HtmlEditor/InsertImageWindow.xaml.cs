using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using System.Collections.Generic;

namespace HtmlEditor
{
	/// <summary>
	/// Interaction logic for InsertImageWindow.xaml
	/// </summary>
	public partial class InsertImageWindow : InsertWindow
	{
		public InsertImageWindow()
		{
			InitializeComponent();
		}

		/// <summary>
		/// Call the InsertImg() function from HtmlEditorWindow
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void InsertPressed(object sender, RoutedEventArgs e)
		{
            Text = new List<string> { "<img src=\"" + ImgPath.Text + "\" />" };
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
}
