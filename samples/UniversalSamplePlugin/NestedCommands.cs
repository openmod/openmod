using System.Threading.Tasks;
using OpenMod.API.Commands;
using OpenMod.Core.Commands;

namespace UniversalSamplePlugin
{
    public static class NestedCommands
    {
        [Command("Test")]
        [CommandDescription("Does something very useful")]
        [CommandSyntax("<int>")]
        public static Task TestRootAsync(ICommandContext context)
        {
            return context.Actor.PrintMessageAsync($"{context.CommandPrefix}{context.CommandAlias}");
        }

        [Command("Sub1")]
        [CommandParent(typeof(NestedCommands), nameof(TestRootAsync))]
        [CommandDescription("Sub command on first level")]
        [CommandSyntax("<string>")]
        public static Task TestSub1Async(ICommandContext context)
        {
            return context.Actor.PrintMessageAsync($"{context.CommandPrefix}{context.CommandAlias}");
        }

        [Command("Sub2")]
        [CommandParent(typeof(NestedCommands), nameof(TestRootAsync))]
        [CommandDescription("Second sub command on first level")]
        [CommandSyntax("<player> <count>")]
        public static Task TestSub2Async(ICommandContext context)
        {
            return context.Actor.PrintMessageAsync($"{context.CommandPrefix}{context.CommandAlias}");
        }

        [Command("Sub1")]
        [CommandParent(typeof(NestedCommands), nameof(TestSub1Async))]
        [CommandDescription("First sub command on second level")]
        [CommandSyntax("<source> <target>")]
        public static Task TestSub1Sub1Async(ICommandContext context)
        {
            return context.Actor.PrintMessageAsync($"{context.CommandPrefix}{context.CommandAlias}");
        }

        [Command("Sub2")]
        [CommandParent(typeof(NestedCommands), nameof(TestSub1Async))]
        [CommandDescription("Second sub command on second level")]
        [CommandSyntax("<amount>")]
        public static Task TestSub1Sub2Async(ICommandContext context)
        {
            return context.Actor.PrintMessageAsync($"{context.CommandPrefix}{context.CommandAlias}");
        }


        [Command("Sub3")]
        [CommandParent(typeof(NestedCommands), nameof(TestSub1Async))]
        [CommandDescription("Third sub command on second level")]
        [CommandSyntax("<bool>")]
        public static Task TestSub1Sub3Async(ICommandContext context)
        {
            return context.Actor.PrintMessageAsync($"{context.CommandPrefix}{context.CommandAlias}");
        }

        [Command("Sub1")]
        [CommandParent(typeof(NestedCommands), nameof(TestSub2Async))]
        [CommandDescription("Another nested sub command")]
        [CommandSyntax("<task> <player> <message>")]
        public static Task TestSub2Sub1Async(ICommandContext context)
        {
            return context.Actor.PrintMessageAsync($"{context.CommandPrefix}{context.CommandAlias}");
        }

        [Command("Sub2")]
        [CommandParent(typeof(NestedCommands), nameof(TestSub2Async))]
        [CommandDescription("I like trees.")]
        [CommandSyntax("<intent>")]
        public static Task TestSub2Sub2Async(ICommandContext context)
        {
            return context.Actor.PrintMessageAsync($"{context.CommandPrefix}{context.CommandAlias}");
        }
    }
}