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
        
        [SerializeField, ReadOnly] private int playerSteamAvatar;
        [SerializeField, ReadOnly] private string playerSteamName;

        public CSteamID SteamID => steamID;
        
        public string PlayerSteamName
        {
            get => playerSteamName;
            private set
            {
                if (playerSteamName != null && value == playerSteamName)
                {
                    return;
                }
                
                playerSteamName = value;
                onPlayerSteamNameChanged?.Invoke(this);
            }
        }

        public int PlayerSteamAvatar
        {
            get => playerSteamAvatar;
            private set
            {
                if (value == playerSteamAvatar)
                {
                    return;
                }
                
                playerSteamAvatar = value;
                onPlayerSteamAvatarChanged?.Invoke(this);
            }
        }

        #region SyncVars

        [Header("Debug")]
        [SyncVar(hook = nameof(SteamIDChanged))]
        [SerializeField, ReadOnly] private CSteamID steamID;

        private void SteamIDChanged(CSteamID _, CSteamID newSteamID)
        {
            PlayerSteamName = SteamFriends.GetFriendPersonaName(steamID);
            
            PlayerSteamAvatar = SteamFriends.GetLargeFriendAvatar(steamID);
            
            onSteamIDChanged?.Invoke(this);
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
            var indexLastConnectedPlayer = SteamMatchmaking.GetNumLobbyMembers(BaseSteamManager.Instance.CurrentLobbyID) - 1;
            steamID = SteamMatchmaking.GetLobbyMemberByIndex(BaseSteamManager.Instance.CurrentLobbyID, indexLastConnectedPlayer);
        }
        
        #endregion
        
        #region Callbacks
        
        private void OnPersonaStateChange(PersonaStateChange_t callback)
        {
            if (callback.m_ulSteamID == steamID.m_SteamID && callback.m_nChangeFlags == EPersonaChange.k_EPersonaChangeName)
            {
                PlayerSteamName = SteamFriends.GetFriendPersonaName(steamID);
            }
            else if (callback.m_ulSteamID == steamID.m_SteamID && callback.m_nChangeFlags == EPersonaChange.k_EPersonaChangeAvatar)
            {
                PlayerSteamAvatar = SteamFriends.GetLargeFriendAvatar(steamID);
            }
        }
        
        private void OnAvatarImageLoaded(AvatarImageLoaded_t callback)
        {
            if (callback.m_steamID.m_SteamID == steamID.m_SteamID)
            {
                PlayerSteamAvatar = callback.m_iImage;
            }
        }

        #endregion
    }
}
