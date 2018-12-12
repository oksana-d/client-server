using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using SimpleTCP;

namespace server_window
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        SimpleTCP.SimpleTcpServer server;

        private static string root()
        {
            string dirName = @"";
            string content = "";
            System.IO.StreamReader objReader = new System.IO.StreamReader("D:\\3kurs\\Osnovy_PI\\root_folder.cfg");
            dirName = objReader.ReadLine();
            if (dirName == null)
            {
                return ("Не удалось считать корень сервера." + Environment.NewLine);
            }
            else
            {
                if (System.IO.Directory.Exists(dirName))
                {
                    content = content + "Содержимое файловой системы" + Environment.NewLine + "Подкаталоги:" + Environment.NewLine;
                    string[] dirs = System.IO.Directory.GetDirectories(dirName);
                    foreach (string s in dirs)
                    {
                        content = content + s + Environment.NewLine;
                    }
                    content = content + Environment.NewLine;
                    content = content + "Файлы:" + Environment.NewLine;
                    string[] files = System.IO.Directory.GetFiles(dirName);
                    foreach (string s in files)
                    {
                        content = content + s + Environment.NewLine;
                    }
                    return content;
                }
                else return ("Указанная директория не существует." + Environment.NewLine);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            richTextBox1.ReadOnly = true;
            buttonStop.Enabled = false;
            txtHost.ReadOnly = true;
            txtPort.ReadOnly = true;
            server = new SimpleTCP.SimpleTcpServer();
            server.StringEncoder = Encoding.UTF8;
            server.DataReceived += Server_DataReceived;
        }

        private void Server_DataReceived(object sender, SimpleTCP.Message e)
        {
            richTextBox1.Invoke((MethodInvoker)delegate ()
            {
                StringBuilder builder =  new StringBuilder();
                builder.Append(e.MessageString);
                builder.Length--;
                if (Convert.ToString(builder) == "Новое подключение")
                {
                    richTextBox1.Text += Convert.ToString(builder) + Environment.NewLine;
                    e.ReplyLine("Сообщение доставлено" + Environment.NewLine + root());
                }
                else if(Convert.ToString(builder) == "Соединение разорвано клиентом")
                {
                    richTextBox1.Text += Convert.ToString(builder) + Environment.NewLine;
                    e.ReplyLine("Сообщение доставлено:" + Convert.ToString(builder));
                } 
                else
                {
                    richTextBox1.Text += Convert.ToString(builder) + Environment.NewLine;
                    e.ReplyLine("Сообщение доставлено:" + Convert.ToString(builder));
                }
            });
        }

        private void buttonStart_Click(object sender, EventArgs e)
        {
            richTextBox1.Text += "Сервер запущен..." + Environment.NewLine;
            System.Net.IPAddress ip = System.Net.IPAddress.Parse(txtHost.Text);
            server.Start(ip,Convert.ToInt32(txtPort.Text));
            buttonStart.Enabled = false;
            buttonStop.Enabled = true;
        }

        private void buttonStop_Click(object sender, EventArgs e)
        {
            if (server.IsStarted)
            {
                buttonStart.Enabled = true;
                buttonStop.Enabled = false;
                richTextBox1.Text += "Сервер остановлен." + Environment.NewLine;
                server.Stop();
            }
             
        }
    }
}
