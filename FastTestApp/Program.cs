using ads_lab_1;
using static ads_lab_1.StringEvaluator;

internal class Program
{
	private static void Main(string[] args)
	{
		var myWorker = new StringEvaluator();
		myWorker.AddFunction("plsdont", (x, y) => 2 * x + 3 * y);
		myWorker.AddOperator("&&", (left, right) => left / 2 + right / 2, 2);
		myWorker.AddOperator("&", (left, right) => left / 4 + right / 4, 2);



		string s = "4&plsdont(1,2)+4&&plsdont(1,2)+4&plsdont(1,2)"; // 3+6+3
		var value = 12;
		Console.WriteLine(myWorker.Eval2(s));
	}
}