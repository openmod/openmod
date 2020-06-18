using System;
using System.Collections.Generic;

namespace Rocket.API
{
    public class RocketPlayer : IRocketPlayer
    {
        private string id;
        public string Id { get { return id; } }

        private string displayName;
        public string DisplayName { get { return displayName; } }

        private bool isAdmin;
        public bool IsAdmin { get { return isAdmin; } }

        public RocketPlayer(string Id, string DisplayName = null, bool IsAdmin = false)
        {
            id = Id;
            if (DisplayName == null)
            {
                DisplayName = Id;
            }
            else
            {
                displayName = DisplayName;
            }
            isAdmin = IsAdmin;
        }

        public int CompareTo(object obj)
        {
            return Id.CompareTo(((IRocketPlayer)obj).Id);
        }
    }
}
