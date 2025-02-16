using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using Google.Apis.YouTube.v3.Data;
using System.Threading.Tasks;

namespace DsBot.Test2.commands.slash
{

    [SlashCommandGroup(name: "калькулятор", description: "комманды для калькулятора")]
    public class Calculator : ApplicationCommandModule
    {
        [SlashCommand(name: "сумма", description: "сумирование дву чисел")]
        public async Task Add(InteractionContext ctx,
            [Option(name: "первое_число", description: ".")] double number1,
            [Option(name: "второе_число", description: ".")] double number2)
        {
            await ctx.DeferAsync();

            var embedMessage = new DiscordEmbedBuilder()
            {
                Title = "Сумма двух чисел",
                Description = $"Результат суммирования: {number1 + number2}",
                Color = DiscordColor.Azure
            };

            await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(embedMessage));
        }


        [SlashCommand(name: "вычитание", description: "вычитание двух чисел")]
        public async Task Subtract(InteractionContext ctx,
            [Option(name: "первое_число", description: ".")] double number1,
            [Option(name: "второе_число", description: ".")] double number2)
        {
            await ctx.DeferAsync();

            var embedMessage = new DiscordEmbedBuilder()
            {
                Title = "Вычитание двух чисел",
                Description = $"Результат вычетания: {number1 - number2}",
                Color = DiscordColor.Azure
            };

            await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(embedMessage));
        }
    }
}
