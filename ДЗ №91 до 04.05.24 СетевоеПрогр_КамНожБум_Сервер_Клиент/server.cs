using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;

class Server
{
    private TcpListener listener;
    private TcpClient[] clients = new TcpClient[2]; // массив для хранения подключений клиентов
    private StreamReader[] readers = new StreamReader[2]; // массив для чтения данных от клиентов
    private StreamWriter[] writers = new StreamWriter[2]; // массив для отправки данных клиентам

    // Конструктор сервера, который инициализирует слушателя на заданном IP адресе и порте
    public Server(string ip, int port)
    {
        IPAddress ipAddress = IPAddress.Parse(ip); // Преобразование IP-адреса из строки в объект IPAddress
        listener = new TcpListener(ipAddress, port); // Создание объекта TcpListener
        listener.Start(); // Запуск прослушивания порта
    }

    // Метод для принятия клиентских подключений
    public void AcceptClients()
    {
        Console.WriteLine("Waiting for clients...");
        for (int i = 0; i < clients.Length; i++)
        {
            clients[i] = listener.AcceptTcpClient(); // Принятие нового клиента
            var stream = clients[i].GetStream(); // Получение потока данных клиента
            readers[i] = new StreamReader(stream); // Инициализация объекта для чтения из потока
            writers[i] = new StreamWriter(stream) { AutoFlush = true }; // Инициализация объекта для записи в поток с автоочисткой
            Console.WriteLine($"Client {i + 1} connected."); // Сообщение о подключении клиента
        }
    }

    // Метод для начала игры
    public void StartGame()
    {
        int player1Wins = 0; // Счётчик побед первого игрока
        int player2Wins = 0; // Счётчик побед второго игрока

        for (int round = 1; round <= 5; round++) // Игра состоит из 5 раундов
        {
            Console.WriteLine($"Round {round}");
            string[] choices = new string[2]; // Массив для хранения выборов игроков
            for (int i = 0; i < clients.Length; i++)
            {
                choices[i] = readers[i].ReadLine(); // Чтение выбора от каждого клиента
                Console.WriteLine($"Client {i + 1} choice: {choices[i]}");
            }

            string result = DetermineWinner(choices[0], choices[1]); // Определение победителя раунда

            // Учет результатов для подсчета итоговых результатов игры
            if (result.Contains("Player 1 wins")) player1Wins++;
            if (result.Contains("Player 2 wins")) player2Wins++;

            // Отправка результатов раунда обратно клиентам
            for (int i = 0; i < clients.Length; i++)
            {
                writers[i].WriteLine($"Round {round} - Your choice: {choices[i]}, Opponent's choice: {choices[(i + 1) % 2]}, Result: {result}");
            }

            Console.WriteLine($"Round {round} result: {result}");
        }

        // Определение и объявление итоговой победы или ничьей
        string finalResult;
        if (player1Wins > player2Wins)
            finalResult = "Player 1 wins the game!";
        else if (player2Wins > player1Wins)
            finalResult = "Player 2 wins the game!";
        else
            finalResult = "The game is a draw!";

        Console.WriteLine($"Final score - Player 1: {player1Wins}, Player 2: {player2Wins}. {finalResult}");

        // Отправка финального результата клиентам
        foreach (var writer in writers)
        {
            writer.WriteLine($"Final score - Player 1: {player1Wins}, Player 2: {player2Wins}. {finalResult}");
            writer.Flush();  // Принудительная отправка данных
        }

        Thread.Sleep(10000);  // Задержка перед закрытием соединений, чтобы клиенты могли прочитать сообщение

        // Закрытие клиентских соединений
        for (int i = 0; i < clients.Length; i++)
        {
            clients[i].Close();
        }
    }

    // Метод для определения победителя раунда на основе выборов игроков
    public string DetermineWinner(string choice1, string choice2)
    {
        if (choice1 == choice2) // Ничья
        {
            return "Draw";
        }

        // Правила игры "камень, ножницы, бумага"
        switch (choice1)
        {
            case "Rock":
                return (choice2 == "Scissors") ? "Player 1 wins" : "Player 2 wins";
            case "Paper":
                return (choice2 == "Rock") ? "Player 1 wins" : "Player 2 wins";
            case "Scissors":
                return (choice2 == "Paper") ? "Player 1 wins" : "Player 2 wins";
            default:
                return "Invalid choice";
        }
    }

    static void Main(string[] args)
    {
        Server server = new Server("127.0.0.1", 8888);

        while (true)  // Бесконечный цикл для непрерывного приема клиентов и проведения игр
        {
            server.AcceptClients(); // Принятие подключений от двух клиентов
            server.StartGame(); // Запуск игры
        }
    }
}
