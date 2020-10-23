using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

public class TrackModule : ModuleBase<SocketCommandContext>
{
    [Command("latest")]
    public async Task TestAsync()
    {
        DirectoryInfo trackDirectory = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory + @"Track Files");
        FileInfo trackFile = trackDirectory.GetFiles().Where(file => file.Extension == ".trk").OrderByDescending(file => file.LastWriteTime).First();

        await Context.Channel.SendFileAsync(trackFile.FullName);
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

        if (!(message.HasStringPrefix("!trackbot ", ref argPos) || message.HasMentionPrefix(_client.CurrentUser, ref argPos)) || message.Author.IsBot) return;

        var context = new SocketCommandContext(_client, message);

        await _commands.ExecuteAsync(context: context, argPos: argPos, services: null);
    }
}