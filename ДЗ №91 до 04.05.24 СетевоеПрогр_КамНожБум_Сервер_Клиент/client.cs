using System;
using System.IO;
using System.Net.Sockets;
using System.Threading;

class Client
{
    private TcpClient tcpClient;
    private StreamReader reader;
    private StreamWriter writer;

    // Конструктор клиента, инициирующий подключение к серверу
    public Client(string ip, int port)
    {
        tcpClient = new TcpClient();
        tcpClient.Connect(ip, port); // Подключение к серверу по IP адресу и порту
        Console.WriteLine("Connected to the server!");

        NetworkStream stream = tcpClient.GetStream(); // Получение сетевого потока для чтения и записи данных
        reader = new StreamReader(stream); // Инициализация объекта для чтения данных из потока
        writer = new StreamWriter(stream) { AutoFlush = true }; // Инициализация объекта для записи данных в поток с автоматической отправкой данных
    }

    // Основной метод, управляющий процессом игры клиента
    public void PlayGame()
    {
        try
        {
            for (int round = 1; round <= 5; round++) // Цикл игры из 5 раундов
            {
                Console.Write("Round {0}. Enter your choice (Rock, Paper, Scissors): ", round);
                string userChoice = Console.ReadLine(); // Чтение выбора пользователя с консоли

                writer.WriteLine(userChoice); // Отправка выбора пользователя на сервер

                string response = reader.ReadLine(); // Чтение ответа от сервера
                if (response == null) throw new Exception("Server disconnected unexpectedly."); // Проверка на внезапное отключение сервера
                Console.WriteLine(response); // Вывод результата раунда
            }

            string finalResult = reader.ReadLine(); // Чтение итогового сообщения игры
            if (finalResult == null) throw new Exception("Server disconnected unexpectedly."); // Проверка на внезапное отключение
            Console.WriteLine(finalResult); // Вывод итогового сообщения

            Console.WriteLine("You will be disconnected in 10 seconds..."); // Уведомление о предстоящем отключении
            Thread.Sleep(10000); // Задержка перед закрытием клиента, чтобы пользователь мог прочитать сообщение
        }
        catch (Exception ex)
        {
            Console.WriteLine("An error occurred: " + ex.Message); // Вывод сообщения об ошибке
        }
        finally
        {
            tcpClient.Close(); // Закрытие подключения
            Console.WriteLine("Disconnected from the server."); // Сообщение об отключении
        }
    }

    static void Main(string[] args)
    {
        try
        {
            Client client = new Client("127.0.0.1", 8888); // Создание объекта клиента и подключение к серверу
            client.PlayGame(); // Запуск игры
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message); // Обработка ошибок подключения или других исключений
        }
    }
}
