using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using System.Threading.Tasks;

namespace DsBot.Test2.commands.slach
{
    public class BasicSlashCommands : ApplicationCommandModule
    {
        [SlashCommand("test", "Тестовая слеш-команда")]
        public async Task TestSlachCommand(InteractionContext ctx)
        {
            await ctx.DeferAsync();

            var embedMessage = new DiscordEmbedBuilder()
            {
                Title = "Тестовая слеш-команда",
                Description = "Проверка данной команды",
                Color = DiscordColor.Azure
            };

            await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(embedMessage));
        }

        [SlashCommand("parametr", "Тестовая слеш-команда с параметрами")]
        public async Task ParametrSlachCommand(InteractionContext ctx, [Option(name: "test", description: "test parametr")] string testParametr, )
        {
            await ctx.DeferAsync();

            var embedMessage = new DiscordEmbedBuilder()
            {
                Title = "Тестовая слеш-команда с параметром",
                Description = $"Это тестовая слеш-команда с параметром",
                Color = DiscordColor.Azure
            };

            await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(embedMessage));
        }
    }
} 
