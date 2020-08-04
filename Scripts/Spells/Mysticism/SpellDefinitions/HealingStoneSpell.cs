using Server.Items;
using System;

namespace Server.Spells.Mysticism
{
    public class HealingStoneSpell : MysticSpell
    {
        public override SpellCircle Circle => SpellCircle.First;

        public override TimeSpan CastDelayBase => TimeSpan.FromSeconds(5);

        private static readonly SpellInfo m_Info = new SpellInfo(
                "Healing Stone", "Kal In Mani",
                230,
                9022,
                Reagent.Bone,
                Reagent.Garlic,
                Reagent.Ginseng,
                Reagent.SpidersSilk
            );

        public HealingStoneSpell(Mobile caster, Item scroll) : base(caster, scroll, m_Info)
        {
        }

        public override void OnCast()
        {
            if (Caster.Backpack != null && CheckSequence())
            {
                Item[] stones = Caster.Backpack.FindItemsByType(typeof(HealingStone));

                for (int i = 0; i < stones.Length; i++)
                    stones[i].Delete();

                int amount = (int)((Caster.Skills[CastSkill].Value + Caster.Skills[DamageSkill].Value) * 1.25);
                int maxHeal = (int)((Caster.Skills[CastSkill].Value + Caster.Skills[DamageSkill].Value) / 6);

                Caster.PlaySound(0x650);
                Caster.FixedParticles(0x3779, 1, 15, 0x251E, 0, 0, EffectLayer.Waist);

                Caster.Backpack.DropItem(new HealingStone(Caster, amount, maxHeal));
                Caster.SendLocalizedMessage(1080115); // A Healing Stone appears in your backpack.
            }

            FinishSequence();
        }
    }
}
