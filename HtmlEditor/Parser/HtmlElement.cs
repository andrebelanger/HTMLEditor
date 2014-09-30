using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HtmlEditor.Parser
{
	[DebuggerDisplay("<{Tag}>")]
    public class HtmlElement : HtmlObject
	{
		private static readonly Regex TagRegex = new Regex(@"<(?<tag>\w+)(?<attribs>( \w+=""[^>""]+"")*)/?>");
		private static readonly Regex AttribRegex = new Regex(@"(?<key>\w+)=""(?<value>[^>""]+)""");

		public string Tag { get;  set; }
		public ICollection<HtmlObject> Children { get; private set; }
		public Dictionary<string, string> Attributes { get; private set; }

	    public HtmlElement()
	    {
		    Children = new List<HtmlObject>();
		    Attributes = new Dictionary<string, string>();
	    }

		public static HtmlElement Parse(string directiveText)
		{
			var ele = new HtmlElement();

			var t = TagRegex.Match(directiveText);
			if (!t.Success)
				throw new InvalidDataException("Invalid html tag");

			ele.Tag = t.Groups["tag"].Value;

			var a = AttribRegex.Match(t.Groups["attribs"].Value);

			while (a.Success)
			{
				ele.Attributes[a.Groups["key"].Value] = a.Groups["value"].Value;
				a = a.NextMatch();
			}

			return ele;
		}
	}
}
