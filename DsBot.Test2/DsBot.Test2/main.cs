using DsBot.Test2.commands;
using DsBot.Test2.config;
using DsBot.Test2.YouTube;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using System.Threading.Tasks;
using Timer = System.Timers.Timer;
using System;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlus.EventArgs;
using DSharpPlus.CommandsNext.Exceptions;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

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
            Client.MessageDeleted += Client_Message_Deleted;


            var commandsConfig = new CommandsNextConfiguration()
            {
                StringPrefixes = new[] { jsonReader.prefix },
                EnableMentionPrefix = true,
                EnableDms = true,
                EnableDefaultHelp = false
            };

            Commands = Client.UseCommandsNext(commandsConfig);
            Commands.RegisterCommands<TestCommands>();

            Commands.CommandErrored += On_Command_Errored;

            await Client.ConnectAsync();
            await StartVideoUploadCheck();
            await Task.Delay(-1);
        }

        private static async Task On_Command_Errored(CommandsNextExtension sender, CommandErrorEventArgs args)
        {
            if (args.Exception is ChecksFailedException exception)
            {
                string timeLeft = "";
                string roleName = "";

                foreach (var check in exception.FailedChecks)
                {
                    if (check is CooldownAttribute cooldown)
                    {
                        timeLeft = cooldown.GetRemainingCooldown(args.Context).ToString(@"hh\:mm\:ss");
                        var coolDownMessage = new DiscordEmbedBuilder()
                        {
                            Title = "Ошибка",
                            Description = $"Подождите {timeLeft}",
                            Color = DiscordColor.Red
                        };


                        await args.Context.Channel.SendMessageAsync(embed: coolDownMessage);
                    }
                    
                    else if (check is RequireRolesAttribute role)
                    {
                        roleName = role.RoleNames[0];

                        var roleMessage = new DiscordEmbedBuilder()
                        {
                            Title = "Ошибка",
                            Description = $"У вас нет роли {roleName}",
                            Color = DiscordColor.Red
                        };

                        await args.Context.Channel.SendMessageAsync(embed: roleMessage);
                    }
                }
            }
        }


        private static async Task Client_Message_Deleted(DiscordClient sender, MessageDeleteEventArgs args)
        {
            await  args.Channel.SendMessageAsync("Сообщение удалено");
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
