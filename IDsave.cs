using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace tempclient
{
    internal class IDsave //아이디 관련 클래스
    {

        public string name = "";
        public string password = "";
        public Socket socket;
        public byte[] mode;
        public byte[] data;
        public int login = 0;

        public IDsave()
        {
            this.name = name;
            this.password = password;
            this.socket = socket;

        }
        public IDsave(string name, string password, Socket socket)
        {
            this.name = name;
            this.password = password;
            this.socket = socket;

        }

        public void IdSend()
        {
            string idpw = name + " " + password; 
            mode = Encoding.Unicode.GetBytes("LOG"); data = Encoding.Unicode.GetBytes(idpw);
            byte[] temp = new byte[mode.Length + data.Length];
            Array.Copy(mode, 0, temp, 0, mode.Length);//mode 0부터 mode.length 만큼 temp 0~ 에 저장
            Array.Copy(data, 0, temp, mode.Length, data.Length); //mode 0~length 만큼 temp 0~ 에 저장
            socket.Send(temp);
        }

        public void idcheck(string check) //아이디 비번 맞는지 확인
        {
            if (check == "ACL")
                login = 1;
            else if (check == "FAL")
                login = 2;
        }

        //서버에 저장할 거
        public void sqlsave()
        {

        }

    }
}
