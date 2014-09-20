using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HtmlEditor.Domain
{
    public class HtmlElement : HtmlObject
    {
        public string HtmlTag { get; set; }
        public HtmlObject ChildTags { get; set; }
    }
}
