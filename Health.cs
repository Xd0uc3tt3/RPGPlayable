using System;
using System.Collections.Generic;

namespace RPGPlayable
{
    class Health
    {
        public int Current { get; private set; }
        public int Max { get; private set; }

        public int ShieldCurrent { get; private set; } = 0;
        public int ShieldMax { get; private set; } = 0;

        public bool IsDead => Current <= 0;

        public Health(int max)
        {
            Max = max;
            Current = max;
        }

        public void SetShield(int shield)
        {
            ShieldMax = shield;
            ShieldCurrent = shield;
        }

        public void TakeDamage(int amount)
        {
            if (ShieldCurrent > 0)
            {
                if (amount <= ShieldCurrent)
                    ShieldCurrent -= amount;
                else
                {
                    int leftover = amount - ShieldCurrent;
                    ShieldCurrent = 0;
                    Current -= leftover;
                    if (Current < 0) Current = 0;
                }
            }
            else
            {
                Current -= amount;
                if (Current < 0) Current = 0;
            }
        }

        public void Heal(int amount)
        {
            Current += amount;
            if (Current > Max) Current = Max;
        }
    }
}