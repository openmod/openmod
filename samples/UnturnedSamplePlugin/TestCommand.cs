using System.Threading.Tasks;
using OpenMod.API.Commands;
using OpenMod.Core.Commands;

namespace UnturnedSamplePlugin
{
    public static class NestedCommands
    {
        [Command("Test")]
        [CommandDescription("Does something very useful")]
        [CommandSyntax("<int>")]
        public static async Task TestRoot(ICommandContext context)
        {
            await context.Actor.PrintMessageAsync($"{context.CommandPrefix}{context.CommandAlias}");
        }

        [Command("Sub1")]
        [CommandParent(typeof(NestedCommands), nameof(TestRoot))]
        [CommandDescription("Sub command on first level")]
        [CommandSyntax("<string>")]
        public static async Task TestSub1(ICommandContext context)
        {
            await context.Actor.PrintMessageAsync($"{context.CommandPrefix}{context.CommandAlias}");
        }

        [Command("Sub2")]
        [CommandParent(typeof(NestedCommands), nameof(TestRoot))]
        [CommandDescription("Second sub command on first level")]
        [CommandSyntax("<player> <count>")]
        public static async Task TestSub2(ICommandContext context)
        {
            await context.Actor.PrintMessageAsync($"{context.CommandPrefix}{context.CommandAlias}");
        }

        [Command("Sub1")]
        [CommandParent(typeof(NestedCommands), nameof(TestSub1))]
        [CommandDescription("First sub command on second level")]
        [CommandSyntax("<source> <target>")]
        public static async Task TestSub1Sub1(ICommandContext context)
        {
            await context.Actor.PrintMessageAsync($"{context.CommandPrefix}{context.CommandAlias}");
        }

        [Command("Sub2")]
        [CommandParent(typeof(NestedCommands), nameof(TestSub1))]
        [CommandDescription("Second sub command on second level")]
        [CommandSyntax("<amount>")]
        public static async Task TestSub1Sub2(ICommandContext context)
        {
            await context.Actor.PrintMessageAsync($"{context.CommandPrefix}{context.CommandAlias}");
        }


        [Command("Sub3")]
        [CommandParent(typeof(NestedCommands), nameof(TestSub1))]
        [CommandDescription("Third sub command on second level")]
        [CommandSyntax("<bool>")]
        public static async Task TestSub1Sub3(ICommandContext context)
        {
            await context.Actor.PrintMessageAsync($"{context.CommandPrefix}{context.CommandAlias}");
        }

        [Command("Sub1")]
        [CommandParent(typeof(NestedCommands), nameof(TestSub2))]
        [CommandDescription("Another nested sub command")]
        [CommandSyntax("<task> <player> <message>")]
        public static async Task TestSub2Sub1(ICommandContext context)
        {
            await context.Actor.PrintMessageAsync($"{context.CommandPrefix}{context.CommandAlias}");
        }

        [Command("Sub2")]
        [CommandParent(typeof(NestedCommands), nameof(TestSub2))]
        [CommandDescription("I like trees.")]
        [CommandSyntax("<intent>")]
        public static async Task TestSub2Sub2(ICommandContext context)
        {
            await context.Actor.PrintMessageAsync($"{context.CommandPrefix}{context.CommandAlias}");
        }
    }
}