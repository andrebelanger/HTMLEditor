using System.Collections.Generic;
using HtmlEditor.Parser;
using System.Windows.Controls;

namespace HtmlEditor.CodeEditors
{
	/// <summary>
	/// Defines basic operations supported by all code editors
	/// </summary>
	public interface ICodeEditor : System.Windows.IInputElement
	{
		/// <summary>
		/// Gets or sets a value indicating whether word wrap is enabled.
		/// </summary>
		/// <value>
		///   <c>true</c> if word wrap; otherwise, <c>false</c>.
		/// </value>
		bool WordWrap { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether automatic indentation is enabled.
		/// </summary>
		/// <value>
		///   <c>true</c> if automatic indent; otherwise, <c>false</c>.
		/// </value>
		bool AutoIndent { get; set; }

		/// <summary>
		/// Gets or sets the automatic indentation amount.
		/// </summary>
		/// <remarks>
		/// This value is how much "nested" items are indented.
		/// </remarks>
		/// <value>
		/// The automatic indent amount.
		/// </value>
		int AutoIndentAmount { get; set; }

		/// <summary>
		/// Loads the specified lines into the editor.
		/// </summary>
		/// <param name="lines">The lines.</param>
		void Load(IEnumerable<string> lines);

		/// <summary>
		/// Returns the current lines in the editor
		/// </summary>
		/// <returns>The current lines</returns>
		IEnumerable<string> Save();

		/// <summary>
		/// Gets or sets a value indicating whether this instance is dirty.
		/// </summary>
		/// <value>
		///   <c>true</c> if this instance is dirty; otherwise, <c>false</c>.
		/// </value>
		bool IsDirty { get; set; }

		/*void IndentLine();

		void IndentSelection();

		void IndentBuffer();*/

		/// <summary>
		/// Inserts the specified lines.
		/// </summary>
		/// <param name="lines">The lines.</param>
		void Insert(IEnumerable<string> lines);

		/// <summary>
		/// Parses the buffer into an HTML tree.
		/// </summary>
		/// <returns>A list of root-level objects</returns>
		List<HtmlObject> ParseHtml();
	}
}
