using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace L7_Chat
{
    internal class Program
    {
        static int RemotePort;
        static int LocalPort;
        static IPAddress RemoteIP;
        static string UserName;

        static void Main(string[] args)
        {
            try
            {
                Console.SetWindowSize(40, 20);
                Console.Title = "Chat";
                Console.Write("Введите имя пользователя: ");
                UserName = Console.ReadLine();
                Console.Write("Введите удалённый IP: ");
                RemoteIP = IPAddress.Parse(Console.ReadLine());
                Console.Write("Введите удалённый порт: ");
                RemotePort = Convert.ToInt32(Console.ReadLine());
                Console.Write("Введите локальный порт: ");
                LocalPort = Convert.ToInt32(Console.ReadLine());
                Thread thread = new Thread(new ThreadStart(ReceiveThread));
                thread.IsBackground = true;
                thread.Start();
                Console.ForegroundColor = ConsoleColor.Red;
                while (true)
                {
                    string message = Console.ReadLine();
                    string formattedMessage = $"{UserName}: {message}";
                    SendData(formattedMessage);
                }
            }
            catch (FormatException ex)
            {
                Console.WriteLine("Преобразование невозможно: " + ex.Message);
            }
            catch (SocketException ex)
            {
                Console.WriteLine("Ошибка соединения: " + ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        static void ReceiveThread()
        {
            try
            {
                UdpClient uClient = new UdpClient(LocalPort);
                IPEndPoint ipEnd = null;
                while (true)
                {
                    byte[] response = uClient.Receive(ref ipEnd);
                    string res = Encoding.Unicode.GetString(response);
                    int colonIndex = res.IndexOf(':');
                    if (colonIndex != -1)
                    {
                        string senderName = res.Substring(0, colonIndex + 1);
                        string messageText = res.Substring(colonIndex + 1);

                        Console.ForegroundColor = ConsoleColor.Magenta; // Цвет имени пользователя
                        Console.Write(senderName + " ");
                        Console.ForegroundColor = ConsoleColor.Green; // Цвет сообщения
                        Console.WriteLine(messageText);
                    }
                    Console.ForegroundColor = ConsoleColor.Red; // Возврат цвета для ввода
                }
            }
            catch (SocketException ex)
            {
                Console.WriteLine("Ошибка соединения: " + ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        static void SendData(string datagram)
        {
            UdpClient uClient = new UdpClient();
            IPEndPoint ipEnd = new IPEndPoint(RemoteIP, RemotePort);
            try
            {
                byte[] bytes = Encoding.Unicode.GetBytes(datagram);
                uClient.Send(bytes, bytes.Length, ipEnd);
            }

            catch (SocketException ex)
            {
                Console.WriteLine("Ошибка соединения: " + ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                uClient.Close();
            }
        }
    }
}

