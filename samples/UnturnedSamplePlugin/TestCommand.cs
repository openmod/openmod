using System.Threading.Tasks;
using JetBrains.Annotations;
using OpenMod.API.Commands;
using OpenMod.Core.Commands;

namespace UnturnedSamplePlugin
{
    public static class NestedCommands
    {
        [Command("Test")]
        [CommandSummary("Does something useful")]
        [CommandSyntax("<int>")]
        public static async Task TestRoot(ICommandContext context)
        {
            await context.Actor.PrintMessageAsync($"{context.CommandPrefix}{context.CommandAlias}");
        }

        [Command("Sub1")]
        [CommandParent(typeof(NestedCommands), nameof(TestRoot))]
        [CommandSummary("Sub command on first level")]
        [CommandSyntax("<string>")]
        public static async Task TestSub1(ICommandContext context)
        {
            await context.Actor.PrintMessageAsync($"{context.CommandPrefix}{context.CommandAlias}");
        }

        [Command("Sub2")]
        [CommandParent(typeof(NestedCommands), nameof(TestRoot))]
        [CommandSummary("Second sub command on first level")]
        [CommandSyntax("<player> <count>")]
        public static async Task TestSub2(ICommandContext context)
        {
            await context.Actor.PrintMessageAsync($"{context.CommandPrefix}{context.CommandAlias}");
        }

        [Command("Sub1")]
        [CommandParent(typeof(NestedCommands), nameof(TestSub1))]
        [CommandSummary("First sub command on second level")]
        [CommandSyntax("<source> <target>")]
        public static async Task TestSub1Sub1(ICommandContext context)
        {
            await context.Actor.PrintMessageAsync($"{context.CommandPrefix}{context.CommandAlias}");
        }

        [Command("Sub2")]
        [CommandParent(typeof(NestedCommands), nameof(TestSub1))]
        [CommandSummary("Second sub command on second level")]
        [CommandSyntax("<amount>")]
        public static async Task TestSub1Sub2(ICommandContext context)
        {
            await context.Actor.PrintMessageAsync($"{context.CommandPrefix}{context.CommandAlias}");
        }


        [Command("Sub3")]
        [CommandParent(typeof(NestedCommands), nameof(TestSub1))]
        [CommandSummary("Third sub command on second level")]
        [CommandSyntax("<bool>")]
        public static async Task TestSub1Sub3(ICommandContext context)
        {
            await context.Actor.PrintMessageAsync($"{context.CommandPrefix}{context.CommandAlias}");
        }

        [Command("Sub1")]
        [CommandParent(typeof(NestedCommands), nameof(TestSub2))]
        [CommandSummary("Another nested sub command")]
        [CommandSyntax("<task> <player> <message>")]
        public static async Task TestSub2Sub1(ICommandContext context)
        {
            await context.Actor.PrintMessageAsync($"{context.CommandPrefix}{context.CommandAlias}");
        }

        [Command("Sub2")]
        [CommandParent(typeof(NestedCommands), nameof(TestSub2))]
        [CommandSummary("I like trees.")]
        [CommandSyntax("<intent>")]
        public static async Task TestSub2Sub2(ICommandContext context)
        {
            await context.Actor.PrintMessageAsync($"{context.CommandPrefix}{context.CommandAlias}");
        }
    }
}