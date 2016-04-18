using System;

namespace Server.Items
{
    /// <summary>
    /// Also known as the Haymaker, this attack dramatically increases the damage done by a weapon reaching its mark.
    /// </summary>
    public class CrushingBlow : WeaponAbility
    {
        public CrushingBlow()
        {
        }

        public override int BaseMana
        {
            get
            {
                return 20;
            }
        }
        public override double DamageScalar
        {
            get
            {
                return 1.5;
            }
        }
        public override void OnHit(Mobile attacker, Mobile defender, int damage)
        {
            if (!this.Validate(attacker) || !this.CheckMana(attacker, true))
                return;

            ClearCurrentAbility(attacker);

            attacker.SendLocalizedMessage(1060090); // You have delivered a crushing blow!
            defender.SendLocalizedMessage(1060091); // You take extra damage from the crushing attack!

            defender.PlaySound(0x1E1);
            defender.FixedParticles(0, 1, 0, 9946, EffectLayer.Head);

            Effects.SendMovingParticles(new Entity(Serial.Zero, new Point3D(defender.X, defender.Y, defender.Z + 50), defender.Map), new Entity(Serial.Zero, new Point3D(defender.X, defender.Y, defender.Z + 20), defender.Map), 0xFB4, 1, 0, false, false, 0, 3, 9501, 1, 0, EffectLayer.Head, 0x100);
        }
    }
}