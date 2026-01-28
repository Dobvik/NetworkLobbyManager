using System.Collections;
using Mirror;
using Steamworks;
using UnityEngine;

namespace NetworkLobbyManager
{
    public class BaseSteamManager : SteamManager
    {
#if !DISABLESTEAMWORKS

        private static BaseSteamManager instance;
        public new static BaseSteamManager Instance => instance ?? new GameObject("Steam Manager").AddComponent<BaseSteamManager>();

        public new static bool Initialized => Instance.m_bInitialized;

#endif

        [Header("Debug")]
        [SerializeField, ReadOnly] private CSteamID currentLobbyID;

        public CSteamID CurrentLobbyID
        {
            get => currentLobbyID;
            set
            {
                if (currentLobbyID == value)
                {
                    return;
                }
            
                SteamMatchmaking.LeaveLobby(currentLobbyID);
                currentLobbyID = value;
        
                if (value.m_SteamID != 0 && NetworkServer.active)
                {
                    SteamMatchmaking.SetLobbyData(value, ServerSteamInformation.HOST_ADDRESS_KEY, SteamUser.GetSteamID().ToString());
                    SteamMatchmaking.SetLobbyData(value, ServerSteamInformation.NAME_LOBBY_KEY, $"Room {SteamFriends.GetPersonaName()}");
                    SteamMatchmaking.SetLobbyData(value, ServerSteamInformation.NUMBER_CURRENT_PLAYERS_KEY, $"{NetworkServer.connections.Count}");
                    SteamMatchmaking.SetLobbyData(value, ServerSteamInformation.MAXIMUM_NUMBER_PLAYERS_KEY, $"{NetworkServer.maxConnections}");
                    SteamMatchmaking.SetLobbyData(value, ServerSteamInformation.KEY_WORD_GAME_KEY, ServerSteamInformation.KEY_WORD_GAME_KEY); // If you use appid = 480, then this is needed for lobby filtering
                    StartCoroutine(UpdatePingLocation(value));
                    SubscribeEvents();
                }
            }
        }

        protected override void Awake()
        {
            if (instance != null)
            {
                Destroy(gameObject);
                return;
            }
    
            instance = this;
            base.Awake();
        }

        private IEnumerator UpdatePingLocation(CSteamID lobbyID)
        {
            while (SteamNetworkingUtils.GetLocalPingLocation(out _) < 0)
            {
                yield return null;
            }
    
            SteamNetworkingUtils.GetLocalPingLocation(out var steamLocation);
            SteamNetworkingUtils.ConvertPingLocationToString(ref steamLocation, out var steamLocationString, Constants.k_cchMaxSteamNetworkingPingLocationString);
            SteamMatchmaking.SetLobbyData(lobbyID, ServerSteamInformation.STEAM_LOCATION_KEY, steamLocationString);
        }

        #region Events

        private void SubscribeEvents()
        {
            if (!NetworkServer.active)
            {
                return;
            }
    
            NetworkServer.OnConnectedEvent += _ => OnLobbyDataChanged();
            NetworkServer.OnDisconnectedEvent += _ => OnLobbyDataChanged();
        }

        private void OnLobbyDataChanged()
        {
            if (!NetworkServer.active)
            {
                return;
            }
    
            SteamMatchmaking.SetLobbyData(Instance.CurrentLobbyID, ServerSteamInformation.NAME_LOBBY_KEY, $"Room {SteamFriends.GetPersonaName()}");
            SteamMatchmaking.SetLobbyData(Instance.CurrentLobbyID, ServerSteamInformation.NUMBER_CURRENT_PLAYERS_KEY, $"{NetworkServer.connections.Count}");
            SteamMatchmaking.SetLobbyData(Instance.CurrentLobbyID, ServerSteamInformation.MAXIMUM_NUMBER_PLAYERS_KEY, $"{NetworkServer.maxConnections}");
        }

        #endregion
    }
}
