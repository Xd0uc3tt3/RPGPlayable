using System;
using System.Collections.Generic;

namespace RPGPlayable
{
    class Sword : Item
    {
        public Sword(int x, int y) : base(x, y, 'S') { }

        public override void OnPickup(Player player)
        {
            player.EquipSword();
        }
    }

}