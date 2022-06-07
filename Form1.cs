using System;
using System.Globalization;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Diagnostics;
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

        private void button1_Click(object sender, EventArgs e) //채팅 출력 버튼
        {
            if (key == true)
            {
                string sNow; //시간 설정변수
                sNow = DateTime.Now.ToString("HH:mm:ss"); //시간 설정
                data = sNow + "," + textBox8.Text + ","+ name + "," + textBox2.Text; //MSG형태로 데이터 만들기
                DataBroadCast(Encoding.Unicode.GetBytes("MSG"), Encoding.Unicode.GetBytes(data)); //데이터보내기 이 값 보내면 서버에서도 다른 클라이언트에 띄울 MSG값을 보내줌
                textBox1.AppendText("\r\n"+ sNow + " " + name + ">>" + textBox2.Text); //내 폼에 출력
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
                temp_data = temp_data.Trim('\0'); // 버퍼로 받은 데이터 문자열 끝에 지우기(걍붙이면댐)
                mode = temp_data.Substring(0, 3); //데이터 이름
                data = temp_data.Substring(3);    //데이터 이름과 분리한 데이터
                if (mode == "MSG") // MSG는 메세지 데이터 이름으로 설정된 거임
                {

                    if (key == true)
                    {
                        print(data); // 통신해서 받은 실시간 메세지 송출
                    }
                }
                else if (mode == "ACL") // 로그인 성공 !
                {
                    id.idcheck(mode);
                }
                else if (mode == "FAL") //로그인 실패!!
                {
                    id.idcheck(mode);
                }
                else if (mode == "SAM") //같은 아이디 있음!
                {
                    MessageBox.Show("같은 아이디가 있으므로 회원가입에 실패했습니다");
                }
                else if (mode == "SNA") // 회원가입 성공!
                {
                    MessageBox.Show("회원가입에 성공했습니다!");
                }
                else if (mode == "MYL") // MYL : 내가 속해 있는 방 리스트 알고 싶을 때 아이디와 함께 보내기 똑같이 MYL 형태로 받음  	데이터 형태 -> MYL 2(데이터 갯수), 방이름, 방이름
                {
                    room.setmyroomlist(data);
                }
                else if (mode == "MXY")
                {   
                      //받을 순 있는데 딱히 쓸모없을듯
                   //MXY: mylocation xy 내 좌표 전송 데이터 형태->MXY X좌표, Y좌표
                }
                else if(mode == "OID") // OLD: otherid 본인 제외 다른 사람들의 아이디 정보 데이터 형태->OLD 2(데이터 갯수), 김똘똘, 민수
                {
 
                    xy.Setotherid(data);
                    textBox7.Text += ("상대 아이디 :" + xy.Getotherid());
                }
                else if (mode == "OLX") //OLX: otherlocation x본인 제외 등록된 모든 클라이언트 x 좌표     데이터 형태 -> OLX 3(데이터 갯수) X좌표1, X좌표2, X좌표3
                {
                    xy.SetotherlocationX(data);
                    textBox7.Text += ("X좌표: " + xy.GetotherlocationX() + "\r\n");
                }
                else if( mode == "OLY") //OLY : otherlocation y본인 제외 등록된 모든 클라이언트 y 좌표		데이터 형태 -> OLY 3(데이터 갯수) Y좌표1, Y좌표2, Y좌표3
                {
                    xy.SetotherlocationY(data);
                    textBox7.Text += ("Y좌표: " + xy.GetotherlocationY() + "\r\n");
                }

                //TTK: talktalk 채팅방 대화내용 저장된 거 불러오기(대화 내용만)      데이터 형태 -> TTK 3(데이터 갯수), 안녕, 하이요, ㅋㅋㅋ(MSG처럼 공백 대신 콤마로 사용)
                //TNE: talkname 채팅 보낸 사람 이름 저장된 거 불러오기(사람만)      데이터 형태 -> TNE 3(데이터 갯수), 김똘똘, 민수, 김똘똘(MSG처럼 공백 대신 콤마로 사용)
                //TTT: talktime 채팅 보낸 시점의 시간 데이터형태 -> TTT 2(데이터 갯수), 11:22:22, 00:33:22(MSG처럼 공백 대신 콤마로 사용)
                else if(mode == "TTK")
                {

                    room.SetTalkTalk(data);
                }
                else if(mode == "TNE")
                {
                    room.SetTalkName(data);
                }
                else if (mode == "TTT")
                {
                    room.setTalkTime(data);
                }
                else if(mode == "CRM") //CRM : chatroom      채팅방이름					데이터 형태 -> CRM 채팅방이름
                {
                    room.SetRoomName(data);
                    textBox8.Text = room.GetRoomName();
                }

                Array.Clear(buffer, 0, buffer.Length);
            }
        }

        public void DataBroadCast(byte[] mode, byte[] data)  //데이터 이름이랑 데이터값 합치는 메소드
        {
            byte[] temp = new byte[mode.Length + data.Length];
            Array.Copy(mode, 0, temp, 0, mode.Length);//mode 0부터 mode.length 만큼 temp 0~ 에 저장
            Array.Copy(data, 0, temp, mode.Length, data.Length); //mode 0~length 만큼 temp 0~ 에 저장

            ClientSocket.Send(temp);
        } 

        public void print(string data) //출력 메소드(모든 클라이언트로 메세지가 가기때문에 if문으로 따로 조절해줘야함)
        {
            string roomlist = room.getmyroomlist();
            string[] MyRoom = roomlist.Split(' '); // Split 으로 데이터 분할 MyRoom[0] ~ 저장됨
            string[] chatdata = data.Split(',');
            Thread.Sleep(500); //바로 출력하면 값이 안나와서 쓰레드로 잠시 멈춤

            if (chatdata[1] == room.GetRoomName()) //메세지 보낼 때 보내진 방이름이랑 현재 설정된 방이 같을 경우 출력
                 textBox1.AppendText("\r\n" + chatdata[0] + " "+ chatdata[2] + ">> " + chatdata[3]);
                
            
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
                        MessageBox.Show("로그인 되었습니다"); //대충 걍 여기까지 로그인 시킨거임
                        key = true;
                        textBox5.Text = id.name;
                        name = id.name;
                        id.login = 0;
                        DataBroadCast(Encoding.Unicode.GetBytes("MYL"), Encoding.Unicode.GetBytes(id.name)); //내 현재 룸리스트를 알기위해 아이디를 서버에 보내줌. 
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

        private void button6_Click(object sender, EventArgs e) // 내 좌표 보내기
        {
            string locationxy = textBox5.Text + " " + textBox12.Text + " " +textBox13.Text;
            DataBroadCast(Encoding.Unicode.GetBytes("LCS"), Encoding.Unicode.GetBytes(locationxy));
            textBox7.Clear();
        }

        private void button5_Click(object sender, EventArgs e) //방 입장 코드
        {
            string id = textBox5.Text + " " + textBox14.Text; //아이디 두개 합침(내거랑 상대거)
            DataBroadCast(Encoding.Unicode.GetBytes("CIN"), Encoding.Unicode.GetBytes(id)); //서버로 보냄

            //cin을 보내면 서버에서 talkname talktalk talktime 3개 값을 보내줌
            MessageBox.Show("로딩 중... ");
            Thread.Sleep(2000);

            
            string[] talknm = room.GetTalkName().Split(','); //요부분은 받아온 방 채팅 내역 뿌려주는 거
            string[] talktk = room.GetTalkTalk().Split(',');
            string[] talktm = room.getTalkTime().Split(',');
            for (int i =1; i<talknm.Length; i++)
            {
                textBox1.AppendText("\r\n" + talktm[i] + " " + talknm[i] + " >> " + talktk[i]);
            }

        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {

        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e) // 폼 종료시 보낼 데이터 클라이언트가 종료됨을 알림
        {
            if (serverON == true)
            {
                ClientSocket.Send(Encoding.Unicode.GetBytes("CLO"));
            }
        }
    }

    class XY // 좌표관련 클래스
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

    class Room //방 관련 클래스
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
        string talktime = "";
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
        
        public void setTalkTime(string data)
        {
            talktime = data;
        }
        public string getTalkTime()
        {
            return talktime;
        }

    }

}


