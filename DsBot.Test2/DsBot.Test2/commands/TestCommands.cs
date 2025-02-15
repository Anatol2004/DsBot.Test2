using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using System.Threading.Tasks;

namespace DsBot.Test2.commands
{
    public class TestCommands : BaseCommandModule
    {
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
    }
}
