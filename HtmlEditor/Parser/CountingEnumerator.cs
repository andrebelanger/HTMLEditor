using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HtmlEditor.Parser
{
	/// <summary>
	/// Simple wrapper around an IEnumerator that tracks the index of the current item.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class CountingEnumerator<T> : IEnumerator<T>
	{
		private readonly IEnumerator<T> _iter;

		/// <summary>
		/// Gets the index.
		/// </summary>
		/// <value>
		/// The index.
		/// </value>
		public int Index { get; private set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="CountingEnumerator{T}"/> class.
		/// </summary>
		/// <param name="iter">The iter.</param>
		public CountingEnumerator(IEnumerator<T> iter)
		{
			_iter = iter;
			Index = -1;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="CountingEnumerator{T}"/> class.
		/// </summary>
		/// <param name="collection">The collection to iterate over.</param>
		public CountingEnumerator(IEnumerable<T> collection)
			: this(collection.GetEnumerator())
		{

		}

		/// <summary>
		/// Gets the current.
		/// </summary>
		/// <value>
		/// The current.
		/// </value>
		public T Current { get { return _iter.Current; } }

		public void Dispose()
		{
			_iter.Dispose();
		}

		/// <summary>
		/// Gets the current.
		/// </summary>
		/// <value>
		/// The current.
		/// </value>
		object System.Collections.IEnumerator.Current
		{
			get { return ((System.Collections.IEnumerator)_iter).Current; }
		}

		/// <summary>
		/// Moves the next.
		/// </summary>
		/// <returns></returns>
		public bool MoveNext()
		{
			if (_iter.MoveNext())
			{
				Index++;
				return true;
			}

			return false;
		}

		/// <summary>
		/// Resets this instance.
		/// </summary>
		public void Reset()
		{
			_iter.Reset();
			Index = -1;
		}
	}

	/// <summary>
	/// Extension methods relating to counting iterator
	/// </summary>
	public static class IterExt
	{
		public static CountingEnumerator<T> GetCountingEnumerator<T>(this IEnumerator<T> iter)
		{
			return new CountingEnumerator<T>(iter);
		}

		public static CountingEnumerator<T> GetCountingEnumerator<T>(this IEnumerable<T> collection)
		{
			return new CountingEnumerator<T>(collection);
		}
	}
}
