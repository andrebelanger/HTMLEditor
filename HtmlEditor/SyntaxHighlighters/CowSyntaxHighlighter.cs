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
	/// Cow formatter - italicizes "moo", bolds "mooo", italic-bolds "mooo(o)+", and colors "mooooo!" red
	/// </summary>
	public class CowSyntaxHighlighter : ISyntaxHighlighter
	{
		private static readonly SolidColorBrush RedBrush = new SolidColorBrush(Colors.Red);

		public Inline Highlight(string text)
		{
			if (text.Length == 0)
				return new Run();

			var ret = new Span(); // Container for all of our text

			var prevPos = 0;
			var pos = 0;

			while ((pos = text.IndexOf("moo", pos, StringComparison.OrdinalIgnoreCase)) != -1)
			{
				// If there was text between the last moo and this one
				// We need to add it
				if (prevPos != pos)
				{
					ret.Inlines.Add(new Run(text.Substring(prevPos, pos - prevPos)));
				}

				// Get the last 'o' in the moo
				var lastO = pos + 3;

				while (text.Length != lastO && (text[lastO] == 'O' || text[lastO] == 'o'))
					lastO++;

				lastO--; // We went one beyond

				// If there's an '!' after the o, special case
				if (text.Length > (lastO+1) && text[lastO+1] == '!')
				{
					lastO++;

					ret.Inlines.Add(new Span(new Run(text.Substring(pos, lastO - pos+1)) {Foreground = RedBrush}));
				}
				else
				{
					var run = new Run(text.Substring(pos, lastO - pos+1));

					switch (lastO - pos)
					{
						case 2: 
							ret.Inlines.Add(new Italic(run)); break;
						case 3:
							ret.Inlines.Add(new Bold(run)); break;
						default:
							ret.Inlines.Add(new Bold(new Italic(run))); break;
					}
				}

				pos = prevPos = (lastO+1); // We can skip the moo we just converted
			}

			pos = text.Length;

			// Grab any remaining text
			if (prevPos != pos)
			{
				ret.Inlines.Add(new Run(text.Substring(prevPos)));
			}

			return ret;
		}
	}
}
