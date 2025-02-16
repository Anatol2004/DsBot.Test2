using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using System.Threading.Tasks;

namespace DsBot.Test2.commands.slach
{
    public class BasicSlashCommands : ApplicationCommandModule
    {
        [SlashCommand("TEST", "Тестовая слеш-команда")]
        public async Task TestSlachCommand(InteractionContext ctx)
        {
            await ctx.DeferAsync();

            var embedMessage = new DiscordEmbedBuilder()
            {
                Title = "Тестовая слеш-команда",
                Description = "Проверка данной команды, ёпта",
                Color = DiscordColor.Azure
            };

            await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(embedMessage));
        }


        [SlashCommand("parametr", "Тестовая слеш-команда с параметрами")]
        public async Task ParametrSlachCommand(InteractionContext ctx, 
            [Option(name: "Тест-стринг", description: "test-parametr")] string testParametr1,
            [Option(name: "Тест-лонг", description: "test-Long-parametr")] long testParametr2)
        {
            await ctx.DeferAsync();

            var embedMessage = new DiscordEmbedBuilder()
            {
                Title = "Тестовая слеш-команда с параметром",
                Description = $"Это тестовая слеш-команда с параметром:{testParametr1}, и long-параметром: {testParametr2}",
                Color = DiscordColor.Azure
            };

            await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(embedMessage));
        }


        [SlashCommand("userDetail", "Слеш-команда с данными пользователя")]
        public async Task TestUserCommand(InteractionContext ctx,
            [Option(name: "пользователь", description: "получить информацию о выбранном пользователе")] DiscordUser user)
        {
            await ctx.DeferAsync();

            var member = (DiscordMember)user; 

            var embedMessage = new DiscordEmbedBuilder()
            {
                Title = "Тестовая слеш-команда с данными пользователя",
                Description = $"Пользователь {member.Nickname}, ID: {user.Id}",
                Color = DiscordColor.Azure
            };

            await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(embedMessage));
        }

        [SlashCommand(name: "кнопка", description: "обычная слеш-команда с кнопкой")]
        public async Task ButtonCommand(InteractionContext ctx)
        {
            await ctx.DeferAsync();

            var button1 = new DiscordButtonComponent(ButtonStyle.Primary, "testButton1", "Test Button 1");
            var button2 = new DiscordButtonComponent(ButtonStyle.Danger, "testButton2", "Test Button 2");

            var embedMessage = new DiscordEmbedBuilder()
            {
                Title = "Button Command",
                Color = DiscordColor.Blue
            };

            await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(embedMessage).AddComponents(button1, button2));
        }
    }
} 
