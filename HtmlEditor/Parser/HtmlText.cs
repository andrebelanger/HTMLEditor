using System.Diagnostics;

namespace HtmlEditor.Parser
{
	[DebuggerDisplay("Text: \"{Text}\"")]
	public class HtmlText : HtmlObject
	{
		public string Text { get; set; }
	}
}
