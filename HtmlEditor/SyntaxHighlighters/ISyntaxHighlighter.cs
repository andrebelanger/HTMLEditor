using System.Windows.Documents;

namespace HtmlEditor.SyntaxHighlighters
{
	public interface ISyntaxHighlighter
	{
		Inline Highlight(string text);
	}
}
