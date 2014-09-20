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

			var style = new Style(typeof(Paragraph));
			style.Setters.Add(new Setter(Block.MarginProperty, new Thickness(0)));
			Resources.Add(typeof (Paragraph), style);

			Document = new FlowDocument();

			foreach (var line in _buffer.Lines)
				Document.Blocks.Add(new Paragraph(new Run(line)));
		}

		private IEnumerable<Inline> FormatLine(string text)
		{
			return new []{ Highlighter.Highlight(text) };
		}

		private bool _reformatting;
		protected override void OnTextChanged(TextChangedEventArgs e)
		{
			if (_reformatting)
				return;

			var changes = new HashSet<Paragraph>();

			foreach (var change in e.Changes)
			{
				if (change.AddedLength > 0)
				{
					var start = Document.ContentStart.GetPositionAtOffset(change.Offset);
					var end = Document.ContentStart.GetPositionAtOffset(change.Offset + change.AddedLength);

					if (start == null || end == null)
						continue;

					var changedParagraph = start.Paragraph ?? start.GetAdjacentElement(LogicalDirection.Forward) as Paragraph;

					while (changedParagraph != null)
					{
						if (changedParagraph.ContentStart.CompareTo(end) >= 0)
							break;

						changes.Add(changedParagraph);
						changedParagraph = changedParagraph.NextBlock as Paragraph;
					}
				}
			}

			ReformatParagraphs(changes);

			base.OnTextChanged(e);
		}

		private void ReformatParagraphs(IEnumerable<Paragraph> paragraphs)
		{
			_reformatting = true;
			BeginChange();

			Inline cursorElement = null;

			foreach (var p in paragraphs)
			{
				IEnumerable<Inline> reflow;

				var containsCursor = CaretPosition != null && CaretPosition.CompareTo(p.ContentStart) >= 0 &&
				                     CaretPosition.CompareTo(p.ContentEnd) <= 0;

				if (containsCursor)
				{
					var before = new TextRange(p.ContentStart, CaretPosition).Text;
					var after = new TextRange(CaretPosition, p.ContentEnd).Text;
					var reflowList = new List<Inline>();



					// TODO: Whitespace goes here

					if (!string.IsNullOrEmpty(before))
						reflowList.AddRange(FormatLine(before));
					reflowList.Add((cursorElement = new Run()));
					if (!string.IsNullOrEmpty(after))
						reflowList.AddRange(FormatLine(after));

					reflow = reflowList;
				}
				else
				{
					reflow = FormatLine(new TextRange(p.ContentStart, p.ContentEnd).Text);
				}

				p.Inlines.Clear();
				p.Inlines.AddRange(reflow);
			}

			if (cursorElement != null)
				CaretPosition = cursorElement.ContentStart;

			EndChange();
			_reformatting = false;
		}
	}
}
