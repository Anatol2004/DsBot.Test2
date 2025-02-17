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
        // СТАТИЧЕСКИЕ ОБЪЕКТЫ ------------------------------------------------
        public static DiscordClient Client { get; set; }
        public static CommandsNextExtension Commands { get; set; }
        private static youtubeVideo _video = new youtubeVideo();    
        private static youtubeVideo _temp = new youtubeVideo();
        private static Engine _engine = new Engine();
        //---------------------------------------------------------------------



        // АСИНХРОННЫЙ МЕТОД "Main"-------------------------------------------------------------------------------------------------------
        public static async Task Main(string[] args)
        {
            var jsonReader = new JSONReader();
            await jsonReader.ReadJson();

            var discordConfig = new DiscordConfiguration()
            {
                Token = jsonReader.token,
                TokenType = TokenType.Bot,          // ЧТЕНИЕ ТОКЕНА БОТА И ПРЕФИКСА ИЗ КОМАНД JSON-ФАЙЛА
                Intents = DiscordIntents.All,       // УКАЗЫВАЕТСЯ ТОКЕН БОТА И INTENTS
                AutoReconnect = true
                 
            };

            Client = new DiscordClient(discordConfig);

            Client.UseInteractivity(new DSharpPlus.Interactivity.InteractivityConfiguration()
            {
                Timeout = TimeSpan.FromMinutes(2)  // НАСТРОЙКА ТАЙМАУТА ДЛЯ ИНТЕРАКТИВНЫХ ФУНКЦИЙ
            });

            // ПОДПИСКИ НА СОБЫТИЯ
            Client.Ready += Client_Ready; // СОБЫТИЕ ПРИ ГОТОВНОСТИ БОТА
            Client.MessageDeleted += Client_Message_Deleted; // СОБЫТИЕ ПРИ УДАЛЕНИИ СООБЩЕНИЯ
            Client.ComponentInteractionCreated += Client_ComponentInteractionCreated; // СОБЫТИЕ ПРИ ВЗАИМОДЕСТВИИ С КОМПОНЕНТАМИ (КНОПКИ)

            var commandsConfig = new CommandsNextConfiguration()
            {
                StringPrefixes = new[] { jsonReader.prefix },
                EnableMentionPrefix = true,
                EnableDms = true,
                EnableDefaultHelp = false
            };

            // РЕГИСТРАЦИЯ ПРЕФИКСНЫХ КОММАНД (Command) И СЛЕШ-КОМАНД (slachCommandsConfig)
            var slachCommandsConfig = Client.UseSlashCommands();
            Commands = Client.UseCommandsNext(commandsConfig);

            slachCommandsConfig.RegisterCommands<BasicSlashCommands>();
            slachCommandsConfig.RegisterCommands<Calculator>();
            Commands.RegisterCommands<TestCommands>();


            Commands.CommandErrored += On_Command_Errored; // СОБЫТИЕ ПРИ ВОЗНИКНОВЕНИЯ ОШИБОК ПРИ НАПИСАНИИ КОМАНД

            // ЗАПУСК БОТА
            await Client.ConnectAsync(); // ПОДКЛЮЧЕНИЕ БОТА К DISCORD
            await StartVideoUploadCheck(); // ПРОВЕРКА НОВЫХ ВИДЕО 
            await Task.Delay(-1); // ПРОГРАММА РАБОТАЕТ ДО ТЕХ ПОР, ПОКА РАБОТАЕТ ПРОГРАММА
        }//------------------------------------------------------------------------------------------------------------------------------------



        // ОБРАБОТКА СОБЫТИЯ "Client_ComponentInteractionCreated"------------------------------------------------------------------------------------------------
        private static async Task Client_ComponentInteractionCreated(DiscordClient sender, ComponentInteractionCreateEventArgs args)
        {

            // ОБРАБАТЫВАЕТ НАЖАТИЯ КНОПОК (testButton1, testButton2, back)
            // В ЗАВИСИМОСТИ ОТ НАЖАТОЙ КНОПКИ ОТПРАВЛЯЕТ СООТВЕТСВУЮЩЕЕ СООБЩЕНИЕ ИЛИ ОБНОВЛЯЕТ ТЕКУЩЕЕ

            switch (args.Interaction.Data.CustomId)
            {
                case "testButton1": // НАЖАТИЕ НА КНОПКУ "testButton1"

                    await args.Interaction.DeferAsync();

                    var embedMessage = new DiscordEmbedBuilder()
                    {
                        Title = "Test Button 1",
                        Color = DiscordColor.Green
                    };

                    await args.Interaction.EditOriginalResponseAsync(new DiscordWebhookBuilder().AddEmbed(embedMessage));

                    break;


                case "testButton2": // НАЖАТИЕ НА КНОПКУ "testButton2"

                    var button = new DiscordButtonComponent(ButtonStyle.Danger, "back", "back"); // ПОЯВЛЕНИЕ ДРУГОЙ КНОПКИ ПРИ НАЖАТИИ КНОПКИ "testButton2"

                    var embedMessage2 = new DiscordEmbedBuilder()
                    {
                        Title = "Test Button 2",
                        Color = DiscordColor.Red
                    };

                    await args.Interaction.CreateResponseAsync(InteractionResponseType.UpdateMessage, new DiscordInteractionResponseBuilder().AddEmbed(embedMessage2).
                        AddComponents(button));

                    break;


                case "back": // НАЖАТИЕ НА КНОПКУ "back"

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
        }//---------------------------------------------------------------------------------------------------------------------------------------------------------------



        // ОБРАБОТКА СОБЫТИЯ "On_Command_Errored"-------------------------------------------------------------------------------
        private static async Task On_Command_Errored(CommandsNextExtension sender, CommandErrorEventArgs args) 
            
            // ОБРАБАТЫВАЕТ ОШИБКИ ПРИ ВЫПОЛНЕНИИ КОММАНД (НЕТ НУЖНОЙ РОЛИ ИЛИ КОМАНДА НА ПЕРЕЗАРЯДКЕ)

        {
            if (args.Exception is ChecksFailedException exception)
            {
                string timeLeft = "";
                string roleName = "";

                foreach (var check in exception.FailedChecks)
                {

                    // ЕСЛИ КОММАНДА ЕЩЁ НЕ ПЕРЕЗАРЯДИЛАСЬ

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
                    
                    // ЕСЛИ НЕТ РОЛИ ДЛЯ ВЫПОЛНЕНИЯ КОМАНДЫ

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
        }//------------------------------------------------------------------------------------------------------------------



        // ОБРАБОТКА СОБЫТИЯ "Client_Message_Deleted"------------------------------------------------------------
        private static async Task Client_Message_Deleted(DiscordClient sender, MessageDeleteEventArgs args)

            // ПОСЛЕ УДАЛЕНИЯ ЛЮБОГО СООБЩЕНИЯ ПОВЯВЛЯЕТСЯ СООБЩЕНИЕ О ЕГО УДАЛЕНИИ

        {
            await  args.Channel.SendMessageAsync("Сообщение удалено");
            await args.Channel.DeleteAsync();
        }//------------------------------------------------------------------------------------------------------



        // ОБРАБОТКА СОБЫТИЯ "Client_Ready"------------------------------------------------------------------
        private static Task Client_Ready(DiscordClient sender, DSharpPlus.EventArgs.ReadyEventArgs args)

            // ГОТОВНОСТЬ БОТА К РАБОТЕ

        {
            return Task.CompletedTask;
        }//--------------------------------------------------------------------------------------------------



        // АСИНХРОННЫЙ МЕТОД "StartVideoUploadCheck"-------------------------------------------------------------------------------------------------------------
        private static async Task StartVideoUploadCheck()

            // МЕТОД ДЛЯ ПРОВЕРКИ НОВЫХ ВИДЕО НА YOUTUBE-КАНАЛЕ

        {
            var timer = new Timer(30000); // ОБЪЯВЛЕНИЕ ТАЙМЕРА НА КАЖДЫЕ 30 СЕКУНД (УКАЗЫВАЕТСЯ В МИЛЛИСЕКУНДАХ)
            timer.Elapsed += async (sender, e) =>
            {
                _video = _engine.GetLatestVideo();
                var lastTimeCheackAt = DateTime.Now;

                if (_video != null)
                {
                    // ЕСЛИ ВИДЕО БЫЛО ОТПРАВЛЕННО РАНЕЕ, ТО ОНО ИГНОРИРУЕТСЯ

                    if (_temp.VideoTitle == _video.VideoTitle)
                    {
                        Console.WriteLine("Нет нового видео.");
                    }

                    // ЕСЛИ ПОЯВИЛОСТ НОВОЕ ВИДЕО, ТО СООБЩАЕТСЯ В В УКАЗАННЫЙ КАНАЛ В DISCORD-СЕРВЕРЕ

                    else if (_video.PublishedAt < lastTimeCheackAt)
                    {
                        var message = $"Вышло новое видео! {_video.VideoTitle}\n" + $"Опубликованно: {_video.PublishedAt}\n" + $"Ссылка: {_video.VideoURL}\n";
                        _temp = _video;

                        await Client.GetChannelAsync(1335883551916429375).Result.SendMessageAsync(message);
                    }                    
                }
            };

            timer.Start(); // ЗАПУСК ТАЙМЕРА
            
        }//------------------------------------------------------------------------------------------------------------------------------------------------------
    }
}
