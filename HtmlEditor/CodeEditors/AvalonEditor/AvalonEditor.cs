using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Threading;
using System.Xml;
using HtmlEditor.Parser;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Folding;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using ICSharpCode.AvalonEdit.Indentation;

namespace HtmlEditor.CodeEditors.AvalonEditor
{
	/// <summary>
	/// Adapts the AvalonEditor to the ICodeEditor interface
	/// </summary>
	public class AvalonEditor : TextEditor, ICodeEditor
	{
		/// <summary>
		/// Gets or sets a value indicating whether automatic indentation is enabled.
		/// </summary>
		/// <value>
		///   <c>true</c> if automatic indent; otherwise, <c>false</c>.
		/// </value>
		public bool AutoIndent
		{
			get { return TextArea.IndentationStrategy == _htmlIndent; }
			set { TextArea.IndentationStrategy = value ? _htmlIndent : _defaultIndent; }
		}

		/// <summary>
		/// Gets or sets the automatic indentation amount.
		/// </summary>
		/// <value>
		/// The automatic indent amount.
		/// </value>
		/// <remarks>
		/// This value is how much "nested" items are indented.
		/// </remarks>
		public int AutoIndentAmount
		{
			get { return _htmlIndent.AutoIndentAmount; }
			set { _htmlIndent.AutoIndentAmount = value; }
		}

		public bool IsDirty { get; set; }

		public void Insert(IEnumerable<string> lines)
		{
			var start = CaretOffset;

			TextArea.Document.Insert(start, string.Join(Environment.NewLine, lines));

			TextArea.IndentationStrategy.IndentLines(Document, Document.GetLineByOffset(start).LineNumber,
				Document.GetLineByOffset(CaretOffset).LineNumber);
		}

		private readonly FoldingManager _foldingManager;
		private readonly AbstractFoldingStrategy _folding;

		private readonly HtmlIndentationStrategy _htmlIndent;
		private readonly IIndentationStrategy _defaultIndent;

		/// <summary>
		/// Initializes a new instance of the <see cref="AvalonEditor"/> class.
		/// </summary>
		/// <exception cref="System.InvalidOperationException">Failed to load syntax definition</exception>
		public AvalonEditor()
		{
			_foldingManager = FoldingManager.Install(TextArea);
			_folding = new XmlFoldingStrategy();

			_htmlIndent = new HtmlIndentationStrategy();
			_defaultIndent = new DefaultIndentationStrategy();

			AutoIndent = true;
			AutoIndentAmount = 1;

			// Load our HTML highlighting
			using (var s = typeof(AvalonEditor).Assembly.GetManifestResourceStream(typeof(AvalonEditor), "HtmlHighlighting.xml"))
			{
				if (s == null)
					throw new InvalidOperationException("Failed to load syntax definition");
				using (var r = XmlReader.Create(s))
				{
					var highlightingDefinition = HighlightingLoader.Load(r, HighlightingManager.Instance);

					SyntaxHighlighting = highlightingDefinition;
				}
			}

			Task.Factory.StartNew(FoldingUpdateLoop);
		}

		/// <summary>
		/// Updates code foldings every 2 seconds
		/// </summary>
		private async void FoldingUpdateLoop()
		{
			while (true)
			{
				Dispatcher.Invoke(() => _folding.UpdateFoldings(_foldingManager, Document), DispatcherPriority.Background);

				await Task.Delay(2000); // Wait 2 sec.
				// Note that this type of delay is cheap because an await on a Task
				// yields the thread instead of blocking it.
			}
		}

		/// <summary>
		/// Loads the specified lines into the editor.
		/// </summary>
		/// <param name="lines">The lines.</param>
		public void Load(IEnumerable<string> lines)
		{
			Document.Text = string.Join(Environment.NewLine, lines);
		}

		/// <summary>
		/// Returns the current lines in the editor
		/// </summary>
		/// <returns>
		/// The current lines
		/// </returns>
		public IEnumerable<string> Save()
		{
			return Document.Text.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
		}

		protected override void OnTextChanged(EventArgs e)
		{
			IsDirty = true;

			base.OnTextChanged(e);
		}

		public List<HtmlObject> ParseHtml()
		{
			return HtmlParser.Parse(Save());
		}
	}
}
