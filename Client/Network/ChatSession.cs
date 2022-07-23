using System;
using System.Net;
using System.Windows;
using System.Windows.Media;
using Core;
using Core.Packet;

namespace Client.Network
{
    public class ChatSession : PacketSession
    {
        public static ChatSession Instance { get; private set; }
        public string _NickName;
        public long _UID; // 추후 사용 
        // accountType은 추후에 추가

        public ChatSession()
        {
            Instance = this;
        }

        public override void OnConnected(EndPoint endPoint)
        {
	        Application.Current.Dispatcher.Invoke(() =>
	        {
		        // 연결에 성공했으므로 타이머를 종료합니다.
		        App app = Application.Current as App;
		        if (app != null)
		        {
			        app.TimerStop();

			        MainWindow mainWindow = Application.Current.MainWindow as MainWindow;
			        mainWindow.StateBlock.Text = "서버와 연결되었습니다.";
			        mainWindow.StateBlock.Foreground = Brushes.Green;
                }
	        });
        }

        public override void OnReceivePacket(ArraySegment<byte> buffer)
        {
            PacketManager.Instance.OnRecvPacket(this, buffer);
        }

        public override void OnSend(int numOfBytes)
        {
            //Console.WriteLine($"보낸 바이트: {numOfBytes}");
        }

        public override void OnDisconnected(EndPoint endPoint)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                // 현재 윈도우를 분리 후, 새로 열 윈도우로 교체한 다음 기존 윈도우를 닫고 새 윈도우를 연다.
	            ChatWindow chatWindow = Application.Current.MainWindow as ChatWindow;
	            if (chatWindow != null)
                {
	                Application.Current.MainWindow = new MainWindow();
	                Application.Current.MainWindow.Show();
	                chatWindow.Close();
                }

                MainWindow mainWindow = Application.Current.MainWindow as MainWindow;
                mainWindow.StateBlock.Text = "서버와 접속이 끊어졌습니다.";
                mainWindow.StateBlock.Foreground = Brushes.Red;

                // 서버에 연결을 다시 시도합니다.
                App app = Application.Current as App;
                if (app != null)
                {
	                app.ConnectServer();
                }
            });

            //Application.Current.Dispatcher.Invoke(() =>
            //{
            //    // 현재 윈도우를 분리 후, 새로 열 윈도우로 교체한 다음 기존 윈도우를 닫고 새 윈도우를 연다.
            //    ChatWindow chatWindow = Application.Current.MainWindow as ChatWindow;
            //    chatWindow.ChatBox.Text = "서버와 연결이 끊겼습니다.";
            //    chatWindow.ChatBox.Foreground = Brushes.Red;

            //});
        }
    }
}
