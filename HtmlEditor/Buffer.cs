using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HtmlEditor
{
	/// <summary>
	/// A memory-backed text buffer
	/// </summary>
    public sealed class Buffer
    {
		/// <summary>
		/// The lines of text in the buffer
		/// </summary>
		/// <remarks>
		/// In the interest of efficiency, it's probably not wise to make individual
		/// character modifications on a string, so whatever consumes this object
		/// should try to make changes to a single line all at once
		/// </remarks>
		public IList<string> Lines { get; private set; }

		public Buffer()
		{
			Lines = new List<string>();
		}
    }
}
