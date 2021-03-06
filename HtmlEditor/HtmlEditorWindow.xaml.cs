﻿using System;
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

        private const int MAX_BUFFERS = 20;

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
            if (CodeEditors.Items.Count < MAX_BUFFERS)
            {
                CodeEditors.SelectedIndex = CodeEditors.Items.Add(b);
                b.Focus();
                b.CodeEditor.Focus();
            }
            else
            {
                MessageBox.Show("You have reached the maximum number of tabs allowed. Please close one before opening another.");
            }
		}

		//EVENT HANDLERS
        private void Open(object sender, RoutedEventArgs e)
		{
            if (CodeEditors.Items.Count < MAX_BUFFERS)
            {
                foreach (var b in OpenBuffers())
                {
                    AddBuffer(b);
                }
            }
            else
            {
                MessageBox.Show("You have reached the maximum number of tabs allowed. Please close one before opening another.");
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
            ShowInputWindow(typeof(InsertTableWindow));
        }

        /// <summary>
        /// Opens prompt to create a new list
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OpenListWindow(object sender, RoutedEventArgs e)
        {
            ShowInputWindow(typeof(InsertListWindow));
        }

        private void OpenSpacingWindow(object sender, RoutedEventArgs e)
        {
            var spacingWindow = new SpacingWindow(this, CurrentBuffer.CodeEditor.AutoIndentAmount);
            spacingWindow.ShowDialog();
        }

        private void OpenLinkWindow(object sender, RoutedEventArgs e)
        {
            ShowInputWindow(typeof(InsertLinkWindow));
        }

        private void OpenImageWindow(object sender, RoutedEventArgs e)
        {
            ShowInputWindow(typeof(InsertImageWindow));
        }

        private void ShowInputWindow(Type t)
        {
            var w = (InsertWindow)Activator.CreateInstance(t);

            w.Owner = this;

            w.ShowDialog();

            CurrentBuffer.CodeEditor.Insert(w.Text);

            CurrentBuffer.RefreshLinks();
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

        private void Undo(object sender, RoutedEventArgs e)
        {
	        CurrentBuffer.CodeEditor.Undo();
        }
        
        private void Redo(object sender, RoutedEventArgs e)
        {
	        CurrentBuffer.CodeEditor.Redo();
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

		private void OpenOnlineHelp(object sender, RoutedEventArgs e)
		{
			System.Diagnostics.Process.Start("https://google.com");
		}

		private void OpenAbout(object sender, RoutedEventArgs e)
		{
			new AboutBox { Owner = this }.ShowDialog();
		}
	}
}
