using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HtmlEditor
{
    public class BufferState
    {
        public BufferState(Buffer buffer)
        {
            this.Buffer = buffer;
        }
        public Buffer Buffer { get; set; }
    }
}
