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
using thinger.cn.DataConvertHelper;

namespace SocketClientDemo
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private Socket ReceiveSocket;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void ConnectionBtn_Click(object sender, RoutedEventArgs e)
        {
            ReceiveSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            ReceiveSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);//有关套接字设置

            try
            {
                //设置服务器IP
                IPAddress ip = IPAddress.Parse(IpTxt.Text);
                int prot = int.Parse(PortTxt.Text);
                IPEndPoint iPEnd = new IPEndPoint(ip, prot);//获取连接的端口号和ip地址
                ReceiveSocket.Connect(iPEnd);//建立连接
                
                ShowMsg("链接成功！");
                Thread tred = new Thread(Recive);
                tred.IsBackground = true;
                tred.Start(ReceiveSocket);
            }
            catch (Exception)
            {
                ShowMsg("ip地址或端口格式有误！");
            }
        }

        /// <summary>
        /// 设置textbox的值
        /// </summary>
        /// <param name="str"></param>
        private void ShowMsg(string str) {
            this.Dispatcher.Invoke(new Action(delegate {
                MsgTxt.AppendText(str + "\r\n");
            }));
           
        }

        private void ClearMsgBtn_Click(object sender, RoutedEventArgs e)
        {
            MsgTxt.Text = null;
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            ReceiveSocket.Dispose();
        }

        private void SendBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string str = SendTxt.Text.ToString().Trim().Replace(" ", "");
                if (str.Length == 1)
                {
                    str = "0" + str;
                }
                byte[] vs = ByteArrayLib.GetByteArrayFromHexStringWithoutSpilt(str);
                ReceiveSocket.Send(vs);
            }
            catch (Exception ex)
            {
                ShowMsg(ex.Message);
            }
          
        }
        /// <summary>
        /// 循环接受服务器端发来的数据
        /// </summary>
        /// <param name="o"></param>
        void Recive(object o)
        {
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
                    if (count == 0)
                    {
                        ShowMsg(socketSend.RemoteEndPoint.ToString() + "：断开连接！--" + DateTime.Now.ToString());
                        break;
                    }

                    byte[] b = new byte[count];
                    Array.Copy(buffer, 0, b, 0, count);

                    double db = SocketSend.GetContext(b);
                    //ShowMsg(socketSend.RemoteEndPoint.ToString() + "：" + str);
                    ShowMsg(socketSend.RemoteEndPoint.ToString() + "服务器返回压力：" + (decimal)db/10+"Mpar"+"报文："+StringLib.GetHexStringFromByteArray(b));


                }
                catch (Exception ex)
                {
                    a = false;
                    socketSend.Dispose();
                    ShowMsg(ex.Message);
                }
            }
        }
    }
}
