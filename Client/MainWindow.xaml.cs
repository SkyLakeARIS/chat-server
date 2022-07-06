using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
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
using Client.Network;
using Core;

namespace Client
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{

		public MainWindow()
		{
			InitializeComponent();
			//ChatSession.Instance.x = x;
			//ChatSession.Instance.y = y;

			/*
			      Console.SetCursorPosition(chatPOS.Item1, chatPOS.Item2);
			    var message = Console.ReadLine();
			    Console.SetCursorPosition(chatPOS.Item1, chatPOS.Item2);
			    Console.WriteLine("                                          ");

			    if (!string.IsNullOrEmpty(message) && ChatSession.Instance != null)
			    {
			        C_SendChat packet = new C_SendChat() {Message= message};
			        ChatSession.Instance.Send(packet.Write());
			    }

			 */
			//bool isSignIn = false;
			//while (true)
			//{
			//	if (isSignIn) { }

			//	// dummy mode
			//	try
			//	{
			//		SessionManager.Instance.DummyChatForeach();

			//	}
			//	catch (Exception e)
			//	{
			//		Console.WriteLine($"dummy mode / fail send : {e}");
			//	}

			//	// 250인 이유는 초당 4번을 보내도록 하기 위해서.
			//	// 예로 이동패킷 같은 경우는 초에 4번 보내도록 되어있기도 하므로 따라함.
			//	Thread.Sleep(250);
			//}
			StateBlock.Text = "asdsad";
			StateBlock.Foreground = Brushes.Red;
		}

		

		private void OnSignInButton(object sender, RoutedEventArgs e)
        {

			SignInWindow signInWindow = new SignInWindow();
			if (signInWindow.ShowDialog() == true)
			{
				C_RequestSignIn signInPacket = new C_RequestSignIn();
				signInPacket.ID = signInWindow.IDInputBox.Text;
				signInPacket.Password = signInWindow.PWInputBox.Password;
				ChatSession.Instance.Send(signInPacket.Write());
			}
			
			//TextBox textBox = new TextBox();
			//textBox.Name = "new";
			//textBox.Width = 100;
			//textBox.Height = 100;
			//textBox.Text = "buttonClick!";
			//textBox.Visibility = Visibility.Visible;
			//textBox.TextWrapping = TextWrapping.Wrap;
			//textBox.Margin = new Thickness(100, 200, 0, 0);
			//var btn = sender as Button;
			//btn.Content = "selected";

        }

		private void OnSignUpButton(object sender, RoutedEventArgs e)
		{
			SignUpWindow signUpWindow = new SignUpWindow();
			if (signUpWindow.ShowDialog() == true)
			{
				C_RequestSignUp signUpPacket = new C_RequestSignUp();
				signUpPacket.ID = signUpWindow.IDInputBox.Text;
				signUpPacket.Password = signUpWindow.PWInputBox.Password;
				signUpPacket.UserName = signUpWindow.NickNameInputBox.Text;
				ChatSession.Instance.Send(signUpPacket.Write());
			}
		}

		private void OnExitProgram(object sender, RoutedEventArgs e)
		{
			Close();
		}

		public void MainPage(string username)
		{

			StateBlock.Text = $"{username} 환영합니다.";
			StateBlock.Foreground = Brushes.Green;
		}
	}
}
