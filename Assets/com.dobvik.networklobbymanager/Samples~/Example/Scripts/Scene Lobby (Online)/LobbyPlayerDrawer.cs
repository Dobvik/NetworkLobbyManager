using TMPro;
using UnityEngine;

namespace NetworkLobbyManager.Example
{
    public class LobbyPlayerDrawer : MonoBehaviour
    {
        [SerializeField] private TMP_Text playerNameText;
    
        private BaseNetworkLobbyPlayer currentBaseNetworkLobbyPlayer;
    
        public void Initialize(BaseNetworkLobbyPlayer baseNetworkLobbyPlayer)
        {
            currentBaseNetworkLobbyPlayer = baseNetworkLobbyPlayer;
            baseNetworkLobbyPlayer.onPlayerSteamNameChanged.AddListener(OnPlayerNameChanged);
            OnPlayerNameChanged(currentBaseNetworkLobbyPlayer);
        }
    
        private void OnPlayerNameChanged(BaseNetworkLobbyPlayer player)
        {
            playerNameText.text = player.PlayerSteamName;
        }
    }
}
