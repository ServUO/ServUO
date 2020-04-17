#region References
using Server.Items;
using Server.Mobiles;
using Server.Spells.SkillMasteries;
using System;
using System.Globalization;
#endregion

namespace Server.Spells.Spellweaving
{
    public abstract class ArcanistSpell : Spell
    {
        private int m_CastTimeFocusLevel;

        public ArcanistSpell(Mobile caster, Item scroll, SpellInfo info)
            : base(caster, scroll, info)
        { }

        public abstract double RequiredSkill { get; }
        public abstract int RequiredMana { get; }
        public override SkillName CastSkill => SkillName.Spellweaving;
        public override SkillName DamageSkill => SkillName.Spellweaving;
        public override bool ClearHandsOnCast => false;
        public virtual int FocusLevel => m_CastTimeFocusLevel;

        public static int GetFocusLevel(Mobile from)
        {
            ArcaneFocus focus = FindArcaneFocus(from);

            if (focus == null || focus.Deleted)
            {
                if (from is BaseCreature && from.Skills[SkillName.Spellweaving].Value > 0)
                {
                    return (int)Math.Max(1, Math.Min(6, from.Skills[SkillName.Spellweaving].Value / 20));
                }

                return Math.Max(GetMasteryFocusLevel(from), 0);
            }

            return Math.Max(GetMasteryFocusLevel(from), focus.StrengthBonus);
        }

        public static int GetMasteryFocusLevel(Mobile from)
        {
            if (from.Skills.CurrentMastery == SkillName.Spellweaving)
            {
                return Math.Max(1, MasteryInfo.GetMasteryLevel(from, SkillName.Spellweaving));
            }

            return 0;
        }

        public static ArcaneFocus FindArcaneFocus(Mobile from)
        {
            if (from == null || from.Backpack == null)
            {
                return null;
            }

            if (from.Holding is ArcaneFocus)
            {
                return (ArcaneFocus)from.Holding;
            }

            return from.Backpack.FindItemByType<ArcaneFocus>();
        }

        public override bool CheckCast()
        {
            if (!base.CheckCast())
            {
                return false;
            }

            if (!MondainsLegacy.Spellweaving)
            {
                Caster.SendLocalizedMessage(1042753, "Spellweaving"); // ~1_SOMETHING~ has been temporarily disabled.
                return false;
            }

            if (Caster is PlayerMobile && !((PlayerMobile)Caster).Spellweaving)
            {
                Caster.SendLocalizedMessage(1073220); // You must have completed the epic arcanist quest to use this ability.
                return false;
            }

            int mana = ScaleMana(RequiredMana);

            if (Caster.Mana < mana)
            {
                Caster.SendLocalizedMessage(1060174, mana.ToString(CultureInfo.InvariantCulture));
                // You must have at least ~1_MANA_REQUIREMENT~ Mana to use this ability.
                return false;
            }

            if (Caster.Skills[CastSkill].Value < RequiredSkill)
            {
                Caster.SendLocalizedMessage(1063013, String.Format("{0}\t{1}", RequiredSkill.ToString("F1"), "#1044114"));
                // You need at least ~1_SKILL_REQUIREMENT~ ~2_SKILL_NAME~ skill to use that ability.
                return false;
            }

            return true;
        }

        public override void GetCastSkills(out double min, out double max)
        {
            min = RequiredSkill - 12.5; //per 5 on friday, 2/16/07
            max = RequiredSkill + 37.5;
        }

        public override int GetMana()
        {
            return RequiredMana;
        }

        public override void DoFizzle()
        {
            Caster.PlaySound(0x1D6);
            Caster.NextSpellTime = Core.TickCount;
        }

        public override void DoHurtFizzle()
        {
            Caster.PlaySound(0x1D6);
        }

        public override void OnDisturb(DisturbType type, bool message)
        {
            base.OnDisturb(type, message);

            if (message)
            {
                Caster.PlaySound(0x1D6);
            }
        }

        public override void OnBeginCast()
        {
            base.OnBeginCast();

            m_CastTimeFocusLevel = GetFocusLevel(Caster);
        }

        public override void SendCastEffect()
        {
            if (Caster.Player)
                Caster.FixedEffect(0x37C4, 87, (int)(GetCastDelay().TotalSeconds * 28), 4, 3);
        }

        public virtual bool CheckResisted(Mobile m)
        {
            double percent = (50 + 2 * (GetResistSkill(m) - GetDamageSkill(Caster))) / 100;
            //TODO: According to the guide this is it.. but.. is it correct per OSI?

            if (percent <= 0)
            {
                return false;
            }

            if (percent >= 1.0)
            {
                return true;
            }

            return (percent >= Utility.RandomDouble());
        }
    }
}
