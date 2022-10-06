using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;
using System.Windows.Media;

namespace ads_lab_1
{
	/* В выражении могут быть использованы целые числа, вещественные числа, арифметические операторы: +,-,*,/, 
	* скобки (), а также функции и операторы в соответствии с вариантом. 
	* 18.   sin
	*/

	public class StringEvaluator
	{
		public enum MathOperators
		{
			devide,
			multiply,
			minus,
			plus,
			pow
		}
		public struct PrioritizedOperation
		{
			public MathOperators Operation;
			public int Priority;
		}
		public struct PrioritizedOperationOnIndex
		{
			public PrioritizedOperation Operation;
			public int Index;
		}

		public List<string> Ops;
		public List<string> Funcs;
		public List<MathOperators> OpsPriority = (new MathOperators[] {
			MathOperators.pow,MathOperators.devide, MathOperators.multiply, MathOperators.minus, MathOperators.plus
		}).ToList();
		public Dictionary<string, Func<double, double>> FunctionsSignatures1 = new(); // 1 param
		public Dictionary<string, Func<double, double, double>> FunctionsSignatures2 = new(); // 2 params

		public Regex isNumberRegex;
		public Regex isFunctionCallRegex;

		public StringEvaluator()
		{
			Debug.WriteLine($"Constructor of {nameof(StringEvaluator)} is being called.");

			Ops = "^^,/,*,-,+".Split(',').ToList();

			FunctionsSignatures1.Add("sin", Math.Sin);
			FunctionsSignatures1.Add("cos", Math.Cos);
			FunctionsSignatures2.Add("pow", Math.Pow);
			FunctionsSignatures1.Add("tan", Math.Tan);
			FunctionsSignatures1.Add("log10", Math.Log10);
			FunctionsSignatures1.Add("log2", Math.Log2);
			FunctionsSignatures1.Add("ln", Math.Log);

			Funcs = FunctionsSignatures1.Keys.Concat(FunctionsSignatures2.Keys).ToList();
			Ops = "^^,/,*,-,+".Split(',').ToList();

			isNumberRegex = new Regex(@"^[-]?\d+(\.\d+)?$");
			isFunctionCallRegex = new Regex(@"^((\A|\))[-])?({ALLOWEDFUNCS})[\(](\n|.)+[\)]$".Replace("{ALLOWEDFUNCS}", string.Join('|', Funcs)));
		}
		public MathOperators OperatorFromString(string s)
		{
			switch (s)
			{
				case "+":
					return MathOperators.plus;
				case "-":
					return MathOperators.minus;
				case "*":
					return MathOperators.multiply;
				case "/":
					return MathOperators.devide;
				case "^^":
					return MathOperators.pow;
				case "**":
					return MathOperators.pow;

				default:
					throw new ArgumentException($"Unknown operator {s}.");
			}
		}
		public string OperatorToString(MathOperators op)
		{
			switch (op)
			{
				case MathOperators.plus:
					return "+";
				case MathOperators.minus:
					return "-";
				case MathOperators.multiply:
					return "*";
				case MathOperators.devide:
					return "/";
				case MathOperators.pow:
					return "^^";

				default:
					throw new ArgumentException($"Unknown operator {op}.");
			}
		}

		public bool isOnThisIndex(string orig, string search, int startIndex)
		{
			if (orig.Length < (startIndex + search.Length)) return false;

			for (int i = 0; i < search.Length; i++)
				if (orig[startIndex + i] != search[i]) return false;

			return true;
		}
		public IEnumerable<PrioritizedOperationOnIndex> GetIndexesOfTopLevelOperators(string s, List<string> OrderedByPriority = null!, bool SortByPriority = true)
		{
			if (OrderedByPriority is null) OrderedByPriority = Ops;
			List<PrioritizedOperationOnIndex> IndexesOfOperators = new();
			int BracketsOpened = 0;
			for (int i = 0; i < s.Length; i++)
			{
				if (s[i] == '(')
				{
					BracketsOpened++;
				}
				else if (s[i] == ')')
				{
					BracketsOpened--;
				}
				else if (BracketsOpened == 0)
				{
					var found = OrderedByPriority.Find(op => isOnThisIndex(s, op, i));
					if (found is not null)
					{
						if (found == "-" && (i == 0 || s[i - 1] == '(')) continue;
						IndexesOfOperators.Add(new PrioritizedOperationOnIndex
						{

							Operation = new()
							{
								Operation = OperatorFromString(found),
								Priority = int.MaxValue - OrderedByPriority.FindIndex(x => x.Equals(found))
							},
							Index = i
						});
					}
				}
			}

			if (SortByPriority)
				return IndexesOfOperators.OrderByDescending(x => x.Operation.Priority);
			else
				return IndexesOfOperators;
		}

		public int? GetIndexOfFirstPriorTopOp(string s, out int opLen)
		{
			var t = GetIndexesOfTopLevelOperators(s);
			if (t.Any())
			{
				opLen = OperatorToString(t.First().Operation.Operation).Length;
				return t.First().Index;
			}
			else
			{
				opLen = -1;
				return null;
			}
		}
		public void GetExprsForOpAt(string s, int opIndex, int opLen, out string Left, out string Right)
		{
			getIndexesOfExprs(s, opIndex, opLen, out var exprLeft, out var exprRight);
			Left = s.Substring(exprLeft.Start, exprLeft.Count);
			Right = s.Substring(exprRight.Start, exprRight.Count);
			return;
		}
		struct ExpressionIndex
		{
			public int Start;
			public int Count;
		}
		private void getIndexesOfExprs(string s, int opIndex, int opLen, out ExpressionIndex Left, out ExpressionIndex Right)
		{
			int? leftClosestOp = null, rightClosestOp = null;
			int? leftClosestOpLen = null, rightClosestOpLen = null;
			var topLevelOps = GetIndexesOfTopLevelOperators(s);
			if (topLevelOps.Any(x => x.Index < opIndex))
			{
				var t = topLevelOps.Where(x => x.Index < opIndex).MaxBy(x => x.Index);
				leftClosestOp = t.Index;
				leftClosestOpLen = OperatorToString(t.Operation.Operation).Length;
			}
			if (topLevelOps.Any(x => x.Index > opIndex))
			{
				var t = topLevelOps.Where(x => x.Index > opIndex).MinBy(x => x.Index);
				rightClosestOp = t.Index;
				rightClosestOpLen = OperatorToString(t.Operation.Operation).Length;
			}

			if (leftClosestOp.HasValue && leftClosestOpLen.HasValue)
				Left = new ExpressionIndex() { Start = leftClosestOp.Value + leftClosestOpLen.Value, Count = opIndex - leftClosestOp.Value - leftClosestOpLen.Value };	
			else
				Left = new ExpressionIndex() { Start = 0, Count = opIndex };

			if (rightClosestOp.HasValue && rightClosestOpLen.HasValue)
				Right = new ExpressionIndex() { Start = opIndex + opLen, Count = rightClosestOp.Value - (opIndex + opLen) };
			else
				Right = Right = new ExpressionIndex() { Start = opIndex + opLen, Count = s.Length - (opIndex + opLen) };
		}

		public int CountParametersOfFuncCall(string s, out List<string> parameters)
		{
			int count = 0;
			var indexOfBracket = s.IndexOf('(');
			int bracketsOpened = 0;
			int prevCommaIndex = indexOfBracket;

			parameters = new();

			for (int i = indexOfBracket + 1; i < (s.Length - 1); i++)
			{
				if (s[i] == '(')
				{
					bracketsOpened++;
				}
				else if (s[i] == ')')
				{
					bracketsOpened--;
				}
				else if (bracketsOpened == 0 && s[i] == ',')
				{
					parameters.Add(s.Substring(prevCommaIndex + 1, (i - prevCommaIndex - 1)));
					prevCommaIndex = i;
					count++;
				}
			}
			parameters.Add(s.Substring(prevCommaIndex + 1, (s.Length - prevCommaIndex - 2)));
			count++;
			return count;
		}

		// 1. константа? - вернуть константу
		// 2. есть операторы? разбить по операторам
		// 3. выражение в боковых скобках? - убрать их
		// 4. вызов функции? - вызвать фунцкии, повторить вызов для параметра
		// Порядок выбора случая важен !!!
		public double Eval2(string s)
		{
			Debug.WriteLine($"Evaluation of string \'{s}\' started.");

			if (new Regex(@"\s").IsMatch(s)) throw new ArgumentException("White-spaces is not allowed in the expression");
			if (isNumberRegex.IsMatch(s)) return double.Parse(s);

			var opToCalc = GetIndexOfFirstPriorTopOp(s, out var len);
			if (opToCalc is not null)
			{
				GetExprsForOpAt(s, opToCalc.Value, len, out var left, out var right);
				getIndexesOfExprs(s, opToCalc.Value, len, out var leftReplace, out var rightReplace);
				var operation = OperatorFromString(s.Substring(opToCalc.Value, len));

				double evaluated;
				switch (operation)
				{
					case MathOperators.devide:
						evaluated = Eval2(left) / Eval2(right);
						break;
					case MathOperators.multiply:
						evaluated = Eval2(left) * Eval2(right);
						break;
					case MathOperators.plus:
						evaluated = Eval2(left) + Eval2(right);
						break;
					case MathOperators.minus:
						evaluated = Eval2(left) - Eval2(right);
						break;
					case MathOperators.pow:
						evaluated = Math.Pow(Eval2(left), Eval2(right));
						break;

					default:
						throw new ArgumentException("Unknown operator found.");
				}

				var newS = s.Remove(leftReplace.Start, leftReplace.Count + rightReplace.Count + len).Insert(leftReplace.Start, evaluated.ToString());

				if (evaluated is double.NaN) return double.NaN;
				return Eval2(newS);
			}

			if (s.StartsWith("-("))
			{
				return Eval2(s.Insert(1, "1*"));
			};
			if (s.StartsWith('(') && s.EndsWith(')')) {

				return Eval2(s.Substring(1, s.Length - 2));
			};

			if (isFunctionCallRegex.IsMatch(s))
			{
				var indexOfBracket = s.IndexOf('(');
				var funcName = s.Substring(0, indexOfBracket);
				var numOfParams = CountParametersOfFuncCall(s, out var parameters);

				bool negative = false;
				if (funcName.StartsWith('-'))
				{
					funcName = funcName.Substring(1);
					negative = true;
				}
				if (numOfParams == 1)
				{
					if (!FunctionsSignatures1.TryGetValue(funcName, out var func))
					{
						throw new ArgumentException($"Unknown function \'{funcName}\' with {numOfParams} parameters.");
					}
					return (negative ? -1 : 1) * func(Eval2(parameters[0]));
				}
				else if (numOfParams == 2)
				{
					if (!FunctionsSignatures2.TryGetValue(funcName, out var func))
					{
						throw new ArgumentException($"Unknown function \'{funcName}\' with {numOfParams} parameters.");
					}
					return (negative ? -1 : 1) * func(Eval2(parameters[0]), Eval2(parameters[1]));
				}

			}

			throw new ArgumentException($"Could not determinate the type of expression \'{s}\'.");
		}
	}
}
