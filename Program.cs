using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace RPGPlayable
{
    class Health
    {
        public int Current { get; private set; }
        public int Max { get; private set; }

        public bool IsDead => Current <= 0;

        public Health(int max)
        {
            Max = max;
            Current = max;
        }

        public void TakeDamage(int amount)
        {
            Current -= amount;
            if (Current < 0) Current = 0;
        }
    }

    abstract class Entity
    {
        public int X { get; protected set; }
        public int Y { get; protected set; }
        public char Mark { get; protected set; }
        public Health Health { get; protected set; }

        protected Entity(int x, int y, char mark, int hp)
        {
            X = x;
            Y = y;
            Mark = mark;
            Health = new Health(hp);
        }

        public abstract void TakeTurn(Game game);
    }

    class Player : Entity
    {
        public Player(int x, int y) : base(x, y, 'P', 50) { }

        public override void TakeTurn(Game game)
        {
            var key = Console.ReadKey(true).Key;

            int dx = 0, dy = 0;

            switch (key)
            {
                case ConsoleKey.W:
                    dy = -1; 
                    break;
                case ConsoleKey.S:
                    dy = 1; 
                    break;
                case ConsoleKey.A:
                    dx = -1; 
                    break;
                case ConsoleKey.D:
                    dx = 1; break;

                default: 
                    return;
            }

            AttemptMove(game, dx, dy);
        }

        private void AttemptMove(Game game, int dx, int dy)
        {
            int nx = X + dx;
            int ny = Y + dy;

            Enemy enemy = game.GetEnemyAt(nx, ny);
            if (enemy != null)
            {
                enemy.Health.TakeDamage(2);
                return;
            }

            if (game.Map.IsWalkable(nx, ny))
            {
                X = nx;
                Y = ny;
            }
        }
    }

    class Enemy : Entity
    {
        public Enemy(int x, int y) : base(x, y, 'E', 5) { }

        public override void TakeTurn(Game game)
        {
            int dx = Math.Sign(game.Player.X - X);
            int dy = Math.Sign(game.Player.Y - Y);

            AttemptMove(game, dx, dy);
        }

        private void AttemptMove(Game game, int dx, int dy)
        {
            int nx = X + dx;
            int ny = Y + dy;

            if (game.Player.X == nx && game.Player.Y == ny)
            {
                game.Player.Health.TakeDamage(1);
                return;
            }

            if (game.Map.IsWalkable(nx, ny) && game.GetEnemyAt(nx, ny) == null)
            {
                X = nx;
                Y = ny;
            }
        }
    }

    class Map
    {
        private char[,] tiles;
        public int Width { get; }
        public int Height { get; }

        public Player Player { get; private set; }
        public List<Enemy> Enemies { get; private set; }

        public Map(string filePath)
        {
            var lines = File.ReadAllLines(filePath);
            Height = lines.Length;
            Width = lines[0].Length;

            tiles = new char[Width, Height];
            Enemies = new List<Enemy>();

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
                        Enemies.Add(new Enemy(x, y));
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

            return tiles[x, y] == '.';
        }

        public void Draw(Player player, List<Enemy> enemies)
        {
            Console.Clear();

            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    if (player.X == x && player.Y == y)
                    {
                        Console.Write(player.Mark);
                    }
                        
                    else if (enemies.Exists(e => e.X == x && e.Y == y && !e.Health.IsDead))
                    {
                        Console.Write('E');
                    }
                    else
                    {
                        Console.Write(tiles[x, y]);
                    }

                }
                Console.WriteLine();
            }

            Console.WriteLine();
            Console.WriteLine($"Player HP: {player.Health.Current}");
        }
    }

    class Game
    {
        public Map Map { get; }
        public Player Player { get; }
        private List<Enemy> enemies = new List<Enemy>();

        public Game()
        {
            Map = new Map("map.txt");

            Player = Map.Player;
            enemies = Map.Enemies;
        }

        public void Run()
        {
            while (!Player.Health.IsDead)
            {
                enemies.RemoveAll(e => e.Health.IsDead);

                Map.Draw(Player, enemies);

                Player.TakeTurn(this);

                foreach (var enemy in enemies)
                {
                    enemy.TakeTurn(this);
                }

                if (Player.Health.IsDead)
                {
                    break;
                }

            }

            Console.Clear();
            Console.WriteLine("Game Over.");
        }

        public Enemy GetEnemyAt(int x, int y)
        {
            return enemies.Find(e => e.X == x && e.Y == y && !e.Health.IsDead);
        }
    }


    internal class Program
    {
        static void Main(string[] args)
        {
            new Game().Run();
        }
    }
}
