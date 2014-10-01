using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
using HtmlEditor.CodeEditors.AvalonEditor;
using HtmlEditor.CodeEditors.PlainEditor;
using Microsoft.Win32;
using HtmlEditor.Parser;
using System.ComponentModel;

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

			var filenames = Environment.GetCommandLineArgs().Skip(1);

			foreach (var buff in filenames.Select(Buffer.Load))
				AddBuffer(buff);

            AddBuffer(new Buffer());
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
				Title = "Open File",
				Multiselect = true
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
		private void SaveBuffer(Buffer buffer)
		{
			if (buffer.Filename != null)
				buffer.Save();
			else
				SaveBufferAs(buffer);
		}

		/// <summary>
		/// Saves all provided buffers.
		/// </summary>
		/// <param name="buffers">The buffers.</param>
		private void SaveAllBuffers(IEnumerable<Buffer> buffers)
		{
			foreach (var b in buffers)
				SaveBuffer(b);
		}

		private void AddBuffer(Buffer b)
		{			
			CodeEditors.SelectedIndex = CodeEditors.Items.Add(b);
			b.Focus();
			b.CodeEditor.Focus();
		}

		//EVENT HANDLERS
        private void Open(object sender, RoutedEventArgs e)
		{
         	foreach (var b in OpenBuffers())
			{
				AddBuffer(b);
			}

            Validate(sender, e);
		}

        private void Validate(object sender, RoutedEventArgs e)
        {
            Buffer buffer = (Buffer)CodeEditors.SelectedItem;

            try
            {
                buffer.CodeEditor.ParseHtml();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }

            MessageBox.Show("You have valid HTML!");
        }

        private void Save(object sender, RoutedEventArgs e)
        {
            Buffer buffer = (Buffer)CodeEditors.SelectedItem;

            SaveBuffer(buffer);

            Validate(sender, e);
        }

        private void SaveAs(object sender, RoutedEventArgs e)
        {
            Buffer buffer = (Buffer)CodeEditors.SelectedItem;

            SaveBufferAs(buffer);

            Validate(sender, e);
        }

        /// <summary>
        /// Opens prompt to create a new table
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OpenTableWindow(object sender, RoutedEventArgs e)
        {

            InsertTableWindow tableWindow = new InsertTableWindow(this);
            tableWindow.Show();
        }

        /// <summary>
        /// Opens prompt to create a new list
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OpenListWindow(object sender, RoutedEventArgs e)
        {

            InsertListWindow listWindow = new InsertListWindow(this);
            listWindow.Show();
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            if (CodeEditors.Items.Cast<Buffer>().Any(b => b.IsDirty))
            {
                if (MessageBox.Show("You have unsaved changes! Are you sure you want to quit?", "Warning!", MessageBoxButton.YesNo) == MessageBoxResult.No)
                {
                    e.Cancel = true;
                }
            }
        }

        public void InsertTable(int rows, int columns)
        {
            // get current buffer
            var buffer = (Buffer)CodeEditors.SelectedItem;

            var table = new List<string>{"<table>"};


            for (int i = 0; i < rows; i++)
            {
                table.Add("<tr>");
                for (int j = 0; j < columns; j++)
                    table.Add("<td></td>");
                table.Add("</tr>");
            }

            table.Add("</table>");

            buffer.CodeEditor.Insert(table);

        }

        public void InsertOrderedList(int items)
        {
            // get current buffer
            var buffer = (Buffer)CodeEditors.SelectedItem;

            var table = new List<string> { "<ol>" };

            for (int i = 0; i < items; i++)
            {
                table.Add("<li>");
                table.Add("</li>");
            }

            table.Add("</ol>");

            buffer.CodeEditor.Insert(table);
        }

        public void InsertUnorderedList(int items)
        {
            // get current buffer
            var buffer = (Buffer)CodeEditors.SelectedItem;

            var table = new List<string> { "<ul>" };

            for (int i = 0; i < items; i++)
            {
                table.Add("<li>");
                table.Add("</li>");
            }

            table.Add("</ul>");

            buffer.CodeEditor.Insert(table);
        }

		private void Plain_Checked(object sender, RoutedEventArgs e)
		{
			var b = (Buffer) CodeEditors.SelectedItem;

			b.CodeEditorType = typeof (PlainEditor);
			MenuAvalon.IsChecked = false;
		}

		private void AvalonEdit_Checked(object sender, RoutedEventArgs e)
		{
			var b = (Buffer)CodeEditors.SelectedItem;

			b.CodeEditorType = typeof(AvalonEditor);
			MenuPlain.IsChecked = false;
		}

		private void CodeEditors_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			var b = CodeEditors.SelectedItem as Buffer;

			MenuAvalon.IsChecked = b.CodeEditorType == typeof (AvalonEditor);
			MenuPlain.IsChecked = b.CodeEditorType == typeof (PlainEditor);
		}
	}
}
