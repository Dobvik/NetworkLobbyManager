using Steamworks;
using UnityEngine;

namespace NetworkLobbyManager
{
    public static class SteamImageHelper
    {
        public static Texture2D GetSprite(int imageId)
        {
            if (!SteamManager.Initialized || imageId == -1)
            {
                return null;
            }

            SteamUtils.GetImageSize(imageId, out var width, out var height);

            var image = new byte[width * height * 4];
            SteamUtils.GetImageRGBA(imageId, image, (int)(width * height * 4));

            var flippedImage = new byte[image.Length];
            var rowSize = (int)width * 4;

            for (var y = 0; y < height; y++)
            {
                var sourceRowStart = y * rowSize;
                var targetRowStart = ((int)height - 1 - y) * rowSize;

                System.Buffer.BlockCopy(image, sourceRowStart, flippedImage, targetRowStart, rowSize);
            }

            var texture = new Texture2D((int)width, (int)height, TextureFormat.RGBA32, false);
            texture.LoadRawTextureData(flippedImage);
            texture.Apply();

            return texture;
        }
    }
}