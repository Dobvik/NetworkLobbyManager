using Mirror;
using UnityEngine;

namespace NetworkLobbyManager
{
    public class BaseNetworkLobbyPlayer : NetworkBehaviour
    {
        [SerializeField] private SteamLobbyPlayerInformation steamLobbyPlayerInformation;

        public SteamLobbyPlayerInformation SteamLobbyPlayerInformation => steamLobbyPlayerInformation;

        #region MonoBehavior Functions

        private void Awake()
        {
            if (NetworkManager.singleton.dontDestroyOnLoad)
            {
                DontDestroyOnLoad(gameObject);
            }
        }

        #endregion
    }
}
