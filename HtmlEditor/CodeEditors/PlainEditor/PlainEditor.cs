using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;

namespace HtmlEditor.CodeEditors.PlainEditor
{
	/// <summary>
	/// Implements an editor that meets bare functional requirements
	/// </summary>
	public class PlainEditor : RichTextBox, ICodeEditor
	{
		private bool _reformatting;

		/// <summary>
		/// Gets or sets a value indicating whether word wrap is enabled.
		/// </summary>
		/// <value>
		///   <c>true</c> if word wrap; otherwise, <c>false</c>.
		/// </value>
		public bool WordWrap { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether automatic indentation is enabled.
		/// </summary>
		/// <value>
		///   <c>true</c> if automatic indent; otherwise, <c>false</c>.
		/// </value>
		public bool AutoIndent { get; set; }

		/// <summary>
		/// Gets or sets the automatic indentation amount.
		/// </summary>
		/// <value>
		/// The automatic indent amount.
		/// </value>
		/// <remarks>
		/// This value is how much "nested" items are indented.
		/// </remarks>
		public int AutoIndentAmount { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="PlainEditor"/> class.
		/// </summary>
		public PlainEditor()
		{
			AcceptsReturn = AcceptsTab = true;

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

		/// <summary>
		/// Called when the <see cref="E:System.Windows.UIElement.KeyDown" /> occurs.
		/// </summary>
		/// <remarks>
		/// By default, the RichTextBox sets the TextIndent property when the tab key is pressed,
		/// but that's a little awkward in terms of editing (You can't select any whitespace before the line)
		/// so we'll override that and actually insert the \t.
		/// </remarks>
		/// <param name="e">The event data.</param>
		protected override void OnPreviewKeyDown(KeyEventArgs e)
		{
			if (e.Key == Key.Tab)
			{
				Selection.Text = string.Empty;

				var pos = CaretPosition.GetPositionAtOffset(0, LogicalDirection.Forward);
				CaretPosition.InsertTextInRun("\t");
				CaretPosition = pos;

				e.Handled = true;
			}

			base.OnPreviewKeyDown(e);
		}

		/// <summary>
		/// Is called when content in this editing control changes.
		/// </summary>
		/// <param name="e">The arguments that are associated with the <see cref="E:System.Windows.Controls.Primitives.TextBoxBase.TextChanged" /> event.</param>
		protected override void OnTextChanged(TextChangedEventArgs e)
		{
			if (_reformatting)
				return;

			var changedParas = new HashSet<Paragraph>();

			foreach (var change in e.Changes)
			{
				var start = Document.ContentStart.GetPositionAtOffset(change.Offset);
				var end = Document.ContentStart.GetPositionAtOffset(change.Offset + change.AddedLength);

				if (start == null || end == null)
					continue;

				var para = start.Paragraph ?? start.GetAdjacentElement(LogicalDirection.Forward) as Paragraph;

				while (para != null && para.ContentStart.CompareTo(end) < 0)
				{
					changedParas.Add(para);
					para = para.NextBlock as Paragraph;
				}
			}

			ReformatParagraphs(changedParas);

			base.OnTextChanged(e);
		}

		/// <summary>
		/// Reformats (reflows and auto-indents) the paragraphs.
		/// </summary>
		/// <param name="paragraphs">The paragraphs.</param>
		protected void ReformatParagraphs(IEnumerable<Paragraph> paragraphs)
		{
			
		}
	}
}
