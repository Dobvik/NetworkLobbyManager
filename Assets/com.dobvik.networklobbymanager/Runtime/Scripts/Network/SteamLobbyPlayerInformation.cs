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
        private Callback<AvatarImageLoaded_t> avatarImageLoaded;

        [Header("Events")]
        [HideInInspector] public UnityEvent<SteamLobbyPlayerInformation> onSteamIDChanged;
        [HideInInspector] public UnityEvent<SteamLobbyPlayerInformation> onPlayerSteamNameChanged;
        [HideInInspector] public UnityEvent<SteamLobbyPlayerInformation> onPlayerSteamAvatarChanged;

        public CSteamID SteamID => steamID;
        public string PlayerSteamName => playerSteamName;
        public int PlayerSteamAvatar => playerSteamAvatar;

        #region SyncVars

        [Header("Debug")]
        [SyncVar(hook = nameof(SteamIDChanged))]
        [SerializeField, ReadOnly] private CSteamID steamID;

        [SyncVar(hook = nameof(PlayerSteamNameChanged))]
        [SerializeField, ReadOnly] private string playerSteamName;
        
        [SyncVar(hook = nameof(PlayerSteamAvatarChanged))]
        [SerializeField, ReadOnly] private int playerSteamAvatar;

        private void SteamIDChanged(CSteamID _, CSteamID newSteamID)
        {
            onSteamIDChanged?.Invoke(this);
        }

        private void PlayerSteamNameChanged(string _, string newPlayerSteamName)
        {
            onPlayerSteamNameChanged?.Invoke(this);
        }
        
        private void PlayerSteamAvatarChanged(int _, int newPlayerSteamAvatar)
        {
            onPlayerSteamAvatarChanged?.Invoke(this);
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
            avatarImageLoaded = Callback<AvatarImageLoaded_t>.Create(OnAvatarImageLoaded);
        }

        public void OnDisable()
        {
            personaStateChange?.Dispose();
            avatarImageLoaded?.Dispose();
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
            else if (callback.m_ulSteamID == steamID.m_SteamID && callback.m_nChangeFlags == EPersonaChange.k_EPersonaChangeAvatar)
            {
                playerSteamAvatar = SteamFriends.GetLargeFriendAvatar(steamID);
            }
        }
        
        [ServerCallback]
        private void OnAvatarImageLoaded(AvatarImageLoaded_t callback)
        {
            if (callback.m_steamID.m_SteamID == steamID.m_SteamID)
            {
                playerSteamAvatar = callback.m_iImage;
            }
        }

        #endregion
        
        #region Events

        [Server]
        private void OnSteamIDChanged()
        {
            playerSteamName = SteamFriends.GetFriendPersonaName(steamID);
            playerSteamAvatar = SteamFriends.GetLargeFriendAvatar(steamID);
        }

        #endregion
    }
}
