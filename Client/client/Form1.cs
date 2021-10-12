using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace client
{
    public partial class Form1 : Form
    {

        bool terminating = false;
        bool connected = false;
        Socket clientSocket;
        public Form1()
        {
            Control.CheckForIllegalCrossThreadCalls = false;
            this.FormClosing += new FormClosingEventHandler(Form1_FormClosing);
            InitializeComponent();
        }
        private void Form1_FormClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            connected = false;
            terminating = true;
            Environment.Exit(0);
        }


        private void button_Click(object sender, EventArgs e)
        {

            clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            string r = request.Text;
            string k = "127.0.0.1";
            int p;
            if (Int32.TryParse(port.Text, out p))
            {
                try
                {
                    clientSocket.Connect(k, p);
                    button.Enabled = true;
                    connected = true;
                    Byte[] b = new Byte[300];
                    b = Encoding.Default.GetBytes(r);
                    clientSocket.Send(b);
                    try
                    {
                        Byte[] buffer = new Byte[8192];
                        clientSocket.Receive(buffer);
                        string incomingMessage = Encoding.Default.GetString(buffer);
                        incomingMessage = incomingMessage.Substring(0, incomingMessage.IndexOf("\0"));
                        response.AppendText(incomingMessage);
                        clientSocket.Close();

                    }
                    catch
                    {
                        if (!terminating)
                        {
                            button.Enabled = true;
                        }

                        clientSocket.Close();
                        connected = false;
                    }
                }
                catch
                {
                    response.AppendText("could not connect to the server\n");
                }

            }
            else
            {
                response.AppendText("check port");
            }
        }
    }
}
