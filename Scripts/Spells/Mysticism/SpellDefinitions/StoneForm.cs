using System;
using System.Collections.Generic;
using Server.Mobiles;
using Server.Spells.Seventh;

namespace Server.Spells.Mystic
{
    public class StoneFormSpell : MysticSpell
    { 
        private static readonly SpellInfo m_Info = new SpellInfo(
            "Stone Form", "In Rel Ylem",
            230,
            9022,
            Reagent.Bloodmoss,
            Reagent.FertileDirt,
            Reagent.Garlic);
        private static readonly Dictionary<Mobile, List<ResistanceMod>> m_Table = new Dictionary<Mobile, List<ResistanceMod>>();
        public StoneFormSpell(Mobile caster, Item scroll)
            : base(caster, scroll, m_Info)
        {
        }

        public override TimeSpan CastDelayBase
        {
            get
            {
                return TimeSpan.FromSeconds(2.0);
            }
        }
        public override double RequiredSkill
        {
            get
            {
                return 33.0;
            }
        }
        public override int RequiredMana
        {
            get
            {
                return 11;
            }
        }
        public static bool HasEffect(Mobile m)
        {
            return m_Table.ContainsKey(m);
        }

        public static void RemoveEffect(Mobile m)
        {
            if (!m_Table.ContainsKey(m))
                return;

            List<ResistanceMod> mods = m_Table[m];

            for (int i = 0; i < m_Table[m].Count; i++)
            {
                m.RemoveResistanceMod(mods[i]);
            }

            m_Table.Remove(m);
            m.EndAction(typeof(StoneFormSpell));
            m.PlaySound(0x201);  
            m.FixedParticles(0x3728, 1, 13, 9918, 92, 3, EffectLayer.Head);
            m.BodyMod = 0;
        }

        public override bool CheckCast()
        {
            if (!base.CheckCast())
            {
                return false;
            }
            else if (!this.Caster.CanBeginAction(typeof(StoneFormSpell)))
            {
                this.Caster.SendLocalizedMessage(1005559);
                return false;
            }
            else if (this.Caster.BodyMod != 0)	
            {
                this.Caster.SendMessage("You cannot transform while in that form.");
                return false;
            }
            else if (this.Caster.BodyMod == 183 || this.Caster.BodyMod == 184)
            {
                this.Caster.SendMessage("You cannot transform while wearing body paint.");
                return false;
            }
            else if (!this.Caster.CanBeginAction(typeof(PolymorphSpell)))
            {
                this.Caster.SendMessage("You cannot transform while polymorphed.");
                return false;
            }
            /* else if ( !Caster.CanBeginAction( typeof( StoneFormSpell ) ) )
            {
            StoneFormSpell.RemoveEffect( Caster );
            Caster.SendMessage( "You are no longer in Stone Form." );
            return false;
            }*/

            return true;
        }

        public override void OnCast()
        {
            if (!this.Caster.CanBeginAction(typeof(StoneFormSpell)))
            {
                this.Caster.SendLocalizedMessage(1005559); // This spell is already in effect.      
            }
            else if (this.Caster.BodyMod != 0)
            {
                this.Caster.SendMessage("You cannot transform while in that form.");
            }
            else if (this.Caster.BodyMod == 183 || this.Caster.BodyMod == 184)
            {
                this.Caster.SendMessage("You cannot transform while wearing body paint.");
            }
            else if (!this.Caster.CanBeginAction(typeof(PolymorphSpell)))
            {
                this.Caster.SendMessage("You cannot transform while polymorphed.");
            }
            else if (this.CheckSequence())
            {
                // Values
                int bonus1 = 2 + (int)(this.Caster.Skills[SkillName.Mysticism].Value / 20);
                int bonus = 1 + (int)(this.Caster.Skills[SkillName.Focus].Value / 20);

                double span = 7.0 + (this.Caster.Skills[SkillName.Mysticism].Value * 0.4);

                // Mount
                IMount mount = this.Caster.Mount;

                if (mount != null)
                    mount.Rider = null;

                // Resists
                List<ResistanceMod> mods = new List<ResistanceMod>();
                mods.Add(new ResistanceMod(ResistanceType.Physical, bonus1 + bonus));
                mods.Add(new ResistanceMod(ResistanceType.Fire, bonus1 + bonus));
                mods.Add(new ResistanceMod(ResistanceType.Cold, bonus1 + bonus));
                mods.Add(new ResistanceMod(ResistanceType.Poison, bonus1 + bonus));
                mods.Add(new ResistanceMod(ResistanceType.Energy, bonus1 + bonus));

                if (m_Table.ContainsKey(this.Caster))
                {
                    for (int i = 0; i < m_Table[this.Caster].Count; i++)
                        this.Caster.AddResistanceMod(mods[i]);
                }

                // Skill
                this.Caster.AddSkillMod(new TimedSkillMod(SkillName.MagicResist, true, -40, DateTime.UtcNow + TimeSpan.FromSeconds(span)));

                // Effects
                this.Caster.BodyMod = 705;
                this.Caster.PlaySound(0x65A);
                this.Caster.FixedParticles(0x3728, 1, 13, 9918, 92, 3, EffectLayer.Head);

                m_Table.Add(this.Caster, mods);
                new InternalTimer(this.Caster, TimeSpan.FromSeconds((int)span)).Start();

                object[] objs =
                {
                    AosAttribute.CastSpeed, -2,
                    AosAttribute.WeaponSpeed, -10
                };
                new EnhancementTimer(this.Caster, 27, "Stone Form", objs).Start();

                this.Caster.BeginAction(typeof(StoneFormSpell));
            }

            this.FinishSequence();
        }

        private class InternalTimer : Timer
        {
            private readonly Mobile m_Owner;
            private readonly DateTime m_Expire;
            public InternalTimer(Mobile owner, TimeSpan duration)
                : base(TimeSpan.Zero, TimeSpan.FromSeconds(0.1))
            {
                this.m_Owner = owner;
                this.m_Expire = DateTime.UtcNow + duration;
            }

            protected override void OnTick()
            {
                if (DateTime.UtcNow >= this.m_Expire)
                {
                    StoneFormSpell.RemoveEffect(this.m_Owner);
                    this.Stop();
                }
            }
        }
    }
}