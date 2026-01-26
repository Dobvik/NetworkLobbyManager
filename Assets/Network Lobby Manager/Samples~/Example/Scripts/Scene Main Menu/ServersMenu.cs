using Steamworks;
using UnityEngine;
using UnityEngine.UI;

namespace NetworkLobbyManager.Example
{
    public class ServersMenu : MonoBehaviour
    {
        [SerializeField] private EntryServerButton entryServerButtonPrefab;
        [SerializeField] private Transform listDiscoveredServersInterface;
    
        [Header("Buttons")]
        [SerializeField] private Button buttonRefreshServersList;
        [SerializeField] private Button buttonBack;
    
        [Header("Steam Callbacks")]
        private Callback<LobbyMatchList_t> lobbyMatchList;
        private Callback<LobbyEnter_t> lobbyEnter;

        private uint numberServersFound;

        #region MonoBehavior Functions

        private void Awake()
        {
            SubscribeButtons();
        }
    
        private void OnEnable()
        {
            if (!CustomSteamManager.Initialized)
            {
                return;
            }
        
            lobbyMatchList = Callback<LobbyMatchList_t>.Create(OnLobbyMatchList);
            lobbyEnter = Callback<LobbyEnter_t>.Create(OnLobbyEntered);
        }
    
        private void OnDisable()
        {
            lobbyMatchList?.Dispose();
            lobbyEnter?.Dispose();
        }

        #endregion
    
        #region Buttons
    
        private void SubscribeButtons()
        {
            buttonBack.onClick.AddListener(Back);
            buttonRefreshServersList.onClick.AddListener(RefreshServerList);
        }

        private void RefreshServerList()
        {
            buttonRefreshServersList.interactable = false; // Animation. By Ruslan
            SteamMatchmaking.AddRequestLobbyListStringFilter(ServerSteamInformation.KEY_WORD_GAME_KEY, ServerSteamInformation.KEY_WORD_GAME_KEY, ELobbyComparison.k_ELobbyComparisonEqual); // Temporarily, since we do not have our own appid in steam by Ruslan
            SteamMatchmaking.RequestLobbyList(); // Be sure to look at the documentation, there is a lot of BUT. By Ruslan
        
        }

        private void Back()
        {
            gameObject.SetActive(false);
        }
    
        #endregion

        #region Steam Callbacks
    
        private void OnLobbyMatchList(LobbyMatchList_t callback)
        {
            Debug.Log($"List lobby updated, count: {callback.m_nLobbiesMatching}");
            numberServersFound = callback.m_nLobbiesMatching;
            DrawServerList();
            buttonRefreshServersList.interactable = true;
        }
    
        private void OnLobbyEntered(LobbyEnter_t callback)
        {
            var hostAddress = SteamMatchmaking.GetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), ServerSteamInformation.HOST_ADDRESS_KEY);
            CustomSteamManager.Instance.CurrentLobbyID = new CSteamID(callback.m_ulSteamIDLobby);
            CustomNetworkManager.singleton.networkAddress = hostAddress;
            CustomNetworkManager.singleton.StartClient();
        }
    
        #endregion
    
        #region Other Functions
    
        private void DrawServerList()
        {
            foreach (Transform child in listDiscoveredServersInterface)
            {
                Destroy(child.gameObject);
            }
    
            for (var i = 0; i < numberServersFound; i++)
            {
                var newEntryServer = Instantiate(entryServerButtonPrefab.gameObject, listDiscoveredServersInterface).GetComponent<EntryServerButton>();
                newEntryServer.Initialize(SteamMatchmaking.GetLobbyByIndex(i));
            }
        }
    
        #endregion
    }
}
