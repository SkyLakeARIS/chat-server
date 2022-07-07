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

			if (IDInputBox.Text.Length <= 0 || PWInputBox.Password.Length <= 0)
	        {
				DisplayMessage.Text = "아이디나 비밀번호를 입력하세요.";
		        DisplayMessage.Foreground = Brushes.Red;
		        return;
			}

			if (IDInputBox.Text.Length >= 20 )
	        {
		        DisplayMessage.Text = "아이디가 너무 깁니다.";
		        DisplayMessage.Foreground = Brushes.Red;
		        return;
	        }
			if (PWInputBox.Password.Length < 8 || PWInputBox.Password.Length > 20)
	        {
		        DisplayMessage.Text = "패스워드는 8이상 20이하";
		        DisplayMessage.Foreground = Brushes.Red;
		        return;
	        }

			this.DialogResult = true;
        }
    }
}
