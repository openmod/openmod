using SDG.Unturned;
using UnityEngine;

namespace Rocket.Unturned.Player
{
    public class UnturnedPlayerComponent : MonoBehaviour
    {
        private UnturnedPlayer player;
        public UnturnedPlayer Player
        {
            get { return player; }
        }
        private void Awake()
        {
            player = UnturnedPlayer.FromPlayer(gameObject.transform.GetComponent<SDG.Unturned.Player>());
        }

        private void OnEnable()
        {
            Load();
        }

        private void OnDisable()
        {
            Unload();
        }

        protected virtual void Load()
        {

        }


        protected virtual void Unload()
        {

        }

    }
}