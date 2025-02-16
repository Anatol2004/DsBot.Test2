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
using DSharpPlus.SlashCommands;
using DsBot.Test2.commands.slach;
using DsBot.Test2.commands.slash;

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
            Client.ComponentInteractionCreated += Client_ComponentInteractionCreated;


            var commandsConfig = new CommandsNextConfiguration()
            {
                StringPrefixes = new[] { jsonReader.prefix },
                EnableMentionPrefix = true,
                EnableDms = true,
                EnableDefaultHelp = false
            };

            var slachCommandsConfig = Client.UseSlashCommands();
            slachCommandsConfig.RegisterCommands<BasicSlashCommands>();
            slachCommandsConfig.RegisterCommands<Calculator>();

            Commands = Client.UseCommandsNext(commandsConfig);
            Commands.RegisterCommands<TestCommands>();

            Commands.CommandErrored += On_Command_Errored;

            await Client.ConnectAsync();
            await StartVideoUploadCheck();
            await Task.Delay(-1);
        }

        private static async Task Client_ComponentInteractionCreated(DiscordClient sender, ComponentInteractionCreateEventArgs args)
        {
            switch (args.Interaction.Data.CustomId)
            {
                case "testButton1":

                    await args.Interaction.DeferAsync();

                    var embedMessage = new DiscordEmbedBuilder()
                    {
                        Title = "Test Button 1",
                        Color = DiscordColor.Green
                    };

                    await args.Interaction.EditOriginalResponseAsync(new DiscordWebhookBuilder().AddEmbed(embedMessage));

                    break;


                case "testButton2":

                    var button = new DiscordButtonComponent(ButtonStyle.Danger, "back", "back");

                    var embedMessage2 = new DiscordEmbedBuilder()
                    {
                        Title = "Test Button 2",
                        Color = DiscordColor.Red
                    };

                    await args.Interaction.CreateResponseAsync(InteractionResponseType.UpdateMessage, new DiscordInteractionResponseBuilder().AddEmbed(embedMessage2).AddComponents(button));

                    break;


                case "back":

                    var button1 = new DiscordButtonComponent(ButtonStyle.Primary, "testButton1", "Test Button 1");
                    var button2 = new DiscordButtonComponent(ButtonStyle.Danger, "testButton2", "Test Button 2");

                    var embedMessage3  = new DiscordEmbedBuilder()
                    {
                        Title = "Button Command",
                        Color = DiscordColor.Blue
                    };

                    await args.Interaction.CreateResponseAsync(InteractionResponseType.UpdateMessage, 
                        new DiscordInteractionResponseBuilder().AddEmbed(embedMessage3).
                        AddComponents(button1, button2));

                    break;
            };
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
