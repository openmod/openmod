using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using OpenMod.API.Commands;

namespace OpenMod.Standalone
{
    public class ConsoleActor : ICommandActor
    {
        public string Id { get; } = "openmod-standalone-console";
        public string Type { get; } = "console";
        public Dictionary<string, object> Data { get; } = new Dictionary<string, object>();
        public string DisplayName { get; } = "Console";
        public Task PrintMessageAsync(Color color, string message)
        {
            // todo: add support for colors
            Console.WriteLine(message);
            return Task.CompletedTask;
        }
    }
}