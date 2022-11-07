using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ads_lab_1
{
	public class LazyString :
		IEnumerable<char>,
		IEquatable<LazyString>,
		IEquatable<string>,
		IComparable<LazyString>,
		IComparable<string>
	{
		protected readonly IEnumerable<char> _originString;
		protected readonly int _startIndex;
		protected readonly int _count;
		public int Length => _count;

		protected readonly LazyString _inserted = null!;
		protected readonly int _insertedIndex = -1;

		protected readonly int _removedStart = -1;
		protected readonly int _removedCount = int.MaxValue;

		protected int getMaxIndex() => Math.Max(0, 
			Math.Max(0,_count-1)
			+ (_insertedIndex == -1 ? 0 : _inserted.Length)
			- (_removedStart == -1 ? 0 : _removedCount));
		protected bool failsCheck() => (_startIndex < 0
				|| _count < 0);

		public LazyString(LazyString s, int startIndex, int count)
		{
			_originString = s;
			_startIndex = startIndex;
			_count = count;

			if (failsCheck() || (s.Length - startIndex) < count)
				throw new ArgumentOutOfRangeException();
		}
		public LazyString(string s, int startIndex, int count)
		{
			_originString = s;
			_startIndex = startIndex;
			_count = count;

			if (failsCheck() || (s.Length-startIndex)<count)
				throw new ArgumentOutOfRangeException();
		}
		protected LazyString(LazyString s, int startIndex, int count, LazyString inserted, int insertedIndex)
		{
			_inserted = inserted;
			_insertedIndex = insertedIndex;
			_originString = s;
			_startIndex = startIndex;
			_count = count;

			if (failsCheck() || (s.Length - startIndex + inserted.Length) < count)
				throw new ArgumentOutOfRangeException();
		}
		protected LazyString(LazyString s, int startIndex, int count, int removedStart, int removedCount)
		{
			_removedStart = removedStart;
			_removedCount = removedCount;
			_originString = s;
			_startIndex = startIndex;
			_count = count;

			if (failsCheck() || (s.Length - startIndex - removedCount) < count)
				throw new ArgumentOutOfRangeException();
		}

		public static implicit operator LazyString(string s)
		{
			return new LazyString(s, 0, s.Length);
		}
		public static explicit operator string(LazyString s)
		{
			return s.ToString();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}
		public IEnumerator<char> GetEnumerator()
		{
			var enumer = _originString.GetEnumerator();

			for (int i = 0; i < _startIndex; i++)
			{
				enumer.MoveNext();
			}

			for (int currentIndex = 0; currentIndex < _count;)
			{
				if (currentIndex == _insertedIndex)
				{
					var enumerInserted = _inserted.GetEnumerator();
					while (enumerInserted.MoveNext())
					{
						yield return enumerInserted.Current;
						currentIndex++;
					}
				}
				if (currentIndex == _removedStart)
				{
					for (int r = 0; r < _removedCount; r++)
					{
						enumer.MoveNext();
					}
				}

				if (enumer.MoveNext())
				{
					yield return enumer.Current;
					currentIndex++;
				}
#if DEBUG
				else
				{
					if (currentIndex < _count)
					{
						throw new IndexOutOfRangeException("Collection should reach its end here.");
					}
				}
#endif
			}

			yield break;
		}

		public LazyString Substring(int start, int count)
		{
			return new LazyString(this, start, count);
		}

		public bool EndsWith(LazyString end)
		{
			return this._originString.Skip(_startIndex).Take(_count).TakeLast(end.Length).SequenceEqual(end);
		}
		public bool EndsWith(char c)
		{
			return this._originString.Skip(_startIndex).Take(_count).LastOrDefault().Equals(c);
		}
		public bool StartsWith(LazyString start)
		{
			return this._originString.Skip(_startIndex).Take(_count).Take(start.Length).SequenceEqual(start);
		}
		public bool StartsWith(char c)
		{
			return this._originString.Skip(_startIndex).Take(_count).FirstOrDefault().Equals(c);
		}
		public int IndexOf(char c)
		{
			var enumerThis = this.GetEnumerator();

			for (int i = 0; enumerThis.MoveNext(); i++)
			{
				if (enumerThis.Current.Equals(c)) return i;
			}

			return -1;
		}


		public LazyString Insert(int index, LazyString inserted)
		{
			return new LazyString(this, 0, this._count + inserted.Length, inserted, index);
		}

		public LazyString Remove(int startIndex, int count)
		{
			return new LazyString(this, 0, this._count - count, startIndex, count);
		}

		public override string ToString()
		{
			var result = new char[this.Length];
			var enumer = this.GetEnumerator();
			for (int i = 0; i < this.Length; i++)
			{
#if RELEASE
				enumer.MoveNext();
#endif
#if DEBUG
				if (!enumer.MoveNext())
				{
					throw new IndexOutOfRangeException("Collection should not reach its end here.");
				}
#endif
				result[i] = enumer.Current;
			}

			return new string(result);
		}

		bool IEquatable<LazyString>.Equals(LazyString? other)
		{
			if (other is null) return this is null ? true : false;

			return other.SequenceEqual(this._originString);
		}
		bool IEquatable<string>.Equals(string? other)
		{
			if (other is null) return this is null ? true : false;

			return other.SequenceEqual(this._originString);
		}

		int IComparable<LazyString>.CompareTo(ads_lab_1.LazyString? other)
		{
			if (other is null) return 1;
			var enumerThis = this.GetEnumerator();
			var enumerOther = other.GetEnumerator();

			while (true)
			{
				var cThis = enumerThis.MoveNext();
				var cOther = enumerOther.MoveNext();

				if (cThis != cOther)
				{
					return cThis == false ? -1 : 1;
				}

				if (enumerThis.Current != enumerOther.Current)
				{
					return enumerThis.Current - enumerOther.Current;
				}
			}
		}
		int IComparable<string>.CompareTo(string? other)
		{
			if (other is null) return 1;
			return (this as IComparable<LazyString>).CompareTo((LazyString)other);
		}

	}
}
