using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Media;

namespace HtmlEditor.SyntaxHighlighters
{
    /// <summary>
    /// HTML Formatter - for now, highlights tags
    /// </summary>
    public class HTMLSyntaxHighlighter: ISyntaxHighlighter
    {
        private static readonly SolidColorBrush BlueBrush = new SolidColorBrush(Colors.Blue);

        public Inline Highlight(string text)
        {
            if (text.Length == 0)
                return new Run();

            var ret = new Span(); // Container for all of our text

            bool elementFlag = false;

            // TODO:: Clean this up to make it a bit more efficient?
            for (int i = 0; i < text.Length; i++)
            {
                if (text[i] == '<')
                    elementFlag = true;
                if (text[i] == '>')
                {
                    elementFlag = false;
                    ret.Inlines.Add(new Span(new Run(text[i].ToString()) { Foreground = BlueBrush }));
                    continue;
                }
                    

                if (elementFlag)
                    ret.Inlines.Add(new Span(new Run(text[i].ToString()) { Foreground = BlueBrush }));
                else
                    ret.Inlines.Add(new Run(text[i].ToString()));

            }
                return ret;
        }
    }
}
