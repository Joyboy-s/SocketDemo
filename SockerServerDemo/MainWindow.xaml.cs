using SocketLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
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
using System.Windows.Threading;
using thinger.cn.DataConvertHelper;

namespace SocketServerDemo
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        //创建Socker对象
        private Socket ReceiveSocket;
        public MainWindow()
        {
            InitializeComponent();
        }
        
        private void ShowMsg(string str) {
            this.Dispatcher.Invoke(new Action(delegate
            {
                MsgText.AppendText(str + "\r\n");
            }));
         
        }
        /// <summary>
        /// 开始监听
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ListenSocker_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int port = 8885;
                IPAddress iP = IPAddress.Any;//侦听所有网络接口
                ReceiveSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                ReceiveSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);//有关套接字设置
                IPEndPoint iPEnd = new IPEndPoint(iP, port);
                ReceiveSocket.Bind(iPEnd);//绑定ip地址和端口号
                ReceiveSocket.Listen(10);//设置最多可以有10个排队链接请求
                
                ShowMsg("开始监听... ...");
                //使用多线程创建与客户端的通信
                Thread thread = new Thread(ListenSocket);
                thread.IsBackground = true;
                thread.Start(ReceiveSocket);
            }
            catch (Exception ex)
            {
                ShowMsg("开始监听失败："+ex.Message);
            }

           
        }
        /// <summary>
        /// 请求监听
        /// </summary>
        /// <param name="o">socket对象</param>
        void ListenSocket(object o) {
            Socket ReceiveSocket = o as Socket;
            bool a= true;
            try
            {
                while (a)
                {
                    Socket socketSend = ReceiveSocket.Accept();//创建同于通讯的socket
                    ShowMsg(socketSend.RemoteEndPoint.ToString() + "：连接成功 --"+DateTime.Now.ToString());
                    Thread thread = new Thread(Recive);
                    thread.IsBackground = true;
                    thread.Start(socketSend);


                    Thread.Sleep(100);
                }
            }
            catch (Exception)
            {
                ReceiveSocket.Dispose();
            } 
         

        }
       /// <summary>
       /// 循环接受客户端发来的数据
       /// </summary>
       /// <param name="o"></param>
        void Recive(object o) {
            Socket socketSend = o as Socket;
            bool a = true;
            while (a)
            {
                try
                {
                    byte[] buffer = new byte[1024 * 1024 * 2];
                    //实际接收到的有效字节数
                    int count = socketSend.Receive(buffer);
                    string str = Encoding.UTF8.GetString(buffer, 0, count);
                    if (count==0)
                    {
                        ShowMsg(socketSend.RemoteEndPoint.ToString() + "：断开连接！--" + DateTime.Now.ToString());
                        break;
                    }

                    byte[] b = new byte[count];
                    Array.Copy(buffer, 0, b, 0, count);
                    //ShowMsg(socketSend.RemoteEndPoint.ToString() + "：" + str);
                    ShowMsg(socketSend.RemoteEndPoint.ToString() + "：" + ByteToStr(b));

                    //服务器端发送
                    SendContext(socketSend);

                }
                catch (Exception ex)
                {
                    a = false;
                    socketSend.Dispose();
                    ShowMsg(ex.Message);
                }
            }
        }


        /// <summary>
        /// 服务器接收
        /// </summary>
        /// <param name="socketSend"></param>
        private void SendContext(Socket socketSend)
        {
            //随机数
            Random rm = new Random();
            int ma = rm.Next(1,1025);
            SocketSend sd = new SocketSend();
         
            byte[] sendby = SocketSend.Sendcontext(ma);
            socketSend.Send(sendby);
            ShowMsg("当前液压"+ (decimal)ma/10);
        }

        /// <summary>
        /// 清空消息框
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ClearMsg_Click(object sender, RoutedEventArgs e)
        {
            MsgText.Text = null;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Control.CheckForIllegalCrossThreadCalls = false; //设置线程

         
        }
        /// <summary>
        /// 把byte转换成字符串
        /// </summary>
        /// <param name="vs"></param>
        /// <returns></returns>
        public string ByteToStr(byte[] vs)
        {
            string returnStr = "";
            if (vs != null)
            {
                for (int i = 0; i < vs.Length; i++)
                {
                    returnStr += vs[i].ToString("X").PadLeft(2, '0') + " ";
                }
            }
            return returnStr;
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            ReceiveSocket.Dispose();
        }
    }
}
