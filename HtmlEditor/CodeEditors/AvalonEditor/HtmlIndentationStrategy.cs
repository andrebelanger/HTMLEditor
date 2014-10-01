using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlEditor.Parser;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Indentation;

namespace HtmlEditor.CodeEditors.AvalonEditor
{
	/// <summary>
	/// Implements Html-specific indentation
	/// </summary>
	class HtmlIndentationStrategy : IIndentationStrategy
	{
		/// <summary>
		/// Gets or sets the automatic indentation amount.
		/// </summary>
		/// <value>
		/// The automatic indent amount.
		/// </value>
		/// <remarks>
		/// This value is how much "nested" items are indented.
		/// </remarks>
		public int AutoIndentAmount { get; set; }

		/// <summary>
		/// Sets the indentation for the specified line.
		/// Usually this is constructed from the indentation of the previous line.
		/// </summary>
		/// <param name="document"></param>
		/// <param name="line"></param>
		public void IndentLine(TextDocument document, DocumentLine line)
		{
			var pLine = line.PreviousLine;
			if (pLine != null)
			{
				var segment = TextUtilities.GetWhitespaceAfter(document, pLine.Offset);
				var indentation = document.GetText(segment);

				var amount = HtmlParser.CountUnclosedTags(document.GetText(pLine));
				if (amount > 0)
					indentation += new string('\t', amount);

				document.Replace(TextUtilities.GetWhitespaceAfter(document, line.Offset), indentation);
			}
		}

		/// <summary>
		/// Reindents a set of lines.
		/// </summary>
		/// <param name="document"></param>
		/// <param name="beginLine"></param>
		/// <param name="endLine"></param>
		public void IndentLines(TextDocument document, int beginLine, int endLine)
		{
			for (var i = beginLine; i <= endLine; i++)
			{
				IndentLine(document, document.GetLineByNumber(i));
			}
		}
	}
}
