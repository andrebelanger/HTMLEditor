using System;
using System.Collections.Generic;
using System.IO;
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

		/// <summary>
		/// Gets or sets the filename the buffer is associated with.
		/// </summary>
		/// <value>
		/// The filename.
		/// </value>
		public string Filename { get; set; }

		/// <summary>
		/// Initializes a new empty instance of the <see cref="Buffer"/> class.
		/// </summary>
		public Buffer()
		{
			Lines = new List<string>();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Buffer"/> class from the given lines.
		/// </summary>
		/// <param name="lines">The initial lines.</param>
		public Buffer(IEnumerable<string> lines)
		{
			Lines = new List<string>(lines);
		}

		/// <summary>
		/// Saves the lines in this buffer to the given <see cref="filename"/>.
		/// 
		/// If the file already exists, it is overwritten.
		/// </summary>
		/// <remarks>
		/// Uses UTF8 encoding and the default platform line ending.
		/// 
		/// It is up to the caller to verify existing files etc
		/// </remarks>
		/// <param name="filename">The file to save to.</param>
		public void Save(string filename)
		{
			File.WriteAllLines(filename, Lines);
		}

		/// <summary>
		/// Saves this buffer by calling the <see cref="Save(string)"/> method with the set filename.
		/// </summary>
		/// <exception cref="System.InvalidOperationException">Filename must be set</exception>
		public void Save()
		{
			if (Filename == null)
				throw new InvalidOperationException("Filename must be set");

			Save(Filename);
		}

		/// <summary>
		/// Creates a new buffer from the contents of the specified file.
		/// </summary>
		/// <remarks>
		/// Uses UTF8 and the dfault platform line ending.
		/// 
		/// It is up to the caller to verify files exist etc.
		/// </remarks>
		/// <param name="filename">The file to load</param>
		/// <returns>The constructed buffer.</returns>
		public static Buffer Load(string filename)
		{
			return new Buffer(File.ReadAllLines(filename)) {Filename = filename};
		}
    }
}
