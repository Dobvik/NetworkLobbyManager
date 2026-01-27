using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace NetworkLobbyManager.Example
{
    public class PlayerLobbyListDrawer : MonoBehaviour
    {
        [SerializeField] private LayoutGroup drawersLayoutGroup;
        [SerializeField] private LobbyPlayerDrawer lobbyPlayerDrawerPrefab;

        private readonly Dictionary<BaseNetworkLobbyPlayer, LobbyPlayerDrawer> playerDrawers = new ();

        public void AddPlayer(BaseNetworkLobbyPlayer baseNetworkLobbyPlayer)
        {
            if (playerDrawers.ContainsKey(baseNetworkLobbyPlayer))
            {
                return;
            }
    
            var instance = Instantiate(lobbyPlayerDrawerPrefab, drawersLayoutGroup.transform);
            instance.Initialize(baseNetworkLobbyPlayer);
            playerDrawers[baseNetworkLobbyPlayer] = instance;
        }

        public void RemovePlayer(BaseNetworkLobbyPlayer player)
        {
            Destroy(playerDrawers[player].gameObject);
            playerDrawers.Remove(player);
        }
    }
}

