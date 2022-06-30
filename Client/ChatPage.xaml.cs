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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Client.Network;
using Core;

namespace Client
{
    /// <summary>
    /// ChatPage.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ChatPage : Page
    {
        List<string> _chatList = new List<string>();
        public ChatPage()
        {
            InitializeComponent();
        }

        private void OnSendButton(object sender, RoutedEventArgs e)
        {
	        if (this.ChatBox.Text.Length <= 0 || string.IsNullOrWhiteSpace(this.ChatBox.Text))
	        {
		        return;
	        }

	        C_SendChat packet = new C_SendChat();
            packet.Message = this.ChatBox.Text;
            ChatSession.Instance.Send(packet.Write());
            this.ChatBox.Text = "";
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
	        if (e.Key == Key.Enter)
	        {
		        OnSendButton(null, new RoutedEventArgs());
	        }
        }

        private void OnSignOutButton(object sender, RoutedEventArgs e)
        {
            C_RequestSignOut packet = new C_RequestSignOut();
            packet.UserID = 0; // session.UserID;
            //ChatSession.Instance.Send(packet.Write());
        }

        public void RefreshChatListView(string chatMessage)
        {

	        ChatListView.Items.Add(chatMessage);

            _chatList.Add(chatMessage);
	        //ChatListView.ItemsSource = _chatList;
            //ChatListView.Items.Refresh();
            
	        //if (VisualTreeHelper.GetChildrenCount(ChatListView) > 0) 
         //   {
	        //    Border border = (Border)VisualTreeHelper.GetChild(ChatListView, 0);
	        //    ScrollViewer scrollViewer = VisualTreeHelper.GetChild(border, 0) as ScrollViewer;
         //       scrollViewer.ScrollToBottom();
         //   }
        }
    }
}
