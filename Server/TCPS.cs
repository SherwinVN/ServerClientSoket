using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Server
{
    public  class TCPS
    {
        //Địa chỉ mặc định
        static string IP = "192.168.1.12";
        static int PORT = 100;

        //Các luồng
        Thread thKetNoiDenServer, thKetNoiTaoServer, thLienLacKhach;
        TcpListener langNghe;

        //Biến dùng để gửi, nhận dữ liệu
        byte[] dlNhan = new byte[1024];
        byte[] dlGui = new byte[1024];

        //Socket tạo server và kết nối
        Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        IPEndPoint iep = new IPEndPoint(IPAddress.Parse(IP), PORT);//IP = "127.0.0.1"; PORT = 100;
                                                                   //Liên kết cổng s với địa chỉ IP và cổng
        
    }
}
