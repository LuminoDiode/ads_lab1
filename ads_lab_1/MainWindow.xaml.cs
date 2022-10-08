using Microsoft.Win32;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

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
			InputRTB.AppendText("int a = 1;\r\nint b = 2;\r\nint c =3;\r\ndo {\r\na/b*c} \r\nwhile(a>b)\r\n;\r\nfor(int i =0; i<2;i++) a+=2;\r\nint a = 1;\r\nint b = 2;\r\nint c =3;\r\ndo {a/m*c} while(a>b);\r\nfor(int i =0; i<2;i++) a+=2;\r\ndo {a/m*c} while(a>b);");
		}
		private void LoadTemplate1_Click(object sender, RoutedEventArgs e)
		{
			setCurrentOuput("#define MYCONST 2147483999\r\n\r\nint testarr[255];int a = 1; int b = 2.2;\r\nint c =3;\r\n\r\ndo {\r\na/b*c} \r\nwhile(a>b);\r\n\r\na = 1;\r\ndo {a/m*c} while(a>b);\r\ndo {a//m**c} while(a>b);\r\n\r\nint c = 999;\r\nint\r\nt61 = 2,\r\nt62 = 2.,\r\nt63 = 2.2,\r\nt64 = 2.3e,\r\nt65 = 2.2e10,\r\nt66 = 2e11b;\r\nt67 = 2ee12;\r\n\r\nint bin = 0b11112;\r\nint hex = 0x11112;\r\nint ukn = 0m111;"
				, InputRTB);
		}
		private void LoadTemplate2_Click(object sender, RoutedEventArgs e)
		{
			setCurrentOuput(@"Трагедия Пушкина «Моцарт и Сальери» занимает всего десять страниц. О чем она? О зависти или о том, что «гений и злодейство — две вещи несовместные»? Есть ли оправдание Сальери, который, по версии Пушкина, отравил Моцарта?"
			, InputRTB);
		}
		private void LoadTemplate3_Click(object sender, RoutedEventArgs e)
		{
			setCurrentOuput(@"1+log10(-tan(22*cos(122)+2)+pow(22,-sin(122)+2))/3"
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
		private int CountNewLinesBeforeIndex(string s, int index)
		{
			int count = 0;
			for (int i = 1; i < index; i++)
			{
				if (s[i] == '\n' || s[i - 1] == '\r') count++;
			}
			return count;
		}
		private void Lab1_Click(object sender, RoutedEventArgs e)
		{
			var text = getCurrentInput();

			// Оператор цикла do while и знаки математических операций / и *.
			var matches = new ClaimMatcher(new Regex[] {
				new Regex(@"(?<=(\s|\A))do[\s]*[{]\s*.+\s*[}][\s]*while[(].+[)]\s*[;]"),
				new Regex(@"\w\s*[/]\s*\w"),
				new Regex(@"\w\s*[*]\s*\w")}
			).getMatches(text, out var indexes).ToList();
			var lines = indexes.Select(x => CountNewLinesBeforeIndex(text, x)).ToList();

			OutputRTB.Document.Blocks.Clear();
			for (int i = 0; i < matches.Count; i++)
			{
				OutputRTB.AppendText($"Строка {lines[i] + 1}:\r\n{matches[i]}\r\n\r\n\r\n");
			}
		}

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
			var text = getCurrentInput();
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
					matches = new Regex((@"(?<=(\s|\A|[;])){MYVARTYPE}(\s)+(\w|\d|[,=\[\]]|\s|(\d+\.\d+))+(?=[;])").Replace(@"{MYVARTYPE}", x)).Matches(text)
						.SelectMany(m =>
						{
							var splitType = m.Value.Split("\n\r ".ToCharArray(), 2);

							return splitType[1]
								.Replace("\n", String.Empty)
								.Replace("\r", String.Empty)
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

			setCurrentOuput(text);
		}

		private void Lab6_Click(object sender, RoutedEventArgs e)
		{
			var text = getCurrentInput();

			var allowedSuffixes = "U,l,L,ul, uL, Ul, UL, lu, lU, Lu LU,ll,LL,ull, uLL, Ull, ULL, llu, llU, LLu LLU,f,F,l,L}"
				.Split(" ,".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

			setCurrentOuput(string.Join("\n\n", new Regex(@"(?<=(\s|\A|[\[\(+\-=/*]+?))((\d+(\.\d+)?(e\d+)?)|(0[xb]\d+))(?=[\]\)+\-=/*;,\s])")
				.Matches(text).Select(x => x.Value)));
		}

		private void Lab7_Click(object sender, RoutedEventArgs e)
		{
			var text = getCurrentInput().Replace("\n", String.Empty).Replace("\r", String.Empty).Replace(" ", String.Empty);
			var worker = new StringEvaluator();
			worker.AddFunction("atan", x => Math.Atan(x));
			worker.AddFunction("min", (x, y) => x < y ? x : y);
			worker.AddFunction("min", (x, y) => x < y ? x : y);
			worker.AddFunction("min", (x, y, z) => Math.Min(Math.Min(x, y), z));
			worker.AddOperator("**", (x, y) => Math.Pow(x, y), 6);

			try
			{
				setCurrentOuput(worker.Eval2(text).ToString());
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}
		}

	}
}
