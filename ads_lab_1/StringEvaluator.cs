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


	//class ExpressionNode
	//{
	//	public static ExpressionNode expressionNode;
	//}
	//class MultiactionNode : ExpressionNode
	//{
	//	IList<ExpressionNode> expressionNodes;
	//}
	//class OperatorNode : ExpressionNode
	//{
	//	MathOperators Operator;
	//	ExpressionNode LeftNode;
	//	ExpressionNode RightNode;
	//}
	//class FunctionNode : ExpressionNode
	//{
	//	Func<double, double>? Function1;
	//	Func<double, double, double>? Function2;
	//	IList<ExpressionNode> FunctionParameters;
	//}
	//class LiteralNode : ExpressionNode
	//{
	//	double Value;
	//}

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

		public static List<string> Ops = "^^,/,*,-,+".Split(',').ToList();
		public static List<string> Funcs = "sin,cos,pow".Split(',').ToList();
		public static List<MathOperators> OpsPriority = (new MathOperators[] {
			MathOperators.pow,MathOperators.devide, MathOperators.multiply, MathOperators.minus, MathOperators.plus
		}).ToList();
		public static Dictionary<string, Func<double, double>> FunctionsSignatures1 = new(); // 1 param
		public static Dictionary<string, Func<double, double, double>> FunctionsSignatures2 = new(); // 2 params

		static StringEvaluator()
		{
			FunctionsSignatures1.Add("sin", Math.Sin);
			FunctionsSignatures1.Add("cos", Math.Cos);
			FunctionsSignatures2.Add("pow", Math.Pow);
		}
		public static MathOperators OperatorFromString(string s)
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
		public static string OperatorToString(MathOperators op)
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


		public static bool isOnThisIndex(string orig, string search, int startIndex)
		{
			if (orig.Length < (startIndex + search.Length)) return false;

			for (int i = 0; i < search.Length; i++)
				if (orig[startIndex + i] != search[i]) return false;

			return true;
		}
		public static IEnumerable<PrioritizedOperationOnIndex> GetIndexesOfTopLevelOperators(string s, List<string> OrderedByPriority = null!, bool SortByPriority = true)
		{
			if (OrderedByPriority is null) OrderedByPriority = StringEvaluator.Ops;
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
					if (found is not null) IndexesOfOperators.Add(new PrioritizedOperationOnIndex
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

			if (SortByPriority)
				return IndexesOfOperators.OrderByDescending(x => x.Operation.Priority);
			else
				return IndexesOfOperators;
		}
		//public static double Eval2(string s)
		//{
		//	// если в строке все еще есть топ лвл операторы
		//	var IndexesOfOperators = GetIndexesOfTopLevelOperators(s).ToList();

		//	List<string> Nodes = new();
		//	if (IndexesOfOperators.Count > 0)
		//	{
		//		int prevOpIndex = 0;
		//		for (int i = 0; i < IndexesOfOperators.Count; i++)
		//		{
		//			Nodes.Add(s.Substring(prevOpIndex, IndexesOfOperators[i].Index));
		//		}
		//	}

		//	double result;


		//}

		public static int? GetIndexOfFirstPriorTopOp(string s, out int opLen)
		{
			var priors = Ops;

			opLen = -1;
			foreach (var op in priors)
			{
				var BracketsOpened = 0;
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
					if (BracketsOpened == 0 && isOnThisIndex(s, op, i))
					{
						opLen = op.Length;
						return i;
					}
				}
			}

			return null;
		}
		public static void GetExprsForOpAt(string s, int opIndex, int opLen, out string Left, out string Right)
		{
			//string[] priors = "*,/,+,-".Split(',');

			//Left = Right = null!;

			//foreach (var op in priors)
			//{
			//	for (int i = opIndex - 1; i >= 0; i--)
			//	{
			//		if (isOnThisIndex(s, op, i))
			//		{
			//			//Left = new Node { StartIndex = i + op.Length, Count = opIndex - i - 1 };
			//			Left = s.Substring( i + op.Length, opIndex - i - 1);
			//		}
			//	}
			//	for (int i = opIndex + opLen; i < s.Length; i++)
			//	{
			//		if (isOnThisIndex(s, op, i))
			//		{
			//			//Right = new Node { StartIndex = opIndex + opLen, Count = i - opIndex - opLen };
			//			Right = s.Substring(opIndex + opLen, i - opIndex - opLen);
			//		}
			//	}
			//}

			//if (Left is null || Right is null)
			//	throw new ArgumentException($"Could not find left and right operands in the expression \'{s}\' and operator at indexes={opIndex}-{opIndex+opLen-1}.");

			int? leftClosestOp=null, rightClosestOp=null;
			int? leftClosestOpLen = null, rightClosestOpLen = null;
			var topLevelOps = GetIndexesOfTopLevelOperators(s);
			if (topLevelOps.Any(x=> x.Index<opIndex))
			{
				var t = topLevelOps.Where(x => x.Index < opIndex).MaxBy(x => x.Index);
				leftClosestOp = t.Index;
				leftClosestOpLen = OperatorToString(t.Operation.Operation).Length;
			}
			if(topLevelOps.Any(x => x.Index > opIndex))
			{
				var t = topLevelOps.Where(x => x.Index > opIndex).MinBy(x => x.Index);
				rightClosestOp = t.Index;
				rightClosestOpLen = OperatorToString(t.Operation.Operation).Length;
			}
			//var leftClosestOp = topLevelOps.Min(x => x.Index);
			//var rightClosestOp = topLevelOps.Max(x => x.Index);

			if (leftClosestOp.HasValue && leftClosestOpLen.HasValue)
			{
				Left = s.Substring(leftClosestOp.Value + leftClosestOpLen.Value, opIndex - leftClosestOp.Value - leftClosestOpLen.Value);
			}
			else
				Left = s.Substring(0, opIndex);

			if(rightClosestOp.HasValue && rightClosestOpLen.HasValue)
				Right = s.Substring(opIndex + opLen, rightClosestOp.Value-(opIndex+opLen));
			else
				Right = s.Substring(opIndex + opLen);
		}
		struct ExpressionIndex
		{
			public int Start;
			public int Count;
		}
		private static void getIndexesOfExprs(string s, int opIndex, int opLen, out ExpressionIndex Left, out ExpressionIndex Right)
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
			//var leftClosestOp = topLevelOps.Min(x => x.Index);
			//var rightClosestOp = topLevelOps.Max(x => x.Index);

			if (leftClosestOp.HasValue && leftClosestOpLen.HasValue)
			{
				Left = new ExpressionIndex() { Start = leftClosestOp.Value + leftClosestOpLen.Value, Count = opIndex - leftClosestOp.Value-leftClosestOpLen.Value};
			}
			else
				Left = new ExpressionIndex() { Start = 0, Count = opIndex };
			if (rightClosestOp.HasValue && rightClosestOpLen.HasValue)
				Right = new ExpressionIndex() { Start = opIndex + opLen, Count = rightClosestOp.Value - (opIndex + opLen) };
			else
				Right = Right = new ExpressionIndex() { Start = opIndex + opLen, Count = s.Length- (opIndex + opLen) };
		}

		//public struct Node
		//{
		//	public int StartIndex;
		//	public int Count;
		//}
		public static int CountParametersOfFuncCall(string s, out List<string> parameters)
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

		// константа? - вернуть константу
		// есть операторы? разбить по операторам
		// выражение в боковых скобках? - убрать их
		// вызов функции? - вызвать фунцкии, повторить вызов для параметра
		public static Regex isNumberRegex = new Regex(@"^\d+(\.\d+)?$");
		public static Regex isFunctionCallRegex = new Regex(@"^({ALLOWEDFUNCS})[\(](\n|.)+[\)]$".Replace("{ALLOWEDFUNCS}", string.Join('|', Funcs)));
		public static double Eval2(string s)
		{
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
						evaluated= Eval2(left) / Eval2(right);
						break;
					case MathOperators.multiply:
						evaluated= Eval2(left) * Eval2(right);
						break;
					case MathOperators.plus:
						evaluated= Eval2(left) + Eval2(right);
						break;
					case MathOperators.minus:
						evaluated= Eval2(left) - Eval2(right);
						break;
					case MathOperators.pow:
						evaluated= Math.Pow(Eval2(left), Eval2(right));
						break;

					default:
						throw new ArgumentException("Unknown operator found.");
				}

				var newS = s.Remove(leftReplace.Start, leftReplace.Count+rightReplace.Count+len).Insert(leftReplace.Start,evaluated.ToString());

				if (evaluated is double.NaN) return double.NaN;
				return Eval2(newS);
			}

			if (s.StartsWith('(') && s.EndsWith(')')) return Eval2(s.Substring(1, s.Length - 2));

			if (isFunctionCallRegex.IsMatch(s))
			{
				var indexOfBracket = s.IndexOf('(');
				var funcName = s.Substring(0, indexOfBracket);
				var numOfParams = CountParametersOfFuncCall(s, out var parameters);

				if (numOfParams == 1)
				{
					if (!FunctionsSignatures1.TryGetValue(funcName, out var func))
					{
						throw new ArgumentException($"Unknown function \'{funcName}\'");
					}
					return func(StringEvaluator.Eval2(parameters[0]));
				}
				else if (numOfParams == 2)
				{
					if (!FunctionsSignatures2.TryGetValue(funcName, out var func))
					{
						throw new ArgumentException($"Unknown function \'{funcName}\'");
					}
					return func(StringEvaluator.Eval2(parameters[0]), StringEvaluator.Eval2(parameters[1]));
				}

			}

			throw new ArgumentException($"Could not determinate the type of expression \'{s}\'.");
		}
	}
}
