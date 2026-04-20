using System.Net;
using System.Net.Sockets;
using System.Text;

class Program
{
    private static UdpClient udpClient = new UdpClient();
    private static IPEndPoint serverEndpoint = null!;

    static async Task Main(string[] args)
    {
        if (args.Length < 3)
        {
            Console.WriteLine("Использование: Client <server_ip> <port> <username>");
            return;
        }

        string serverHost = args[0];
        int port = int.Parse(args[1]);
        string username = args[2];

        var addresses = await Dns.GetHostAddressesAsync(serverHost);
        serverEndpoint = new IPEndPoint(addresses[0], port);

        Console.WriteLine($"Клиент {username} подключен к {serverHost}:{port}");
        Console.WriteLine("Пишите сообщения:");

        _ = Task.Run(ReceiveMessages);

        while (true)
        {
            string? input = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(input)) continue;

            string fullMessage = $"{username}: {input}";
            byte[] data = Encoding.UTF8.GetBytes(fullMessage);

            await udpClient.SendAsync(data, data.Length, serverEndpoint);
        }
    }

    private static async Task ReceiveMessages()
    {
        while (true)
        {
            try
            {
                var result = await udpClient.ReceiveAsync();
                string message = Encoding.UTF8.GetString(result.Buffer);
                Console.WriteLine(message);
            }
            catch { }
        }
    }
}
