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
        //
    }

    class Entity
    {
        //
    }

    class Player : Entity
    {
        //
    }

    class Enemy : Entity
    {
        //
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
