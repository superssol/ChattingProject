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

namespace ChatClient
{
    public partial class Form1 : Form
    {
        private Socket socket;
        private Thread receivedThread;

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
            {
                this.Invoke(act);
            }
            else 
                act();

            
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            textBox1.Focus();
            Log("클라이언트 로드됨!");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            IPAddress ipaddress = IPAddress.Parse(textBox1.Text);
            IPEndPoint endPoint = new IPEndPoint(ipaddress, int.Parse(textBox2.Text));

            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            Log("서버에 연결시도중");
            socket.Connect(endPoint);
            Log("서버에 접속됨");

            receivedThread = new Thread(new ThreadStart(receive));
            receivedThread.IsBackground = true;
            receivedThread.Start();

        }

        private void receive()
        {
            while(true)
            {
                byte[] receiveBuffer = new byte[512];
                int length = socket.Receive(receiveBuffer, receiveBuffer.Length, SocketFlags.None);
                string msg = Encoding.UTF8.GetString(receiveBuffer, 0, length);
                showMsg("상대]" + msg);
            }
        }

        

        private void showMsg(string msg)
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
            {
                this.Invoke(act);
            }
            else
                act();
        }

        private void textBox3_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
            if(textBox3.Text.Trim() != "" && e.KeyCode == Keys.Enter)
            {
                byte[] sendBuffer = Encoding.UTF8.GetBytes(textBox3.Text.Trim());
                socket.Send(sendBuffer);
                Log("메세지 전송됨");
                showMsg("나]" + textBox3.Text);
                textBox3.Text = "";
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
