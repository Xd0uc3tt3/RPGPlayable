using System;
using System.Collections.Generic;

namespace RPGPlayable
{
    class BasicEnemy : Enemy
    {
        public BasicEnemy(int x, int y) : base(x, y, 'E', GameSettings.BasicEnemyHP) { }

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
                int damage = random.Next(1, 6);
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