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

        private void button1_Click(object sender, EventArgs e) //ä�� ��� ��ư
        {
            if (key == true)
            {
                string sNow; //�ð� ��������
                sNow = DateTime.Now.ToString("HH:mm:ss"); //�ð� ����
                data = sNow + "," + textBox8.Text + ","+ name + "," + textBox2.Text; //MSG���·� ������ �����
                DataBroadCast(Encoding.Unicode.GetBytes("MSG"), Encoding.Unicode.GetBytes(data)); //�����ͺ����� �� �� ������ ���������� �ٸ� Ŭ���̾�Ʈ�� ��� MSG���� ������
                textBox1.AppendText("\r\n"+ sNow + " " + name + ">>" + textBox2.Text); //�� ���� ���
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
                temp_data = temp_data.Trim('\0'); // ���۷� ���� ������ ���ڿ� ���� �����(�����̸��)
                mode = temp_data.Substring(0, 3); //������ �̸�
                data = temp_data.Substring(3);    //������ �̸��� �и��� ������
                if (mode == "MSG") // MSG�� �޼��� ������ �̸����� ������ ����
                {

                    if (key == true)
                    {
                        print(data); // ����ؼ� ���� �ǽð� �޼��� ����
                    }
                }
                else if (mode == "ACL") // �α��� ���� !
                {
                    id.idcheck(mode);
                }
                else if (mode == "FAL") //�α��� ����!!
                {
                    id.idcheck(mode);
                }
                else if (mode == "SAM") //���� ���̵� ����!
                {
                    MessageBox.Show("���� ���̵� �����Ƿ� ȸ�����Կ� �����߽��ϴ�");
                }
                else if (mode == "SNA") // ȸ������ ����!
                {
                    MessageBox.Show("ȸ�����Կ� �����߽��ϴ�!");
                }
                else if (mode == "MYL") // MYL : ���� ���� �ִ� �� ����Ʈ �˰� ���� �� ���̵�� �Բ� ������ �Ȱ��� MYL ���·� ����  	������ ���� -> MYL 2(������ ����), ���̸�, ���̸�
                {
                    room.setmyroomlist(data);
                }
                else if (mode == "MXY")
                {   
                      //���� �� �ִµ� ���� ���������
                   //MXY: mylocation xy �� ��ǥ ���� ������ ����->MXY X��ǥ, Y��ǥ
                }
                else if(mode == "OID") // OLD: otherid ���� ���� �ٸ� ������� ���̵� ���� ������ ����->OLD 2(������ ����), ��ʶ�, �μ�
                {
 
                    xy.Setotherid(data);
                    textBox7.Text += ("��� ���̵� :" + xy.Getotherid());
                }
                else if (mode == "OLX") //OLX: otherlocation x���� ���� ��ϵ� ��� Ŭ���̾�Ʈ x ��ǥ     ������ ���� -> OLX 3(������ ����) X��ǥ1, X��ǥ2, X��ǥ3
                {
                    xy.SetotherlocationX(data);
                    textBox7.Text += ("X��ǥ: " + xy.GetotherlocationX() + "\r\n");
                }
                else if( mode == "OLY") //OLY : otherlocation y���� ���� ��ϵ� ��� Ŭ���̾�Ʈ y ��ǥ		������ ���� -> OLY 3(������ ����) Y��ǥ1, Y��ǥ2, Y��ǥ3
                {
                    xy.SetotherlocationY(data);
                    textBox7.Text += ("Y��ǥ: " + xy.GetotherlocationY() + "\r\n");
                }

                //TTK: talktalk ä�ù� ��ȭ���� ����� �� �ҷ�����(��ȭ ���븸)      ������ ���� -> TTK 3(������ ����), �ȳ�, ���̿�, ������(MSGó�� ���� ��� �޸��� ���)
                //TNE: talkname ä�� ���� ��� �̸� ����� �� �ҷ�����(�����)      ������ ���� -> TNE 3(������ ����), ��ʶ�, �μ�, ��ʶ�(MSGó�� ���� ��� �޸��� ���)
                //TTT: talktime ä�� ���� ������ �ð� ���������� -> TTT 2(������ ����), 11:22:22, 00:33:22(MSGó�� ���� ��� �޸��� ���)
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
                else if(mode == "CRM") //CRM : chatroom      ä�ù��̸�					������ ���� -> CRM ä�ù��̸�
                {
                    room.SetRoomName(data);
                    textBox8.Text = room.GetRoomName();
                }

                Array.Clear(buffer, 0, buffer.Length);
            }
        }

        public void DataBroadCast(byte[] mode, byte[] data)  //������ �̸��̶� �����Ͱ� ��ġ�� �޼ҵ�
        {
            byte[] temp = new byte[mode.Length + data.Length];
            Array.Copy(mode, 0, temp, 0, mode.Length);//mode 0���� mode.length ��ŭ temp 0~ �� ����
            Array.Copy(data, 0, temp, mode.Length, data.Length); //mode 0~length ��ŭ temp 0~ �� ����

            ClientSocket.Send(temp);
        } 

        public void print(string data) //��� �޼ҵ�(��� Ŭ���̾�Ʈ�� �޼����� ���⶧���� if������ ���� �����������)
        {
            string roomlist = room.getmyroomlist();
            string[] MyRoom = roomlist.Split(' '); // Split ���� ������ ���� MyRoom[0] ~ �����
            string[] chatdata = data.Split(',');
            Thread.Sleep(500); //�ٷ� ����ϸ� ���� �ȳ��ͼ� ������� ��� ����

            if (chatdata[1] == room.GetRoomName()) //�޼��� ���� �� ������ ���̸��̶� ���� ������ ���� ���� ��� ���
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
                    //IDsave Ŭ������ id password socket �� ����
                    id.name = textBox3.Text;
                    id.password = textBox4.Text;
                    id.socket = ClientSocket;
                    id.IdSend();


                    Thread.Sleep(2000);
                    if (id.login == 1) //�̸� ��й�ȣ üũ
                    {
                        MessageBox.Show("�α��� �Ǿ����ϴ�"); //���� �� ������� �α��� ��Ų����
                        key = true;
                        textBox5.Text = id.name;
                        name = id.name;
                        id.login = 0;
                        DataBroadCast(Encoding.Unicode.GetBytes("MYL"), Encoding.Unicode.GetBytes(id.name)); //�� ���� �븮��Ʈ�� �˱����� ���̵� ������ ������. 
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

        private void button6_Click(object sender, EventArgs e) // �� ��ǥ ������
        {
            string locationxy = textBox5.Text + " " + textBox12.Text + " " +textBox13.Text;
            DataBroadCast(Encoding.Unicode.GetBytes("LCS"), Encoding.Unicode.GetBytes(locationxy));
            textBox7.Clear();
        }

        private void button5_Click(object sender, EventArgs e) //�� ���� �ڵ�
        {
            string id = textBox5.Text + " " + textBox14.Text; //���̵� �ΰ� ��ħ(���Ŷ� ����)
            DataBroadCast(Encoding.Unicode.GetBytes("CIN"), Encoding.Unicode.GetBytes(id)); //������ ����

            //cin�� ������ �������� talkname talktalk talktime 3�� ���� ������
            MessageBox.Show("�ε� ��... ");
            Thread.Sleep(2000);

            
            string[] talknm = room.GetTalkName().Split(','); //��κ��� �޾ƿ� �� ä�� ���� �ѷ��ִ� ��
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

        private void Form1_FormClosed(object sender, FormClosedEventArgs e) // �� ����� ���� ������ Ŭ���̾�Ʈ�� ������� �˸�
        {
            if (serverON == true)
            {
                ClientSocket.Send(Encoding.Unicode.GetBytes("CLO"));
            }
        }
    }

    class XY // ��ǥ���� Ŭ����
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

    class Room //�� ���� Ŭ����
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


