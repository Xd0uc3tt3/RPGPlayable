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

        protected Entity(int x, int y, char symbol, int hp)
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
        public Enemy(int x, int y) : base(x, y, 'P', 5)
        {
            
        }
    }

    class Map
    {
        private char[,] grid;
        public int Width { get; private set; }
        public int Height { get; private set; }

        public Map(string filePath)
        {
            LoadMap(filePath);
        }

        private void LoadMap(string filePath)
        {
            string[] lines = File.ReadAllLines(filePath);

            Height = lines.Length;
            Width = lines[0].Length;

            grid = new char[Width, Height];

            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    grid[x, y] = lines[y][x];
                }
            }
        }

        public void Draw()
        {
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    Console.Write(grid[x, y]);
                }
                Console.WriteLine();
            }
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            Map map = new Map("map.txt");
            map.Draw();

            Console.ReadKey();
        }
    }


}
