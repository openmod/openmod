using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rocket.Core.Steam
{
    public static class Steam
    {
        public static bool IsValidCSteamID(string CSteamID)
        {
            ulong id = 0;
            if (ulong.TryParse(CSteamID, out id) && id > 76561197960265728)
            {
                return true;
            }
            return false;
        }
    }
}
