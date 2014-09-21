using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace HtmlEditor.CodeEditors.PlainEditor
{
	/// <summary>
	/// Implements an editor that meets bare functional requirements
	/// </summary>
	public class PlainEditor : RichTextBox, ICodeEditor
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="PlainEditor"/> class.
		/// </summary>
		public PlainEditor()
		{
			// By default, Paragraphs have a 10-px border, so let's nuke that
			var pStyle = new Style(typeof (Paragraph));
			pStyle.Setters.Add(new Setter(Block.MarginProperty, new Thickness(0)));
			Resources.Add(typeof (Paragraph), pStyle);

			// Re-initialize the document so the above style is applied
			Document = new FlowDocument();
		}

		/// <summary>
		/// Loads the specified lines into the editor.
		/// </summary>
		/// <param name="lines">The lines.</param>
		public void Load(IEnumerable<string> lines)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Returns the current lines in the editor
		/// </summary>
		/// <returns>
		/// The current lines
		/// </returns>
		public IEnumerable<string> Save()
		{
			throw new NotImplementedException();
		}
	}
}
