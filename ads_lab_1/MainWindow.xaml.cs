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

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			ClaimMatcher matcher = new ClaimMatcher();

			OutputRTB.Document.Blocks.Clear();
			OutputRTB.AppendText(string.Join("\n\n", matcher.matches(new TextRange(InputRTB.Document.ContentStart, InputRTB.Document.ContentEnd).Text)));
		}

		private void Button_Click_1(object sender, RoutedEventArgs e)
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
	}
}
