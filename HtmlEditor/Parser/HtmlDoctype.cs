using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HtmlEditor.Parser
{
	[DebuggerDisplay("Doctype: {Doctype}")]
	public class HtmlDoctype : HtmlObject
	{
		public string Doctype { get; set; }

		public static HtmlDoctype Parse(string directiveText)
		{
			var dt = directiveText.Substring(directiveText.IndexOf(' ') + 1);
			dt = dt.Remove(dt.Length - 1);

			return new HtmlDoctype {Doctype = dt};
		}
	}
}
