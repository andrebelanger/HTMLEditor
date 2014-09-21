using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HtmlEditor.Parser
{
    public class HtmlElement : HtmlObject
    {
	    public string Tag { get; private set; }
        public ICollection<HtmlObject> Children { get; private set; }

	    public HtmlElement(string tag)
	    {
		    Tag = tag;

		    Children = new List<HtmlObject>();
	    }
    }
}
