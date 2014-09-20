using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HtmlEditor.Domain
{
    public class HtmlObject
    {
        public HtmlObject()
        {
            HtmlElements = new List<HtmlElement>();
        }
        public List<HtmlElement> HtmlElements { get; set; }
    }
}
