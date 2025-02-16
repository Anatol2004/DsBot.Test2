using DsBot.Test2.commands;
using DsBot.Test2.config;
using DsBot.Test2.YouTube;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using System.Threading.Tasks;
using Timer = System.Timers.Timer;
using System;
using DSharpPlus.Interactivity.Extensions;

namespace DsBot.Test2
{
    internal class main
    {

        public static DiscordClient Client { get; set; }
        public static CommandsNextExtension Commands { get; set; }

        private static youtubeVideo _video = new youtubeVideo();
        private static youtubeVideo _temp = new youtubeVideo();
        private static Engine _engine = new Engine();

        public static async Task Main(string[] args)
        {
            var jsonReader = new JSONReader();
            await jsonReader.ReadJson();

            var discordConfig = new DiscordConfiguration()
            {
                Token = jsonReader.token,
                TokenType = TokenType.Bot,
                Intents = DiscordIntents.All,
                AutoReconnect = true
                 
            };

            Client = new DiscordClient(discordConfig);

            Client.UseInteractivity(new DSharpPlus.Interactivity.InteractivityConfiguration()
            {
                Timeout = TimeSpan.FromMinutes(2)
            });

            Client.Ready += Client_Ready;

            var commandsConfig = new CommandsNextConfiguration()
            {
                StringPrefixes = new[] { jsonReader.prefix },
                EnableMentionPrefix = true,
                EnableDms = true,
                EnableDefaultHelp = false
            };

            Commands = Client.UseCommandsNext(commandsConfig);
            Commands.RegisterCommands<TestCommands>();

            await Client.ConnectAsync();
            await StartVideoUploadCheck();
            await Task.Delay(-1);
        }

        private static Task Client_Ready(DiscordClient sender, DSharpPlus.EventArgs.ReadyEventArgs args)
        {
            return Task.CompletedTask;
        }

        private static async Task StartVideoUploadCheck()
        {
            var timer = new Timer(30000);
            timer.Elapsed += async (sender, e) =>
            {
                _video = _engine.GetLatestVideo();
                var lastTimeCheackAt = DateTime.Now;

                if (_video != null)
                {
                    if (_temp.VideoTitle == _video.VideoTitle)
                    {
                        Console.WriteLine("Нет нового видео.");
                    }
                    else if (_video.PublishedAt < lastTimeCheackAt)
                    {
                        var message = $"Вышло новое видео! {_video.VideoTitle}\n" + $"Опубликованно: {_video.PublishedAt}\n" + $"Ссылка: {_video.VideoURL}\n";
                        _temp = _video;

                        await Client.GetChannelAsync(1335883551916429375).Result.SendMessageAsync(message);
                    }                    
                }
            };

            timer.Stop(); // ОСТАНОВИЛ ОПОВЕЩЕНИЯ ЮТУБ
        }
    }
}
