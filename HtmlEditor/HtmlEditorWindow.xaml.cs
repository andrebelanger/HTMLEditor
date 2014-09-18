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
using System.Windows.Navigation;
using System.Windows.Shapes;
using HtmlEditor.SyntaxHighlighters;
using Microsoft.Win32;

namespace HtmlEditor
{
	/// <summary>
	/// Interaction logic for HtmlEditorWindow.xaml
	/// </summary>
	public partial class HtmlEditorWindow : Window
	{
		public HtmlEditorWindow()
		{
			InitializeComponent();
		}

		/// <summary>
		/// Shows an open file box and returns buffers loaded from the select files.
		/// </summary>
		/// <returns></returns>
		public IEnumerable<Buffer> OpenBuffers()
		{
			var ofd = new OpenFileDialog
			{
				DefaultExt = ".html",
				Filter = "HTML files|*.htm;*.html|All files|*",
				Title = "Open File"
			};

			if (ofd.ShowDialog() == true)
			{
				// This LINQ replaces a for-each loop
				// The Select method performs some operation
				// on each item in a list and returns a list of the results.
				// So in effect, this code loads a new buffer for each given filename
				return ofd.FileNames.Select(Buffer.Load);
			}

			// If they cancelled, we have nothing to load
			return Enumerable.Empty<Buffer>();
		}

		/// <summary>
		/// Displays a save as prompt, changing the buffer's FileName property.
		/// </summary>
		/// <param name="buffer">The buffer.</param>
		public void SaveBufferAs(Buffer buffer)
		{
			var sdf = new SaveFileDialog()
			{
				DefaultExt = ".html",
				Filter = "HTML files|*.htm;*.html|All files|*",
				OverwritePrompt = true,
				Title = "Save File As"
			};

			if (sdf.ShowDialog() == true)
			{
				buffer.Filename = sdf.FileName;
				buffer.Save();
			}
		}

		/// <summary>
		/// Saves a single buffer, prompting for a filename if needed.
		/// </summary>
		/// <param name="buffer">The buffer</param>
		public void SaveBuffer(Buffer buffer)
		{
			if (buffer.Filename == null)
				buffer.Save();
			else
				SaveBufferAs(buffer);
		}

		/// <summary>
		/// Saves all provided buffers.
		/// </summary>
		/// <param name="buffers">The buffers.</param>
		public void SaveAllBuffers(IEnumerable<Buffer> buffers)
		{
			foreach (var b in buffers)
				SaveBuffer(b);
		}

		private void OpenExample(object sender, RoutedEventArgs e)
		{
			foreach (var b in OpenBuffers())
			{
				var ce = new CodeEditor(b, new CowSyntaxHighlighter());
				var tab = new TabItem {Header = System.IO.Path.GetFileName(b.Filename), Content = ce};
				CodeEditors.SelectedIndex = CodeEditors.Items.Add(tab);
				tab.Focus();
				ce.Focus();
			}
		}
	}
}
