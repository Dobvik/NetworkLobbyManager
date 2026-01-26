using Mirror;
//using NetworkLobbyManager.Example; need for example
using Steamworks;
using UnityEngine;
using UnityEngine.Events;

namespace NetworkLobbyManager
{
    public class NetworkLobbyPlayer : NetworkBehaviour
    {
        public CSteamID SteamID => steamID;
        public string PlayerSteamName => playerSteamName;

        [Header("Callbacks")]
        private Callback<PersonaStateChange_t> personaStateChange;

        [Header("Events")]
        [HideInInspector] public UnityEvent<NetworkLobbyPlayer> onSteamIDChanged;
        [HideInInspector] public UnityEvent<NetworkLobbyPlayer> onPlayerSteamNameChanged;

        #region SyncVars

        [Header("Debug")]

        [SyncVar(hook = nameof(SteamIDChanged))]
        [SerializeField, ReadOnly] private CSteamID steamID;

        [SyncVar(hook = nameof(PlayerNameChanged))]
        [SerializeField, ReadOnly] private string playerSteamName;

        private void SteamIDChanged(CSteamID _, CSteamID newSteamID)
        {
            onSteamIDChanged?.Invoke(this);
        }

        private void PlayerNameChanged(string _, string newPlayerSteamName)
        {
            onPlayerSteamNameChanged?.Invoke(this);
        }

        #endregion

        #region MonoBehavior Functions

        private void Awake()
        {
            if (CustomNetworkManager.singleton.dontDestroyOnLoad)
            {
                DontDestroyOnLoad(gameObject);
            }
        }

        private void OnEnable()
        {
            if (!CustomSteamManager.Initialized)
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
        }

        [Command]
        private void CommandUpdateSteamID(CSteamID newSteamID)
        {
            steamID = newSteamID;
        }

        #endregion

        #region Client

        public override void OnStartClient()
        {
            InitializeNewPlayerUI();
        }

        public override void OnStopClient()
        {
            //LobbyUIController.Instance.RemovePlayerUI(this); need for example
        }

        public override void OnStartAuthority()
        {
            CommandUpdateSteamID(SteamUser.GetSteamID());
        }

        #endregion

        #region Steam Callbacks

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

        private void InvokeAllClientEvents()
        {
            onPlayerSteamNameChanged.Invoke(this);
        }

        #endregion

        #region Other Functions

        private void InitializeNewPlayerUI()
        {
            //LobbyUIController.Instance.AddPlayerUI(this); Need for example
            InvokeAllClientEvents();
        }

        #endregion
    }
}
