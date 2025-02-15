using DsBot.Test2.config;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using System.Threading.Tasks;

namespace DsBot.Test2
{
    internal class Program
    {

        public static DiscordClient Client { get; set; }
        public static CommandsNextExtension Commands { get; set; }

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
            Client.Ready += Client_Ready;

            await Client.ConnectAsync();
            await Task.Delay(-1);
        }

        private static Task Client_Ready(DiscordClient sender, DSharpPlus.EventArgs.ReadyEventArgs args)
        {
            return Task.CompletedTask;
        }
    }
}
