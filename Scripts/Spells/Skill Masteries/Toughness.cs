using System;
using Server;
using Server.Spells;
using Server.Network;
using Server.Mobiles;

namespace Server.Spells.SkillMasteries
{
    public class ToughnessSpell : SkillMasterySpell
    {
        private static SpellInfo m_Info = new SpellInfo(
                "Toughness", "",
                -1,
                9002
            );

        public override double UpKeep { get { return 20; } }
        public override int RequiredMana { get { return 20; } }

        public override SkillName CastSkill { get { return SkillName.Macing; } }
        public override SkillName DamageSkill { get { return SkillName.Tactics; } }

        private int _HPBonus;

        public ToughnessSpell(Mobile caster, Item scroll)
            : base(caster, scroll, m_Info)
        {
        }

        public override bool CheckCast()
        {
            if (!CheckWeapon())
            {
                Caster.SendLocalizedMessage(1155983); // You must have a mace weapon equipped to use this ability!
                return false;
            }

            ToughnessSpell spell = GetSpell(Caster, this.GetType()) as ToughnessSpell;

            if (spell != null)
            {
                spell.Expire();
                return false;
            }

            return base.CheckCast();
        }

        public override void SendCastEffect()
        {
            if (Caster.Player)
            {
                Caster.PlaySound(Caster.Female ? 0x338 : 0x44A);
            }
            else if (Caster is BaseCreature)
            {
                Caster.PlaySound(((BaseCreature)Caster).GetAngerSound());
            }
        }

        public override void OnCast()
        {
            if (CheckSequence())
            {
                Effects.SendTargetParticles(Caster, 0x37CC, 1, 40, 1953, 0, 9907, EffectLayer.LeftFoot, 0);
                Caster.PlaySound(0x1EE);

                _HPBonus = (int)(BaseSkillBonus / 4);

                BeginTimer();

                BuffInfo.AddBuff(Caster, new BuffInfo(BuffIcon.Toughness, 1155985, 1155986, String.Format("{0}\t{1}", _HPBonus.ToString(), ScaleMana((int)UpKeep)))); // Hit Point Increase: ~1_VAL~.<br>Mana Upkeep Cost: ~2_VAL~.
            }

            FinishSequence();
        }

        public override void EndEffects()
        {
            BuffInfo.RemoveBuff(Caster, BuffIcon.Toughness);
        }

        public static int GetHPBonus(Mobile m)
        {
            ToughnessSpell spell = GetSpell(m, typeof(ToughnessSpell)) as ToughnessSpell;

            if (spell != null)
                return spell._HPBonus;

            return 0;
        }
    }
}