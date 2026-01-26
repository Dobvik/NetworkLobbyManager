using TMPro;
using UnityEngine;

namespace NetworkLobbyManager.Example
{
    public class LobbyPlayerDrawer : MonoBehaviour
    {
        [SerializeField] private TMP_Text playerNameText;
    
        private NetworkLobbyPlayer currentNetworkLobbyPlayer;
    
        public void Initialize(NetworkLobbyPlayer networkLobbyPlayer)
        {
            currentNetworkLobbyPlayer = networkLobbyPlayer;
            networkLobbyPlayer.onPlayerSteamNameChanged.AddListener(OnPlayerNameChanged);
            OnPlayerNameChanged(currentNetworkLobbyPlayer);
        }
    
        private void OnPlayerNameChanged(NetworkLobbyPlayer player)
        {
            playerNameText.text = player.PlayerSteamName;
        }
    }
}
