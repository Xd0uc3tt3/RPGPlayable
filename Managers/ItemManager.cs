using System;
using System.Collections.Generic;

namespace RPGPlayable
{
    class ItemManager
    {
        public List<Item> Items { get; private set; }
        private Random rand = new Random();

        public ItemManager(List<Item> items)
        {
            Items = items;
        }

        public void Respawn(Map map, Player player, int count)
        {
            int spawned = 0;

            while (spawned < count)
            {
                int x = rand.Next(0, map.Width);
                int y = rand.Next(0, map.Height);

                bool isWalkable = map.IsWalkable(x, y);
                bool isPlayerThere = (player.X == x && player.Y == y);
                bool isEnemyThere = map.Enemies.Exists(e => e.X == x && e.Y == y && !e.Health.IsDead);

                if (!isWalkable || isPlayerThere || isEnemyThere)
                    continue;

                Item item;
                int type = rand.Next(3);

                if (type == 0)
                {
                    item = new Medkit(x, y);
                }
                else if (type == 1)
                {
                    item = new Sword(x, y);
                }
                else
                {
                    item = new Sheild(x, y);
                }

                Items.Add(item);
                spawned++;
            }
        }

        public void Remove(Item item)
        {
            Items.Remove(item);
        }

        public Item GetItemAt(int x, int y)
        {
            return Items.Find(i => i.X == x && i.Y == y);
        }
    }

}