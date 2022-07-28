using System.Windows;
using System.Windows.Media;


namespace Client
{
    /// <summary>
    /// SignInWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class SignInWindow : Window
    {
        public SignInWindow()
        {
            InitializeComponent();
        }

        private void SignInOK_Clicked(object sender, RoutedEventArgs e)
        {
	        IDInputBox.Text = IDInputBox.Text.Trim();
	        IDInputBox.Text = IDInputBox.Text.Replace(" ", "");
			PWInputBox.Password = PWInputBox.Password.Trim();
			PWInputBox.Password = PWInputBox.Password.Replace(" ", "");

			// id
			if (string.IsNullOrWhiteSpace(IDInputBox.Text))
	        {
		        DisplayMessage.Text = "아이디를 입력하세요.";
		        DisplayMessage.Foreground = Brushes.Red;
		        return;
	        }

			if (IDInputBox.Text.Length < 4 || IDInputBox.Text.Length > 20)
	        {
		        DisplayMessage.Text = "아이디 또는 비밀번호가 다릅니다.";
		        DisplayMessage.Foreground = Brushes.Red;
		        return;
	        }

			// password
	        if (string.IsNullOrWhiteSpace(PWInputBox.Password))
	        {
		        DisplayMessage.Text = "비밀번호를 입력하세요.";
		        DisplayMessage.Foreground = Brushes.Red;
		        return;
	        }

	        if (PWInputBox.Password.Length < 8 || PWInputBox.Password.Length > 20)
	        {
		        DisplayMessage.Text = "아이디 또는 비밀번호가 다릅니다.";
		        DisplayMessage.Foreground = Brushes.Red;
		        return;
	        }

	        this.DialogResult = true;
        }
    }
}
