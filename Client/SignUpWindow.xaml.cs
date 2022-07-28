using System.Windows;
using System.Windows.Media;


namespace Client
{
    /// <summary>
    /// SignUpWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class SignUpWindow : Window
    {
        public SignUpWindow()
        {
            InitializeComponent();
        }

        private void SummitButton_OnClick(object sender, RoutedEventArgs e)
        {
	        IDInputBox.Text = IDInputBox.Text.Trim();
	        IDInputBox.Text = IDInputBox.Text.Replace(" ", "");
	        PWInputBox.Password = PWInputBox.Password.Trim();
			PWInputBox.Password = PWInputBox.Password.Replace(" ", "");
			NickNameInputBox.Text = NickNameInputBox.Text.Trim();

			// id
	        if (string.IsNullOrWhiteSpace(IDInputBox.Text))
	        {
		        StateBox.Text = "아이디를 입력하세요.";
		        StateBox.Foreground = Brushes.Red;
		        return;
	        }

			if (IDInputBox.Text.Length < 4 || IDInputBox.Text.Length > 20)
	        {
		        StateBox.Text = "아이디는 4자 이상 20자 이하여야 합니다.";
		        StateBox.Foreground = Brushes.Red;
		        return;
	        }

			// password
	        if (string.IsNullOrWhiteSpace(PWInputBox.Password))
	        {
		        StateBox.Text = "비밀번호를 입력하세요.";
		        StateBox.Foreground = Brushes.Red;
		        return;
	        }

			if (PWInputBox.Password.Length < 8 || PWInputBox.Password.Length > 20)
	        {
		        StateBox.Text = "패스워드는 8자 이상 20자 이하여야 합니다.";
		        StateBox.Foreground = Brushes.Red;
		        return;
	        }

			// nickname
	        if (string.IsNullOrWhiteSpace(NickNameInputBox.Text))
	        {
		        StateBox.Text = "닉네임을 입력하세요.";
		        StateBox.Foreground = Brushes.Red;
		        return;
	        }

	        if (NickNameInputBox.Text.Length < 2 || NickNameInputBox.Text.Length > 10)
	        {
		        StateBox.Text = "닉네임은 2자 이상 10자 이하여야 합니다.";
		        StateBox.Foreground = Brushes.Red;
		        return;
	        }

	        this.DialogResult = true;
		}

    }
}
