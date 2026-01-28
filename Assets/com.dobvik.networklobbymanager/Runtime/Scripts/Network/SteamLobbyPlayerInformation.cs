using Mirror;
using Steamworks;
using UnityEngine;
using UnityEngine.Events;

namespace NetworkLobbyManager
{
    public class SteamLobbyPlayerInformation : NetworkBehaviour
    {
        [Header("Callbacks")]
        private Callback<PersonaStateChange_t> personaStateChange;

        [Header("Events")]
        [HideInInspector] public UnityEvent<SteamLobbyPlayerInformation> onSteamIDChanged;
        [HideInInspector] public UnityEvent<SteamLobbyPlayerInformation> onPlayerSteamNameChanged;

        public CSteamID SteamID => steamID;
        public string PlayerSteamName => playerSteamName;

        #region SyncVars

        [Header("Debug")]
        [SyncVar(hook = nameof(SteamIDChanged))]
        [SerializeField, ReadOnly] private CSteamID steamID;

        [SyncVar(hook = nameof(PlayerSteamNameChanged))]
        [SerializeField, ReadOnly] private string playerSteamName;

        private void SteamIDChanged(CSteamID _, CSteamID newSteamID)
        {
            onSteamIDChanged?.Invoke(this);
        }

        private void PlayerSteamNameChanged(string _, string newPlayerSteamName)
        {
            onPlayerSteamNameChanged?.Invoke(this);
        }

        #endregion
        
        #region MonoBehavior Functions

        private void OnEnable()
        {
            if (!BaseSteamManager.Initialized)
            {
                return;
            }
            
            personaStateChange = Callback<PersonaStateChange_t>.Create(OnPersonaStateChange);
        }

        public void OnDisable()
        {
            personaStateChange?.Dispose();
        }

        #endregion
        
        #region Server
        
        public override void OnStartServer()
        {
            onSteamIDChanged.AddListener(_ => OnSteamIDChanged());
            
            var indexLastConnectedPlayer = SteamMatchmaking.GetNumLobbyMembers(BaseSteamManager.Instance.CurrentLobbyID) - 1;
            steamID = SteamMatchmaking.GetLobbyMemberByIndex(BaseSteamManager.Instance.CurrentLobbyID, indexLastConnectedPlayer);
        }
        
        #endregion
        
        #region Callbacks

        [ServerCallback]
        private void OnPersonaStateChange(PersonaStateChange_t callback)
        {
            if (callback.m_ulSteamID == steamID.m_SteamID && callback.m_nChangeFlags == EPersonaChange.k_EPersonaChangeName)
            {
                playerSteamName = SteamFriends.GetFriendPersonaName(steamID);
            }
        }

        #endregion
        
        #region Events

        [Server]
        private void OnSteamIDChanged()
        {
            playerSteamName = SteamFriends.GetFriendPersonaName(steamID);
        }

        #endregion
    }
}
