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
            Console.SetCursorPosition(0, 0);

            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    if (player.X == x && player.Y == y)
                    {
                        Console.ForegroundColor = ConsoleColor.DarkBlue;
                        Console.Write(player.Mark);
                    }

                    else if (enemies.Exists(e => e.X == x && e.Y == y && !e.Health.IsDead))
                    {
                        var enemy = enemies.Find(e => e.X == x && e.Y == y && !e.Health.IsDead);

                        if (enemy.Mark == 'E')
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                        }
                        else if (enemy.Mark == 'B')
                        {
                            Console.ForegroundColor = ConsoleColor.DarkRed;
                        }
                        else if (enemy.Mark == 'C')
                        {
                            Console.ForegroundColor = ConsoleColor.DarkYellow;
                        }

                        Console.Write(enemy.Mark);
                    }
                    else if (Items.Exists(i => i.X == x && i.Y == y))
                    {
                        var item = Items.Find(i => i.X == x && i.Y == y);

                        if (item.Mark == 'M')
                        {
                            Console.ForegroundColor = ConsoleColor.Green;
                        }
                        else if (item.Mark == 'S')
                        {
                            Console.ForegroundColor = ConsoleColor.Cyan;
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.Magenta;
                        }

                        Console.Write(item.Mark);
                    }
                    else
                    {
                        char tile = tiles[x, y];

                        if (tile == '#')
                        {
                            Console.ForegroundColor = ConsoleColor.Gray;
                        }
                        else if (tile == '~')
                        {
                            Console.ForegroundColor = ConsoleColor.DarkRed;
                        }
                        else if (tile == '.')
                        {
                            Console.ForegroundColor = ConsoleColor.DarkGray;
                        }

                        Console.Write(tile);
                    }

                }
                Console.WriteLine();
            }

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine();
            Console.WriteLine($"Player HP: {player.Health.Current} ");

            if (player.Health.ShieldMax > 0 && player.Health.ShieldCurrent > 0)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine($"Shield: {player.Health.ShieldCurrent} ");
            }

            Console.WriteLine("                    ");

            if (player.LastEnemyEncountered != null)
            {
                var enemy = player.LastEnemyEncountered;
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine();
                Console.WriteLine("Last Enemy Encountered:");
                Console.WriteLine($"Type: {enemy.Mark}                 ");
                Console.WriteLine($"HP: {enemy.Health.Current}/{enemy.Health.Max}     ");
                if (enemy.Health.ShieldMax > 0)
                {
                    Console.WriteLine($"Shield: {enemy.Health.ShieldCurrent}/{enemy.Health.ShieldMax}     ");
                }
                else
                {
                    Console.WriteLine("                     ");
                }
            }
        }
    }


}