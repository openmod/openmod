using System;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenMod.API.Plugins;
using OpenMod.API.Prioritization;

namespace OpenMod.Core.Commands.OpenModCommands
{
    [Command("plugins", Priority = Priority.Highest)]
    [CommandDescription("Lists loaded plugins")]
    [CommandSyntax("[page]")]
    [CommandParent(typeof(CommandOpenMod))]
    public class CommandOpenModPlugins : Command
    {
        private readonly IPluginActivator m_PluginActivator;

        public CommandOpenModPlugins(IPluginActivator pluginActivator, IServiceProvider serviceProvider) : base(serviceProvider)
        {
            m_PluginActivator = pluginActivator;
        }

        protected override async Task OnExecuteAsync()
        {
            var pageNumber = Context.Parameters.Length > 0 ? await Context.Parameters.GetAsync<int>(0) : 1;
            const int itemsPerPage = 10;

            var totalCount = m_PluginActivator.ActivatedPlugins.Count;
            var pageCount = (int) Math.Ceiling((double) totalCount / itemsPerPage);
            var plugins = m_PluginActivator.ActivatedPlugins
                .Skip((pageNumber - 1) * itemsPerPage)
                .Take(itemsPerPage)
                .ToList();

            await PrintAsync($"[{pageNumber}/{pageCount}] OpenMod Plugins", Color.CornflowerBlue);
          
            if (plugins.Count == 0)
            {
                await PrintAsync("No plugins found.", Color.Red);
            }

            foreach (var plugin in plugins)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append($"{plugin.DisplayName} v{plugin.Version}");

                if (!string.IsNullOrEmpty(plugin.Author))
                {
                    sb.Append($" by {plugin.Author}");
                }

                await PrintAsync(sb.ToString(), Color.Green);
            }
        }
    }
}