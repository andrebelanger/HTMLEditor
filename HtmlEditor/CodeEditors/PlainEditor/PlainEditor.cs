using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace HtmlEditor.CodeEditors.PlainEditor
{
	/// <summary>
	/// Implements an editor that meets bare functional requirements
	/// </summary>
	public class PlainEditor : RichTextBox, ICodeEditor
	{
		private bool _reformatting;
		private double _sizeOfTab;

		/// <summary>
		/// Gets or sets a value indicating whether word wrap is enabled.
		/// </summary>
		/// <value>
		///   <c>true</c> if word wrap; otherwise, <c>false</c>.
		/// </value>
		public bool WordWrap
		{
			get { return !Document.PageWidth.Equals(double.MaxValue); }
			set { Document.PageWidth = value ? ViewportWidth : double.MaxValue; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether automatic indentation is enabled.
		/// </summary>
		/// <value>
		///   <c>true</c> if automatic indent; otherwise, <c>false</c>.
		/// </value>
		public bool AutoIndent { get; set; }

		/// <summary>
		/// Gets or sets the automatic indentation amount in terms of \t characters.
		/// </summary>
		/// <value>
		/// The automatic indent amount.
		/// </value>
		/// <remarks>
		/// This value is how much "nested" items are indented. Default 1
		/// </remarks>
		public int AutoIndentAmount { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="PlainEditor"/> class.
		/// </summary>
		public PlainEditor()
		{
			AcceptsReturn = AcceptsTab = true;

			AutoIndent = true;

			// By default, Paragraphs have a 10-px border, so let's nuke that
			var pStyle = new Style(typeof (Paragraph));
			pStyle.Setters.Add(new Setter(Block.MarginProperty, new Thickness(0)));
			Resources.Add(typeof (Paragraph), pStyle);

			// Re-initialize the document so the above style is applied
			Document = new FlowDocument();

			_sizeOfTab = GetTabSize();
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
		/// We implement tabbing here by setting the margin property
		/// </remarks>
		/// <param name="e">The event data.</param>
		protected override void OnPreviewKeyDown(KeyEventArgs e)
		{
			var para = CaretPosition.Paragraph;

			if (para != null && para.ContentStart.GetOffsetToPosition(CaretPosition) == 1) // If we're at the start of the line
			{
				var shift = Keyboard.Modifiers.HasFlag(ModifierKeys.Shift);

				if (e.Key == Key.Tab && !shift)
				{
					para.Margin = new Thickness(para.Margin.Left + _sizeOfTab, para.Margin.Top, para.Margin.Right, para.Margin.Bottom);
					e.Handled = true;
				}
				else if ((e.Key == Key.Back && para.Margin.Left > 0) || (e.Key == Key.Tab && shift))
				{
					para.Margin = new Thickness(para.Margin.Left - _sizeOfTab, para.Margin.Top, para.Margin.Right, para.Margin.Bottom);
					e.Handled = true;
				}
			}

			base.OnPreviewKeyDown(e);
		}

		/*
		 *****************************************
		 * Old version of above kept for posterity
		 * The change to using the Paragraph Margin
		 * has made this unneeded
		 *****************************************
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
		 * */

		/// <summary>
		/// Is called when content in this editing control changes.
		/// </summary>
		/// <param name="e">The arguments that are associated with the <see cref="E:System.Windows.Controls.Primitives.TextBoxBase.TextChanged" /> event.</param>
		protected override void OnTextChanged(TextChangedEventArgs e)
		{
			if (_reformatting)
				return;

			var changedParas = new HashSet<Paragraph>();

			foreach (var change in e.Changes.Where(c => c.AddedLength > 0))
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


			// Now let's reformat a subset of them
			// We'll do our filtering with LINQ's WHERE clause
			ReformatParagraphs(changedParas
				.Where(para => (0 <= CaretPosition.CompareTo(para.ContentStart) && CaretPosition.CompareTo(para.ContentEnd) <= 0)) // Caret is in the para
				.Where(para => string.IsNullOrEmpty(new TextRange(para.ContentStart, para.ContentEnd).Text)) // And it's an empty (ie, new) paragraph
				);

			base.OnTextChanged(e);
		}

		/// <summary>
		/// Reformats (reflows and auto-indents) the paragraphs.
		/// </summary>
		/// <param name="paragraphs">The paragraphs.</param>
		protected void ReformatParagraphs(IEnumerable<Paragraph> paragraphs)
		{
			foreach (var para in paragraphs)
			{
				var prevPara = para.PreviousBlock as Paragraph;

				if (prevPara != null)
				{
					// for now, just set it equal to the preceeding
					para.Margin = prevPara.Margin;

					if (AutoIndent)
					{
						var autoDelta = AutoIndentAmount * _sizeOfTab;
						// TODO: AUTO-INDENT by autoDelta * number of indents
					}
				}
			}
		}

		/// <summary>
		/// Gets the size of a single tab character under the current formatting
		/// </summary>
		/// <returns></returns>
		private double GetTabSize()
		{
			return new FormattedText("\t", CultureInfo.CurrentUICulture, FlowDirection.LeftToRight,
				new Typeface(FontFamily, FontStyle, FontWeight, FontStretch), FontSize, Brushes.Black).Width;
		}
	}
}
