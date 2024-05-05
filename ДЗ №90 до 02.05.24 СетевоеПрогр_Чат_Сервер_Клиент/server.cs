using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

public class Server
{
    private TcpListener _server; // Объект, прослушивающий входящие подключения
    private Boolean _isRunning; // Флаг, указывающий, работает ли сервер
    private Dictionary<Socket, int> _clients = new Dictionary<Socket, int>(); // Список подключенных клиентов с их ID
    private int _clientId = 0; // Счетчик для генерации ID клиентов

    public Server(int port)
    {
        _server = new TcpListener(IPAddress.Any, port); // Создаем новый объект TcpListener
        _server.Start(); // Начинаем прослушивание

        _isRunning = true; // Устанавливаем флаг работы сервера

        LoopClients(); // Запускаем цикл обработки клиентов
    }

    public void LoopClients()
    {
        while (_isRunning) // Пока сервер работает...
        {
            // Ожидаем подключение клиента
            Socket newClient = _server.AcceptSocket();
            _clients.Add(newClient, _clientId++); // Добавляем нового клиента в список с уникальным ID

            // Клиент подключился.
            // Создаем новый поток для обработки сообщений от этого клиента
            Thread t = new Thread(() => HandleClient(newClient));
            t.Start();
        }
    }

    public void HandleClient(Socket s)
    {
        // Создаем поток для чтения
        NetworkStream sReader = new NetworkStream(s);

        Boolean bClientConnected = true;
        byte[] buffer = new byte[1024];

        while (bClientConnected)
        {
            // Читаем данные из потока
            int numByteRead = sReader.Read(buffer, 0, buffer.Length);
            if (numByteRead > 0)
            {
                string data = Encoding.UTF8.GetString(buffer, 0, numByteRead);

                // Выводим содержимое на консоль.
                Console.WriteLine("Client {0} > {1}", _clients[s], data);

                // Отправляем данные обратно всем клиентам
                foreach (var client in _clients.Keys)
                {
                    if (client != s) // Не отправляем данные тому клиенту, который их отправил
                    {
                        NetworkStream sWriter = new NetworkStream(client);
                        byte[] dataToSend = Encoding.UTF8.GetBytes($"Сообщение от клиента {_clients[s]}: {data}");
                        sWriter.Write(dataToSend, 0, dataToSend.Length);
                        sWriter.Flush();
                    }
                }
            }
            else
            {
                bClientConnected = false;
            }
        }

        // Корректно закрываем сокет
        s.Shutdown(SocketShutdown.Both);
        s.Close();
    }
}

class Program
{
    static void Main(string[] args)
    {
        Server server = new Server(13000); // Создаем новый сервер на порту 13000
    }
}
