using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;


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
		private void LoadFileButton_Click(object sender, RoutedEventArgs e)
		{
			OpenFileDialog d = new();
			d.Filter = @"Text files (*.txt)|*.txt|All files (*.*)|*.*";
			d.ShowDialog();

			if (!string.IsNullOrEmpty(d.FileName))
			{
				InputRTB.Document.Blocks.Clear();
				InputRTB.AppendText(File.ReadAllText(d.FileName));
			}
		}

		private void Lab1_Click(object sender, RoutedEventArgs e)
		{
			ClaimMatcher matcher = new ClaimMatcher();

			OutputRTB.Document.Blocks.Clear();
			OutputRTB.AppendText(string.Join("\n\n", matcher.matches(new TextRange(InputRTB.Document.ContentStart, InputRTB.Document.ContentEnd).Text)));
		}
		private void Lab2_Click(object sender, RoutedEventArgs e)
		{
			// 18. целой длинной константой. Модуль > 2^31
			var found = new Regex(@"(?<=(\s|\A)[#]define\s+MYCONST\s+)\d+").Match(new TextRange(InputRTB.Document.ContentStart, InputRTB.Document.ContentEnd).Text).Value;
			if (!string.IsNullOrEmpty(found) && long.TryParse(found, out var parsed) && Math.Abs(parsed) > Math.Pow(2, 31))
			{
				MessageBox.Show($"Объявлена длинная целая константа MYCONST со значением {parsed}");
			}
			else
			{
				MessageBox.Show("Объявления длинной константы MYCONST не найдено.");
			}

		}
		private void Lab3_Click(object sender, RoutedEventArgs e)
		{
			// 18. целой длинной константой. Модуль > 2^31
			var found = new Regex(@"(?<=(\s|\A)[#]define\s+MYCONST\s+)\d+").Match(new TextRange(InputRTB.Document.ContentStart, InputRTB.Document.ContentEnd).Text).Value;
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
			var canBeShortenedRegex = new Regex(@"(?<=(\w*?))[ауоыэяюёие]\w+?");
			// Вариант 2 – для чётных номеров.
			// В тексте сократить все слова после согласных букв, расположенных за первой гласной буквой или последовательными гласными буквами.
			// Знаки препинания должны остаться без изменений.
			StringBuilder sb = new StringBuilder(text.Length * 2 / 3);

			var Vowels = "ауоыэяюёие".ToCharArray();
			int currentWordStart = 0;
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
						if (haveSeenVowel && !(i>0 && Vowels.Contains(text[i-1]))) // не первая гласная и не подряд? оборвать слово.
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
	}
}
