using System.Threading.Tasks;
using OpenMod.API.Eventing;

namespace OpenMod.Core.Users.Events
{
    public interface IUserConnectingEvent : IUserEvent, ICancellableEvent
    {
        public string RejectionReason { get; }
        
        Task RejectAsync(string reason);
    }
}