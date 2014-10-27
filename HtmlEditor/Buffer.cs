using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using HtmlEditor.CodeEditors;
using HtmlEditor.CodeEditors.AvalonEditor;
using HtmlEditor.CodeEditors.PlainEditor;
using HtmlEditor.Parser;

namespace HtmlEditor
{
	public class Buffer : TabItem
	{
		private string _filename;

		public ObservableCollection<string> Links { get; private set; }

		public bool IsDirty { get { return CodeEditor.IsDirty; } }

		// Using a DependencyProperty as the backing store for CodeEditorType.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty CodeEditorTypeProperty =
			DependencyProperty.Register("CodeEditorType", typeof(Type), typeof(Buffer), new PropertyMetadata(typeof(AvalonEditor)));

		/// <summary>
		/// Gets or sets the filename represented by this buffer.
		/// </summary>
		/// <value>
		/// The filename.
		/// </value>
		public string Filename
		{
			get { return _filename; }
			set { _filename = value; Header = Path.GetFileName(value); }
		}

		/// <summary>
		/// Gets or sets the type of the code editor.
		/// </summary>
		/// <value>
		/// The type of the code editor.
		/// </value>
		/// <exception cref="System.InvalidCastException"></exception>
		public Type CodeEditorType
		{
			get { return (Type)GetValue(CodeEditorTypeProperty); }
			set
			{
				if (!typeof (ICodeEditor).IsAssignableFrom(value))
					throw new InvalidCastException(value.Name + " is not a child of ICodeEditor");
				SetValue(CodeEditorTypeProperty, value);

				var t = CodeEditor.Save().ToList();
				Content = Activator.CreateInstance(value);
				CodeEditor.Load(t);
			}
		}

		/// <summary>
		/// Gets the code editor.
		/// </summary>
		/// <value>
		/// The code editor.
		/// </value>
		public ICodeEditor CodeEditor
		{
			get { return Content as ICodeEditor; }
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Buffer"/> class.
		/// </summary>
		public Buffer()
		{
			Links = new ObservableCollection<string>();

			Content = Activator.CreateInstance(CodeEditorType);
			Header = "New file";
            CodeEditor.IsDirty = false;
		}

		/// <summary>
		/// Saves this buffer to the filename it was loaded from.
		/// </summary>
		public void Save()
		{
			Save(_filename);
		}

		/// <summary>
		/// Saves the buffer to the specified filename.
		/// </summary>
		/// <param name="filename">The filename.</param>
		public void Save(string filename)
		{
			File.WriteAllLines(filename, CodeEditor.Save());
			CodeEditor.IsDirty = false;
		}

		/// <summary>
		/// Loads the specified filename.
		/// </summary>
		/// <param name="filename">The filename.</param>
		/// <returns></returns>
		public static Buffer Load(string filename)
		{
			var b = new Buffer();

			b.Filename = filename;
			b.CodeEditor.Load(File.ReadAllLines(filename));

			b.CodeEditor.IsDirty = false;

			return b;
		}

		public void RefreshLinks()
		{
			var links = Regex.Matches(string.Join("", CodeEditor.Save()), @"<a [^>]*href=""(?<href>.+?)"".*?>", RegexOptions.IgnoreCase);

			Links.Clear();

			foreach (Match l in links)
				Links.Add(l.Groups["href"].Value);
		}
	}
}
