using System.Windows;
using System.Windows.Documents;

namespace HtmlEditor.SyntaxHighlighters
{
	/// <summary>
	/// Implements a syntax highlighter that doesn't modify the text
	/// </summary>
	public class TextSyntaxHighlighter : ISyntaxHighlighter
	{
		public Inline Highlight(string text)
		{
			return new Run(text);
		}
	}
}
