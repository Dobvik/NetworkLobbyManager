namespace NetworkLobbyManager.Example
{
    public class CustomNetworkLobbyPlayer : BaseNetworkLobbyPlayer
    {
        public override void OnStartClient()
        {
            LobbyUIController.Instance.AddPlayerUI(this);
        }

        public override void OnStopClient()
        {
            LobbyUIController.Instance.RemovePlayerUI(this);
        }
    }
}