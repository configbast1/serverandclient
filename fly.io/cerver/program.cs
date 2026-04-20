using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Collections.Concurrent;

class Program
{
    private static UdpClient? udpClient;
    private static readonly ConcurrentBag<IPEndPoint> clients = new();
    private const int Port = 12345;

    static async Task Main(string[] args)
    {
        string host = "0.0.0.0";
        
        var localEndpoint = new IPEndPoint(IPAddress.Parse(host), Port);
        udpClient = new UdpClient(localEndpoint);

        Console.WriteLine("Сервер запущен на порту " + Port);

        while (true)
        {
            var result = await udpClient.ReceiveAsync();
            var senderEndpoint = result.RemoteEndPoint;
            string message = Encoding.UTF8.GetString(result.Buffer);

            if (!clients.Any(c => c.Equals(senderEndpoint)))
            {
                clients.Add(senderEndpoint);
            }

            var allClients = clients.ToArray();
            foreach (var client in allClients)
            {
                if (client.Equals(senderEndpoint)) continue;

                try
                {
                    await udpClient.SendAsync(result.Buffer, result.Buffer.Length, client);
                }
                catch { }
            }
        }
    }
}
