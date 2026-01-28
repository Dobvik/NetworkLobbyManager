using TMPro;
using UnityEngine;

namespace NetworkLobbyManager.Example
{
    public class LobbyPlayerDrawer : MonoBehaviour
    {
        [SerializeField] private TMP_Text playerNameText;
    
        private SteamLobbyPlayerInformation currentSteamLobbyPlayerInformation;
    
        public void Initialize(BaseNetworkLobbyPlayer baseNetworkLobbyPlayer)
        {
            currentSteamLobbyPlayerInformation = baseNetworkLobbyPlayer.SteamLobbyPlayerInformation;
            currentSteamLobbyPlayerInformation.onPlayerSteamNameChanged.AddListener(OnPlayerNameChanged);
            OnPlayerNameChanged(currentSteamLobbyPlayerInformation);
        }
    
        private void OnPlayerNameChanged(SteamLobbyPlayerInformation player)
        {
            playerNameText.text = player.PlayerSteamName;
        }
    }
}
