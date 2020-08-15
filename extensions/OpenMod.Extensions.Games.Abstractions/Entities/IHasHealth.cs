using System.Threading.Tasks;

namespace OpenMod.Extensions.Games.Abstractions.Entities
{
    public interface IHasHealth
    {
        bool IsAlive { get; }

        double MaxHealth { get; }

        double Health { get; }

        Task SetHealthAsync(double health);

        Task KillAsync();
    }
}