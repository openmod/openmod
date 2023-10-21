using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NuGet.Protocol.Plugins;
using OpenMod.API.Plugins;
using OpenMod.API.Prioritization;

namespace OpenMod.Core.Commands.OpenModCommands
{
    [Command("plugins", Priority = Priority.Lowest)]
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
            const int itemsPerPage = 10;
            var pageNumber = await GetPageNumberAsync();
            var totalCount = m_PluginActivator.ActivatedPlugins.Count;
            var pageCount = CalculatePageCount(totalCount, itemsPerPage);

            var plugins = GetPluginsForPage(pageNumber, itemsPerPage);

            await DisplayPluginsInfoAsync(pageNumber, pageCount, plugins);
        }

        private async Task<int> GetPageNumberAsync()
        {
            return Context.Parameters.Length > 0
                ? await Context.Parameters.GetAsync<int>(0)
                : 1;
        }
        private int CalculatePageCount(int total, int perPage)
        {
            return (int)Math.Ceiling((double)total / perPage);
        }
        private List<IOpenModPlugin> GetPluginsForPage(int pageNumber, int itemsPerPage)
        {
            return m_PluginActivator.ActivatedPlugins
                .Skip((pageNumber - 1) * itemsPerPage)
                .Take(itemsPerPage)
                .ToList();
        }
        private async Task DisplayPluginsInfoAsync(int pageNumber, int pageCount, List<IOpenModPlugin> plugins)
        {
            await PrintAsync($"[{pageNumber}/{pageCount}] OpenMod Plugins", Color.CornflowerBlue);

            if (plugins.Count == 0)
            {
                await PrintAsync("No plugins found.", Color.Red);
            }
            else
            {
                foreach (var plugin in plugins)
                {
                    await PrintPluginInfoAsync(plugin);
                }
            }
        }
        private Task PrintPluginInfoAsync(IOpenModPlugin plugin)
        {
            var sb = new StringBuilder();
            sb.Append($"{plugin.DisplayName} v{plugin.Version}");

            if (!string.IsNullOrEmpty(plugin.Author))
            {
                sb.Append($" by {plugin.Author}");
            }

            return PrintAsync(sb.ToString(), Color.Green);
        }
    }
}