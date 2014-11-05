using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using HtmlEditor.Parser;

namespace HtmlEditor.CodeEditors.PlainEditor
{
	/// <summary>
	/// Implements an editor that meets bare functional requirements
	/// </summary>
	public class PlainEditor : RichTextBox, ICodeEditor
	{
		private bool _reformatting;
		private double _sizeOfTab;
		private Dictionary<Paragraph, List<Paragraph>> _collapsedParas;

		public Stack<FlowDocument> undoStack = new Stack<FlowDocument>();
		public Stack<FlowDocument> redoStack = new Stack<FlowDocument>();

		/// <summary>
		/// Gets or sets a value indicating whether word wrap is enabled.
		/// </summary>
		/// <value>
		///   <c>true</c> if word wrap; otherwise, <c>false</c>.
		/// </value>
		public bool WordWrap
		{
			get { return !Document.PageWidth.Equals(double.MaxValue); }
			set { Document.PageWidth = value ? Width : 1000000d; } // 1000000 is the max a FlowDoc supports (as found in the decompiled assembly)
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

		public bool IsDirty { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="PlainEditor"/> class.
		/// </summary>
		public PlainEditor()
		{
			AcceptsReturn = AcceptsTab = true;

			AutoIndent = true;
			AutoIndentAmount = 1;

			// By default, Paragraphs have a 10-px border, so let's nuke that
			var pStyle = new Style(typeof(Paragraph));
			pStyle.Setters.Add(new Setter(Block.MarginProperty, new Thickness(0)));
			Resources.Add(typeof(Paragraph), pStyle);

			// Re-initialize the document so the above style is applied
			Document = new FlowDocument();

			_sizeOfTab = GetTabSize();

			_collapsedParas = new Dictionary<Paragraph, List<Paragraph>>();

			// Initialiaze PlainEditor Specific ContextMenu
			this.ContextMenu = new ContextMenu();
			var mi = new MenuItem();
			mi.Header = "Collapse Element";
			mi.Click += new RoutedEventHandler(CollapseParagraph);
			this.ContextMenu.Items.Add(mi);
		}

		/// <summary>
		/// Loads the specified lines into the editor.
		/// </summary>
		/// <param name="lines">The lines.</param>
		public void Load(IEnumerable<string> lines)
		{
			var newDoc = new FlowDocument();

			_sizeOfTab = GetTabSize();

			foreach (var line in lines)
			{
				var tabs = line.TakeWhile(c => c == '\t').Count();

				var p = new Paragraph(new Run(line.Substring(tabs)))
				{
					Margin = new Thickness(tabs * _sizeOfTab, 0, 0, 0)
				};

				newDoc.Blocks.Add(p);
			}

			Document = newDoc;
		}

		/// <summary>
		/// Returns the current lines in the editor
		/// </summary>
		/// <returns>
		/// The current lines
		/// </returns>
		public IEnumerable<string> Save()
		{
			var firstPara = Document.Blocks.OfType<Paragraph>().FirstOrDefault();

			if (firstPara == null)
				return new string[0];

			return GetDocumentParagraphs(firstPara)
				.Select(p => new string('\t', (int)(p.Margin.Left / _sizeOfTab)) + new TextRange(p.ContentStart, p.ContentEnd).Text);
		}

		/// <summary>
		/// Gets the content paragraphs for the document. Replaces collapsed paras
		/// with the content they represent.
		/// </summary>
		/// <param name="start">The start.</param>
		/// <returns></returns>
		private IEnumerable<Paragraph> GetDocumentParagraphs(Paragraph start)
		{
			do
			{
				if (_collapsedParas.ContainsKey(start)) // This is a placeholder. Let's look up the actual text
					foreach (var collapsed in _collapsedParas[start])
						foreach (var p in GetDocumentParagraphs(collapsed))
							yield return p;
				else
					yield return start; // Normal paragraph

			} while ((start = start.NextBlock as Paragraph) != null);
		}

		/// <summary>
		/// Invoked whenever the effective value of any dependency property on this <see cref="T:System.Windows.FrameworkElement" />
		/// has been updated. The specific dependency property that changed is reported in the arguments parameter. Overrides
		/// <see cref="M:System.Windows.DependencyObject.OnPropertyChanged(System.Windows.DependencyPropertyChangedEventArgs)" />.
		/// </summary>
		/// <param name="e">The event data that describes the property that changed, as well as old and new values.</param>
		protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
		{
			// When one of these triggers, we need to recalculate the size of our tabs
			if (e.Property == Control.FontFamilyProperty || e.Property == Control.FontSizeProperty ||
				e.Property == Control.FontStretchProperty || e.Property == Control.FontStyleProperty ||
				e.Property == Control.FontWeightProperty || e.Property == FrameworkElement.StyleProperty ||
				e.Property == Control.TemplateProperty)
			{
				var newTabSize = GetTabSize();

				foreach (var b in Document.Blocks.OfType<Paragraph>())
				{
					b.Margin = new Thickness((b.Margin.Left / _sizeOfTab) * newTabSize,
						b.Margin.Top,
						b.Margin.Right,
						b.Margin.Bottom);
				}

				_sizeOfTab = newTabSize;
			}

			base.OnPropertyChanged(e);
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
			if (Selection.IsEmpty)
			{
				var para = CaretPosition.Paragraph;

				if (para != null && para.ContentStart.GetOffsetToPosition(CaretPosition) == 1) // If we're at the start of the line
				{
					var shift = Keyboard.Modifiers.HasFlag(ModifierKeys.Shift);

					if (e.Key == Key.Tab && !shift)
					{
						IndentBlock(para, 1);
						e.Handled = true;
					}
					else if ((e.Key == Key.Back && para.Margin.Left > 0) || (e.Key == Key.Tab && shift))
					{
						IndentBlock(para, -1);
						e.Handled = true;
					}
				}
			}
			else
			{
				foreach (var p in GetParagraphsBetweenPositions(Selection.Start, Selection.End))
				{
					var shift = Keyboard.Modifiers.HasFlag(ModifierKeys.Shift);

					if (e.Key == Key.Tab && !shift)
					{
						IndentBlock(p, 1);
						e.Handled = true;
					}
					else if (e.Key == Key.Tab && shift)
					{
						IndentBlock(p, -1);
						e.Handled = true;
					}

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

			var changedParas = new List<Paragraph>();

			foreach (var change in e.Changes.Where(c => c.AddedLength > 0))
			{
				var start = Document.ContentStart.GetPositionAtOffset(change.Offset);
				var end = Document.ContentStart.GetPositionAtOffset(change.Offset + change.AddedLength);

				if (start == null || end == null)
					continue;

				changedParas.AddRange(GetParagraphsBetweenPositions(start, end));
			}

			//Undo/Redo
			undoStack.Push(Document);

			// Now let's reformat a subset of them
			// We'll do our filtering with LINQ's WHERE clause
			ReformatParagraphs(changedParas
				.Distinct()
				.Where(para => (0 <= CaretPosition.CompareTo(para.ContentStart) && CaretPosition.CompareTo(para.ContentEnd) <= 0)) // Caret is in the para
				.Where(para => string.IsNullOrEmpty(new TextRange(para.ContentStart, para.ContentEnd).Text)) // And it's an empty (ie, new) paragraph
				);

			IsDirty = true;

			base.OnTextChanged(e);
		}

		private static IEnumerable<Paragraph> GetParagraphsBetweenPositions(TextPointer start, TextPointer end)
		{
			var changedParas = new HashSet<Paragraph>();
			var para = start.Paragraph ?? start.GetAdjacentElement(LogicalDirection.Forward) as Paragraph;

			while (para != null && para.ContentStart.CompareTo(end) < 0)
			{
				changedParas.Add(para);
				para = para.NextBlock as Paragraph;
			}

			return changedParas;
		}

		public void IndentLine()
		{
			IndentBlock(CaretPosition.Paragraph, 1);
		}

		public void IndentSelection()
		{
			foreach (var p in GetParagraphsBetweenPositions(Selection.Start, Selection.End))
			{
				IndentBlock(p, 1);
			}
		}

		public void IndentBuffer()
		{
			foreach (var p in Document.Blocks)
			{
				IndentBlock(p, 1);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void IndentBlock(Block para, int amount)
		{
			para.Margin = new Thickness(Math.Max(para.Margin.Left + _sizeOfTab * amount, 0), para.Margin.Top, para.Margin.Right, para.Margin.Bottom);
		}

		public void Insert(IEnumerable<string> lines)
		{
			var changed = new List<Paragraph>();

			using (var e = lines.GetEnumerator())
			{
				if (!e.MoveNext())
					return;

				var p = CaretPosition.Paragraph;

				var r = new Run(e.Current);
				p.Inlines.Add(r);
				changed.Add(p);

				while (e.MoveNext())
				{
					r = new Run(e.Current);
					var newP = new Paragraph(r);
					Document.Blocks.InsertAfter(p, newP);
					p = newP;
					changed.Add(p);
				}

				ReformatParagraphs(changed);

				CaretPosition = r.ContentEnd;
			}
		}


		/// <summary>
		/// Reformats (reflows and auto-indents) the paragraphs.
		/// </summary>
		/// <param name="paragraphs">The paragraphs.</param>
		protected void ReformatParagraphs(IEnumerable<Paragraph> paragraphs)
		{
			_reformatting = true;
			BeginChange();

			foreach (var para in paragraphs)
			{
				var prevPara = para.PreviousBlock as Paragraph;

				if (prevPara != null)
				{
					// for now, just set it equal to the preceeding
					para.Margin = prevPara.Margin;

					if (AutoIndent)
					{
						var autoDelta = AutoIndentAmount * _sizeOfTab * HtmlParser.CountUnclosedTags(new TextRange(prevPara.ContentStart, prevPara.ContentEnd).Text);

						if (autoDelta > 0)
							para.Margin = new Thickness(para.Margin.Left + autoDelta, para.Margin.Top, para.Margin.Right, para.Margin.Bottom);
					}
				}
			}

			EndChange();
			_reformatting = false;
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


		public List<HtmlObject> ParseHtml()
		{
			return HtmlParser.Parse(Save());
		}

		/// <summary>
		/// Collapses the selected HTML Element
		/// </summary>
		/// <param name="p">Selected paragraph containing beginning HTML element tag </param>
		public void CollapseParagraph(object sender, RoutedEventArgs e)
		{
			var p = CaretPosition.Paragraph;

			//Console.WriteLine(HtmlParser.GetOpeningTag(new TextRange(p.ContentStart, p.ContentEnd).Text));

			if (_collapsedParas.ContainsKey(p))
			{
				// TODO: Uncollapse
				var previous = p;
				foreach (var para in _collapsedParas[p])
				{
					Document.Blocks.InsertAfter(previous, para);
					previous = para;
				}

				// Remove Placeholder from Dictionary and Document
				_collapsedParas.Remove(p);
				Document.Blocks.Remove(p);
			}
			else
			{
				// create placeholder paragraph
				var tag = HtmlParser.GetOpeningTag(new TextRange(p.ContentStart, p.ContentEnd).Text);
				var placeholder = new Paragraph(new Run("<" + tag + ">...")
				{
					Foreground = Brushes.Gray
				});

				placeholder.Margin = p.Margin;

				_collapsedParas.Add(placeholder, new List<Paragraph>());


				Document.Blocks.InsertBefore(p, placeholder);

				var openTagCount = 0;

				var currentPara = p;
				do
				{
					// If this paragraph is a placeholder, we don't want it to contribute to the tag count!
					// Otherwise <div>... (collapsed) is treated as an unclosed div tag!!
					if (!_collapsedParas.ContainsKey(currentPara))
					{
						var line = new TextRange(currentPara.ContentStart, currentPara.ContentEnd).Text;
						openTagCount += HtmlParser.CountOpeningTags(tag, line);
						openTagCount -= HtmlParser.CountClosingTags(tag, line);
					}

					_collapsedParas[placeholder].Add(currentPara);

					var tempPara = currentPara;
					currentPara = currentPara.NextBlock as Paragraph;					
					Document.Blocks.Remove(tempPara);

				} while (openTagCount > 0 && currentPara != null);
			}
		}
	}
}
