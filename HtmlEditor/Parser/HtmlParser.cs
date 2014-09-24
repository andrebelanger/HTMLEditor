using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HtmlEditor.Parser
{
    public class HtmlParser
    {
        HtmlElement ValidatedHtml;
        public HtmlObject ParseHtml(string html)
        {
            Parse(ValidatedHtml, html);
            return ValidatedHtml;
        }
            
        private void Parse(HtmlElement currentTag, string html)
        {
            HtmlElement CurrentTag = currentTag;
            HtmlElement ParentTag = currentTag.Parent;

            int TagStart;
            int TagEnd;

            TagStart = html.IndexOf("<");   //Gets start position of a tag
            TagEnd = html.IndexOf(">"); //Gets end position of a tag

            if (TagStart > TagEnd)   //If a End tag comes before Start tag: DIE
                throw new Exception("End tag found before start tag");

            if(html[TagStart+1].ToString() == "/")  //If this is closing a parent tag
            {
                if (ParentTag.Equals(null))
                    throw new Exception("Parent tag should never be null on closing tag, not sure what happened");

                ParentTag.Tag += html.Substring(TagStart, TagEnd - TagStart);  //Retrieves tag 
                html.Remove(TagStart, TagEnd - TagStart);   //Remove tag from HTML text


            }
            else
                CurrentTag = new HtmlElement();

            CurrentTag.Tag = html.Substring(TagStart, TagEnd - TagStart);  //Retrieves tag 
            html.Remove(TagStart, TagEnd - TagStart);   //Remove tag from HTML text
            //CurrentTag.TagType = GetTagType(CurrentTag.Tag)   //Gets the type of tag

            if (html[TagEnd - 1].ToString() == "/")    //Handle a self closing tag
            {
                if (ParentTag != null)   //If there is a parent tag
                {
                    ParentTag.Children.Add(CurrentTag); //Add it under the parent tag

                    if (IsAnotherTag(html))   //If theres another HTML element
                        Parse(ParentTag, html);
                    else
                        throw new Exception("No closing tag"); //If theres a parent element and no more elments, theres an unclosed tag somewhere
                }  
                //WILL END: No parent tag so no more Html to parse
            }
            else   //Not a self closing tag
            {
                TagStart = html.IndexOf("<");   //Gets next tag start
                    
                string TempString = html.Substring(0, TagStart-1);
                if(TempString.TakeWhile(c => !char.IsWhiteSpace(c)).Count() > 0)    //If there is text or digits in the string
                {
                    CurrentTag.Tag += TempString;
                    html.Remove(0,TagStart-1);  //Remove any tag text between start and end tag's
                }
                    
                TagStart = html.IndexOf("<");   //Gets next tag start
                TagEnd = html.IndexOf("<");    //Gets next tag end
                    
                if(html.Substring(TagStart, TagEnd) == "</"+CurrentTag.TagType+">") //If this is the end of the current tag
                {
                    CurrentTag.Tag += TempString;
                    html.Remove(TagStart,TagEnd-TagStart);  //Remove the closing tag
                    if(IsAnotherTag(html))
                        Parse(ParentTag, html);
                    //WILL END: No more tags to parse
                }
                else
	            {   //This is a new tag: Create new tag element with parent and re-run parse
                    HtmlElement NewTag = new HtmlElement();
                    NewTag.Parent = CurrentTag;
                    Parse(NewTag, html);
	            }
            }
        }

        private bool IsAnotherTag(string html)
        {
            int NextTag = html.IndexOf("<");

            if (!NextTag.Equals(null))   //If theres another HTML element
                return true;
            else
                return false;
        }

        private void CheckForValidTag()
        {
            //TODO
            //Check 
    
        }
    }
}
