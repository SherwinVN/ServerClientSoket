using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Client
{
    public partial class Client : Form
    {

        private Socket clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        byte[] receivedBuf = new byte[1024];
        public Client()
        {
            InitializeComponent();

            txt_port.Text = "2203";
            timer1.Tick += Timer1_Tick;
            timer1.Start();

        }

        private void Timer1_Tick(object sender, EventArgs e)
        {
            lab_dong_ho.Text = DateTime.Now.ToString("dd/MM/yyyy HH:MM:ss");
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            if (clientSocket.Connected)
            {

                byte[] buffer = Encoding.UTF8.GetBytes(txt_text.Text);
                clientSocket.Send(buffer);
            }

        }
        private void ReceiveData(IAsyncResult ar)
        {
            if (clientSocket.Connected)
            {
                Socket socket = (Socket)ar.AsyncState;
                int received = socket.EndReceive(ar);
                byte[] dataBuf = new byte[received];
                Array.Copy(receivedBuf, dataBuf, received);
                lb_stt.Text = "Nhận: " + (Encoding.ASCII.GetString(dataBuf));
                if (Encoding.ASCII.GetString(dataBuf) == "BYE")
                {
                    clientSocket.Shutdown(SocketShutdown.Both);
                    clientSocket.Dispose();
                    clientSocket.Close();

                }
                else

                clientSocket.BeginReceive(receivedBuf, 0, receivedBuf.Length, SocketFlags.None, new AsyncCallback(ReceiveData), clientSocket);
            }
            
        }

        private void LoopConnect()
        {
            int attempts = 0;
            while (!clientSocket.Connected)
            {
                try
                {
                    attempts++;
                    clientSocket.Connect(IPAddress.Parse(txIP.Text), int.Parse(txt_port.Text));
                    lb_stt.Text = ("Kết nối!");
                }
                catch (SocketException)
                {
                    lb_stt.Text = ("Lỗi Kết nối: " + attempts.ToString());
                }
            }
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            LoopConnect();
            clientSocket.BeginReceive(receivedBuf, 0, receivedBuf.Length, SocketFlags.None, new AsyncCallback(ReceiveData), clientSocket);
            byte[] buffer = Encoding.ASCII.GetBytes("IP Clien:" + LocalIPAddress().ToString());
            clientSocket.Send(buffer);

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
    }
}
