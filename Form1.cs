using System;
using System.Globalization;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
namespace tempclient
{
    public partial class Form1 : Form
    {
        public static IPEndPoint ip = new IPEndPoint(IPAddress.Parse("115.40.241.164"), port: 9000);
        Socket ClientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        public static String temp_data = "";
        public static Boolean key = false; //채팅 가능 여부
        public string name;
        public string mode = ""; //데이터 이름
        public string data = ""; // 데이터 이름 제외시킨 순수 데이터
        public int temptemp = 1; //임시 변수 (회원가입 실패 때 사용)
        public static Boolean serverON = false;
        IDsave id = new IDsave();
        XY xy = new XY(); 
        Room room = new Room();
        public Form1()
        {
            InitializeComponent();
        }


        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                ClientSocket.Connect(ip);
                MessageBox.Show("서버가 연결됨");
                serverON = true;
                new Thread(Reciever).Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show("서버가 연결되지 않음");
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (key == true)
            {
                data = textBox8.Text + ","+ name + "," + textBox2.Text;
                DataBroadCast(Encoding.Unicode.GetBytes("MSG"), Encoding.Unicode.GetBytes(data));
                textBox1.AppendText("\r\n"+ name + ">>" + textBox2.Text);
            }
            else
            {
                MessageBox.Show("로그인 되지 않음");
            }
        }
        // 채팅방 이름 받아오기 + TTK TNE 작업 + SPLIT바꿔주기
        public void Reciever()
        {
            byte[] buffer = new byte[1024];
            while (true)
            {
                ClientSocket.Receive(buffer);
                temp_data = Encoding.Unicode.GetString(buffer);
                temp_data = temp_data.Trim('\0');
                mode = temp_data.Substring(0, 3); //데이터 이름
                data = temp_data.Substring(3);    //데이터 이름과 분리한 데이터
                if (mode == "MSG") // MSG는 메세지 데이터 이름으로 설정된 거임
                {

                    if (key == true)
                    {
                        print(data);
                    }
                }
                else if (mode == "ACL")
                {
                    id.idcheck(mode);
                }
                else if (mode == "FAL")
                {
                    id.idcheck(mode);
                }
                else if (mode == "SAM")
                {
                    MessageBox.Show("같은 아이디가 있으므로 회원가입에 실패했습니다");
                }
                else if (mode == "SNA")
                {
                    MessageBox.Show("회원가입에 성공했습니다!");
                }
                else if (mode == "MYL") // 방 리스트 들고옴
                {
                    room.setmyroomlist(data);
                }
                else if (mode == "MXY")
                {
              
                }
                else if(mode == "OID")
                {
 
                    xy.Setotherid(data);
                    textBox7.Text += ("상대 아이디 :" + xy.Getotherid());
                }
                else if (mode == "OLX")
                {
                    xy.SetotherlocationX(data);
                    textBox7.Text += ("X좌표: " + xy.GetotherlocationX() + "\r\n");
                }
                else if( mode == "OLY")
                {
                    xy.SetotherlocationY(data);
                    textBox7.Text += ("Y좌표: " + xy.GetotherlocationY() + "\r\n");
                }
                else if(mode == "TTK")
                {

                    room.SetTalkTalk(data);
                }
                else if(mode == "TNE")
                {
                    room.SetTalkName(data);
                }
                else if(mode == "CRM")
                {
                    room.SetRoomName(data);
                    textBox8.Text = room.GetRoomName();
                }
                Array.Clear(buffer, 0, buffer.Length);
            }
        }

        public void DataBroadCast(byte[] mode, byte[] data)
        {
            byte[] temp = new byte[mode.Length + data.Length];
            Array.Copy(mode, 0, temp, 0, mode.Length);//mode 0부터 mode.length 만큼 temp 0~ 에 저장
            Array.Copy(data, 0, temp, mode.Length, data.Length); //mode 0~length 만큼 temp 0~ 에 저장

            ClientSocket.Send(temp);
        }
        public void print(string data)
        {
            string roomlist = room.getmyroomlist();
            string[] MyRoom = roomlist.Split(' ');
            string[] chatdata = data.Split(',');
            Thread.Sleep(500);
            for (int i = 1; i < MyRoom.Length; i++)
            {
                if (room.GetRoomName() == MyRoom[i])
                {
                    textBox1.AppendText("\r\n" + chatdata[1] + ">> " + chatdata[2]);
                    break;
                }
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                if ((textBox3.Text.Length != 0) && (textBox4.Text.Length != 0))
                {
                    //IDsave 클래스에 id password socket 값 대입
                    id.name = textBox3.Text;
                    id.password = textBox4.Text;
                    id.socket = ClientSocket;
                    id.IdSend();


                    Thread.Sleep(2000);
                    if (id.login == 1) //이름 비밀번호 체크
                    {
                        MessageBox.Show("로그인 되었습니다");
                        key = true;
                        textBox5.Text = id.name;
                        name = id.name;
                        id.login = 0;
                        DataBroadCast(Encoding.Unicode.GetBytes("MYL"), Encoding.Unicode.GetBytes(id.name));
                        Thread.Sleep(1000);
 
                        string roomlist = room.getmyroomlist();
     
                        string[] MyRoom = roomlist.Split(' ');
                        for (int i = 1; i< MyRoom.Length; i++)
                        {
                            textBox6.Text += MyRoom[i] + "\r\n";

                        }
                    }
                    else if (id.login == 2)
                    {
                        MessageBox.Show("이름과 비밀번호를 다시 확인하세요");
                        id.login = 0;
                    }

                }
                else
                    MessageBox.Show("이름과 비밀번호를 입력하세요");



            }
            catch (Exception ex)
            {

            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button4_Click(object sender, EventArgs e)
        {
            IDsave id_save = new IDsave(textBox3.Text, textBox4.Text, ClientSocket);
            string save = id_save.name + " " + id_save.password;
            DataBroadCast(Encoding.Unicode.GetBytes("ANA"), Encoding.Unicode.GetBytes(save));
        }

        private void textBox6_TextChanged(object sender, EventArgs e)
        {

            
        }

        private void button6_Click(object sender, EventArgs e)
        {
            string locationxy = textBox5.Text + " " + textBox12.Text + " " +textBox13.Text;
            DataBroadCast(Encoding.Unicode.GetBytes("LCS"), Encoding.Unicode.GetBytes(locationxy));
            textBox7.Clear();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            string id = textBox5.Text + " " + textBox14.Text;
            DataBroadCast(Encoding.Unicode.GetBytes("CIN"), Encoding.Unicode.GetBytes(id));

            //방참여
            MessageBox.Show("로딩 중... ");
            Thread.Sleep(2000);

            string[] talknm = room.GetTalkName().Split(',');
            string[] talktk = room.GetTalkTalk().Split(',');
            for(int i =1; i<talknm.Length; i++)
            {
                textBox1.AppendText("\r\n" + talknm[i] + " >> " + talktk[i]);
            }

        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {

        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (serverON == true)
            {
                ClientSocket.Send(Encoding.Unicode.GetBytes("CLO"));
            }
        }
    }

    class XY
    {
        string x = "";
        string y = "";
        string name = "";
        public void Setotherid(string data)
        {
            name = data;
        }
        public string Getotherid()
        {
            return name;
        }
        public void SetotherlocationX(string data)
        {
            x = data;
        }

        public void SetotherlocationY(string data)
        {
            y = data;
        }

        public string GetotherlocationX()
        {
            return x;
        }
        public string GetotherlocationY()
        {
            return y;
        }
    }

    class Room
    {
        string roomname = "";
        public void SetRoomName(string data)
        {
            roomname = data;
        }
        public string GetRoomName()
        {
            return roomname;
        }
        string talktalk = "";
        string talkname = "";
        public void SetTalkTalk(string data)
        {
            talktalk = data;
        }

        public string GetTalkTalk()
        {
            return talktalk;
        }

        public void SetTalkName(string data)
        {
            talkname = data;
        }
        public string GetTalkName()
        {
            return talkname;
        }

        string roomlt = "";
        public void setmyroomlist(string data)
        {
            roomlt = data;
        }
        public string getmyroomlist()
        {
            return roomlt;
        }
        
     

    }

}


