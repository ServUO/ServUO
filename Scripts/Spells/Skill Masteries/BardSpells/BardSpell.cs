using Server.Items;
using Server.Mobiles;
using System;

namespace Server.Spells.SkillMasteries
{
    public abstract class BardSpell : SkillMasterySpell
    {
        private BaseInstrument m_Instrument;
        public BaseInstrument Instrument => m_Instrument;

        public virtual double SlayerBonus => 1.5;

        public override double RequiredSkill => 90;
        public override double UpKeep => 0;
        public override int RequiredMana => 0;
        public override bool PartyEffects => false;
        public override bool DamageCanDisrupt => true;
        public override bool CheckManaBeforeCast => !HasSpell(Caster, GetType());

        public override double BaseSkillBonus => Math.Floor(2 + (((Caster.Skills[CastSkill].Base - 90) / 10) + ((Caster.Skills[DamageSkill].Base - 90) / 10)));

        public override double CollectiveBonus
        {
            get
            {
                double bonus = 0;
                double prov = Caster.Skills[SkillName.Provocation].Base;
                double peac = Caster.Skills[SkillName.Peacemaking].Base;
                double disc = Caster.Skills[SkillName.Discordance].Base;

                switch (CastSkill)
                {
                    default: return 0.0;
                    case SkillName.Provocation:
                        if (peac >= 100) bonus += 1 + ((peac - 100) / 10);
                        if (disc >= 100) bonus += 1 + ((disc - 100) / 10);
                        break;
                    case SkillName.Peacemaking:
                        if (prov >= 100) bonus += 1 + ((peac - 100) / 10);
                        if (disc >= 100) bonus += 1 + ((disc - 100) / 10);
                        break;
                    case SkillName.Discordance:
                        if (prov >= 100) bonus += 1 + ((peac - 100) / 10);
                        if (peac >= 100) bonus += 1 + ((disc - 100) / 10);
                        break;
                }

                return bonus;
            }
        }

        public override SkillName DamageSkill => SkillName.Musicianship;

        public override int UpkeepCancelMessage => 1115665;  // You do not have enough mana to continue infusing your song with magic.
        public override int OutOfRangeMessage => 1115771;  // Your target is no longer in range of your spellsong.
        public override int DisruptMessage => 1115710;  // Your spell song has been interrupted.

        public BardSpell(Mobile caster, Item scroll, SpellInfo info) : base(caster, null, info)
        {
        }

        public override bool CheckCast()
        {
            int mana = ScaleMana(RequiredMana);

            if (!base.CheckCast())
                return false;

            m_Instrument = BaseInstrument.GetInstrument(Caster);

            if (m_Instrument == null)
            {
                BaseInstrument.PickInstrument(Caster, OnPickedInstrument);
                return false;
            }

            return true;
        }

        public void OnPickedInstrument(Mobile from, BaseInstrument instrument)
        {
            from.SendMessage("You choose a muscial instrument. Try using the bard mastery again.");
        }

        public override bool CheckFizzle()
        {
            bool check = base.CheckFizzle();

            if (check)
            {
                if (m_Instrument != null)
                {
                    m_Instrument.PlayInstrumentWell(Caster);
                    m_Instrument.ConsumeUse(Caster);
                }
            }
            else
            {
                Caster.SendLocalizedMessage(500612); // You play poorly, and there is no effect.

                if (m_Instrument != null)
                    m_Instrument.PlayInstrumentBadly(Caster);
            }

            return check;
        }

        public override void SendCastEffect()
        {
            Caster.FixedEffect(0x37C4, 10, (int)(GetCastDelay().TotalSeconds * 28), 4, 3);
        }

        public override void GetCastSkills(out double min, out double max)
        {
            min = RequiredSkill;
            max = RequiredSkill + 25.0;
        }

        public static int GetMasteryBonus(PlayerMobile pm, SkillName useSkill)
        {
            if (useSkill == pm.Skills.CurrentMastery)
                return 10;

            if (pm.Skills.CurrentMastery == SkillName.Provocation
                || pm.Skills.CurrentMastery == SkillName.Discordance
                || pm.Skills.CurrentMastery == SkillName.Peacemaking)
                return 5;

            return 0;
        }

        public virtual double GetSlayerBonus()
        {
            if (Target == null)
                return 1.0;

            ISlayer slayer = Instrument as ISlayer;

            if (slayer != null)
            {
                SlayerEntry se1 = SlayerGroup.GetEntryByName(slayer.Slayer);
                SlayerEntry se2 = SlayerGroup.GetEntryByName(slayer.Slayer2);

                if ((se1 != null && se1.Slays(Target)) || (se2 != null && se2.Slays(Target)))
                {
                    return SlayerBonus;
                }
            }

            return 1.0;
        }

        public override int GetUpkeep()
        {
            var upkeep = base.GetUpkeep();

            if (CastSkill != SkillName.Provocation && Caster.Skills[SkillName.Provocation].Base > 100.0)
            {
                upkeep--;
            }

            if (CastSkill != SkillName.Peacemaking && Caster.Skills[SkillName.Peacemaking].Base > 100.0)
            {
                upkeep--;
            }

            if (CastSkill != SkillName.Discordance && Caster.Skills[SkillName.Discordance].Base > 100.0)
            {
                upkeep--;
            }

            return upkeep;
        }
    }
}
