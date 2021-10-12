using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace projetest
{
    public class HttpRequest
    {
        String CRLF = "\r\n";
        int HTTP_PORT = 80;

        /** Store the request parameters */
        String method;
        String URI;
        String version;
        String headers = "";

        /** Server and port */
        private String host;
        private int port;

        Socket clientSocket;

        /** Create HttpRequest by reading it from the client socket */
        public HttpRequest(Byte[] from)
        {
            /* TODO: Convert byte array "from" to string to process it.
       
             */
            string incomingMessage = Encoding.Default.GetString(from);
            incomingMessage = incomingMessage.Substring(0, incomingMessage.IndexOf("\0"));
            /* TODO: Extract first line of the converted string.
           
            */
            String firstLine = "";
            try
            {
                String[] result = incomingMessage.Split('\n');
                firstLine = result[0];
            }
            catch (IOException e)
            {
                Console.WriteLine("Error reading request line: " + e);
            }

            /*
             TO DO: Split byte array and assign appropriate values to method, URI and version 
            */
            string[] words = firstLine.Split(' ');
            method = words[0];
            URI = words[1];
            version = words[2].Substring(0,words[2].IndexOf("\r"));

            Console.WriteLine("URI is: " + URI);
            if (method != "GET")
            {
                Console.WriteLine("Error: Method not GET");
            }

            try
            {
                string l;
                String[] lines = incomingMessage.Split('\n'); //error will be gone when you do TODOs above
                foreach (string line in lines)
                {
                    if (line == lines[0])
                    {
                        l = line.Substring(0, line.IndexOf("\r"));
                        headers += l + CRLF;
                    }
                    else
                    {
                        headers += line + CRLF;
                    }
                    
                    /* We need to find host header to know which server to
                    * contact in case the request URI is not complete. */

                    /*
                     * TO DO determine the Host and Port number ***
                    */

                }
                host = lines[1].Substring(lines[1].IndexOf(" ") + 1);

                //
                string url = "http://"+host + URI;
                
                var splitted = url.Split(':');

                if (splitted.Length == 3)
                {
                    string p = splitted[2].Substring(0, 4);
                    Int32.TryParse(p, out port);
                }
                else
                {
                    port = HTTP_PORT;
                }
            }
            catch (IOException e)
            {
                Console.WriteLine("Error reading from socket: " + e);
                return;
            }
        }

        /** Return host for which this request is intended */
        public String getHost()
        {
            return host;
        }

        /** Return port for server */
        public int getPort()
        {
            return port;
        }
        /**
         * Convert request into a string for easy re-sending.
         */
        public String toString()
        {
            String req = "";

            //req = method + " " + URI + " " + version + CRLF;
            req += headers;
            /* This proxy does not support persistent connections */
            req += "Connection: close" + CRLF;
            req += CRLF;

            return req;
        }
    }
}
