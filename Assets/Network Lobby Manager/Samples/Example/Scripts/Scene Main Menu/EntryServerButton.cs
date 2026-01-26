using System.Collections;
using Steamworks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace NetworkLobbyManager.Example
{
    public class EntryServerButton : MonoBehaviour
    {
        [SerializeField] private TMP_Text roomNameText;
        [SerializeField] private TMP_Text numberPlayersText;
        [SerializeField] private TMP_Text pingText;
        [SerializeField] private Button buttonEntry;
    
        public void Initialize(CSteamID steamLobbyID)
        {
            buttonEntry.onClick.AddListener(() => { SteamMatchmaking.JoinLobby(steamLobbyID); });
            roomNameText.text = SteamMatchmaking.GetLobbyData(steamLobbyID, ServerSteamInformation.NAME_LOBBY_KEY);
            numberPlayersText.text = $"{SteamMatchmaking.GetLobbyData(steamLobbyID, ServerSteamInformation.NUMBER_CURRENT_PLAYERS_KEY)}/{SteamMatchmaking.GetLobbyData(steamLobbyID, ServerSteamInformation.MAXIMUM_NUMBER_PLAYERS_KEY)}";
        
            StartCoroutine(UpdatePingText(steamLobbyID));
        }
    
        private IEnumerator UpdatePingText(CSteamID steamLobbyID)
        {
            SteamNetworkingUtils.ParsePingLocationString(SteamMatchmaking.GetLobbyData(steamLobbyID, ServerSteamInformation.STEAM_LOCATION_KEY), out var steamLocation);
            var ping = SteamNetworkingUtils.EstimatePingTimeFromLocalHost(ref steamLocation);
            pingText.text = "Updating";
        
            while (ping < 0)
            {
                ping = SteamNetworkingUtils.EstimatePingTimeFromLocalHost(ref steamLocation);
                yield return null;
            }
        
            pingText.text = $"{ping}";
        }
    }
}
