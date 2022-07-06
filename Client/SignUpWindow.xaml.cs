using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

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
	        if (IDInputBox.Text.Length <= 0 || PWInputBox.Password.Length <= 0)
	        {
		        StateBox.Text = "아이디나 비밀번호를 입력하세요.";
		        StateBox.Foreground = Brushes.Red;
		        return;
	        }

	        if (NickNameInputBox.Text.Length <= 0)
	        {
		        StateBox.Text = "닉네임을 입력하세요.";
		        StateBox.Foreground = Brushes.Red;
		        return;
	        }

	        if (NickNameInputBox.Text.Length > 10)
	        {
		        StateBox.Text = "닉네임은 10자리 이하";
		        StateBox.Foreground = Brushes.Red;
		        return;
	        }

			if (IDInputBox.Text.Length >= 20)
	        {
		        StateBox.Text = "아이디가 너무 깁니다.";
		        StateBox.Foreground = Brushes.Red;
		        return;
	        }

	        if (PWInputBox.Password.Length < 8 || PWInputBox.Password.Length > 20)
	        {
		        StateBox.Text = "패스워드는 8이상 20이하";
		        StateBox.Foreground = Brushes.Red;
		        return;
	        }

	        this.DialogResult = true;
		}

    }
}
