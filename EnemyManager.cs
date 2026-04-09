using System;
using System.Collections.Generic;

namespace RPGPlayable
{
    class EnemyManager
    {
        public List<Enemy> Enemies { get; private set; }
        private Random rand = new Random();

        public EnemyManager(List<Enemy> enemies)
        {
            Enemies = enemies;
        }

        public void Respawn(Map map, Player player, int count)
        {
            int spawned = 0;
            map.ResetDraw();

            while (spawned < count)
            {
                int x = rand.Next(map.Width);
                int y = rand.Next(map.Height);

                bool isWalkable = map.IsWalkable(x, y);
                bool isPlayerThere = (player.X == x && player.Y == y);
                bool isEnemyThere = GetEnemyAt(x, y) != null;

                if (!(isWalkable && !isPlayerThere && !isEnemyThere))
                {
                    continue;
                }

                Enemy enemy;
                int type = rand.Next(3);

                if (type == 0)
                {
                    enemy = new BasicEnemy(x, y);
                }
                else if (type == 1)
                {
                    enemy = new ShieldedEnemy(x, y);
                }
                else
                {
                    enemy = new CowardEnemy(x, y);
                }

                Enemies.Add(enemy);
                spawned++;
            }
        }

        public Enemy GetEnemyAt(int x, int y)
        {
            return Enemies.Find(e => e.X == x && e.Y == y && !e.Health.IsDead);
        }

        public void RemoveDead()
        {
            Enemies.RemoveAll(e => e.Health.IsDead);
        }

        public void Update(Game game)
        {
            foreach (var enemy in Enemies)
            {
                enemy.TakeTurn(game);
            }

        }
    }

}