using System;
using System.Collections.Generic;

namespace RPGPlayable
{
    abstract class Item
    {
        public int X { get; protected set; }
        public int Y { get; protected set; }
        public char Mark { get; protected set; }

        protected Item(int x, int y, char mark)
        {
            X = x;
            Y = y;
            Mark = mark;
        }

        public abstract void OnPickup(Player player);
    }

}