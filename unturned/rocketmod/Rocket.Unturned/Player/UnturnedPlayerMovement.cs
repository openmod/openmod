using Rocket.Core;
using Rocket.Core.Logging;
using Rocket.Core.Utils;
using Rocket.Unturned.Player;
using SDG.Provider;
using SDG.Provider.Services.Achievements;
using SDG.Unturned;
using Steamworks;
using System;
using System.Collections.Generic;
using UnityEngine;
using Math = System.Math;

namespace Rocket.Unturned
{
    public class UnturnedPlayerMovement : UnturnedPlayerComponent
    {
        public bool VanishMode = false;
        DateTime lastUpdate = DateTime.Now;
        Vector3 lastVector = new Vector3(0,-1,0);

        private void FixedUpdate()
        {
            PlayerMovement movement = (PlayerMovement)Player.GetComponent<PlayerMovement>();

            if (!VanishMode)
            {
                if (U.Settings.Instance.LogSuspiciousPlayerMovement && lastUpdate.AddSeconds(1) < DateTime.Now)
                {
                    lastUpdate = DateTime.Now;

                    Vector3 positon = movement.real;

                    if (lastVector.y != -1)
                    {
                        float x = Math.Abs(lastVector.x - positon.x);
                        float y = positon.y - lastVector.y;
                        float z = Math.Abs(lastVector.z - positon.z);
                        if (y > 15)
                        {
                            RaycastHit raycastHit = new RaycastHit();
                            Physics.Raycast(positon, Vector3.down, out raycastHit);
                            Vector3 floor = raycastHit.point;
                            float distance = Math.Abs(floor.y - positon.y);
                            Core.Logging.Logger.Log(Player.DisplayName + " moved x:" + positon.x + " y:" + positon.y + "(+" + y + ") z:" + positon.z + " in the last second (" + distance + ")");
                        }
                    }
                    lastVector = movement.real;
                }
            }
        }
    }
}
