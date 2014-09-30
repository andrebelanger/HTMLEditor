using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HtmlEditor.Parser
{
	[DebuggerDisplay("Text: \"{Text}\"")]
	public class HtmlText : HtmlObject
	{
		public string Text { get; set; }
	}
}
