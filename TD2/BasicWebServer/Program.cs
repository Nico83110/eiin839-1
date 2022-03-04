using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Web;
using System.Reflection;
using System.Diagnostics;

namespace BasicServerHTTPlistener

{
    public class MyMethods
    {
        public string method1(string var1, string var2)
        {
            return $"<html><body>Hello {var1} et {var2}</body></html>";
        }

        public string method2(string res)
        {
            return $"<html><body>Hello {res}</body></html>";
        }


        public string callExternalApp(string var1, string var2)
        {
            Process.Start("C:\\");
            return "";
        }
    }

    internal class Program
    {
        private static void Main(string[] args)
        {

            //if HttpListener is not supported by the Framework
            if (!HttpListener.IsSupported)
            {
                Console.WriteLine("A more recent Windows version is required to use the HttpListener class.");
                return;
            }
 
 
            // Create a listener.
            HttpListener listener = new HttpListener();

            // Add the prefixes.
            if (args.Length != 0)
            {
                foreach (string s in args)
                {
                    listener.Prefixes.Add(s);
                    // don't forget to authorize access to the TCP/IP addresses localhost:xxxx and localhost:yyyy 
                    // with netsh http add urlacl url=http://localhost:xxxx/ user="Tout le monde"
                    // and netsh http add urlacl url=http://localhost:yyyy/ user="Tout le monde"
                    // user="Tout le monde" is language dependent, use user=Everyone in english 

                }
            }
            else
            {
                Console.WriteLine("Syntax error: the call must contain at least one web server url as argument");
            }
            listener.Start();

            // get args 
            foreach (string s in args)
            {
                Console.WriteLine("Listening for connections on " + s);
            }

            // Trap Ctrl-C on console to exit 
            Console.CancelKeyPress += delegate {
                // call methods to close socket and exit
                listener.Stop();
                listener.Close();
                Environment.Exit(0);
            };


            while (true)
            {
                // Note: The GetContext method blocks while waiting for a request.
                HttpListenerContext context = listener.GetContext();
                HttpListenerRequest request = context.Request;

                string documentContents;
               /*Type type = typeof(MyMethods);
                MethodInfo method = type.GetMethod("method1");
                MyMethods m = new MyMethods();

                HttpListenerRequest[] params = new HttpListenerRequest[1];
                params[0] = request;
                string result = (string)method.Invoke(m, params);*/

                using (Stream receiveStream = request.InputStream)
                {
                    using (StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8))
                    {
                        documentContents = readStream.ReadToEnd();
                    }
                }

                //Console.WriteLine(m.method2(documentContents));

                // get url 
                //Console.WriteLine($"Received request for {request.Url}");
                //var response = context.Response;
                //await using var stream = response.OutputStream;
                //await using var sw = new StreamWriter(stream);

                    //await HandleRequest(request, response);
                    //response.StatusCode = (int)HttpStatusCode.OK;
                
        

                // get url 
                Console.WriteLine($"Received request for {request.Url}");

                //get url protocol
                Console.WriteLine(request.Url.Scheme);
                //get user in url
                Console.WriteLine(request.Url.UserInfo);
                //get host in url
                Console.WriteLine(request.Url.Host);
                //get port in url
                Console.WriteLine(request.Url.Port);
                //get path in url 
                Console.WriteLine(request.Url.LocalPath);

                // parse path in url 
                foreach (string str in request.Url.Segments)
                {
                    Console.WriteLine(str);
                }

                //get params un url. After ? and between &

                Console.WriteLine(request.Url.Query);

                //parse params in url
                Console.WriteLine("param1 = " + HttpUtility.ParseQueryString(request.Url.Query).Get("param1"));
                Console.WriteLine("param2 = " + HttpUtility.ParseQueryString(request.Url.Query).Get("param2"));
                Console.WriteLine("param3 = " + HttpUtility.ParseQueryString(request.Url.Query).Get("param3"));
                Console.WriteLine("param4 = " + HttpUtility.ParseQueryString(request.Url.Query).Get("param4"));

                //
                Console.WriteLine(documentContents);

                // Obtain a response object.
                HttpListenerResponse response = context.Response;

                requestHandler(context, request);

                callExternaProgram(request);
            }
            // Httplistener neither stop ... But Ctrl-C do that ...
            // listener.Stop();
        }

        public static void requestHandler(HttpListenerContext context, HttpListenerRequest request)
        {

            string url = request.Url.LocalPath.ToString();

            MyMethods myMethods = new MyMethods();

            HttpListenerResponse response = context.Response;

            string responseString = "";

                Type thisType = myMethods.GetType();
                MethodInfo method = thisType.GetMethod("method2");
                responseString = (string)method.Invoke(myMethods, null);
            
            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseString);
            response.ContentLength64 = buffer.Length;
            System.IO.Stream output = response.OutputStream;
            output.Write(buffer, 0, buffer.Length);
            output.Close();
        }

        public static string callExternaProgram(HttpListenerRequest request)
        {
            Process p = new Process();
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = @"cmd.exe";
            startInfo.Arguments = @"/K echo " + HttpUtility.ParseQueryString(request.Url.Query).Get("param1");
            p.StartInfo = startInfo;
            p.Start();
            return "<HTML><body> <h1>Execution of Windows cmd program :</h1><br>"
                + HttpUtility.ParseQueryString(request.Url.Query).Get("param1") + "<br>"
                + "</body></HTML>";
        }

        public static int incr(int val1)
        {
            return val1 + 1;
        }
    }

    public class HttpException : Exception
    {
        public HttpException(HttpStatusCode code, string message) : base(message)
        {
            Code = code;
        }

        public HttpStatusCode Code { get; }
    }
}