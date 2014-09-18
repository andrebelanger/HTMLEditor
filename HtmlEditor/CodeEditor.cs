using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using HtmlEditor.SyntaxHighlighters;

namespace HtmlEditor
{
	public class CodeEditor : RichTextBox
	{
		/// <summary>
		/// The buffer for this code
		/// </summary>
		private readonly Buffer _buffer;

		public ISyntaxHighlighter Highlighter { get; set; }

		public CodeEditor(Buffer buffer, ISyntaxHighlighter highlight)
		{
			_buffer = buffer;
			Highlighter = highlight;

			RedrawScreen();
		}
		
		// TODO: Replace with dep prop? Unsure
		public void RedrawScreen()
		{
			var screen = new FlowDocument();

			foreach (var line in _buffer.Lines)
			{
				screen.Blocks.Add(new Paragraph(Highlighter.Highlight(line)) { Margin = new Thickness(0)});
			}

			Document = screen;
		}
	}
}
