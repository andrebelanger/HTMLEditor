using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using System.Xml;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Folding;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;

namespace HtmlEditor.CodeEditors
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

		private readonly FoldingManager _foldingManager;
		private readonly AbstractFoldingStrategy _folding;

		public AvalonEditor()
		{
			_foldingManager = FoldingManager.Install(TextArea);
			_folding = new XmlFoldingStrategy();

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

			Task.Factory.StartNew(UpdateLoop);
		}

		private async void UpdateLoop()
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
	}
}
