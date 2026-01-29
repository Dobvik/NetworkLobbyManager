using UnityEngine;
using System.IO;
using System.Text.RegularExpressions;
using Mirror;
using Mirror.Examples.AdditiveLevels;

namespace NetworkLobbyManager.Example
{
    // The code is based on Mirror.Examples.AdditiveLevels.Portal
    public class Portal : NetworkBehaviour
    {
        [Scene, Tooltip("Which scene to send player from here")]
        public string destinationScene;

        [Tooltip("Where to spawn player in Destination Scene")]
        public Vector3 startPosition;

        [Tooltip("Reference to child TextMesh label")]
        public TextMesh label; // don't depend on TMPro. 2019 errors.

        [SyncVar(hook = nameof(OnLabelTextChanged)), ReadOnly]
        public string labelText;

        public void OnLabelTextChanged(string _, string newValue)
        {
            label.text = labelText;
        }

        public override void OnStartServer()
        {
            labelText = Path.GetFileNameWithoutExtension(destinationScene).Replace("MirrorAdditiveLevels", "");

            // Simple Regex to insert spaces before capitals, numbers
            labelText = Regex.Replace(labelText, @"\B[A-Z0-9]+", " $0");
        }

        public override void OnStartClient()
        {
            if (label.TryGetComponent(out LookAtMainCamera lookAtMainCamera))
                lookAtMainCamera.enabled = true;
        }
        
        private void OnTriggerEnter(Collider other)
        {
            if (other is not CapsuleCollider) // ignore CharacterController colliders
            {
                return;
            } 
            
            if (!other.CompareTag("Player"))
            {
                return;
            }

            if (isServer)
            {
                CustomNetworkManager.singleton.StartCoroutine(CustomNetworkManager.singleton.SendPlayerToNewScene(other.gameObject, destinationScene));
            }
        }
    }
}
