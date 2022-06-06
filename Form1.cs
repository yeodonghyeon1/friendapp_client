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
        public static IPEndPoint ip = new IPEndPoint(IPAddress.Parse("127.0.0.1"), port: 9999);
        Socket ClientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        public static String temp_data = "";
        public static Boolean key = false; //ä�� ���� ����
        public string name;
        public string mode = ""; //������ �̸�
        public string data = ""; // ������ �̸� ���ܽ�Ų ���� ������
        public int temptemp = 1; //�ӽ� ���� (ȸ������ ���� �� ���)
        IDsave id = new IDsave();
        public Form1()
        {
            InitializeComponent();
        }
        void PutForms()
        {
            Form child1 = new Form();
            child1.Text = "child1";
            child1.TopLevel = true;
            child1.Dock = DockStyle.Left;

            Form child2 = new Form();
            child2.Text = "child2";
            child2.TopLevel = true;
            child2.Dock = DockStyle.Right;

            this.Controls.Add(child1);
            this.Controls.Add(child2);

            child1.Show();
            child2.Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                ClientSocket.Connect(ip);
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
                data = name + ">> " + textBox2.Text;
                DataBroadCast(Encoding.Unicode.GetBytes("MSG"), Encoding.Unicode.GetBytes(data));
                textBox1.AppendText("\r\n"+ name + ">>" + textBox2.Text);
            }
            else
            {
                MessageBox.Show("�α��� ���� ����");
            }
        }

        public void Reciever()
        {
            byte[] buffer = new byte[1024];
            while (true)
            {
                ClientSocket.Receive(buffer);
                temp_data = Encoding.Unicode.GetString(buffer);
                mode = temp_data.Substring(0, 3); //������ �̸�
                                                  //������ �̸��� �и��� ������
                if (mode == "MSG") // MSG�� �޼��� ������ �̸����� ������ ����
                {
                    data = temp_data.Substring(3);
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
                else if(mode == "SAM")
                {
                    MessageBox.Show("���� ���̵� �����Ƿ� ȸ�����Կ� �����߽��ϴ�");
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
            textBox1.AppendText( "\r\n" + data);
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


                    Thread.Sleep(1000);
                    if (id.login == 1) //�̸� ��й�ȣ üũ
                    {
                        MessageBox.Show("�α��� �Ǿ����ϴ�");
                        key = true;
                        textBox5.Text = id.name;
                        name = id.name;
                        id.login = 0;
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
            DataBroadCast(Encoding.Unicode.GetBytes("SVE"), Encoding.Unicode.GetBytes(save));
        }
    }
}


