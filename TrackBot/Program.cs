using System;
using System.Reflection;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

public class TrackModule : ModuleBase<SocketCommandContext>
{
    [Command("test")]
    public async Task TestAsync()
    {
        await Context.Channel.SendMessageAsync("Test");
    }
}

public class Program
{
    public static void Main(string[] args) => new Program().MainAsync().GetAwaiter().GetResult();

    private DiscordSocketClient _client;
    private CommandService _commands;

    public async Task MainAsync()
    {
        _client = new DiscordSocketClient();
        _client.Log += Log;

        var token = System.IO.File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + "token.txt");
        _commands = new CommandService();
        CommandHandler handler = new CommandHandler(_client, _commands);
        await handler.InstallCommandsAsync();

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

public class CommandHandler
{
    private readonly DiscordSocketClient _client;
    private readonly CommandService _commands;

    public CommandHandler(DiscordSocketClient client, CommandService commands)
    {
        _commands = commands;
        _client = client;
    }

    public async Task InstallCommandsAsync()
    {
        _client.MessageReceived += HandleCommandAsync;

        await _commands.AddModulesAsync(assembly: Assembly.GetEntryAssembly(), services: null);
    }

    private async Task HandleCommandAsync(SocketMessage messageParam)
    {
        var message = messageParam as SocketUserMessage;
        if (message == null) return;

        int argPos = 0;

        if (!(message.HasStringPrefix("!trackbot ", ref argPos) ||
            message.HasMentionPrefix(_client.CurrentUser, ref argPos)) ||
            message.Author.IsBot)
            return;

        if (!message.HasStringPrefix("!trackbot ", ref argPos) || !message.HasMentionPrefix(_client.CurrentUser, ref argPos)) return;
        if (message.Author.IsBot) return;

        var context = new SocketCommandContext(_client, message);

        await _commands.ExecuteAsync(context: context, argPos: argPos, services: null);
    }
}

