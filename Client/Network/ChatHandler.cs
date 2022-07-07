using Core;
using Core.Packet;
using System;
using System.Runtime.CompilerServices;
using System.Security.Permissions;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;

namespace Client.Network;

public static class ChatHandler
{
	// 윈도우 핸들 얻어오는 방법 중 하나

    public static void OnUserChat(ChatSession session, InPacket packet)
    {
        var name = packet.DecodeString();
        var message = packet.DecodeString();

        Console.WriteLine($"{name} : {message}");
    }

  //  [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
	internal static void S_SuccessSignInHandler(PacketSession arg1, IPacket arg2)
	{
	    S_SuccessSignIn packet = arg2 as S_SuccessSignIn;


        // 세션 정보에 서버로부터 받은 유저정보를 초기화 합니다.
        ChatSession.Instance._NickName = packet.UserName;
        ChatSession.Instance._UID = packet.UserID;
        ChatSession.Instance._UserID = packet.UserName;


        Application.Current.Dispatcher.Invoke(() =>
        {
            // 로그인이 성공적으로 되었으므로
            // 기존 메인 윈도우는 닫고 채팅 윈도우를 엽니다.
            MainWindow mainWindow = Application.Current.MainWindow as MainWindow;

            Application.Current.MainWindow = new ChatWindow();
            Application.Current.MainWindow.Show();
            mainWindow.Close();

            Application.Current.Dispatcher.Invoke(() =>
			{ 
				ChatWindow chatWindow = Application.Current.MainWindow as ChatWindow;
				chatWindow.NickNameBlock.Text = $"Hello! \n {packet.UserName}.";
			}
			);
        });



 


        //mainWindow.Dispatcher.Invoke(() => {
        // mainWindow.StateBlock.Text = $"{packet.UserName}, 환영합니다.";
        //    mainWindow.StateBlock.Foreground = Brushes.Red;
        //});
        //DispatcherFrame frame = new DispatcherFrame();
        //mainWindow.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
        //{
        // frame.Continue = false;
        // mainWindow.StateBlock.Text = $"{packet.UserName}, 환영합니다.";
        // mainWindow.StateBlock.Foreground = Brushes.Green;
        //   }));

        //Dispatcher.PushFrame(frame);


        //      mainWindow.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() =>
        //{
        //	mainWindow.StateBlock.Text = $"{packet.UserName}, 환영합니다.";
        //	mainWindow.StateBlock.Foreground = Brushes.Green;
        //}));
    }


    internal static void S_FailSignInHandler(PacketSession arg1, IPacket arg2)
    {
	    S_FailSignIn packet = arg2 as S_FailSignIn;

	    MainWindow mainWindow = Application.Current.MainWindow as MainWindow;

	    mainWindow.Dispatcher.Invoke(() => {
		    mainWindow.StateBlock.Text = packet.Reason;
		    mainWindow.StateBlock.Foreground = Brushes.Red;
        });

	    mainWindow.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() =>
	    {
	        mainWindow.StateBlock.Text = packet.Reason;
		    mainWindow.StateBlock.Foreground = Brushes.Red;
	    }));

    }

    internal static void S_SuccessSignUpHandler(PacketSession arg1, IPacket arg2)
    {
        S_SuccessSignUp packet = arg2 as S_SuccessSignUp;

        MainWindow mainWindow = Application.Current.MainWindow as MainWindow;

        mainWindow.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() =>
        {
	        mainWindow.StateBlock.Text = packet.Reason;
	        mainWindow.StateBlock.Foreground = Brushes.Green;
        }));

    }

    internal static void S_FailSignUpHandler(PacketSession arg1, IPacket arg2)
    {
	    S_FailSignUp packet = arg2 as S_FailSignUp;

	    MainWindow mainWindow = Application.Current.MainWindow as MainWindow;
	    mainWindow.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() =>
	    {
		    mainWindow.StateBlock.Text = packet.Reason;
		    mainWindow.StateBlock.Foreground = Brushes.Red;
	    }));

	    mainWindow.Dispatcher.Invoke(() => {
		    mainWindow.StateBlock.Text = packet.Reason;
		    mainWindow.StateBlock.Foreground = Brushes.Red;
	    });
    }

    internal static void S_SucessSignOutHandler(PacketSession arg1, IPacket arg2)
    {
	    S_SucessSignOut packet = arg2 as S_SucessSignOut;

	    ChatSession.Instance._NickName = "";
        // 혹은 서버에서 UID를 0으로 안주도록 하거나 양식을 다르게
        ChatSession.Instance._UID = -1;
        ChatSession.Instance._UserID = "";


        Application.Current.Dispatcher.Invoke(() =>
	    {
		    ChatWindow chatWindow = Application.Current.MainWindow as ChatWindow;

		    Application.Current.MainWindow = new MainWindow();
		    Application.Current.MainWindow.Show();
            chatWindow.Close();

		    MainWindow mainWindow = Application.Current.MainWindow as MainWindow;
		    mainWindow.StateBlock.Text = packet.Message;
	    });
    }

    internal static void S_FailSignOutHandler(PacketSession arg1, IPacket arg2)
    {
        throw new NotImplementedException();
    }

    internal static void S_SuccessCommandHandler(PacketSession arg1, IPacket arg2)
    {
        throw new NotImplementedException();
    }

    internal static void S_FailCommandHandler(PacketSession arg1, IPacket arg2)
    {
        throw new NotImplementedException();
    }

    internal static void S_SendChatHandler(PacketSession arg1, IPacket arg2)
    {
        S_SendChat packet = arg2 as S_SendChat;

        //string chatMessage = $"{packet.NickName} : {packet.Message}";


        Application.Current.Dispatcher.Invoke(() =>
	        {
		        ChatWindow chatWindow = Application.Current.MainWindow as ChatWindow;
		        chatWindow.RefreshChatListView(packet.NickName, packet.Message);
	        }
        );

        //Console.SetCursorPosition(session.x, session.y);
        //session.y++;
        //if(packet.NickName == "1")
        {

            //string message = $"[{packet.NickName}] | {packet.Message}";
            //Console.WriteLine(message);
        }
    }

    internal static void S_ExitServerHandler(PacketSession arg1, IPacket arg2)
    {
        throw new NotImplementedException();
    }

    internal static void S_JoinServerHandler(PacketSession arg1, IPacket arg2)
    {
        throw new NotImplementedException();
    }

    internal static void S_UserSignInHandler(PacketSession arg1, IPacket arg2)
    {
        ChatSession session = arg2 as ChatSession;
        S_UserSignIn packet = arg2 as S_UserSignIn;
        
        // 브로드캐스트한 클라와 같은 클라이면 무시한다.
        // 나중에는 NickName이 아니라 UID로 체크하도록 변경이 필요하다.
        if(ChatSession.Instance._NickName == packet.UserName)
        {
	        Thread.Sleep(250);
        }

	    Application.Current.Dispatcher.Invoke(() =>
		    {
			    ChatWindow chatWindow = Application.Current.MainWindow as ChatWindow;
			    chatWindow.AddCurrentUserList(packet.UserName);
		    }
	    );
    }

    internal static void S_UserSignOutHandler(PacketSession arg1, IPacket arg2)
    {
	    ChatSession session = arg2 as ChatSession;
	    S_UserSignOut packet = arg2 as S_UserSignOut;

        // 브로드캐스트한 클라와 같은 클라이면 무시한다.
        // 나중에는 NickName이 아니라 UID로 체크하도록 변경이 필요하다.
        if (ChatSession.Instance._NickName == packet.UserName)
	    {
		    return;
	    }

        Application.Current.Dispatcher.Invoke(() =>
		    {
			    ChatWindow chatWindow = Application.Current.MainWindow as ChatWindow;
			    chatWindow.RemoveCurrentUserList(packet.UserName);
		    }
	    );
    }
}