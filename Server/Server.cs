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
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Server
{
    public partial class Server : Form
    {
        public Socket _Socket { get; set; }
        public List<Socket> listS = new List<Socket>();
        private byte[] _buffer = new byte[1024];
        private Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        public string _Name { get; set; }


        public Server()
        {
            InitializeComponent();
           
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {

        }
        private IPAddress LocalIPAddress()
        {
            if (!System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable())
            {
                return null;
            }

            IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
            return host
                .AddressList
                .FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork);
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            txt_port.Text = "2203";
            txt_ip.Text =LocalIPAddress().ToString();
            serverSocket.Bind(new IPEndPoint(LocalIPAddress(), int.Parse(txt_port.Text)));
            serverSocket.Listen(1);
            serverSocket.BeginAccept(new AsyncCallback(AppceptCallback), null);

            timer1.Tick += Timer1_Tick;
            timer1.Start();
            
        }

        private void Timer1_Tick(object sender, EventArgs e)
        {
            lab_dong_ho.Text = DateTime.Now.ToString("dd/MM/yyyy HH:MM:ss");
        }

        private void AppceptCallback(IAsyncResult ar)
        {
            Socket socket = serverSocket.EndAccept(ar);
            listS.Add(socket);
            socket.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), socket);
            serverSocket.BeginAccept(new AsyncCallback(AppceptCallback), null);
        }
        [STAThreadAttribute]
        private void ReceiveCallback(IAsyncResult ar)
        {

            Socket socket = (Socket)ar.AsyncState;
            if (socket.Connected)
            {
                int received;
                try
                {
                    received = socket.EndReceive(ar);
                }
                catch (Exception)
                {
                    // client đóng kết nối  socket.RemoteEndPoint.ToString()
                    for (int i = 0; i < listS.Count; i++)
                    {
                        if (listS[i].RemoteEndPoint.ToString().Equals(socket.RemoteEndPoint.ToString()))
                        {
                            listS.RemoveAt(i);
                        }
                    }
                    // xóa trong list
                    return;
                }
                if (received != 0)
                {
                    byte[] dataBuf = new byte[received];
                    Array.Copy(_buffer, dataBuf, received);
                    string _text = Encoding.UTF8.GetString(dataBuf);

                    Clipboard.SetText(_text);                    
                    lb_stt.Text = _text+" Tin nhắn: " + Clipboard.GetText();
                    Sendata(socket, "OK");
                    SendKeys.SendWait("^v");
                }
                
            }
            socket.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), socket);
        }
        void Sendata(Socket socket, string noidung)
        {
            byte[] data = Encoding.ASCII.GetBytes(noidung);
            socket.BeginSend(data, 0, data.Length, SocketFlags.None, new AsyncCallback(SendCallback), socket);
            serverSocket.BeginAccept(new AsyncCallback(AppceptCallback), null);
        }
        private void SendCallback(IAsyncResult AR)
        {
            Socket socket = (Socket)AR.AsyncState;
            socket.EndSend(AR);
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < listS.Count; i++)
            {
               Sendata(listS[i], txt_text.Text);
            }

        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            for (int i = 0; i < listS.Count; i++)
            {
                Sendata(listS[i], "BYE");
            }

        }
        public static class Clipboard
        {
            public static void SetText(string p_Text)
            {
                Thread STAThread = new Thread(
                    delegate ()
                    {
                // Use a fully qualified name for Clipboard otherwise it
                // will end up calling itself.
                System.Windows.Forms.Clipboard.SetText(p_Text);
                    });
                STAThread.SetApartmentState(ApartmentState.STA);
                STAThread.Start();
            }
            public static string GetText()
            {
                string ReturnValue = string.Empty;
                Thread STAThread = new Thread(
                    delegate ()
                    {
                // Use a fully qualified name for Clipboard otherwise it
                // will end up calling itself.
                ReturnValue = System.Windows.Forms.Clipboard.GetText();
                    });
                STAThread.SetApartmentState(ApartmentState.STA);
                STAThread.Start();

                return ReturnValue;
            }
        }


    }
}
