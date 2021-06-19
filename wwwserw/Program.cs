using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace wwwserw
{
    class Program
    {
        static void Main(string[] args)
        {
            Double MonteSin(double x1, double x2, int N)
            {
                Random r = new Random();
                double x, y;
                double P = Math.Abs(x2 - x1) * 2;
                double n = 0;
                for (int i = 1; i < N; i++)
                {

                    x = r.NextDouble() * (x2 - x1) + x1;

                    y = r.NextDouble() * (1 - (-1)) - 1;

                    if (Math.Sin(x) >= 0 && Math.Sin(x) >= y && y > 0)
                    {
                        n = n + 1;

                    }
                    if (Math.Sin(x) <= 0 && Math.Sin(x) <= y && y < 0)
                    {
                        n = n - 1;
                    }

                }
                return P * n / N;
            }
            Double MonteParallel(double x1, double x2, int N, int w)//wątki
            {
                Double[] wynik = new double[w];
                Double odcinek = Math.Abs((x2 - x1)) / w;
                Parallel.For(0, w, (p) => {
                    wynik[p] = MonteSin(x1 + odcinek * p, x1 + odcinek * p + odcinek, N);
                });
                return wynik.Sum();
            }

           void start() { 
                TcpListener server = null;
                Thread th = new Thread(() =>
                {
                    server = new TcpListener(IPAddress.Parse("127.0.0.1"), 80);
                    server.Start();
                    Console.WriteLine("Start Process");
                    while (true)
                    {
                        TcpClient client = server.AcceptTcpClient();
                        NetworkStream stream = client.GetStream();
                        byte[] bytes = new byte[1025];
                        Int32 i = stream.Read(bytes, 0, bytes.Length);
                        string data = Encoding.UTF8.GetString(bytes, 0, i);
                        string info_dla_web = System.IO.File.ReadAllText(@"c:\c#\main.html");
                        byte[] msg = Encoding.UTF8.GetBytes(info_dla_web);
                        stream.Write(msg, 0, msg.Length);

                        if (data.StartsWith("GET /?x1="))
                        {
                            double x1 = Convert.ToDouble(data.Replace("GET /?x1=", "").Split('&')[0]);
                            double x2 = Convert.ToDouble(data.Replace("x2=", "").Split('&')[1]);
                            int N = Convert.ToInt32(data.Replace("x3=", "").Split('&')[2]);

                            double s = MonteParallel(x1, x2, N, 8);
                            byte[] lr2 = Encoding.UTF8.GetBytes(Convert.ToString(s));
                            stream.Write(lr2, 0, lr2.Length);

                        }
                        data = "";
                        stream.Close();
                    }
                });
                th.IsBackground = true;
                th.Start();
            }
            start();
        }
    }
}
