namespace Server.Items
{
    /// <summary>
    /// This special move allows the skilled warrior to bypass his target's physical resistance, for one shot only.
    /// The Armor Ignore shot does slightly less damage than normal.
    /// Against a heavily armored opponent, this ability is a big win, but when used against a very lightly armored foe, it might be better to use a standard strike!
    /// </summary>
    public class ArmorIgnore : WeaponAbility
    {
        public override int BaseMana => 30;

        public override double DamageScalar => 0.9;

        public override void OnHit(Mobile attacker, Mobile defender, int damage)
        {
            if (!Validate(attacker) || !CheckMana(attacker, true))
                return;

            ClearCurrentAbility(attacker);

            attacker.SendLocalizedMessage(1060076); // Your attack penetrates their armor!
            defender.SendLocalizedMessage(1060077); // The blow penetrated your armor!

            defender.PlaySound(0x56);
            defender.FixedParticles(0x3728, 200, 25, 9942, EffectLayer.Waist);
        }
    }
}
