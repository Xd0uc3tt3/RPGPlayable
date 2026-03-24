using System;
using System.Collections.Generic;

namespace RPGPlayable
{
    class ShieldedEnemy : Enemy
    {
        private bool movedLastTurn = false;
        public ShieldedEnemy(int x, int y) : base(x, y, 'B', GameSettings.ShieldEnemyHP)
        {
            Health.SetShield(GameSettings.ShieldEnemyShield);
        }

        public override void TakeTurn(Game game)
        {
            if (movedLastTurn)
            {
                movedLastTurn = false;
                return;
            }

            int dx = Math.Sign(game.Player.X - X);
            int dy = Math.Sign(game.Player.Y - Y);

            AttemptMove(game, dx, dy);

            movedLastTurn = true;
        }

        private void AttemptMove(Game game, int dx, int dy)
        {
            int nx = X + dx;
            int ny = Y + dy;

            if (game.Player.X == nx && game.Player.Y == ny)
            {
                int damage = new Random().Next(2, 6);
                game.Player.Health.TakeDamage(damage);
                return;
            }

            if (game.Map.IsWalkable(nx, ny) && game.GetEnemyAt(nx, ny) == null)
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
    }

}