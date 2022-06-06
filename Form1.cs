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
        public static Boolean key = false; //ä�� ���� ����
        public string name;
        public string mode = ""; //������ �̸�
        public string data = ""; // ������ �̸� ���ܽ�Ų ���� ������
        public int temptemp = 1; //�ӽ� ���� (ȸ������ ���� �� ���)
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
                MessageBox.Show("������ �����");
                serverON = true;
                new Thread(Reciever).Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show("������ ������� ����");
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
                MessageBox.Show("�α��� ���� ����");
            }
        }
        // ä�ù� �̸� �޾ƿ��� + TTK TNE �۾� + SPLIT�ٲ��ֱ�
        public void Reciever()
        {
            byte[] buffer = new byte[1024];
            while (true)
            {
                ClientSocket.Receive(buffer);
                temp_data = Encoding.Unicode.GetString(buffer);
                temp_data = temp_data.Trim('\0');
                mode = temp_data.Substring(0, 3); //������ �̸�
                data = temp_data.Substring(3);    //������ �̸��� �и��� ������
                if (mode == "MSG") // MSG�� �޼��� ������ �̸����� ������ ����
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
                    MessageBox.Show("���� ���̵� �����Ƿ� ȸ�����Կ� �����߽��ϴ�");
                }
                else if (mode == "SNA")
                {
                    MessageBox.Show("ȸ�����Կ� �����߽��ϴ�!");
                }
                else if (mode == "MYL") // �� ����Ʈ ����
                {
                    room.setmyroomlist(data);
                }
                else if (mode == "MXY")
                {
              
                }
                else if(mode == "OID")
                {
 
                    xy.Setotherid(data);
                    textBox7.Text += ("��� ���̵� :" + xy.Getotherid());
                }
                else if (mode == "OLX")
                {
                    xy.SetotherlocationX(data);
                    textBox7.Text += ("X��ǥ: " + xy.GetotherlocationX() + "\r\n");
                }
                else if( mode == "OLY")
                {
                    xy.SetotherlocationY(data);
                    textBox7.Text += ("Y��ǥ: " + xy.GetotherlocationY() + "\r\n");
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
            Array.Copy(mode, 0, temp, 0, mode.Length);//mode 0���� mode.length ��ŭ temp 0~ �� ����
            Array.Copy(data, 0, temp, mode.Length, data.Length); //mode 0~length ��ŭ temp 0~ �� ����

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
                    //IDsave Ŭ������ id password socket �� ����
                    id.name = textBox3.Text;
                    id.password = textBox4.Text;
                    id.socket = ClientSocket;
                    id.IdSend();


                    Thread.Sleep(2000);
                    if (id.login == 1) //�̸� ��й�ȣ üũ
                    {
                        MessageBox.Show("�α��� �Ǿ����ϴ�");
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
                        MessageBox.Show("�̸��� ��й�ȣ�� �ٽ� Ȯ���ϼ���");
                        id.login = 0;
                    }

                }
                else
                    MessageBox.Show("�̸��� ��й�ȣ�� �Է��ϼ���");



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

            //������
            MessageBox.Show("�ε� ��... ");
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


