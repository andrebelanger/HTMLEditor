using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HtmlEditor.CodeEditors
{
	/// <summary>
	/// Defines basic operations supported by all code editors
	/// </summary>
	public interface ICodeEditor
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
	}
}
