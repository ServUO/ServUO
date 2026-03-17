// ============================================================
//  WeaponLoadAndDrop.cs
//  Server/DNA/WeaponLoadAndDrop.cs
// ============================================================

using System;

namespace Server
{
    public static class WeaponLoadAndDropHelper
    {

        public static double StartSwingMovement(IWeapon weapon, Mobile attacker, Mobile defender)
        {
            if (attacker.Paralyzed || attacker.Frozen)
                return -1.0;

            Direction d = attacker.GetDirectionTo(defender.X, defender.Y);
            if (attacker.Direction != d)
                attacker.Direction = d;

            double animDelay = weapon.GetDelay(attacker).TotalSeconds;
            if (animDelay <= 0) animDelay = 1.5;

            return animDelay;
        }

    }
}