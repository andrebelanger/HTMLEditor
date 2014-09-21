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
