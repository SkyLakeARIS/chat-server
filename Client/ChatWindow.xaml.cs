using System;
using System.Collections.Generic;
using System.Collections.Specialized;
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
using Client.Network;

namespace Client
{
    /// <summary>
    /// ChatWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    class ChatDataModel
    {
	    public ChatDataModel(string nickName, string chatMessage)
	    {
		    NickName = nickName;
			ChatMessage = chatMessage;
	    }
	    public string NickName { get; set; }
	    public string ChatMessage { get; set; }
    }
    public partial class ChatWindow : Window
    {


	    List<ChatDataModel> _chatList = new List<ChatDataModel>();
	    List<string> _userList = new List<string>();

		public ChatWindow()
        {
            InitializeComponent();
            ChatListView.ItemsSource = _chatList;
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
	        packet.UserID = ChatSession.Instance._UID;

			ChatSession.Instance.Send(packet.Write());
		}

		public void RefreshChatListView(string nickName, string chatMessage)
        {
			ChatDataModel chatDataModel = new ChatDataModel(nickName, chatMessage);

			//ChatListView.Items.Add(chatDataModel);

			_chatList.Add(chatDataModel);

			ChatListView.Items.Refresh();

			//ChatListView.SelectedIndex = ChatListView.Items.Count - 1;
			//ChatListView.ScrollIntoView(ChatListView.SelectedItem);

			if (VisualTreeHelper.GetChildrenCount(ChatListView) > 0)
			{
				FrameworkElement border = (FrameworkElement)VisualTreeHelper.GetChild(ChatListView, 0);
				ScrollViewer scrollViewer = (ScrollViewer)VisualTreeHelper.GetChild(border, 0);
				scrollViewer.ScrollToBottom();
			}

			//ChatListView.ItemsSource = _chatList;
			//ChatListView.Items.Refresh();

			//if (VisualTreeHelper.GetChildrenCount(ChatListView) > 0) 
			//   {
			//    Border border = (Border)VisualTreeHelper.GetChild(ChatListView, 0);
			//    ScrollViewer scrollViewer = VisualTreeHelper.GetChild(border, 0) as ScrollViewer;
			//       scrollViewer.ScrollToBottom();
			//   }
		}

		public void AddCurrentUserList(string userName)
        {

	        UserListBox.Items.Add(userName);

	        _userList.Add(userName);
	        //ChatListView.ItemsSource = _chatList;
	        //ChatListView.Items.Refresh();

	        //if (VisualTreeHelper.GetChildrenCount(ChatListView) > 0) 
	        //   {
	        //    Border border = (Border)VisualTreeHelper.GetChild(ChatListView, 0);
	        //    ScrollViewer scrollViewer = VisualTreeHelper.GetChild(border, 0) as ScrollViewer;
	        //       scrollViewer.ScrollToBottom();
	        //   }
        }

        public void RemoveCurrentUserList(string userName)
        {

	        var list = UserListBox.Items;
			list.Remove(userName);
	        _userList.Remove(userName);
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
