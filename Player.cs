using System;
using System.Collections.Generic;

namespace RPGPlayable
{
    class Player : Entity
    {
        public Player(int x, int y) : base(x, y, 'P', GameSettings.PlayerMaxHP) { }
        public int SwordCount { get; private set; } = 0;

        public Enemy LastEnemyEncountered { get; private set; } = null;

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
                LastEnemyEncountered = enemy;


                int damage = GameSettings.PlayerSwordDamage + SwordCount;
                enemy.Health.TakeDamage(damage);
                return;
            }

            Item item = game.GetItemAt(nx, ny);
            if (item != null)
            {
                item.OnPickup(this);
                game.RemoveItem(item);
                return;
            }

            if (game.Map.IsWalkable(nx, ny))
            {
                X = nx;
                Y = ny;

                int damage = game.Map.GetLavaDamage(nx, ny);
                if (damage > 0)
                {
                    Health.TakeDamage(damage);
                }
            }
        }

        public void EquipSword()
        {
            SwordCount++;
        }

        public void EquipShield()
        {
            Health.SetShield(GameSettings.ShieldAmount);
        }
    }

}