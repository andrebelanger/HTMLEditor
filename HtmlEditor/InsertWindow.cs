using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace HtmlEditor
{
    public class InsertWindow : Window
    {
        public IEnumerable<string> Text { get; protected set; }

        protected InsertWindow()
        {
            Text = Enumerable.Empty<string>();
        }
    }
}
