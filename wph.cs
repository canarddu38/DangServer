using System;
using System.IO;
using System.Text;
using System.Net;
using System.Threading.Tasks;

namespace DANGserver
{
    class HttpServer
    {
        public static HttpListener listener;
        public static string url = "http://localhost:8080/";


        public static async Task HandleIncomingConnections()
        {
            bool runServer = true;

            while (runServer)
            {
                HttpListenerContext ctx = await listener.GetContextAsync();

                HttpListenerRequest req = ctx.Request;
                HttpListenerResponse resp = ctx.Response;

                Console.WriteLine(req.Url.ToString());
                Console.WriteLine(req.HttpMethod);
                Console.WriteLine(req.UserHostName);
                Console.WriteLine(req.UserAgent);
                Console.WriteLine();
				
				string pageData = File.ReadAllText(@"website\index.sdang");
				
				if (File.Exists("website"+req.Url.AbsolutePath.Replace("/", @"\")+".sdang"))
				{
					Console.WriteLine("website"+req.Url.AbsolutePath.Replace("/", @"\")+".sdang");
					pageData = File.ReadAllText("website"+req.Url.AbsolutePath.Replace("/", @"\")+".sdang");
				}
				else if (req.Url.AbsolutePath == "")
				{
					
				}
				else
				{
					pageData = File.ReadAllText(@"website\404.sdang");
				}
				
                if ((req.HttpMethod == "POST") && (req.Url.AbsolutePath == "/shutdown"))
                {
                    Console.WriteLine("Shutdown requested");
                    runServer = false;
                }

                // Write the response info
                byte[] data = Encoding.UTF8.GetBytes(pageData);
                // resp.ContentType = "text/html";
                resp.ContentEncoding = Encoding.UTF8;
                resp.ContentLength64 = data.LongLength;

                // Write out to the response stream (asynchronously), then close it
                await resp.OutputStream.WriteAsync(data, 0, data.Length);
                resp.Close();
            }
        }


        public static void Main(string[] args)
        {
            // Create a Http server and start listening for incoming connections
            listener = new HttpListener();
            listener.Prefixes.Add(url);
            listener.Start();
            Console.WriteLine("Listening for connections on {0}", url);

            // Handle requests
            Task listenTask = HandleIncomingConnections();
            listenTask.GetAwaiter().GetResult();

            // Close the listener
            listener.Close();
        }
    }
}