using ads_lab_1;
using static ads_lab_1.StringEvaluator;

internal class Program
{
	private static void Main(string[] args)
	{
		var myWorker = new StringEvaluator();
		var evalFunction = myWorker.Eval2;
		string s; double value;


		var rnd = new Random(DateTime.UtcNow.Millisecond);
		int failed = 0;
		for (int i = 0; i < 30000; i++)
		{
			try
			{
				myWorker.Eval2($"{rnd.NextDouble() * 3}+log10(" +
						$"tan({rnd.NextDouble() * 3}*cos({rnd.NextDouble() * 3})+0)" +
						$"+" +
						$"pow({rnd.NextDouble() * 3},sin({rnd.NextDouble() * 3})+2)" +
					$")/{rnd.NextDouble() * 3}".Replace(" ", String.Empty));

				myWorker.Eval2($"{rnd.Next(0, byte.MaxValue)}+log10(-tan({rnd.NextDouble() * 3}*cos({rnd.NextDouble() * 3})+{rnd.NextDouble() * 3})" +
					$"+pow({rnd.NextDouble() * 3},-sin({rnd.NextDouble() * 3})+2))/{rnd.NextDouble() * 3}");

				myWorker.Eval2($"-(-(1+log10(-tan({rnd.NextDouble() * 3}*cos({rnd.NextDouble() * 3})+2)+pow({rnd.NextDouble() * 3}," +
					$"-sin({rnd.NextDouble() * 3})+2))/3))");
			}
			catch { 
				failed++; 
			}
		}
		Console.WriteLine($"{nameof(failed)}:{failed}");
	}
}