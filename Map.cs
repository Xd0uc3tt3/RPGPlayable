using System;
using System.Collections.Generic;
using System.IO;

namespace RPGPlayable
{
    class Map
    {
        private char[,] tiles;
        public int Width { get; }
        public int Height { get; }

        public Player Player { get; private set; }
        public List<Enemy> Enemies { get; private set; }

        public List<Item> Items { get; private set; }

        private char[,] lastDrawnTiles;
        private bool firstDraw = true;

        public Map(string filePath)
        {
            var lines = File.ReadAllLines(filePath);
            Height = lines.Length;
            Width = lines[0].Length;

            tiles = new char[Width, Height];
            Enemies = new List<Enemy>();
            Items = new List<Item>();

            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    char c = lines[y][x];

                    if (c == 'P')
                    {
                        Player = new Player(x, y);
                        tiles[x, y] = '.';
                    }
                    else if (c == 'E')
                    {
                        Enemies.Add(new BasicEnemy(x, y));
                        tiles[x, y] = '.';
                    }
                    else if (c == 'B')
                    {
                        Enemies.Add(new ShieldedEnemy(x, y));
                        tiles[x, y] = '.';
                    }
                    else if (c == 'C')
                    {
                        Enemies.Add(new CowardEnemy(x, y));
                        tiles[x, y] = '.';
                    }
                    else if (c == 'M')
                    {
                        Items.Add(new Medkit(x, y));
                        tiles[x, y] = '.';
                    }
                    else if (c == 'S')
                    {
                        Items.Add(new Sword(x, y));
                        tiles[x, y] = '.';
                    }
                    else if (c == 'O')
                    {
                        Items.Add(new Sheild(x, y));
                        tiles[x, y] = '.';
                    }
                    else
                    {
                        tiles[x, y] = c;
                    }
                }
            }
        }

        public bool IsWalkable(int x, int y)
        {
            if (x < 0 || y < 0 || x >= Width || y >= Height)
            {
                return false;
            }

            return tiles[x, y] == '.' || tiles[x, y] == '~';
        }

        public int GetLavaDamage(int x, int y)
        {
            char tile = tiles[x, y];
            if (tile == '~')
            {
                return GameSettings.LavaDamage;
            }
            return 0;
        }

        public void Draw(Player player, List<Enemy> enemies)
        {
            if (firstDraw)
            {
                Console.Clear();
                Console.CursorVisible = false;
                lastDrawnTiles = new char[Width, Height];
                firstDraw = false;
            }

            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    char newChar;
                    ConsoleColor newColor;

                    if (player.X == x && player.Y == y)
                    {
                        newChar = player.Mark;
                        newColor = ConsoleColor.DarkBlue;
                    }

                    else if (enemies.Exists(e => e.X == x && e.Y == y && !e.Health.IsDead))
                    {
                        var enemy = enemies.Find(e => e.X == x && e.Y == y && !e.Health.IsDead);
                        newChar = enemy.Mark;

                        if (enemy.Mark == 'E')
                        {
                            newColor = ConsoleColor.Red;
                        }
                        else if (enemy.Mark == 'B')
                        {
                            newColor = ConsoleColor.DarkRed;
                        }
                        else
                        {
                            newColor = ConsoleColor.DarkYellow;
                        }
                    }
                    else if (Items.Exists(i => i.X == x && i.Y == y))
                    {
                        var item = Items.Find(i => i.X == x && i.Y == y);
                        newChar = item.Mark;

                        if (item.Mark == 'M')
                        {
                            newColor = ConsoleColor.Green;
                        }
                        else if (item.Mark == 'S')
                        {
                            newColor = ConsoleColor.Cyan;
                        }
                        else
                        {
                            newColor = ConsoleColor.Magenta;
                        }
                    }
                    else
                    {
                        char tile = tiles[x, y];
                        newChar = tile;

                        if (tile == '#')
                        {
                            newColor = ConsoleColor.Gray;
                        }
                        else if (tile == '~')
                        {
                            newColor = ConsoleColor.DarkRed;
                        }
                        else
                        {
                            newColor = ConsoleColor.DarkGray;
                        }
                    }

                    if (lastDrawnTiles[x, y] != newChar)
                    {
                        Console.SetCursorPosition(x, y);
                        Console.ForegroundColor = newColor;
                        Console.Write(newChar);
                        lastDrawnTiles[x, y] = newChar;
                    }
                }
            }

        }

        public void ResetDraw()
        {
            firstDraw = true;
        }

        public void DrawHUD(Player player, int offsetY)
        {
            Console.SetCursorPosition(0, offsetY + 1);
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"Player HP: {player.Health.Current} ");

            Console.SetCursorPosition(0, offsetY + 2);
            if (player.Health.ShieldMax > 0 && player.Health.ShieldCurrent > 0)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write($"Shield: {player.Health.ShieldCurrent} ");
            }
            else
            {
                Console.Write("                    ");
            }

            Console.SetCursorPosition(0, offsetY + 4);
            if (player.LastEnemyEncountered != null)
            {
                var enemy = player.LastEnemyEncountered;
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine("Last Enemy Encountered:");
                Console.SetCursorPosition(0, offsetY + 5);
                string enemyTypeName = "Enemy";
                if (enemy.Mark == 'E')
                {
                    enemyTypeName = "Basic Enemy    ";
                }
                if (enemy.Mark == 'C')
                {
                    enemyTypeName = "Cowardly Enemy";
                }
                if (enemy.Mark == 'B')
                {
                    enemyTypeName = "Shielded Enemy";
                }
                Console.WriteLine($"Enemy Type: {enemyTypeName}");
                Console.SetCursorPosition(0, offsetY + 6);
                Console.WriteLine($"HP: {enemy.Health.Current}/{enemy.Health.Max}".PadRight(20));
                Console.SetCursorPosition(0, offsetY + 7);
                if (enemy.Health.ShieldMax > 0)
                {
                    Console.WriteLine($"Shield: {enemy.Health.ShieldCurrent}/{enemy.Health.ShieldMax}".PadRight(20));
                }
                    
                else
                {
                    Console.WriteLine("".PadRight(20));
                }
                    
            }
        }

        public void DrawCell(int worldX, int worldY, int screenX, int screenY, Player player, List<Enemy> enemies)
        {
            char newChar;
            ConsoleColor newColor;

            if (player.X == worldX && player.Y == worldY)
            {
                newChar = player.Mark;
                newColor = ConsoleColor.DarkBlue;
            }
            else if (enemies.Exists(e => e.X == worldX && e.Y == worldY && !e.Health.IsDead))
            {
                var enemy = enemies.Find(e => e.X == worldX && e.Y == worldY && !e.Health.IsDead);
                newChar = enemy.Mark;
                newColor = enemy.Mark == 'E' ? ConsoleColor.Red : enemy.Mark == 'B' ? ConsoleColor.DarkRed : ConsoleColor.DarkYellow;
            }
            else if (Items.Exists(i => i.X == worldX && i.Y == worldY))
            {
                var item = Items.Find(i => i.X == worldX && i.Y == worldY);
                newChar = item.Mark;
                newColor = item.Mark == 'M' ? ConsoleColor.Green : item.Mark == 'S' ? ConsoleColor.Cyan : ConsoleColor.Magenta;
            }
            else
            {
                char tile = tiles[worldX, worldY];
                newChar = tile;
                newColor = tile == '#' ? ConsoleColor.Gray : tile == '~' ? ConsoleColor.DarkRed : ConsoleColor.DarkGray;
            }

            Console.SetCursorPosition(screenX, screenY);
            Console.ForegroundColor = newColor;
            Console.Write(newChar);
        }

        public char GetDisplayChar(int worldX, int worldY, Player player, List<Enemy> enemies)
        {
            if (player.X == worldX && player.Y == worldY)
            {
                return player.Mark;
            }

            var enemy = enemies.Find(e => e.X == worldX && e.Y == worldY && !e.Health.IsDead);

            if (enemy != null)
            {
                return enemy.Mark;
            }

            var item = Items.Find(i => i.X == worldX && i.Y == worldY);

            if (item != null)
            {
                return item.Mark;
            }

            return tiles[worldX, worldY];
        }
    }


}