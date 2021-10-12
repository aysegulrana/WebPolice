using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace projetest
{
    class Program
    {
        int serverport;
        private static int port;
        private static Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        bool terminating = false;
        static bool listening = false;

        public static void init(int p)
        {
            port = p;
            try
            {
                IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Any, port);
                //Socket listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                //serverSocket.Blocking = false;
                serverSocket.Bind(localEndPoint);
                serverSocket.Listen(100);
                listening = true;
                /* TODO: Create endpoint, socket then bind and start listening for connections */

            }
            catch (IOException e)
            {
                Console.WriteLine("Error creating socket: " + e);
                return;
            }
        }

        public static void handle(Socket client)
        {
            Socket server = null;

            HttpRequest request = null;
            HttpResponse response = null;

            IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, port);
            /* Process request. If there are any exceptions, then simply
             * return and end this thread. This unfortunately means the
             * client will hang for a while, until it timeouts. */

            /* Read request */
            try
            {
                /* TODO: Receive request text from client by using buffer as always (client socket sent as parameter to the function from main)
                 */
                Byte[] buffer = new Byte[300];
                client.Receive(buffer);
                request = new HttpRequest(buffer); //TODO: error will be gone when you create request object with buffer created above.
                Console.WriteLine("Got request...");
                string r = request.toString();
                Console.WriteLine(r);
                Console.WriteLine("Reading request...");
            }
            catch (IOException e)
            {
                Console.WriteLine("Error reading request from client: " + e);
                return;
            }

            string reqq = request.toString();
            string[] x = reqq.Split(' ');
            string uri = x[1];
            string url = request.getHost() + uri;
            string[] banned;
            var list = new List<string>();
            var fileStream = new FileStream(@"C:\Users\USER\Desktop\bannedwords.txt", FileMode.Open, FileAccess.Read);
            using (var streamReader = new StreamReader(fileStream, Encoding.UTF8))
            {
                string line;
                while ((line = streamReader.ReadLine()) != null)
                {
                    list.Add(line);
                }
            }
            banned = list.ToArray();

            bool found = false;
            Byte[] bx = new Byte[8192];
            foreach (string word in banned)
            {
                //string h = request.getHost();
                if (url.Contains(word) == true)
                {
                    bx = Encoding.Default.GetBytes("Access forbidden\n");
                    client.Send(bx);
                    found = true;
                    break;
                }
            }

            if (!found)
            {
                /* Send request to web page */
                try
                {
                    /* TODO: crate new socket to "server" socket (server = new..) 
                     * and connect to the host with determined port with this socket 
                     * (Hint:use HttpRequest getHost() and getPort())
                     * then send request which is HttpRequest object now 
                     * through the server socket (hint: use toString() for string buffer conversion setup for socket communication.    
                     */
                    server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                    server.Connect(request.getHost(), request.getPort());
                    string req = request.toString();
                    Byte[] b = new Byte[8192];
                    b = Encoding.Default.GetBytes(req);
                    server.Send(b);
                }
                catch (IOException e)
                {
                    Console.WriteLine("Error writing request to server: " + e);
                    return;
                }

                /* Read response and forward it to client */
                try
                {
                    /*TODO: Create new buffer, receive response from host using server socket, convert it to HttpResponse object (response = new...), 
                    * then send it to client with "client" socket (hint: HttpResponse's toString function for converting response to string.

                     */
                    Byte[] buff = new Byte[8192];
                    server.Receive(buff);

                    string incoming = Encoding.Default.GetString(buff);

                    if (found == false)
                    {

                        response = new HttpResponse(buff);
                        string resp = response.toString();
                        buff = Encoding.Default.GetBytes(resp);
                        Console.WriteLine(resp);
                        try
                        {
                            client.Send(buff);
                        }
                        catch
                        {
                            Console.WriteLine("cannot send\n");
                        }
                    }

                    client.Close();
                    server.Close();
                }
                catch (IOException e)
                {
                    Console.WriteLine("Error writing response to client: " + e);
                }
            }
        }

        /** Get input from command prompt and start server */
        public static void Main(string[] args)
        {
            int myPort = 0;
            string val;
            Console.Write("Enter port: ");
            val = Console.ReadLine();
            Console.Write("Port number is "+ val);
            /* TODO: Parse port to integer 
             
            
            */
            Int32.TryParse(val, out myPort);
            init(myPort);

            /** Main loop. Listen for incoming connections and spawn a new thread for handling them */
            Socket client = null;

            while (true)
            {
                try
                {
                    client = serverSocket.Accept();
                    
                    Console.WriteLine("Got connection " + client);
                    /* TODO: Create and start threads with function "handle" for receiving clients at the same time
                     */
                    Thread receiveThread = new Thread(() => handle(client)); // updated
                    receiveThread.Start();
                }
                catch (IOException e)
                {
                    Console.WriteLine("Error reading request from client: " + e);
                    /* Definitely cannot continue, so skip to next
                        * iteration of while loop. */
                    continue;
                }
            }

        }
    }
}
