using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HtmlEditor.Parser
{
	[DebuggerDisplay("<{Tag}>")]
    public class HtmlElement : HtmlObject
    {
		public string Tag { get;  set; }
		public ICollection<HtmlObject> Children { get; private set; }
		public Dictionary<string, string> Attributes { get; private set; }

	    public HtmlElement()
	    {
		    Children = new List<HtmlObject>();
		    Attributes = new Dictionary<string, string>();
	    }
    }
}
