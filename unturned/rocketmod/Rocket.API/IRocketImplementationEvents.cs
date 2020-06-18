using Rocket.API.Collections;
using System.Collections.Generic;

namespace Rocket.API
{
    public delegate void ImplementationShutdown();

    public interface IRocketImplementationEvents
    {
        event ImplementationShutdown OnShutdown;
    }
}
