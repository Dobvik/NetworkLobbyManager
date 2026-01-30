using System.Collections;
using Mirror;
using Steamworks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace NetworkLobbyManager
{
    public class BaseLobbyNetworkManager : NetworkManager
    {
        [Header("Custom settings")]
        [SerializeField] private BaseNetworkLobbyPlayer lobbyPlayerPrefab;
        private bool isInTransition;

        #region Server

        public override void OnServerReady(NetworkConnectionToClient connectionToClient)
        {
            base.OnServerReady(connectionToClient);
        
            if (connectionToClient.identity == null)
            {
                InitializeLobbyPlayer(connectionToClient);
            }
        }

        [Server]
        protected virtual void InitializePlayer(NetworkConnectionToClient connectionToClient, string sceneName)
        {
            var startPosition = GetStartPosition();
            var player = startPosition is not null ? Instantiate(playerPrefab, startPosition.position, startPosition.rotation) : Instantiate(playerPrefab);
            SceneManager.MoveGameObjectToScene(player, SceneManager.GetSceneByPath(sceneName));
    
            player.name = $"{player.name} [connection ID = {connectionToClient.connectionId}]";
            NetworkServer.ReplacePlayerForConnection(connectionToClient, player, ReplacePlayerOptions.KeepAuthority);
        }

        [Server]
        protected virtual void InitializeLobbyPlayer(NetworkConnectionToClient connectionToClient)
        {
            var newLobbyPlayer = Instantiate(lobbyPlayerPrefab.gameObject, Vector3.zero, Quaternion.identity);
            newLobbyPlayer.name = $"{lobbyPlayerPrefab.name} [connection ID = {connectionToClient.connectionId}]";
            NetworkServer.AddPlayerForConnection(connectionToClient, newLobbyPlayer);
        }

        /// <summary>
        /// Sends network gameObject to another scene
        /// </summary>
        [Server]
        protected IEnumerator SendGameObjectToNewScene(GameObject gameObjectToMove, string sceneName)
        {
            if (!gameObjectToMove.TryGetComponent(out NetworkIdentity _))
            {
                yield break;
            }
    
            yield return null;

            SceneManager.MoveGameObjectToScene(gameObjectToMove, SceneManager.GetSceneByPath(sceneName));
        }

        #endregion

        #region Client

        public override void OnStopClient()
        {
            if (!BaseSteamManager.Initialized)
            {
                return;
            }
        
            BaseSteamManager.Instance.CurrentLobbyID = CSteamID.Nil;
        }

        public override void OnClientSceneChanged()
        {
            if (!isInTransition)
            {
                base.OnClientSceneChanged();
            }
        }

        public override void OnClientChangeScene(string sceneName, SceneOperation sceneOperation, bool customHandling)
        {
            switch (sceneOperation)
            {
                case SceneOperation.UnloadAdditive:
                    StartCoroutine(ClientUnloadAdditive(sceneName));
                    break;
                case SceneOperation.LoadAdditive:
                    StartCoroutine(ClientLoadAdditive(sceneName));
                    break;
            }
        }

        #endregion

        #region Loading Scenes

        [Server]
        public IEnumerator ServerLoadSubScene(string sceneName)
        {
            yield return SceneManager.LoadSceneAsync(sceneName, new LoadSceneParameters
            {
                loadSceneMode = LoadSceneMode.Additive,
                localPhysicsMode = LocalPhysicsMode.Physics3D
            });
        }

        [Server]
        public IEnumerator ServerUnloadSubScene(string sceneName)
        {
            if (SceneManager.GetSceneByName(sceneName).IsValid() || SceneManager.GetSceneByPath(sceneName).IsValid())
            {
                yield return SceneManager.UnloadSceneAsync(sceneName);
                yield return Resources.UnloadUnusedAssets();
            }
        }

        private IEnumerator ClientLoadAdditive(string sceneName)
        {
            isInTransition = true;
    
            if (mode == NetworkManagerMode.ClientOnly)
            {
                loadingSceneAsync = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);

                while (loadingSceneAsync is not null && !loadingSceneAsync.isDone)
                    yield return null;
            }
    
            NetworkClient.isLoadingScene = false;
            isInTransition = false;

            OnClientSceneChanged();
        }

        private IEnumerator ClientUnloadAdditive(string sceneName)
        {
            isInTransition = true;
    
            if (mode == NetworkManagerMode.ClientOnly)
            {
                yield return SceneManager.UnloadSceneAsync(sceneName);
                yield return Resources.UnloadUnusedAssets();
            }
    
            NetworkClient.isLoadingScene = false;
            isInTransition = false;

            OnClientSceneChanged();
        }

        #endregion
    }
}
