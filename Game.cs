using System;
using System.Collections.Generic;

namespace RPGPlayable
{
    class Game
    {
        public Map Map { get; }
        public Player Player { get; }
        private EnemyManager enemyManager;
        private ItemManager itemManager;

        private int currentWave = 1;
        private const int maxWaves = GameSettings.MaxWaves;

        private Random rand = new Random();

        public Game()
        {
            Map = new Map("map.txt");

            Player = Map.Player;
            enemyManager = new EnemyManager(Map.Enemies);
            itemManager = new ItemManager(Map.Items);

        }



        public void Run()
        {
            while (!Player.Health.IsDead)
            {
                enemyManager.RemoveDead();

                if (enemyManager.Enemies.Count == 0)
                {
                    if (currentWave == maxWaves)
                    {
                        break;
                    }

                    currentWave++;
                    Console.Clear();

                    enemyManager.Respawn(Map, Player, GameSettings.BaseEnemySpawn + currentWave * GameSettings.EnemyScaling);
                    itemManager.Respawn(Map, Player, GameSettings.BaseItemSpawn + currentWave * GameSettings.EnemyScaling);
                }

                Map.Draw(Player, enemyManager.Enemies);

                Player.TakeTurn(this);

                enemyManager.Update(this);

                if (Player.Health.IsDead)
                {
                    break;
                }

            }

            Console.Clear();

            if (Player.Health.IsDead)
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine("Game Over.");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("You Win!");
            }
        }

        public Enemy GetEnemyAt(int x, int y)
        {
            return enemyManager.GetEnemyAt(x, y);
        }

        public Item GetItemAt(int x, int y)
        {
            return itemManager.GetItemAt(x, y);
        }

        public void RemoveItem(Item item)
        {
            itemManager.Remove(item);
        }

    }

}