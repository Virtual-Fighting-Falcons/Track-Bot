using System;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;

public class Program
{
    public static void Main(string[] args)
    => new Program().MainAsync().GetAwaiter().GetResult();

    private DiscordSocketClient _client;

    public async Task MainAsync()
    {
        _client = new DiscordSocketClient();
        _client.Log += Log;

        var token = System.IO.File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + "token.txt");

        await _client.LoginAsync(TokenType.Bot, token);
        await _client.StartAsync();

        await Task.Delay(-1);
    }

    private Task Log(LogMessage message)
    {
        Console.WriteLine(message.ToString());
        return Task.CompletedTask;
    }
}