using Core;
using Core.Packet;
using System;
using System.Runtime.CompilerServices;
using System.Security.Permissions;
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
   [STAThread]
   internal static void S_SuccessSignInHandler(PacketSession arg1, IPacket arg2)
    {
	    ChatSession session = arg1 as ChatSession;
	    S_SuccessSignIn packet = arg2 as S_SuccessSignIn;


        // 세션 정보에 서버로부터 받은 유저정보를 초기화 합니다.

       Application.Current.Dispatcher.Invoke(() =>
        {
	        MainWindow mainWindow = Application.Current.MainWindow as MainWindow;
            // 로그인이 성공적으로 되었으므로
            // 채팅이 가능한 페이지 화면을 윈도우에 추가합니다.
            mainWindow.Content = new ChatPage();

	        Application.Current.Dispatcher.Invoke(() =>
		        {
			        ChatPage chatPage = Application.Current.MainWindow.Content as ChatPage;
				    chatPage.NickNameBlock.Text = $"{packet.UserName}, 환영합니다.";
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
	    ChatSession session = arg1 as ChatSession;
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
        ChatSession session = arg1 as ChatSession;
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
	    ChatSession session = arg1 as ChatSession;
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
        throw new NotImplementedException();
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
        ChatSession session = arg1 as ChatSession;

        string chatMessage = $"{packet.NickName} : {packet.Message}";


        Application.Current.Dispatcher.Invoke(() =>
	        {
				ChatPage chatPage = Application.Current.MainWindow.Content as ChatPage;
				chatPage.RefreshChatListView(chatMessage);
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
}