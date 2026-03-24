using System;
using System.Collections.Generic;

namespace RPGPlayable
{
    abstract class Entity
    {
        public int X { get; protected set; }
        public int Y { get; protected set; }
        public char Mark { get; protected set; }
        public Health Health { get; protected set; }

        protected Entity(int x, int y, char mark, int hp)
        {
            X = x;
            Y = y;
            Mark = mark;
            Health = new Health(hp);
        }

        public abstract void TakeTurn(Game game);
    }
}