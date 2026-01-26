using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace NetworkLobbyManager.Example
{
    public class PlayerLobbyListDrawer : MonoBehaviour
    {
        [SerializeField] private LayoutGroup drawersLayoutGroup;
        [SerializeField] private LobbyPlayerDrawer lobbyPlayerDrawerPrefab;

        private readonly Dictionary<NetworkLobbyPlayer, LobbyPlayerDrawer> playerDrawers = new ();

        public void AddPlayer(NetworkLobbyPlayer networkLobbyPlayer)
        {
            if (playerDrawers.ContainsKey(networkLobbyPlayer))
            {
                return;
            }
    
            var instance = Instantiate(lobbyPlayerDrawerPrefab, drawersLayoutGroup.transform);
            instance.Initialize(networkLobbyPlayer);
            playerDrawers[networkLobbyPlayer] = instance;
        }

        public void RemovePlayer(NetworkLobbyPlayer player)
        {
            Destroy(playerDrawers[player].gameObject);
            playerDrawers.Remove(player);
        }
    }
}

