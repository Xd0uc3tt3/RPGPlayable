using System;
using System.Collections.Generic;

namespace RPGPlayable
{
    class Medkit : Item
    {
        public Medkit(int x, int y) : base(x, y, 'M') { }

        public override void OnPickup(Player player)
        {
            player.Health.Heal(GameSettings.MedkitHeal);
        }

    }

}