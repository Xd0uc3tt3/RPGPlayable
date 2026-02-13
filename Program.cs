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
        public Health Health { get; private set; }


        protected Entity(int x, int y, char mark, int hp)
        {
            X = x;
            Y = y;
            Mark = mark;
            Health = new Health(hp);
        }

        public void SetPosition(int x, int y)
        {
            X = x;
            Y = y;
        }

        public void Attack(Entity target)
        {
            target.Health.TakeDamage(1);
        }
    }

    class Player : Entity
    {
        public Player(int x, int y) : base(x, y, 'P', 5)
        {

        }
    }

    class Enemy : Entity
    {
        private static Random random = new Random();
        public Enemy(int x, int y) : base(x, y, 'E', 5)
        {
            
        }

        public (int dx, int dy) GetMove(Player player)
        {
            int[] directions = { -1, 0, 1 };
            int dx = directions[random.Next(3)];
            int dy = directions[random.Next(3)];

            if (Math.Abs(dx) == Math.Abs(dy)) return (0, 0);

            return (dx, dy);
        }
    }

    class Map
    {
        private char[,] grid;
        public int Width { get; private set; }
        public int Height { get; private set; }

        public Player Player { get; private set; }
        public List<Enemy> Enemies { get; private set; }

        public Map(string filePath)
        {
            LoadMap(filePath);
        }

        private void LoadMap(string filePath)
        {
            var lines = File.ReadAllLines(filePath);
            Height = lines.Length;
            Width = lines[0].Length;

            grid = new char[Width, Height];
            Enemies = new List<Enemy>();

            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    char c = lines[y][x];

                    if (c == 'P')
                    {
                        Player = new Player(x, y);
                        grid[x, y] = '.';
                    }
                    else if (c == 'E')
                    {
                        Enemies.Add(new Enemy(x, y));
                        grid[x, y] = '.';
                    }
                    else
                    {
                        grid[x, y] = c;
                    }
                }
            }
        }

        public bool IsWall(int x, int y)
        {
            if (x < 0 || y < 0 || x >= Width || y >= Height)
            {
                return true;
            }

            return grid[x, y] == '#';
        }

        public Entity GetEntityAt(int x, int y)
        {
            if (Player.X == x && Player.Y == y && !Player.Health.IsDead)
            {
                return Player;
            }

            foreach (var enemy in Enemies)
            {
                if (enemy.X == x && enemy.Y == y && !enemy.Health.IsDead)
                {
                    return enemy;
                }

            }

            return null;
        }

        public void RemoveDeadEnemies()
        {
            Enemies.RemoveAll(e => e.Health.IsDead);
        }

        public void Draw()
        {
            Console.Clear();

            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    var entity = GetEntityAt(x, y);
                    if (entity != null)
                    {
                        Console.Write(entity.Mark);
                    }
                    else
                    {
                        Console.Write(grid[x, y]);

                    }
                }
                Console.WriteLine();
            }

            Console.WriteLine($"Player HP: {Player.Health.Current}");
        }

        public void TryMove(Entity entity, int dx, int dy)
        {
            int newX = entity.X + dx;
            int newY = entity.Y + dy;

            if (IsWall(newX, newY))
            {
                return;
            }

            var target = GetEntityAt(newX, newY);

            if (target != null && target != entity)
            {
                entity.Attack(target);
                return;
            }

            entity.SetPosition(newX, newY);
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            Map map = new Map("map.txt");

            while (!map.Player.Health.IsDead)
            {
                map.Draw();

                foreach (var enemy in map.Enemies)
                {
                    var move = enemy.GetMove(map.Player);
                    map.TryMove(enemy, move.dx, move.dy);
                }

                Console.ReadKey();
            }

            Console.ReadKey();
        }
    }
}

    



