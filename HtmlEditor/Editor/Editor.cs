using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HtmlEditor
{
    class Editor
    {

        const int MAX_TABS_ALLOWED = 10;
        int num_tabs;
        Tab[] open_tabs = new Tab[MAX_TABS_ALLOWED];
        HTML_File file;


        public Editor()
        {

        }

        void create_tab()
        {
            if (num_tabs < MAX_TABS_ALLOWED)
            {
                for (int i = 0; i < MAX_TABS_ALLOWED; i++)
                {
                    if (open_tabs[i].get_is_hidden_tab())
                    {
                        open_tabs[i] = new Tab(file);
                        break;
                    }
                }
            }
        }

        void delete_tab(int index)
        {
            open_tabs[index] = new Tab();
        }

        void verify()
        {

        }

        void alert()
        {

        }

        void terminate()
        {

        }

        void save()
        {

        }

        void open()
        {

        }

    }
}
