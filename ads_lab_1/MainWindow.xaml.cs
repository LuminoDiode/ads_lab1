using Microsoft.Win32;
using System;
using System.IO;
using System.Linq;
using System;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Security.Policy;
using System.Collections.Generic;

namespace ads_lab_1
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
			this.InputRTB.AppendText("int a = 1;\r\nint b = 2;\r\nint c =3;\r\ndo {\r\na/b*c} \r\nwhile(a>b)\r\n;\r\nfor(int i =0; i<2;i++) a+=2;\r\nint a = 1;\r\nint b = 2;\r\nint c =3;\r\ndo {a/m*c} while(a>b);\r\nfor(int i =0; i<2;i++) a+=2;\r\ndo {a/m*c} while(a>b);");
		}
		private void LoadTemplate1_Click(object sender, RoutedEventArgs e)
		{
			this.setCurrentOuput("#define MYCONST 2147483999\r\n\r\nint testarr[255];int a = 1; int b = 2.2;\r\nint c =3;\r\n\r\ndo {\r\na/b*c} \r\nwhile(a>b);\r\n\r\na = 1;\r\ndo {a/m*c} while(a>b);\r\ndo {a//m**c} while(a>b);\r\n\r\nint c = 999;\r\nint\r\nt61 = 2,\r\nt62 = 2.,\r\nt63 = 2.2,\r\nt64 = 2.3e,\r\nt65 = 2.2e10,\r\nt66 = 2e11b;\r\nt67 = 2ee12;\r\n\r\nint bin = 0b11112;\r\nint hex = 0x11112;\r\nint ukn = 0m111;"
				, InputRTB);
		}
		private void LoadTemplate2_Click(object sender, RoutedEventArgs e)
		{
			this.setCurrentOuput(@"Трагедия Пушкина «Моцарт и Сальери» занимает всего десять страниц. О чем она? О зависти или о том, что «гений и злодейство — две вещи несовместные»? Есть ли оправдание Сальери, который, по версии Пушкина, отравил Моцарта?"
			, InputRTB);
		}
		private void LoadTemplate3_Click(object sender, RoutedEventArgs e)
		{
			this.setCurrentOuput(@"1+log10(-tan(22*cos(122)+2)+pow(22,-sin(122)+2))/3"
			, InputRTB);
		}


		private string getCurrentInput(RichTextBox RTB = null!) => new TextRange((RTB ?? InputRTB).Document.ContentStart, (RTB ?? InputRTB).Document.ContentEnd).Text;
		private void setCurrentOuput(string text, RichTextBox RTB = null!)
		{
			(RTB ?? OutputRTB).Document.Blocks.Clear();
			(RTB ?? OutputRTB).AppendText(text);
		}

		private void LoadFileButton_Click(object sender, RoutedEventArgs e)
		{
			var fd = new OpenFileDialog { Filter = @"All files (*.*)|*.*" };
			fd.ShowDialog();

			if (!string.IsNullOrEmpty(fd.FileName))
			{
				InputRTB.Document.Blocks.Clear();
				InputRTB.AppendText(File.ReadAllText(fd.FileName));
			}
		}

		private void Lab1_Click(object sender, RoutedEventArgs e)
		{
			// Оператор цикла do while и знаки математических операций / и *.
			ClaimMatcher matcher = new ClaimMatcher(new Regex[] {
				new Regex(@"(?<=(\s|\A))do[\s]*[{]\s*.+\s*[}][\s]*while[(].+[)]\s*[;]"),
				new Regex(@"\w\s*[/]\s*\w"),
				new Regex(@"\w\s*[*]\s*\w")}
			);

			OutputRTB.Document.Blocks.Clear();
			OutputRTB.AppendText(string.Join("\n\n", matcher.getMatches(getCurrentInput())));
		}
		//private void Lab1_Click2(object sender, RoutedEventArgs e)
		//{

		//	// Оператор цикла do while и знаки математических операций / и *.
		//	ClaimMatcher matcher = new ClaimMatcher(new Regex[] {
		//		new Regex(@"(?<=(\s|\A))do[\s]*[{]\s*.+\s*[}][\s]*while[(].+[)]\s*[;]"),
		//		new Regex(@"[^/][/][^/]"),
		//		new Regex(@"[^*][*][^*]")}
		//	);

		//	matcher.getMatchesIndexes(getCurrentInput()).ToList().ForEach(x =>
		//	{
		//		Debug.WriteLine($"Detected {countNewLinesBeforeIndex(x.Start)} \\r|\\n before start index");
		//		Debug.WriteLine($"Detected {countNewLinesBeforeIndex(x.Start + x.Count)} \\r|\\n before stop index");
		//		Debug.WriteLine($"Start index is adjusted from {x.Start} to {x.Start + countNewLinesBeforeIndex(x.Start)}");
		//		Debug.WriteLine($"Stop index is adjusted from {x.Start+x.Count} to {x.Start + countNewLinesBeforeIndex(x.Start + x.Count)}");
		//		try
		//		{
		//			HightlightText(
		//			x.Start + countNewLinesBeforeIndex(x.Start), // оптимизация под несколько индексов одновременно?
		//			x.Count + countNewLinesBeforeIndex(x.Start + x.Count),
		//			this.InputRTB);
		//		}
		//		catch { }
		//		Debug.WriteLine($"Text from {x.Start + countNewLinesBeforeIndex(x.Start)} to {x.Start + countNewLinesBeforeIndex(x.Start + x.Count)} is being highlighted");
		//	});
		//}
		//private void HightlightText(int startIndex, int Count, RichTextBox RTB = null!)
		//{
		//	if (RTB is null) RTB = this.InputRTB;
		//	var t = new TextRange(RTB.Document.ContentStart.GetPositionAtOffset(startIndex), RTB.Document.ContentStart.GetPositionAtOffset(startIndex + Count - 1)).Text;
		//	var range = new TextRange(RTB.Document.ContentStart.GetPositionAtOffset(startIndex), RTB.Document.ContentStart.GetPositionAtOffset(startIndex + Count - 1));
		//	range.ApplyPropertyValue(TextElement.BackgroundProperty, Brushes.Yellow);
		//}	
		//private int countNewLinesBeforeIndex(int index, RichTextBox RTB = null!)
		//{
		//	var text = getCurrentInput(RTB);
		//	int seenNewLines = 0;
		//	var toMatch = new char[] { '\r','\n' };
		//	for (int i = 0; i < (index + seenNewLines-1) && i < text.Length-1; i++)
		//	{
		//		if (text[i]=='\r' && text[i+1]=='\n') seenNewLines++;
		//	}
		//	return 0;
		//	return seenNewLines;
		//}

		private void Lab2_Click(object sender, RoutedEventArgs e)
		{
			// 18. целой длинной константой. Модуль > 2^31
			var found = new Regex(@"(?<=(\s|\A)[#]define\s+MYCONST\s+)\d+").Match(getCurrentInput()).Value;
			if (!string.IsNullOrEmpty(found) && long.TryParse(found, out var parsed) && Math.Abs(parsed) > Math.Pow(2, 31))
			{
				MessageBox.Show($"Объявлена длинная целая константа MYCONST со значением {parsed}");
			}
			else
			{
				MessageBox.Show("Объявления длинной константы MYCONST не найдено.");
			}

		}

		private void Lab4_Click(object sender, RoutedEventArgs e)
		{
			var text = new TextRange(InputRTB.Document.ContentStart, InputRTB.Document.ContentEnd).Text;
			// Вариант 2 – для чётных номеров.
			// В тексте сократить все слова после согласных букв, расположенных за первой гласной буквой или последовательными гласными буквами.
			// Знаки препинания должны остаться без изменений.
			StringBuilder sb = new StringBuilder(text.Length * 2 / 3);

			var Vowels = "ауоыэяюёие".ToCharArray();

			bool haveSeenVowel = false;
			bool dotAdded = false;
			for (int i = 0; i < text.Length; i++)
			{
				if (char.IsPunctuation(text[i])) // пунктуация сохранена
				{
					sb.Append(text[i]);
				}
				else if (char.IsWhiteSpace(text[i])) // пробелы сохранены и обнуляют флаги
				{
					haveSeenVowel = false;
					dotAdded = false;
					sb.Append(text[i]);
				}
				else if (!dotAdded) // если точка уже есть, то только пробелы и пунктуация
				{
					if (Vowels.Contains(text[i])) // гласная?
					{
						if (haveSeenVowel && !(i > 0 && Vowels.Contains(text[i - 1]))) // не первая гласная и не подряд? оборвать слово.
						{
							sb.Append('.');
							dotAdded = true;
						}
						else // первая гласная? - продолжить далее.
						{
							sb.Append(text[i]);
							haveSeenVowel = true;
						}
					}
					else
					{
						sb.Append(text[i]);
					}
				}
			}

			OutputRTB.Document.Blocks.Clear();
			OutputRTB.AppendText(sb.ToString());
		}

		private struct DeclaredVariable
		{
			public string Type;
			public string Name;
		}
		private void Lab5_Click(object sender, RoutedEventArgs e)
		{
			// Привести к венгерскому неймингу ве переменные типов 18. int, double
			var searchTypes = (new string[] { "int", "double" }).ToList();

			var text = getCurrentInput();

			searchTypes
				.Select(x => new
				{
					matches = new Regex((@"(?<=(\s|\A|[;])){MYVARTYPE}\s+(\w|\d|[,=\[\]]|\s|(\d+\.\d+))+(?=[;])").Replace(@"{MYVARTYPE}", x)).Matches(getCurrentInput())
						.SelectMany(m =>
						{
							var splitType = m.Value.Split(' ', 2);

							Debug.WriteLine(string.Join(' ', splitType[1]
								.Replace("\n", String.Empty)
								.Replace("\r", String.Empty)
								//.Replace("[", String.Empty)
								//.Replace("]", String.Empty)
								.Split("[],; =".ToCharArray(), System.StringSplitOptions.RemoveEmptyEntries)
								.Select(x => x.Trim()).Distinct().Where(x => char.IsLetter(x[0]))));

							return splitType[1]
								.Replace("\n", String.Empty)
								.Replace("\r", String.Empty)
								//.Replace("[", String.Empty)
								//.Replace("]", String.Empty)
								.Split("[],;".ToCharArray(), System.StringSplitOptions.RemoveEmptyEntries)
								.Select(x => x.Split("=", System.StringSplitOptions.RemoveEmptyEntries)[0].Trim())
								.Where(x => char.IsLetter(x[0]))
								.Select(x => new DeclaredVariable { Type = splitType[0], Name = x }).ToList();
						}).DistinctBy(x => x.Name).ToList(),
					typeName = x
				}).ToList()
				.ForEach(x =>
					x.matches
					.ForEach(m => text = new Regex((@"(?<=[^\w]+){MYVARNAME}(?!^\w+)").Replace(@"{MYVARNAME}", m.Name))
					.Replace(text, x.typeName.Substring(0, 1) + m.Name))); ;

			//Debug.WriteLine($"Found those var names:\n{string.Join("\n", foundVarNames.Select(x => x.typeName + " : " + string.Join(',', x.matches.Select(y => y.Name))))}");


			//System.Collections.Generic.List<string> declaredNames = new();
			//foundVarNames
			//	.ForEach(x =>
			//		x.matches.ForEach(m => text = new Regex((@"(?<=[^\w]+){MYVARNAME}(?!^\w+)").Replace(@"{MYVARNAME}", m.Name)).Replace(text, x.typeName.Substring(0, 1) + m.Name)));

			setCurrentOuput(text);
		}
		//private System.Collections.Generic.List<DeclaredVariable> getVarNamesFromDeclaration(string declarationString)
		//{
		//	var splitType = declarationString.Split(' ', 2);

		//	return splitType[1]
		//		.Replace("\n", String.Empty)
		//		.Replace("\r", String.Empty)
		//		.Split(",;".ToCharArray(), System.StringSplitOptions.RemoveEmptyEntries)
		//		.Select(x => x.Split("=", System.StringSplitOptions.RemoveEmptyEntries)[0].Trim())
		//		.Select(x=> new DeclaredVariable { Type = splitType[0], Name=x }).ToList();
		//}


		private void Lab6_Click(object sender, RoutedEventArgs e)
		{
			var text = getCurrentInput();
			/* Определить, содержатся ли в текстовых строках вещественные числа. 
			 * Правила образования чисел соответствуют правилам языка C++. 
			 * Вещественное число может быть записано в экспоненциальной форме «2.25e1» 
			 * и/или в нормализованной форме «22.5».
			 */
			ClaimMatcher matcher = new ClaimMatcher(new Regex[] {
				new Regex(@"(?<=^\w)\d+(\.?e?\d+)?(?!^\w)") }
			);

			var allowedSuffixes = "U,l,L,ul, uL, Ul, UL, lu, lU, Lu LU,ll,LL,ull, uLL, Ull, ULL, llu, llU, LLu LLU,f,F,l,L}"
				.Split(" ,".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

			var t = new Regex(@"(?<=(\W))\d+(\.?e?\d+)?(?=\W)").Matches(text).Select(x => x.Value);

			//var decReg = new Regex(@"(?<=(\s|\A|[+\-=/*]+?))\d+(\.\d+)?(e\d+)?({SSFX})?(?=[+\-=/*;,\s])".Replace("{SSFX}", string.Join('|', allowedSuffixes)));
			//var hexAndBinReg = new Regex(@"(?<=(\s|\A|[+\-=/*]+?))0x(\d|[ABCDEF])+(?=[+\-=/*;,\s])");
			//var binReg = new Regex(@"(?<=(\s|\A|[+\-=/*]+?))0b(\d|[ABCDEF])+(?=[+\-=/*;,\s])");
			setCurrentOuput(string.Join("\n\n", new Regex(@"(?<=(\s|\A|[\[\(+\-=/*]+?))((\d+(\.\d+)?(e\d+)?)|(0[xb]\d+))(?=[\]\)+\-=/*;,\s])").Matches(text).Select(x => x.Value)));
		}

		private void Lab7_Click(object sender, RoutedEventArgs e)
		{
			var text = getCurrentInput().Replace("\r\n",String.Empty);
			var worker = new StringEvaluator();

			try
			{
				setCurrentOuput(worker.Eval2(text).ToString());
			}
			catch(Exception ex)
			{
				MessageBox.Show(ex.Message);
			}
		}

	}
}
