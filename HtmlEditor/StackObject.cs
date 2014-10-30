using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HtmlEditor
{
    public class StackObject
    {
        public StackObject(Buffer buffer)
        {
            this.Buffer = buffer;
        }
        public Buffer Buffer { get; set; }
    }
}
