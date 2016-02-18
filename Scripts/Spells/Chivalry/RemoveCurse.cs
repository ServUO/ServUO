using System;
using Server.Items;
using Server.Spells.Fourth;
using Server.Spells.Necromancy;
using Server.Targeting;

namespace Server.Spells.Chivalry
{
    public class RemoveCurseSpell : PaladinSpell
    {
        private static readonly SpellInfo m_Info = new SpellInfo(
            "Remove Curse", "Extermo Vomica",
            -1,
            9002);
        public RemoveCurseSpell(Mobile caster, Item scroll)
            : base(caster, scroll, m_Info)
        {
        }

        public override TimeSpan CastDelayBase
        {
            get
            {
                return TimeSpan.FromSeconds(1.5);
            }
        }
        public override double RequiredSkill
        {
            get
            {
                return 5.0;
            }
        }
        public override int RequiredMana
        {
            get
            {
                return 20;
            }
        }
        public override int RequiredTithing
        {
            get
            {
                return 10;
            }
        }
        public override int MantraNumber
        {
            get
            {
                return 1060726;
            }
        }// Extermo Vomica
        public override bool CheckCast()
        {
            if (Engines.ConPVP.DuelContext.CheckSuddenDeath(this.Caster))
            {
                this.Caster.SendMessage(0x22, "You cannot cast this spell when in sudden death.");
                return false;
            }

            return base.CheckCast();
        }

        public override void OnCast()
        {
            this.Caster.Target = new InternalTarget(this);
        }

        public void Target(Mobile m)
        {
            if (this.CheckBSequence(m))
            {
                SpellHelper.Turn(this.Caster, m);

                /* Attempts to remove all Curse effects from Target.
                * Curses include Mage spells such as Clumsy, Weaken, Feeblemind and Paralyze
                * as well as all Necromancer curses.
                * Chance of removing curse is affected by Caster's Karma.
                */

                int chance = 0;

                if (this.Caster.Karma < -5000)
                    chance = 0;
                else if (this.Caster.Karma < 0)
                    chance = (int)Math.Sqrt(20000 + this.Caster.Karma) - 122;
                else if (this.Caster.Karma < 5625)
                    chance = (int)Math.Sqrt(this.Caster.Karma) + 25;
                else
                    chance = 100;

                if (chance > Utility.Random(100))
                {
                    m.PlaySound(0xF6);
                    m.PlaySound(0x1F7);
                    m.FixedParticles(0x3709, 1, 30, 9963, 13, 3, EffectLayer.Head);

                    IEntity from = new Entity(Serial.Zero, new Point3D(m.X, m.Y, m.Z - 10), this.Caster.Map);
                    IEntity to = new Entity(Serial.Zero, new Point3D(m.X, m.Y, m.Z + 50), this.Caster.Map);
                    Effects.SendMovingParticles(from, to, 0x2255, 1, 0, false, false, 13, 3, 9501, 1, 0, EffectLayer.Head, 0x100);

                    m.RemoveStatMod("[Magic] Str Curse");
					m.RemoveStatMod("[Magic] Dex Curse");
					m.RemoveStatMod("[Magic] Int Curse");

                    m.Paralyzed = false;

                    EvilOmenSpell.TryEndEffect(m);
                    StrangleSpell.RemoveCurse(m);
                    CorpseSkinSpell.RemoveCurse(m);
                    CurseSpell.RemoveEffect(m);
                    MortalStrike.EndWound(m);
                    if (Core.ML)
                    {
                        BloodOathSpell.RemoveCurse(m);
                    }
                    MindRotSpell.ClearMindRotScalar(m);

                    BuffInfo.RemoveBuff(m, BuffIcon.Clumsy);
                    BuffInfo.RemoveBuff(m, BuffIcon.FeebleMind);
                    BuffInfo.RemoveBuff(m, BuffIcon.Weaken);
                    BuffInfo.RemoveBuff(m, BuffIcon.Curse);
                    BuffInfo.RemoveBuff(m, BuffIcon.MassCurse);
                    BuffInfo.RemoveBuff(m, BuffIcon.MortalStrike);
                    BuffInfo.RemoveBuff(m, BuffIcon.Mindrot);
                    // TODO: Should this remove blood oath? Pain spike?
                }
                else
                {
                    m.PlaySound(0x1DF);
                }
            }

            this.FinishSequence();
        }

        private class InternalTarget : Target
        {
            private readonly RemoveCurseSpell m_Owner;
            public InternalTarget(RemoveCurseSpell owner)
                : base(Core.ML ? 10 : 12, false, TargetFlags.Beneficial)
            {
                this.m_Owner = owner;
            }

            protected override void OnTarget(Mobile from, object o)
            {
                if (o is Mobile)
                    this.m_Owner.Target((Mobile)o);
            }

            protected override void OnTargetFinish(Mobile from)
            {
                this.m_Owner.FinishSequence();
            }
        }
    }
}