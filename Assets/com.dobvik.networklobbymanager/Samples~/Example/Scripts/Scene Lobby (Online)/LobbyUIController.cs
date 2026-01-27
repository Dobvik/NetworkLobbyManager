using Mirror;
using UnityEngine;
using UnityEngine.UI;

namespace NetworkLobbyManager.Example
{
    public class LobbyUIController : MonoBehaviour
    {
        public static LobbyUIController Instance { get; private set; }

        [SerializeField] private Button returnToMainMenuButton;
        [SerializeField] private Button enterGameButton;
        [SerializeField] private PlayerLobbyListDrawer playerLobbyListDrawer;

        public void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }
    
            Instance = this;
    
            returnToMainMenuButton.onClick.AddListener(ReturnMainMenu);
            enterGameButton.onClick.AddListener(EnterGame);
        }

        public void AddPlayerUI(BaseNetworkLobbyPlayer baseNetworkLobbyPlayer)
        {
            playerLobbyListDrawer.AddPlayer(baseNetworkLobbyPlayer);
        }

        public void RemovePlayerUI(BaseNetworkLobbyPlayer baseNetworkLobbyPlayer)
        {
            playerLobbyListDrawer.RemovePlayer(baseNetworkLobbyPlayer);
        }

        #region Buttons

        private void ReturnMainMenu()
        {
            if (NetworkServer.active && NetworkClient.isConnected)
            {
                CustomNetworkManager.singleton.StopHost();
            }
            else if (NetworkClient.isConnected)
            {
                CustomNetworkManager.singleton.StopClient();
            }
        }

        private void EnterGame()
        {
            Instance.gameObject.SetActive(false);
            NetworkClient.Send(new PlayerEntryGameMessage());
        }

        #endregion
    }
}

