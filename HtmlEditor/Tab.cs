using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HtmlEditor
{
    class Tab
    {
        //for possibly being able to open a new, blank tab in the editor
        bool is_blank_tab;

        //represents an available tab that can be created
        bool is_hidden_tab;


        public Tab()
        {
            is_hidden_tab = true;
        }

        public Tab(HTML_File file){
            is_hidden_tab = false;
        }

        public bool get_is_hidden_tab()
        {
            return is_hidden_tab;
        }
        

    }
}
