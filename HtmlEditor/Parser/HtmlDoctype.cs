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
	}
}
