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

        private Camera camera;

        private Random rand = new Random();

        public Game()
        {
            Map = new Map("map.txt");

            Player = Map.Player;
            enemyManager = new EnemyManager(Map.Enemies);
            itemManager = new ItemManager(Map.Items);
            camera = new Camera(GameSettings.CameraWidth, GameSettings.CameraHeight);

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
                    camera.ResetDraw();

                    enemyManager.Respawn(Map, Player, GameSettings.BaseEnemySpawn + currentWave * GameSettings.EnemyScaling);
                    itemManager.Respawn(Map, Player, GameSettings.BaseItemSpawn + currentWave * GameSettings.EnemyScaling);
                }

                camera.Update(Player, Map);
                camera.Draw(Map, Player, enemyManager.Enemies);

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

            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine();
            Console.WriteLine("Press R to try again or Q to quit");

            while (true)
            {
                var key = Console.ReadKey(true).Key;

                if (key == ConsoleKey.R)
                {
                    Console.Clear();
                    var newGame = new Game();
                    newGame.Run();
                    break;
                }
                else if (key == ConsoleKey.Q)
                {
                    break;
                }
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