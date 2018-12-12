using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SimpleTCP;

namespace client_window
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            buttonSend.Enabled = false;
            statusTextBox.Visible = false;
            statusTextBox.ReadOnly = true;
            txtHost.ReadOnly = true;
            txtPort.ReadOnly = true;
            buttonDisconnect.Enabled = false;
        }

        SimpleTCP.SimpleTcpClient client;

        private void buttonConnect_Click(object sender, EventArgs e)
        {
            statusTextBox.Visible = true;
            
            try
            {
                client.Connect(txtHost.Text, Convert.ToInt32(txtPort.Text));
                statusTextBox.Text += "Установлено соединение с сервером." + Environment.NewLine;
                client.WriteLineAndGetReply("Новое подключение", TimeSpan.FromSeconds(1));
                buttonConnect.Enabled = false;
                buttonSend.Enabled = true;
                buttonDisconnect.Enabled = true;
            }
            catch (Exception ex)
            {
                statusTextBox.Text += ex.Message;
                buttonConnect.Enabled = true;
                buttonDisconnect.Enabled = false;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            client = new SimpleTCP.SimpleTcpClient();
            client.StringEncoder = Encoding.UTF8;
            client.DataReceived += Client_DataReceived;
        }

        private void Client_DataReceived(object sender, SimpleTCP.Message e)
        {
            statusTextBox.Invoke((MethodInvoker)delegate ()
            {
                StringBuilder builder = new StringBuilder();
                builder.Append(e.MessageString);
                builder.Length--;
                statusTextBox.Text += Convert.ToString(builder) + Environment.NewLine;
            });
        }

        private void buttonSend_Click(object sender, EventArgs e)
        {
            try
            {
                client.WriteLineAndGetReply("Новое сообщение: "+ messageTextBox.Text, TimeSpan.FromSeconds(1));
            }
            catch(Exception ex)
            {
                statusTextBox.Text += ex.Message + Environment.NewLine;
                client.Disconnect();
                buttonConnect.Enabled = true;
                buttonDisconnect.Enabled = false;
            }
        }

        private void buttonDisconnect_Click(object sender, EventArgs e)
        {
            statusTextBox.Text += "Соединение с сервером разорвано" + Environment.NewLine;
            buttonConnect.Enabled = true;
            client.WriteLineAndGetReply("Соединение разорвано клиентом", TimeSpan.FromSeconds(1));
            client.Disconnect();
        }
    }
}
