using System;
using System.Collections;

namespace Server.Spells.Second
{
    public class ProtectionSpell : MagerySpell
    {
        private static readonly Hashtable m_Registry = new Hashtable();
        private static readonly SpellInfo m_Info = new SpellInfo(
            "Protection", "Uus Sanct",
            236,
            9011,
            Reagent.Garlic,
            Reagent.Ginseng,
            Reagent.SulfurousAsh);
        private static readonly Hashtable m_Table = new Hashtable();
        public ProtectionSpell(Mobile caster, Item scroll)
            : base(caster, scroll, m_Info)
        {
        }

        public static Hashtable Registry
        {
            get
            {
                return m_Registry;
            }
        }
        public override SpellCircle Circle
        {
            get
            {
                return SpellCircle.Second;
            }
        }
        public static void Toggle(Mobile caster, Mobile target)
        {
            /* Players under the protection spell effect can no longer have their spells "disrupted" when hit.
            * Players under the protection spell have decreased physical resistance stat value (-15 + (Inscription/20),
            * a decreased "resisting spells" skill value by -35 + (Inscription/20),
            * and a slower casting speed modifier (technically, a negative "faster cast speed") of 2 points.
            * The protection spell has an indefinite duration, becoming active when cast, and deactivated when re-cast.
            * Reactive Armor, Protection, and Magic Reflection will stay on—even after logging out,
            * even after dying—until you “turn them off” by casting them again.
            */
            object[] mods = (object[])m_Table[target];

            if (mods == null)
            {
                target.PlaySound(0x1E9);
                target.FixedParticles(0x375A, 9, 20, 5016, EffectLayer.Waist);

                mods = new object[2]
                {
                    new ResistanceMod(ResistanceType.Physical, -15 + Math.Min((int)(caster.Skills[SkillName.Inscribe].Value / 20), 15)),
                    new DefaultSkillMod(SkillName.MagicResist, true, -35 + Math.Min((int)(caster.Skills[SkillName.Inscribe].Value / 20), 35))
                };

                m_Table[target] = mods;
                Registry[target] = 100.0;

                target.AddResistanceMod((ResistanceMod)mods[0]);
                target.AddSkillMod((SkillMod)mods[1]);

                int physloss = -15 + (int)(caster.Skills[SkillName.Inscribe].Value / 20);
                int resistloss = -35 + (int)(caster.Skills[SkillName.Inscribe].Value / 20);
                string args = String.Format("{0}\t{1}", physloss, resistloss);
                BuffInfo.AddBuff(target, new BuffInfo(BuffIcon.Protection, 1075814, 1075815, args.ToString()));
            }
            else
            {
                target.PlaySound(0x1ED);
                target.FixedParticles(0x375A, 9, 20, 5016, EffectLayer.Waist);

                m_Table.Remove(target);
                Registry.Remove(target);

                target.RemoveResistanceMod((ResistanceMod)mods[0]);
                target.RemoveSkillMod((SkillMod)mods[1]);

                BuffInfo.RemoveBuff(target, BuffIcon.Protection);
            }
        }

        public static void EndProtection(Mobile m)
        {
            if (m_Table.Contains(m))
            {
                object[] mods = (object[])m_Table[m];

                m_Table.Remove(m);
                Registry.Remove(m);

                m.RemoveResistanceMod((ResistanceMod)mods[0]);
                m.RemoveSkillMod((SkillMod)mods[1]);

                BuffInfo.RemoveBuff(m, BuffIcon.Protection);
            }
        }

        public override bool CheckCast()
        {
            if (Core.AOS)
                return true;

            if (m_Registry.ContainsKey(this.Caster))
            {
                this.Caster.SendLocalizedMessage(1005559); // This spell is already in effect.
                return false;
            }
            else if (!this.Caster.CanBeginAction(typeof(DefensiveSpell)))
            {
                this.Caster.SendLocalizedMessage(1005385); // The spell will not adhere to you at this time.
                return false;
            }

            return true;
        }

        public override void OnCast()
        {
            if (Core.AOS)
            {
                if (this.CheckSequence())
                    Toggle(this.Caster, this.Caster);

                this.FinishSequence();
            }
            else
            {
                if (m_Registry.ContainsKey(this.Caster))
                {
                    this.Caster.SendLocalizedMessage(1005559); // This spell is already in effect.
                }
                else if (!this.Caster.CanBeginAction(typeof(DefensiveSpell)))
                {
                    this.Caster.SendLocalizedMessage(1005385); // The spell will not adhere to you at this time.
                }
                else if (this.CheckSequence())
                {
                    if (this.Caster.BeginAction(typeof(DefensiveSpell)))
                    {
                        double value = (int)(this.Caster.Skills[SkillName.EvalInt].Value + this.Caster.Skills[SkillName.Meditation].Value + this.Caster.Skills[SkillName.Inscribe].Value);
                        value /= 4;

                        if (value < 0)
                            value = 0;
                        else if (value > 75)
                            value = 75.0;

                        Registry.Add(this.Caster, value);
                        new InternalTimer(this.Caster).Start();

                        this.Caster.FixedParticles(0x375A, 9, 20, 5016, EffectLayer.Waist);
                        this.Caster.PlaySound(0x1ED);
                    }
                    else
                    {
                        this.Caster.SendLocalizedMessage(1005385); // The spell will not adhere to you at this time.
                    }
                }

                this.FinishSequence();
            }
        }

        #region SA
        public static bool HasProtection(Mobile m)
        {
            return m_Table.ContainsKey(m);
        }
        #endregion

        private class InternalTimer : Timer
        {
            private readonly Mobile m_Caster;
            public InternalTimer(Mobile caster)
                : base(TimeSpan.FromSeconds(0))
            {
                double val = caster.Skills[SkillName.Magery].Value * 2.0;
                if (val < 15)
                    val = 15;
                else if (val > 240)
                    val = 240;

                this.m_Caster = caster;
                this.Delay = TimeSpan.FromSeconds(val);
                this.Priority = TimerPriority.OneSecond;
            }

            protected override void OnTick()
            {
                ProtectionSpell.Registry.Remove(this.m_Caster);
                DefensiveSpell.Nullify(this.m_Caster);
            }
        }
    }
}