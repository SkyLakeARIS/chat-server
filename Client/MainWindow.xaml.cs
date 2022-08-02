using System;
using System.Windows;
using System.Windows.Media;
using Client.Network;

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
        }
		private void OnSignUpButton(object sender, RoutedEventArgs e)
		{
			SignUpWindow signUpWindow = new SignUpWindow();
			if (signUpWindow.ShowDialog() == true)
			{
				C_RequestSignUp signUpPacket = new C_RequestSignUp();
				signUpPacket.ID = signUpWindow.IDInputBox.Text;
				signUpPacket.Password = signUpWindow.PWInputBox.Password;
				signUpPacket.Nickname = signUpWindow.NickNameInputBox.Text;
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
