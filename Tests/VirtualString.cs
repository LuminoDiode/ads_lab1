using ads_lab_1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit.Abstractions;

namespace Tests
{
	public class VirtualStringTests
	{
		private readonly ITestOutputHelper output;
		private readonly string testString;
		private readonly LazyString testVirtual;

		public VirtualStringTests(ITestOutputHelper output)
		{
			this.output = output;
			testString = "1234567890";
			testVirtual = (LazyString)testString;
		}

		[Fact]
		public void CanCreateInstance()
		{
			string s = "123";
			var vs = (LazyString)(s);
			Assert.Equal(s, vs.ToString());

			vs = new LazyString(s, 0, s.Length);
			Assert.Equal(s, vs.ToString());

			vs = new LazyString(vs, 0, s.Length);
			Assert.Equal(s, vs.ToString());
		}

		[Fact]
		public void CanSubstring()
		{
			int startIndex = 1, count = 9;

			var s = "1234567890";
			var ss = s.Substring(startIndex, count);

			var vs1 = new LazyString(s, startIndex, count);
			var vs2 = ((LazyString)s).Substring(startIndex, count); ;

			Assert.Equal(ss, vs1.ToString());
			Assert.Equal(ss, vs2.ToString());

			var rnd = new Random(DateTime.UtcNow.Millisecond);
			for (int i = 0; i < 1000; i++)
			{
				s = long.MaxValue.ToString();
				startIndex = rnd.Next(s.Length);
				count = rnd.Next(0,s.Length - startIndex + 1);

				Assert.Equal(s.Substring(startIndex, count), ((LazyString)s).Substring(startIndex, count));
			}
		}

		[Fact]
		public void CanInsert()
		{
			var s = "1234567890";
			var vs = (LazyString)(s);

			Assert.Equal(s.Insert(0, s), vs.Insert(0, s));
			Assert.Equal(s.Insert(s.Length, s), vs.Insert(s.Length, s));
			Assert.Equal(s.Insert(s.Length / 2, s), vs.Insert(s.Length / 2, s));
		}

		[Fact]
		public void CanRemove()
		{
			var s = "1234567890";
			var vs = (LazyString)(s);

			Assert.Equal(s.Remove(0, s.Length), vs.Remove(0, s.Length));

			//Assert.Equal(
			//	s.Remove((int)Math.Floor((s.Length - 1) / 2d), (int)Math.Ceiling(s.Length / 2d)),
			//	vs.Remove((int)Math.Floor((s.Length - 1) / 2d), (int)Math.Ceiling(s.Length / 2d)));

			//Assert.Equal(s.Remove(s.Length - 1, 0), vs.Remove(s.Length - 1, 0));

			//Assert.Equal(s.Remove(1, s.Length - 1), vs.Remove(1, s.Length - 1));
			//Assert.Equal(s.Remove(3, s.Length - 3), vs.Remove(3, s.Length - 3));
		}

		[Fact]
		public void CanDoMultipleOperations()
		{
			int numberOfIterations = 1000;
			var rand = new Random(DateTime.UtcNow.Millisecond);

			var originalString = (int.MaxValue / rand.Next(1, 10)).ToString();
			var virtualString = (LazyString)originalString;

			for (int i = 0; i < numberOfIterations; i++)
			{
				// insert ?
				if (rand.Next(0, 2) == 1)
				{
					var insertionIndex = rand.Next(0, virtualString.Length);
					var insertionValue = rand.Next(0, int.MaxValue).ToString();

					virtualString = virtualString.Insert(insertionIndex, insertionValue);
					originalString = originalString.Insert(insertionIndex, insertionValue);
				}

				// remove ?
				if (rand.Next(0, 2) == 1)
				{
					var removeIndex = rand.Next(0, virtualString.Length);
					var removeCount = rand.Next(0, virtualString.Length - removeIndex);

					virtualString = virtualString.Remove(removeIndex, removeCount);
					originalString = originalString.Remove(removeIndex, removeCount);
				}

				output.WriteLine(originalString);
			}

			Assert.Equal(originalString, virtualString);
		}

		[Fact]
		public void CanGetIndexOf()
		{
			var rand = new Random(DateTime.UtcNow.Millisecond);
			var originalString = (int.MaxValue / rand.Next(1, 10)).ToString();
			var virtualString = (LazyString)originalString;
			char c;

			c = originalString[0];
			Assert.Equal(originalString.IndexOf(c), virtualString.IndexOf(c));

			c = originalString[originalString.Length / 2];
			Assert.Equal(originalString.IndexOf(c), virtualString.IndexOf(c));

			c = originalString[originalString.Length - 1];
			Assert.Equal(originalString.IndexOf(c), virtualString.IndexOf(c));
		}

		[Fact]
		public void CanUseStartsWith()
		{
			var rand = new Random(DateTime.UtcNow.Millisecond);
			var originalString = (int.MaxValue / rand.Next(1, 10)).ToString();
			var virtualString = (LazyString)originalString;
			string s;

			s = originalString.Substring(0);
			Assert.Equal(originalString.StartsWith(s), virtualString.StartsWith(s));

			s = originalString.Substring(originalString.Length / 2);
			Assert.Equal(originalString.StartsWith(s), virtualString.StartsWith(s));

			s = originalString.Substring(originalString.Length);
			Assert.Equal(originalString.StartsWith(s), virtualString.StartsWith(s));
		}


		[Fact]
		public void CanUseEndsWith()
		{
			var rand = new Random(DateTime.UtcNow.Millisecond);
			var originalString = (int.MaxValue / rand.Next(1, 10)).ToString();
			var virtualString = (LazyString)originalString;
			string s;

			s = originalString.Substring(0);
			Assert.Equal(originalString.EndsWith(s), virtualString.EndsWith(s));

			s = originalString.Substring(originalString.Length / 2, originalString.Length - (originalString.Length / 2));
			Assert.Equal(originalString.EndsWith(s), virtualString.EndsWith(s));

			s = originalString.Substring(originalString.Length);
			Assert.Equal(originalString.EndsWith(s), virtualString.EndsWith(s));
		}
	}
}
