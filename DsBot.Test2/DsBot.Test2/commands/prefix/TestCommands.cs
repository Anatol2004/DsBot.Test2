using DsBot.Test2.diceGame;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity.Extensions;
using System.Threading.Tasks;

namespace DsBot.Test2.commands
{
    // ДАННЫЙ КЛАСС СОДЕРЖИТ ПРЕФИКСНЫЕ КОМАНДЫ БОТА
    public class TestCommands : BaseCommandModule
    {
        // ПРЕФИКСНАЯ КОМАНДА "interect" -----------------------------------------------------------------------------
        // Если сообщение содержит "привет", бот отправляет ответ с упоминанием пользователя.
        [Command("interect")]
        public async Task Interact(CommandContext ctx)
        {
            var interactivity = main.Client.GetInteractivity();

            var messageToRecive = await interactivity.WaitForMessageAsync(message => message.Content == "Привет");
            if (messageToRecive.Result.Content == "Привет")
            {
                await ctx.Channel.SendMessageAsync($"{ctx.User.Username} сказал привет.");
            }
        }//------------------------------------------------------------------------------------------------------------



        // ПРЕФИКСНАЯ КОМАНДА "emoji" --------------------------------------------------------------------------------------------
        // Если пользователь под конкретное сообщение поставит реацию, то бот об этом напишет
        [Command("emoji")]
        public async Task Emoji(CommandContext ctx)
        {
            var interactivity = main.Client.GetInteractivity();

            var messageToReact = await interactivity.WaitForReactionAsync(message => message.Message.Id == 1340587914710487062);
            if (messageToReact.Result.Message.Id == 1340587914710487062)
            {
                await ctx.Channel.SendMessageAsync($"{ctx.User.Username} поставил реакцию {messageToReact.Result.Emoji.Name}.");
            }
        }//-----------------------------------------------------------------------------------------------------------------------
            


        // ПРЕФИКСНАЯ КОМАНДА "hello"------------------------------------------------------------------------------------
        // После ввода команды бот отправляет "Привет!" в виде сообщения.
        // Эту команду можно использовать 3 раза за 10 секунд, и могут воспользоваться пользователи с ролью "VIP"
        [Command("hello")]
        [Cooldown(3, 10, CooldownBucketType.User)]
        [RequireRoles(RoleCheckMode.Any, roleNames: new string[] {"VIP"})]
        public async Task Hello(CommandContext ctx)
        {
            await ctx.Channel.SendMessageAsync("Привет!");
        }//--------------------------------------------------------------------------------------------------------------



        // ПРЕФИКСНАЯ КОМАНДА "random"--------------------------------------------------
        // Команда генерирует число в указанном диапозоне
        [Command("random")]
        public async Task Random(CommandContext ctx, int min, int max)
        {
            var randomValue = new System.Random().Next(min, max);
            await ctx.Channel.SendMessageAsync(ctx.User.Username + " " + randomValue);
        }//


        [Command("embed")]
        public async Task Embed(CommandContext ctx)
        {
            var message = new DiscordEmbedBuilder()
            {
                Title = "Первый эмбед",
                Description = $"Данный эмбед создан по запросу {ctx.User.Username}",
                Color = DiscordColor.Orange
            };

            await ctx.Channel.SendMessageAsync(embed: message);
        }

        [Command("dice_game")]
        public async Task DiceGame(CommandContext ctx)
        {
            var playerThrow = new DiceGame();
            var playerScore = playerThrow.RollSum;

            var message = new DiscordEmbedBuilder()
            {
                Title = $"Игрок выбросил: {playerScore}",
                Color = DiscordColor.Aquamarine
            };

            await ctx.Channel.SendMessageAsync(embed: message);

            var botThrow = new DiceGame();
            var botScore = botThrow.RollSum;

            var botMessage = new DiscordEmbedBuilder()
            {
                Title = $"Бот выбросил {botScore}",
                Color = DiscordColor.Aquamarine
            };

            await ctx.Channel.SendMessageAsync(embed: botMessage);

            if (playerScore > botScore)
            {
                var winMessage = new DiscordEmbedBuilder()
                {
                    Title = "Вы победили!",
                    Color = DiscordColor.Green
                };

                await ctx.Channel.SendMessageAsync(embed: winMessage);
            }
            else if (playerScore < botScore)
            {
                var loseMessage = new DiscordEmbedBuilder()
                {
                    Title = "Вы проиграли!",
                    Color = DiscordColor.Red
                };

                await ctx.Channel.SendMessageAsync(embed: loseMessage);
            }
            else
            {
                var drawnMessage = new DiscordEmbedBuilder()
                {
                    Title = "Ничья!",
                    Color = DiscordColor.Yellow
                };

                await ctx.Channel.SendMessageAsync(embed: drawnMessage);
            }
        }
    }
}
