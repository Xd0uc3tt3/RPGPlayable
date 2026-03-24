using System;
using System.Collections.Generic;

namespace RPGPlayable
{
    abstract class Enemy : Entity
    {
        protected static Random random = new Random();

        protected Enemy(int x, int y, char mark, int hp) : base(x, y, mark, hp)
        {
        }

        public abstract override void TakeTurn(Game game);
    }

}