using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace HtmlEditor.Parser
{
	public static class HtmlParser
	{
		/// <summary>
		/// Parses the specified HTML.
		/// </summary>
		/// <param name="html">The HTML.</param>
		/// <returns>A list of root level objects</returns>
		/// <exception cref="System.IO.InvalidDataException">Error on line  + e.Index + :  + ex.Message</exception>
		public static List<HtmlObject> Parse(IEnumerable<string> html)
		{
			using (var e = html.GetCountingEnumerator())
			{
				try
				{
					return ParseInternal(e);
				}
				catch (InvalidDataException ex)
				{
					throw new InvalidDataException("Error on line " + (e.Index + 1) + ": " + ex.Message, ex);
				}
			}
		}

		/// <summary>
		/// Actually does the parsing
		/// </summary>
		/// <param name="lines">The lines.</param>
		/// <returns></returns>
		/// <exception cref="System.IO.InvalidDataException">
		/// Missing element end.
		/// or
		/// Extra end tag
		/// or
		/// or
		/// Unclosed tags
		/// </exception>
		private static List<HtmlObject> ParseInternal(IEnumerator<string> lines)
		{
			// TODO: Refactor me!!

			var roots = new List<HtmlObject>();
			var tagStack = new Stack<HtmlElement>();

			while (lines.MoveNext())
			{
				var index = 0;

				while (lines.Current != null && (index = lines.Current.IndexOf('<', index)) != -1) // All html directives start with <
				{
					var directiveType = IdentifyDirective(lines.Current, index);

					if (directiveType == HtmlDirectives.Comment)
					{
						do // Discard all text until the end of comment marker
						{
							index = lines.Current.IndexOf("-->", StringComparison.OrdinalIgnoreCase);
                            if (index != -1)
                            {
                                index += 3;
                                break;
                            }
						} while (lines.MoveNext());
					}
					else
					{
						var endIndex = lines.Current.IndexOf('>', index);
						if (endIndex == -1)
							throw new InvalidDataException("Missing element end.");

						var directiveText = lines.Current.Substring(index, endIndex - index+1);

						switch (directiveType)
						{
							case HtmlDirectives.Doctype:
								var dt = HtmlDoctype.Parse(directiveText);
								if (tagStack.Any())
									tagStack.Peek().Children.Add(dt);
								else
									roots.Add(dt);
								break;

							case HtmlDirectives.ElementStart:
								var ele = HtmlElement.Parse(directiveText);
								if (tagStack.Any())
									tagStack.Peek().Children.Add(ele);
								else
									roots.Add(ele);

								if (!directiveText.EndsWith("/>"))
									tagStack.Push(ele);
								break;

							case HtmlDirectives.ElementEnd:
								if (!tagStack.Any())
									throw new InvalidDataException("Extra end tag");

								if (directiveText.Trim(' ', '<', '>', '/').Equals(tagStack.Peek().Tag, StringComparison.OrdinalIgnoreCase))
									tagStack.Pop();
								else
									throw new InvalidDataException(string.Format("Unexpected end tag. Got: {0}. Expected: </{1}>", directiveText,
										tagStack.Peek().Tag));
								break;

						}

						index = endIndex;
					}
				}
			}

			if (tagStack.Any())
				throw new InvalidDataException("Unclosed tags");

			return roots;
		}

		/// <summary>
		/// Counts the unclosed tags on a line.
		/// </summary>
		/// <param name="line">The line.</param>
		/// <returns></returns>
		public static int CountUnclosedTags(string line)
		{
			return Regex.Matches(line, "<[^/]").Count - Regex.Matches(line, "(</)|(/>)").Count;
		}

		/// <summary>
		/// ERROR: Attempted to parse HTML with regular expression; system returned Cthulhu.
		/// </summary>
		private static readonly Dictionary<Regex, HtmlDirectives> Identifiers = new Dictionary<Regex, HtmlDirectives>()
		{
			// The \G anchor in .Net matches the previous match with Match objects or
			// the start of the match attempt with IsMatch. This is what we're interested in.
			{ new Regex(@"\G<!DOCTYPE .*?>", RegexOptions.IgnoreCase), HtmlDirectives.Doctype },
			{ new Regex(@"\G<!--"), HtmlDirectives.Comment },
			{ new Regex(@"\G<\w+([^>]*)?>"), HtmlDirectives.ElementStart },
			{ new Regex(@"\G</\w+ *?>"), HtmlDirectives.ElementEnd }
		};

		/// <summary>
		/// Identifies the html directive at the given index.
		/// </summary>
		/// <param name="line">The line.</param>
		/// <param name="index">The index.</param>
		/// <returns></returns>
		/// <exception cref="System.IO.InvalidDataException">Invalid directive</exception>
		private static HtmlDirectives IdentifyDirective(string line, int index)
		{
			var directive = Identifiers.Keys.FirstOrDefault(x => x.IsMatch(line, index));
			if (directive == null)
				throw new InvalidDataException("Invalid directive");

			return Identifiers[directive];
		}

		enum HtmlDirectives
		{
			ElementStart,
			ElementEnd,
			Doctype,
			Comment,
			Text
		}
	}
}
