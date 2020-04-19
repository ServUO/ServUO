namespace Server.Items
{
    public class SerpentArrow : WeaponAbility
    {
        public override int BaseMana => 25;

        public override SkillName GetSecondarySkill(Mobile from)
        {
            return SkillName.Poisoning;
        }

        public override void OnHit(Mobile attacker, Mobile defender, int damage)
        {
            if (!Validate(attacker) || !CheckMana(attacker, true))
                return;

            ClearCurrentAbility(attacker);

            defender.SendLocalizedMessage(1112369); // 	You have been poisoned by a lethal arrow!

            int level;

            if (attacker.InRange(defender, 2))
            {
                int total = (attacker.Skills.Poisoning.Fixed) / 2;

                if (total >= 1000)
                    level = 3;
                else if (total > 850)
                    level = 2;
                else if (total > 650)
                    level = 1;
                else
                    level = 0;
            }
            else
            {
                level = 0;
            }

            defender.ApplyPoison(attacker, Poison.GetPoison(level));

            defender.FixedParticles(0x374A, 10, 15, 5021, EffectLayer.Waist);
            defender.PlaySound(0x474);
        }
    }
}
