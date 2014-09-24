using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HtmlEditor.Parser
{
    public class HtmlElement : HtmlObject
    {
        public string TagType { get; set; }
	    public string Tag { get; set; }
        public ICollection<HtmlObject> Children { get; set; }

        public HtmlElement Parent { get; set; }

	    public HtmlElement()
	    {
		    Children = new List<HtmlObject>();
	    }
    }
}
