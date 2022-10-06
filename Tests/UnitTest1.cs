using ads_lab_1;
using Microsoft.VisualStudio.TestPlatform.Utilities;
using Newtonsoft.Json.Linq;
using Xunit.Abstractions;
using static ads_lab_1.StringEvaluator;

namespace Tests
{
	public class UnitTest1
	{
		private readonly ITestOutputHelper output;

		public UnitTest1(ITestOutputHelper output)
		{
			this.output = output;
		}

		[Fact]
		public void OperatorPriorities()
		{
			Assert.True(
				StringEvaluator.OpsPriority.SequenceEqual(
					new MathOperators[] { MathOperators.pow,MathOperators.devide, MathOperators.multiply, MathOperators.minus, MathOperators.plus })
				);
		}

		[Fact]
		public void TopLevelOperatorsDetection1()
		{
			var expr = "1+1-1";
			var detected = StringEvaluator.GetIndexesOfTopLevelOperators(expr).Select(x => x.Index);
			var expected = new int[] { expr.IndexOf('-'), expr.IndexOf('+') };
			output.WriteLine($"detected: {string.Join(',', detected)}; expected: {string.Join(',', expected)}.");
			Assert.True(detected.SequenceEqual(expected));
		}
		[Fact]
		public void TopLevelOperatorsDetection2()
		{
			var expr = "pow(255,2)+11*32/333-cos(500)";
			var detected = StringEvaluator.GetIndexesOfTopLevelOperators(expr).Select(x => x.Index);
			var expected = new int[] { expr.IndexOf('/'), expr.IndexOf('*'), expr.IndexOf('-'), expr.IndexOf('+') };
			output.WriteLine($"detected: {string.Join(',', detected)}; expected: {string.Join(',', expected)}.");
			Assert.True(detected.SequenceEqual(expected));


			expr = "(sin(255+2224.1)*12)^^(1.1+2.222*(11))";
			detected = StringEvaluator.GetIndexesOfTopLevelOperators(expr).Select(x => x.Index);
			expected = new int[] { expr.IndexOf("^^")};
			output.WriteLine($"detected: {string.Join(',', detected)}; expected: {string.Join(',', expected)}.");
			Assert.True(detected.SequenceEqual(expected));
		}

		[Fact]
		public void IndexOfMostPriorOperation1()
		{
			var expr = "pow(255,2)+11*32-333-cos(500)/200";
			Assert.Equal(expr.IndexOf('/'), StringEvaluator.GetIndexOfFirstPriorTopOp(expr, out var dummy));
			Assert.Equal(1, dummy);
		}
		[Fact]
		public void IndexOfMostPriorOperation2()
		{
			var expr = "pow(255,6/3*1)+11+32+333+cos(500.255)-200";
			Assert.Equal(expr.IndexOf('-'), StringEvaluator.GetIndexOfFirstPriorTopOp(expr, out var dummy));
			Assert.Equal(1, dummy);

			expr = @"pow(22,cos(122))+(cos(122)/3)";
			Assert.Equal(expr.IndexOf('+'), StringEvaluator.GetIndexOfFirstPriorTopOp(expr, out dummy));
			Assert.Equal(1, dummy);
		}

		[Fact]
		public void StringOnIndexDetection1()
		{
			string s = "1234+1234+1";
			Assert.True(StringEvaluator.isOnThisIndex(s, "+", s.IndexOf('+')));
		}
		[Fact]
		public void StringOnIndexDetection2()
		{
			string s = "1234+1234**(22+3/5+2**2)";
			Assert.True(StringEvaluator.isOnThisIndex(s, "**", s.IndexOf("**")));
			Assert.True(StringEvaluator.isOnThisIndex(s, "**", s.LastIndexOf("**")));
		}

		[Fact]
		public void ExpressionForOperator1()
		{
			string s = "123+123";
			StringEvaluator.GetExprsForOpAt(s, s.IndexOf('+'), 1, out var left, out var right);
			Assert.Equal(s.Split('+').First(), left);
			Assert.Equal(s.Split('+').Last(), right);
		}
		[Fact]
		public void ExpressionForOperator2()
		{
			string s = "(sin(255+2224.1)*12)^^(1.1+2.222*(11))";
			StringEvaluator.GetExprsForOpAt(s, s.IndexOf("^^"), 2, out var left, out var right);
			Assert.Equal(s.Split("^^").First(), left);
			Assert.Equal(s.Split("^^").Last(), right);
		}
		[Fact]
		public void ExpressionForOperator3()
		{
			string s = "22^^cos(122)";
			StringEvaluator.GetExprsForOpAt(s, s.IndexOf("^^"), 2, out var left, out var right);
			Assert.Equal(s.Split("^^").First(), left); Assert.Equal(s.Split("^^").First(), "22");
			Assert.Equal(s.Split("^^").Last(), right); Assert.Equal(s.Split("^^").Last(), "cos(122)");

			s = "pow(22,cos(122))+(cos(122)/3)";
			StringEvaluator.GetExprsForOpAt(s, s.IndexOf("+"), 1, out left, out right);
			Assert.Equal("pow(22,cos(122))",left);
			Assert.Equal("(cos(122)/3)",right);

			s = "(pow(22,cos(122)+2))+(pow(22,cos(122)+2))/3";
			StringEvaluator.GetExprsForOpAt(s, s.IndexOf("/"), 1, out left, out right);
			Assert.Equal("(pow(22,cos(122)+2))",left);
			Assert.Equal("3",right);

		}

		[Fact]
		public void NumberOfFuncParameters1()
		{
			string s = "sin(255.1)";
			var result = CountParametersOfFuncCall(s, out var pararms);
			Assert.Equal(1, result);
			Assert.Equal(s.Substring(s.IndexOf('(') + 1, s.Length - s.IndexOf('(') - 2), pararms.First());
		}
		[Fact]
		public void NumberOfFuncParameters2()
		{
			string s = "pow(255.1,999)";
			var result = CountParametersOfFuncCall(s, out var pararms);
			Assert.Equal(2, result);
			output.WriteLine($"First param " +
				$"expected: {s.Substring(s.IndexOf('(') + 1, s.Length - s.IndexOf('(') - 2).Split(',').First()}; " +
				$"actual: {pararms.First()}.");
			output.WriteLine($"Second param " +
				$"expected: {s.Substring(s.IndexOf('(') + 1, s.Length - s.IndexOf('(') - 2).Split(',').Last()}; " +
				$"actual: {pararms.Last()}.");
			Assert.Equal(s.Substring(s.IndexOf('(') + 1, s.Length - s.IndexOf('(') - 2).Split(',').First(), pararms.First());
			Assert.Equal(s.Substring(s.IndexOf('(') + 1, s.Length - s.IndexOf('(') - 2).Split(',').Last(), pararms.Last());
		}

		[Fact]
		public void IsLiteralRegex()
		{
			string s;

			s = "123";
			Assert.Matches(isNumberRegex, s);
			s = "1.0";
			Assert.Matches(isNumberRegex, s);
			s = "0.11";
			Assert.Matches(isNumberRegex, s);

			s = "0.11b";
			Assert.DoesNotMatch(isNumberRegex, s);
			s = "0b1.11b";
			Assert.DoesNotMatch(isNumberRegex, s);
			s = "1222+1";
			Assert.DoesNotMatch(isNumberRegex, s);
			s = "s";
			Assert.DoesNotMatch(isNumberRegex, s);
		}

		[Fact]
		public void IsFuncCallRegex()
		{
			string s;

			s = "pow(255,cos(0.11*2))";
			Assert.Matches(isFunctionCallRegex, s);

			s = "cos(255)";
			Assert.Matches(isFunctionCallRegex, s);

			s = "11+cos(255)";
			Assert.DoesNotMatch(isFunctionCallRegex, s);

			s = "cos(255)/2";
			Assert.DoesNotMatch(isFunctionCallRegex, s);
		}

		[Fact]
		public void Eval2_1() // literals
		{
			double value;

			value = 255;
			Assert.Equal(value, Eval2(value.ToString()));

			value = 1.255;
			Assert.Equal(value, Eval2(value.ToString()));

			value = 0.255;
			Assert.Equal(value, Eval2(value.ToString()));
		}

		[Fact]
		public void Eval2_2() // operators
		{
			string s; double value;

			value = 255 + 1;
			s = "255+1";
			Assert.Equal(value, Eval2(s));

			s = "255.0+1.1";
			value = 255.0 + 1.1;
			Assert.Equal(value, Eval2(s));

			s = "(255.0+1.1)";
			value = (255.0 + 1.1);
			Assert.Equal(value, Eval2(s));

			s = "255.0+(1.1+2)";
			value = 255.0 + (1.1 + 2);
			Assert.Equal(value, Eval2(s));

			s = "255.0+(1.1+2)";
			value = 255.0 + (1.1 + 2);
			Assert.Equal(value, Eval2(s));

			s = "255.0/(1.1+2)";
			value = 255.0 / (1.1 + 2);
			Assert.Equal(value, Eval2(s));

			s = "255.0/(1.1*(2-1))";
			value = 255.0 / (1.1 * (2 - 1));
			Assert.Equal(value, Eval2(s));

			s = "255.0/(1.1*(2-1))";
			value = 255.0 / (1.1 * (2 - 1));
			Assert.Equal(value, Eval2(s));

			s = "255.0^^(1.1*(2-1))";
			value = Math.Pow(255.0, (1.1 * (2 - 1)));
			Assert.Equal(value, Eval2(s));

			s = "255.0+(1.1+2))";
			value = 255.0 + (1.1 + 2);
			Assert.ThrowsAny<Exception>(() => Assert.Equal(value, Eval2(s)));

			s = "255.0//(1.1+2))";
			value = 255.0 + (1.1 + 2);
			Assert.ThrowsAny<Exception>(() => Assert.Equal(value, Eval2(s)));

			s = "((255.0+(1.1+2))";
			value = 255.0 + (1.1 + 2);
			Assert.ThrowsAny<Exception>(() => Assert.Equal(value, Eval2(s)));
		}

		[Fact]
		public void Eval2_3() // functions
		{
			string s; double value;

			s = "sin(1)";
			value = Math.Sin(1d);
			Assert.Equal(value, Eval2(s));

			s = "sin(1.3+2)";
			value = Math.Sin(1.3 + 2);
			Assert.Equal(value, Eval2(s));

			s = "pow(sin(1.3+2),cos(122))";
			value = Math.Pow(Math.Sin(1.3 + 2), Math.Cos(122));
			Assert.Equal(value, Eval2(s));

			s = "pow(sin(1.3+2),cos(122)+2)";
			value = Math.Pow(Math.Sin(1.3 + 2), Math.Pow(Math.Cos(122), 2));
			Assert.Equal(value, Eval2(s));

			s = "pow(sin(1.3+2),cos(122)+2)/pow(sin(1.3+2),cos(122)+2)*pow(sin(1.3+2),cos(122)+2)-pow(sin(1.3+2),cos(122)+2)";
			value = Math.Pow(Math.Sin(1.3 + 2), Math.Cos(122) + 2)
				/ Math.Pow(Math.Sin(1.3 + 2), Math.Cos(122) + 2)
				* Math.Pow(Math.Sin(1.3 + 2), Math.Cos(122) + 2)
				- Math.Pow(Math.Sin(1.3 + 2), Math.Cos(122) + 2);
			Assert.Equal(value, Eval2(s));

			s = "pow(sin(1.3+2),(cos(122)+2)";
			value = Math.Pow(Math.Sin(1.3 + 2), Math.Pow(Math.Cos(122), 2));
			Assert.ThrowsAny<Exception>(() => Eval2(s));

			s = "pow(sin(1.3+2),(cos(122)+2)+)pow(sin(1.3+2),(cos(122)+2)";
			value = Math.Pow(Math.Sin(1.3 + 2), Math.Pow(Math.Cos(122), 2));
			Assert.ThrowsAny<Exception>(() => Eval2(s));

			s = "pow(sin(1.3+2),(cos(122())+2)+)pow(sin(1.3+2),(cos(122)+2)";
			value = Math.Pow(Math.Sin(1.3 + 2), Math.Pow(Math.Cos(122), 2));
			Assert.ThrowsAny<Exception>(() => Eval2(s));
		}

		[Fact]
		public void Eval2_4()
		{
			string s; double value;

			s = "(pow(22,cos(122)+2))";
			value = Math.Pow(22, Math.Cos(122) + 2);
			Assert.Equal(value, Eval2(s));

			s = "(pow(22,cos(122)+2))/3";
			value = Math.Pow(22, Math.Cos(122) + 2)/3;
			Assert.Equal(value, Eval2(s));

			s = @"(pow(22,cos(122)+2))+(pow(22,cos(122)+2))/3";
			value = (
				Math.Pow(22, Math.Cos(122) + 2)
				+
				(Math.Pow(22, Math.Cos(122) + 2) / 3));
			Assert.Equal(value, Eval2(s));
		}
	}
}