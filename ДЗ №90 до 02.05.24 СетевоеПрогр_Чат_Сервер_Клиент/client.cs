using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;

class Program
{
    static void Main(string[] args)
    {
        TcpClient client = new TcpClient("localhost", 13000);
        NetworkStream stream = client.GetStream();

        // Создаем новый поток для чтения входящих сообщений
        Thread t = new Thread(() => ReadMessages(stream));
        t.Start();

        // Создаем новый поток для чтения ввода пользователя
        Thread inputThread = new Thread(() => SendMessages(stream));
        inputThread.Start();
    }

    static void ReadMessages(NetworkStream stream)
    {
        byte[] data = new byte[256];
        string responseData = String.Empty;

        while (true)
        {
            int bytes = stream.Read(data, 0, data.Length);
            responseData = Encoding.UTF8.GetString(data, 0, bytes);
            Console.WriteLine("Получено: {0}", responseData);
            Console.WriteLine("Введите сообщение: ");
        }
    }

    static void SendMessages(NetworkStream stream)
    {
        while (true)
        {
            Console.WriteLine("Введите сообщение: ");
            string message = Console.ReadLine();

            if (message == "exit")
                break;

            byte[] data = Encoding.UTF8.GetBytes(message);
            stream.Write(data, 0, data.Length);

            //Console.WriteLine("Отправлено: {0}", message);
        }
    }
}
