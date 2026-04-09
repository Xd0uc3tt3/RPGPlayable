using System;
using System.Collections.Generic;

namespace RPGPlayable
{
    class Camera
    {
        public int ViewWidth { get; }
        public int ViewHeight { get; }
        public int WorldOffsetX { get; private set; }
        public int WorldOffsetY { get; private set; }

        private char[,] lastDrawnTiles;
        private bool firstDraw = true;

        public Camera(int viewWidth, int viewHeight)
        {
            ViewWidth = viewWidth;
            ViewHeight = viewHeight;
            lastDrawnTiles = new char[ViewWidth, ViewHeight];
        }

        public void Update(Player player, Map map)
        {
            WorldOffsetX = Math.Max(0, Math.Min(player.X - ViewWidth / 2, map.Width - ViewWidth));
            WorldOffsetY = Math.Max(0, Math.Min(player.Y - ViewHeight / 2, map.Height - ViewHeight));
        }

        public void Draw(Map map, Player player, List<Enemy> enemies)
        {
            if (firstDraw)
            {
                Console.Clear();
                Console.CursorVisible = false;
                lastDrawnTiles = new char[ViewWidth, ViewHeight];
                firstDraw = false;
            }

            for (int screenY = 0; screenY < ViewHeight; screenY++)
            {
                for (int screenX = 0; screenX < ViewWidth; screenX++)
                {
                    int worldX = screenX + WorldOffsetX;
                    int worldY = screenY + WorldOffsetY;

                    char current = map.GetDisplayChar(worldX, worldY, player, enemies);

                    if (lastDrawnTiles[screenX, screenY] != current)
                    {
                        map.DrawCell(worldX, worldY, screenX, screenY, player, enemies);
                        lastDrawnTiles[screenX, screenY] = current;
                    }
                }
            }

            map.DrawHUD(player, ViewHeight);
        }

        public void ResetDraw()
        {
            firstDraw = true;
        }
    }


}
    