using Steamworks;
using UnityEngine;
using UnityEngine.UI;

namespace NetworkLobbyManager.Example
{
    public class MainMenu : MonoBehaviour
    {
        [SerializeField] private Transform serversMenu;

        [Header("Buttons")]
        [SerializeField] private Button buttonHostGame;
        [SerializeField] private Button buttonFindGame;
        [SerializeField] private Button buttonExit;
    
        [Header("Callbacks")]
        private Callback<LobbyCreated_t> lobbyCreated;

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
        
            lobbyCreated = Callback<LobbyCreated_t>.Create(OnLobbyCreated);
        }

        private void OnDisable()
        {
            lobbyCreated?.Dispose();
        }

        #endregion

        #region Buttons

        private void SubscribeButtons()
        {
            buttonHostGame.onClick.AddListener(HostGame);
            buttonFindGame.onClick.AddListener(FindGame);
            buttonExit.onClick.AddListener(Exit);
        }

        private void FindGame()
        {
            serversMenu.gameObject.SetActive(true);
        }
    
        private void HostGame()
        {
            buttonHostGame.interactable = false;
            SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypePublic, CustomNetworkManager.singleton.maxConnections);
        }

        private void Exit()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
               Application.Quit();
#endif
        }
    
        #endregion

        #region Steam Callbacks

        private void OnLobbyCreated(LobbyCreated_t callback)
        {
            //Debug.Log($"Lobby {callback.m_eResult}"); // Broadcasts as lobby status.
        
            CustomNetworkManager.singleton.StartHost();
            CustomSteamManager.Instance.CurrentLobbyID = new CSteamID(callback.m_ulSteamIDLobby);
        }

        #endregion
    }
}
