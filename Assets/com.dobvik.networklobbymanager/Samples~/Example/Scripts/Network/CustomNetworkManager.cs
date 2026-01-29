using System.Collections;
using Mirror;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace NetworkLobbyManager.Example
{
    public class CustomNetworkManager : BaseLobbyNetworkManager
    {
        // ReSharper disable once InconsistentNaming
        public new static CustomNetworkManager singleton => (CustomNetworkManager)NetworkManager.singleton;
    
        [Header("All sub-scenes")]
        [Scene, SerializeField] private string firstGameScene;
        [Scene, SerializeField] private string secondGameScene;
        public string FirstGameScene => firstGameScene;
        public string SecondGameScene => secondGameScene;

        #region Server

        public override void OnServerReady(NetworkConnectionToClient connectionToClient)
        {
            base.OnServerReady(connectionToClient);
            
            var message = new SceneMessage { sceneName = firstGameScene, sceneOperation = SceneOperation.LoadAdditive, customHandling = true };
            connectionToClient.Send(message);
        }
    
        public override void OnStartServer()
        {
            NetworkServer.RegisterHandler<PlayerEntryGameMessage>(OnPlayerEntryGame);
        }

        private void OnPlayerEntryGame(NetworkConnectionToClient networkConnectionToClient, PlayerEntryGameMessage playerEntryGameMessage)
        {
            InitializePlayer(networkConnectionToClient, firstGameScene); // TODO: Be careful, players can call this without any restrictions
        }

        public override void OnServerSceneChanged(string sceneName)
        {
            if (sceneName == onlineScene)
            {
                StartCoroutine(ServerLoadSubScene(firstGameScene));
                StartCoroutine(ServerLoadSubScene(secondGameScene));
            }
        }

        [Server]
        public IEnumerator SendPlayerToNewScene(GameObject player, string sceneName)
        {
            if (!player.TryGetComponent(out NetworkIdentity identity))
            {
                yield break;
            }
    
            var connectionToClient = identity.connectionToClient;
        
            if (connectionToClient == null)
            {
                yield break;
            }
        
            connectionToClient.Send(new SceneMessage { sceneName = player.scene.path, sceneOperation = SceneOperation.UnloadAdditive, customHandling = true });
        
            NetworkServer.RemovePlayerForConnection(connectionToClient, RemovePlayerOptions.Unspawn);
        
            yield return null;
        
            var startPosition = GetStartPosition();
            
            player.transform.position = startPosition?.position ?? Vector3.zero;
    
            SceneManager.MoveGameObjectToScene(player, SceneManager.GetSceneByPath(sceneName));
        
            connectionToClient.Send(new SceneMessage { sceneName = sceneName, sceneOperation = SceneOperation.LoadAdditive, customHandling = true });
        
            NetworkServer.AddPlayerForConnection(connectionToClient, player);
        }

        #endregion
    }
}
