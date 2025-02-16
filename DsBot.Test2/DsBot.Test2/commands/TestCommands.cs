using DsBot.Test2.diceGame;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity.Extensions;
using System.Threading.Tasks;

namespace DsBot.Test2.commands
{
    public class TestCommands : BaseCommandModule
    {
        [Command("interect")]
        public async Task Interact(CommandContext ctx)
        {
            var interactivity = main.Client.GetInteractivity();

            var messageToRecive = await interactivity.WaitForMessageAsync(message => message.Content == "Привет");
            if (messageToRecive.Result.Content == "Привет")
            {
                await ctx.Channel.SendMessageAsync($"{ctx.User.Username} сказал привет.");
            }
        }

        [Command("emoji")]
        public async Task Emoji(CommandContext ctx)
        {
            var interactivity = main.Client.GetInteractivity();

            var messageToReact = await interactivity.WaitForReactionAsync(message => message.Message.Id == 1340587914710487062);
            if (messageToReact.Result.Message.Id == 1340587914710487062)
            {
                await ctx.Channel.SendMessageAsync($"{ctx.User.Username} поставил реакцию {messageToReact.Result.Emoji.Name}.");
            }
        }

        [Command("hello")]
        public async Task Hello(CommandContext ctx)
        {
            await ctx.Channel.SendMessageAsync("Привет!");
        }


        [Command("random")]
        public async Task Random(CommandContext ctx, int min, int max)
        {
            var randomValue = new System.Random().Next(min, max);
            await ctx.Channel.SendMessageAsync(ctx.User.Username + " " + randomValue);
        }


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
