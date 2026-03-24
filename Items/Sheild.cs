using System;
using System.Collections.Generic;

namespace RPGPlayable
{
    class Sheild : Item
    {
        public Sheild(int x, int y) : base(x, y, 'O') { }

        public override void OnPickup(Player player)
        {
            player.EquipShield();
        }

    }

}