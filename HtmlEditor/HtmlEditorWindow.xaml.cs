using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using HtmlEditor.CodeEditors.AvalonEditor;
using HtmlEditor.CodeEditors.PlainEditor;
using Microsoft.Win32;
using System.ComponentModel;

namespace HtmlEditor
{
	/// <summary>
	/// Interaction logic for HtmlEditorWindow.xaml
	/// </summary>
	public partial class HtmlEditorWindow : Window
	{
		public Buffer CurrentBuffer { get { return CodeEditors.SelectedItem as Buffer; } }


        public static RoutedCommand NewKeyboardShortcut = new RoutedCommand();

        public static RoutedCommand OpenKeyboardShortcut = new RoutedCommand();

        public static RoutedCommand SaveKeyboardShortcut = new RoutedCommand();

        public static RoutedCommand UndoKeyboardShortcut = new RoutedCommand();

        public static RoutedCommand RedoKeyboardShortcut = new RoutedCommand();

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

            //Validate(sender, e);
		}


        private void OpenNewBuffer(object sender, RoutedEventArgs e)
        {
            AddBuffer(new Buffer());
        }

        private void Validate(object sender, RoutedEventArgs e)
        {
            try
            {
                CurrentBuffer.CodeEditor.ParseHtml();
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
            SaveBuffer(CurrentBuffer);

            //Validate(sender, e);
        }

        private void SaveAs(object sender, RoutedEventArgs e)
        {
            SaveBufferAs(CurrentBuffer);

            //Validate(sender, e);
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


        private void OpenSpacingWindow(object sender, RoutedEventArgs e)
        {
            SpacingWindow spacingWindow = new SpacingWindow(this, CurrentBuffer.CodeEditor.AutoIndentAmount);
            spacingWindow.Show();
        }

        private void OpenATagWindow(object sender, RoutedEventArgs e)
        {
            InsertATagWindow aTagWindow = new InsertATagWindow(this);
            aTagWindow.Show();
        }

        private void OpenImageWindow(object sender, RoutedEventArgs e)
        {
            InsertImageWindow imageWindow = new InsertImageWindow(this);
            imageWindow.Show();
        }
        private void Window_Closing(object sender, CancelEventArgs e)
        {
            var b = (Buffer)CodeEditors.SelectedItem;

            if ( b.IsDirty )
            {
                if (MessageBox.Show("You have unsaved changes! Are you sure you want to quit?", "Warning!", MessageBoxButton.YesNo) == MessageBoxResult.No)
                {
                    e.Cancel = true;
                }
            }
        }

        public void UpdateTabSpacing(int spaces)
        {
			CurrentBuffer.CodeEditor.AutoIndentAmount = spaces;
        }

        public void InsertTable(int rows, int columns)
        {
            var table = new List<string>{"<table>"};


            for (int i = 0; i < rows; i++)
            {
                table.Add("<tr>");
                for (int j = 0; j < columns; j++)
                    table.Add("<td></td>");
                table.Add("</tr>");
            }

            table.Add("</table>");

            CurrentBuffer.CodeEditor.Insert(table);

        }

        public void InsertATag(string href)
        {
            var tag = new List<string>{"<a href=\"" + href + "\"></a>"};

            CurrentBuffer.CodeEditor.Insert(tag);

	        CurrentBuffer.RefreshLinks();
        }

        public void InsertImage(string url)
        {

            var tag = new List<string> { "<img src=\"" + url + "\" />" };

            CurrentBuffer.CodeEditor.Insert(tag);
        }

        public void InsertOrderedList(int items)
        {
            var table = new List<string> { "<ol>" };

            for (int i = 0; i < items; i++)
            {
                table.Add("<li>");
                table.Add("</li>");
            }

            table.Add("</ol>");

            CurrentBuffer.CodeEditor.Insert(table);
        }

        public void InsertUnorderedList(int items)
        {
            var table = new List<string> { "<ul>" };

            for (int i = 0; i < items; i++)
            {
                table.Add("<li>");
                table.Add("</li>");
            }

            table.Add("</ul>");

            CurrentBuffer.CodeEditor.Insert(table);
        }

        private void InsertBold(object sender, RoutedEventArgs e)
        {
            CurrentBuffer.CodeEditor.Insert(new [] {"<strong></strong>"});
        }

        private void InsertItalic(object sender, RoutedEventArgs e)
        {
            CurrentBuffer.CodeEditor.Insert(new []{"<em></em>"});
        }

		private void Plain_Checked(object sender, RoutedEventArgs e)
		{
			var b = (Buffer) CodeEditors.SelectedItem;

			b.CodeEditorType = typeof(PlainEditor);
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
            MenuWordWrap.IsChecked = b.CodeEditor.WordWrap;
		}

        private void WordWrap_Checked(object sender, RoutedEventArgs e)
        {
            var b = (Buffer)CodeEditors.SelectedItem;

            b.CodeEditor.WordWrap = true;
        }

        private void WordWrap_Unchecked(object sender, RoutedEventArgs e)
        {
            var b = (Buffer)CodeEditors.SelectedItem;

            b.CodeEditor.WordWrap = false;
        }

        private void CloseBuffer(object sender, RoutedEventArgs e)
        {
            var i = CodeEditors.SelectedIndex;
            var b = CodeEditors.SelectedItem as Buffer;

            if (b.IsDirty && MessageBox.Show("You have unsaved changes. Really close this buffer?", "Confirm close", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No)
                return;

            if(CodeEditors.Items.Count == 1)
                AddBuffer(new Buffer());

            if (i != 0)
                i--;
            else
                i++;

            CodeEditors.SelectedIndex = i;

            CodeEditors.Items.Remove(b);
        }

		private void OpenLinkview_Click(object sender, RoutedEventArgs e)
		{
			CurrentBuffer.RefreshLinks();
			new LinkView(CurrentBuffer).Show();
		}

        //Place holder for Undo function.
        private void Undo(object sender, RoutedEventArgs e)
        {
            /*CodeEditors.Document
            b.redoStack.Push(b);
            if(b.undoStack.Count > 0)
                b = b.undoStack.Pop();*/
        }
        
        //Place holder for Redo function.
        private void Redo(object sender, RoutedEventArgs e)
        {
            /*var b = CodeEditors.SelectedItem as Buffer;
            b.undoStack.Push(b);
            if (b.redoStack.Count > 0)
                b = b.redoStack.Pop();*/
        }


        private void NewShortcut(object sender, ExecutedRoutedEventArgs e)
        {
            OpenNewBuffer(sender, e);
        }

        private void OpenShortcut(object sender, ExecutedRoutedEventArgs e)
        {
            Open(sender, e);
        }

        private void SaveShortcut(object sender, ExecutedRoutedEventArgs e)
        {
            Save(sender, e);
        }

        private void UndoShortcut(object sender, ExecutedRoutedEventArgs e)
        {
            Undo(sender, e);
        }

        private void RedoShortcut(object sender, ExecutedRoutedEventArgs e)
        {
            Redo(sender, e);
        }
	}
}
