using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace ChatServer
{
    public partial class Form1 : Form
    {
        private string ip = "127.0.0.1";
        private int port = 8090;
        private Thread listenThread;        
        private Thread receiveThread;       
        private Socket clientSocket;
       
        public Form1()
        {
            InitializeComponent();
            
        }

        private void Log(string msg)
        {
            Action act = () =>
            {
                listBox1.Items.Add(string.Format("[{0}]{1}", DateTime.Now.ToString(), msg));
            };
            if (this.InvokeRequired)
                this.Invoke(act);
            else
                act();
        }
      
        private void button1_Click(object sender, EventArgs e)
        {
            if(button1.Text == "시작")
            {
                button1.Text = "멈춤";
                Log("서버시작됨");

                listenThread = new Thread(new ThreadStart(Listen));
                listenThread.IsBackground = true;
                listenThread.Start();
            }
            else
            {
                button1.Text = "시작";
                Log("서버 멈춤");
            }
        }

        private void Listen()
        {
            IPAddress ipaddress = IPAddress.Parse(ip);
            IPEndPoint endPoint = new IPEndPoint(ipaddress, port);

            Socket listenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            listenSocket.Bind(endPoint);

            listenSocket.Listen(50);

            Log("클라이언트 요청 대기중...");

            clientSocket = listenSocket.Accept();

            Log("클라이언트 접속됨");

            receiveThread = new Thread(new ThreadStart(Receive));
            receiveThread.IsBackground = true;
            receiveThread.Start();
        }

        private void Receive()
        {
            while(true)
            {
                if (clientSocket.Connected)
                {
                    byte[] receiveBuffer = new byte[512];
                    int length = clientSocket.Receive(receiveBuffer, receiveBuffer.Length, SocketFlags.None);
                    string msg = Encoding.UTF8.GetString(receiveBuffer);
                    showmsg("상대]" + msg);
                    Log("메세지 수신함");
                }
                else
                {
                    //  재접속
                }
            }
        }

        private void showmsg(string msg)
        {
            Action act = () =>
            {
                richTextBox1.AppendText(msg);
            richTextBox1.AppendText("\r\n");

            this.Activate();
            richTextBox1.Focus();

            richTextBox1.SelectionStart = richTextBox1.Text.Length;
            richTextBox1.ScrollToCaret();
            };
            if (this.InvokeRequired)
                this.Invoke(act);
            else
                act();
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (textBox1.Text.Trim() != "" && e.KeyCode == Keys.Enter)
            {
                byte[] senderBuffer = Encoding.UTF8.GetBytes(textBox1.Text.Trim());
                clientSocket.Send(senderBuffer);
                Log("메시지 전송됨");
                showmsg("나]" + textBox1.Text);
                textBox1.Text = "";
            }
        }
    }
}
