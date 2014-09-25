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
        HtmlElement ValidatedHtml = new HtmlElement();
        public HtmlObject ParseHtml(string html)
        {
            Parse(ValidatedHtml, html);
            return ValidatedHtml;
        }

        private string Parse(HtmlElement currentTag, string html)
        {
            int TagStart;
            int TagEnd;

            TagStart = html.IndexOf("<");   //Gets start position of a tag
            TagEnd = html.IndexOf(">"); //Gets end position of a tag

            if (TagStart > TagEnd)   //If a End tag comes before Start tag: DIE
                throw new Exception("End tag found before start tag");

            if (IsClosingTag(TagStart,html))  //HANDLES: Closing tags (self closing tags not included)
            {
                if (html.Substring(TagStart, (TagEnd + 1) - TagStart) == "</" + currentTag.TagType + ">") //If this is the end of the current tag
                {
                    currentTag.Tag += "</" + currentTag.TagType + ">";
                    html = html.Remove(TagStart, (TagEnd + 1) - TagStart);  //Remove the closing tag

                    TagStart = html.IndexOf("<");   //Gets next tag start
                    if (TagStart != -1)
                    {
                        if (IsClosingTag(TagStart, html))  //If next tag is a closing tag
                            MoveToNextTag(currentTag.Parent, html, false);
                        else
                            MoveToNextTag(currentTag.Parent, html, true);
                    }
                    //WILL END IF: No more tags to parse
                }
                else
                {
                    throw new Exception("Wrong end tag for: " + currentTag.TagType);
                }
            }
            else   //HANDLES: Opening tags & self closing tags
            {
                //Gets the opening tag
                currentTag.Tag = html.Substring(TagStart, (TagEnd + 1) - TagStart);  //Retrieves tag
                currentTag.TagType = GetTagType(html);   //Gets the type of tag 
                html = html.Remove(TagStart, (TagEnd + 1) - TagStart);   //Remove tag from HTML text

                if (currentTag.Tag[currentTag.Tag.Length - 2].ToString() == "/")    //HANDLES: Self closing tags : Checks to see if the char before '>' is a '/'
                {
                    TagStart = html.IndexOf("<");   //Gets next tag start
                    if (TagStart != -1)
                    {
                        if (IsClosingTag(TagStart, html))  //If next tag is a closing tag
                            MoveToNextTag(currentTag.Parent, html, false);
                        else
                            MoveToNextTag(currentTag.Parent, html, true);
                    }
                    //WILL END: No parent tag so no more Html to parse
                }
                else   //HANDLES: Non self closing tag
                {
                    TagStart = html.IndexOf("<");   //Gets next tag start

                    //HANDLES: Text between open and close tag, or next opening tag
                    if (TagStart - 1 != -1)    //if the next tag does not immediately start
                    {
                        string TagText = html.Substring(0, TagStart);

                        currentTag.Tag += TagText;
                        html = html.Remove(0, TagStart);  //Remove any tag text between start and end tag's
                    }

                    TagStart = html.IndexOf("<");   //Gets next tag start
                    TagEnd = html.IndexOf(">");    //Gets next tag end

                    //HANDLES: Ending the current tag
                    if(IsClosingTag(TagStart, html))
                    {
                        if (html.Substring(TagStart, (TagEnd + 1) - TagStart) == "</" + currentTag.TagType + ">") //If this is the end of the current tag
                        {
                            currentTag.Tag += "</" + currentTag.TagType + ">";
                            html = html.Remove(TagStart, (TagEnd + 1) - TagStart);  //Remove the closing tag

                            TagStart = html.IndexOf("<");   //Gets next tag start
                            if (TagStart != -1)
                            {
                                if (IsClosingTag(TagStart, html))  //If next tag is a closing tag
                                    MoveToNextTag(currentTag.Parent, html, false);
                                else
                                    MoveToNextTag(currentTag.Parent, html, true);
                            }
                            //WILL END IF: No more tags to parse
                        }
                        else
                        {
                            throw new Exception("Wrong end tag for: " + currentTag.TagType);
                        }
                    }
                    else   //HANDLES: Starting a NEW child tag
                    {
                        MoveToNextTag(currentTag, html, true);  //This is a new tag: Create new tag element with parent and re-run parse
                    }
                }
            }
            return "HTML Validated successfully!";
            //***Currently returns as many times as there are Html tags, need to stop after first return***
        }

        private bool IsClosingTag(int TagStart, string html)
        {
            if (html[TagStart + 1].ToString() == "/")
                return true;
            else
                return false;
        }

        private void MoveToNextTag(HtmlElement currentTag, string html, bool isNewTag)
        {
            if (IsAnotherTag(html))   //If theres another HTML element
            {
                if (currentTag.Parent == null && isNewTag == false)   //If the current element has no parent (i.e current element is 'Html' tag)
                    if(html == "</html>")   //HANDLES: the final closing html tag
                        currentTag.Tag += html;
                    else
                        throw new Exception("There is an uneven amount of open and close tags");
                else
                {
                    if (isNewTag == false)   //Not a new tag
                        Parse(currentTag, html); //Make parent element new current element
                    else   //New tag
                    {
                        HtmlElement NewTag = new HtmlElement    //Create new tag and set parent to Current
                        {
                            Parent = currentTag
                        };

                        currentTag.Children.Add(NewTag);    //Add new tag to current children
                        Parse(NewTag, html);
                    }
                }
            }
            //WILL END: if no more new tags
        }
        private bool IsAnotherTag(string html)  //Looks for another '<' character
        {
            int NextTag = html.IndexOf("<");

            if (!NextTag.Equals(null))   //If theres another HTML element
                return true;
            else
                return false;
        }

        private string GetTagType(string html)  //Gets the string directly after the '<' character or 'TagType'
        {
            int TagStart = html.IndexOf("<");
            int TagEnd = 0;

            int CloseBracket = html.IndexOf(">");
            int Space = html.IndexOf(" ");

            if (Space == -1)
                TagEnd = CloseBracket;
            else if (CloseBracket == -1)
                TagEnd = Space;
            else if (Space < CloseBracket)
                TagEnd = Space;
            else if (CloseBracket < Space)
                TagEnd = CloseBracket;
            else
                throw new Exception("Tag structure error");

            string TagType = html.Substring(TagStart + 1, (TagEnd - 1) - TagStart);

            //ValidateTagType(TagType);   //Validate it's a valid html tag
            return TagType;
        }
        private void ValidateTagType()
        {
            //TODO
            //Check 

        }
    }
}
