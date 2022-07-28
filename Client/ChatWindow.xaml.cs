using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Client.Network;

namespace Client
{
    /// <summary>
    /// ChatWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public class ChatDataModel
    {
	    public ChatDataModel(string nickname, string chatMessage)
	    {
		    Nickname = nickname;
			ChatMessage = chatMessage;
	    }
	    public string Nickname { get; set; }
	    public string ChatMessage { get; set; }
    }
    public class UserDataModel
    {
	    public UserDataModel(string nickname)
	    {
		    Nickname = nickname;
	    }

	    public override bool Equals(object? obj)
	    {
		    if (base.Equals(obj))
		    {
			    return true;
		    }

			UserDataModel? nickname = obj as UserDataModel;
			if (nickname != null && nickname.Nickname == this.Nickname)
			{
			   return true;
			}

		    return false;
	    }

	    public string Nickname { get; set; }
    }

	public partial class ChatWindow : Window
    {


	    List<ChatDataModel> _chatList = new List<ChatDataModel>();
	    List<string> _userList = new List<string>();
		List<UserDataModel> _userDataModel = new List<UserDataModel>();

		public ChatWindow()
        {
            InitializeComponent();
            ChatListView.ItemsSource = _chatList;
            UserListBox.ItemsSource = _userDataModel;

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
	        packet.UID = ChatSession.Instance._UID;

			ChatSession.Instance.Send(packet.Write());
		}

		public void RefreshChatListView(string nickName, string chatMessage)
        {
			ChatDataModel chatDataModel = new ChatDataModel(nickName, chatMessage);

			_chatList.Add(chatDataModel);

			ChatListView.Items.Refresh();


			if (VisualTreeHelper.GetChildrenCount(ChatListView) > 0)
			{
				FrameworkElement border = (FrameworkElement)VisualTreeHelper.GetChild(ChatListView, 0);
				ScrollViewer scrollViewer = (ScrollViewer)VisualTreeHelper.GetChild(border, 0);
				scrollViewer.ScrollToBottom();
			}

		}

		public void AddCurrentUserList(string nickname)
		{
			UserDataModel user = new UserDataModel(nickname);
			_userDataModel.Add(user);
			UserListBox.Items.Refresh();

		}

		public void AddCurrentUserList(List<UserDataModel> nicknameList)
		{
			_userDataModel.AddRange(nicknameList);
			UserListBox.Items.Refresh();
		}

		public void RemoveCurrentUserList(string nickname)
        {
	        UserDataModel user = new UserDataModel(nickname);

			_userDataModel.Remove(user);
			UserListBox.Items.Refresh();
        }

	}
}
