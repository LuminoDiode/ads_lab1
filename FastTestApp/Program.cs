using ads_lab_1;
using static ads_lab_1.StringEvaluator;

internal class Program
{
	private static void Main(string[] args)
	{
		var myWorker = new StringEvaluator();
		var evalFunction = myWorker.Eval2;
		string s; double value;

		s = "1+log10(" +
				"tan(22*cos(122)+2)" +
				"+" +
				"pow(22,sin(122)+2)" +
			")/3".Replace(" ", String.Empty);
		value = 1 + Math.Log10(
				Math.Tan(22 * Math.Cos(122) + 2)
				+
				Math.Pow(22, Math.Sin(122) + 2)
				) / 3;

		s = @"1+log10(-tan(22*cos(122)+2)+pow(22,-sin(122)+2))/3";
		value = 1 + Math.Log10(
				-Math.Tan(22 * Math.Cos(122) + 2)
				+
				Math.Pow(22, -Math.Sin(122) + 2)
				) / 3;

		s = @"-(-(1+log10(-tan(22*cos(122)+2)+pow(22,-sin(122)+2))/3))";
		value = 1 + Math.Log10(
				-Math.Tan(22 * Math.Cos(122) + 2)
				+
				Math.Pow(22, -Math.Sin(122) + 2)
				) / 3;
	}
}