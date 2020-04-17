using Server.Items;
using Server.Spells.First;
using Server.Spells.Fourth;
using Server.Spells.Mysticism;
using Server.Spells.Necromancy;
using Server.Targeting;
using System;

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

        public override TimeSpan CastDelayBase => TimeSpan.FromSeconds(2.0);

        public override double RequiredSkill => 5.0;

        public override int RequiredMana => 20;

        public override int RequiredTithing => 10;

        public override int MantraNumber => 1060726; // Extermo Vomica

        public override bool CheckDisturb(DisturbType type, bool firstCircle, bool resistable)
        {
            return true;
        }

        public override void OnCast()
        {
            Caster.Target = new InternalTarget(this);
        }

        public void Target(Mobile m)
        {
            if (CheckBSequence(m))
            {
                SpellHelper.Turn(Caster, m);

                /* Attempts to remove all Curse effects from Target.
                * Curses include Mage spells such as Clumsy, Weaken, Feeblemind and Paralyze
                * as well as all Necromancer curses.
                * Chance of removing curse is affected by Caster's Karma.
                */

                int chance = 0;

                if (Caster.Karma < -5000)
                    chance = 0;
                else if (Caster.Karma < 0)
                    chance = (int)Math.Sqrt(20000 + Caster.Karma) - 122;
                else if (Caster.Karma < 5625)
                    chance = (int)Math.Sqrt(Caster.Karma) + 25;
                else
                    chance = 100;

                if (chance > Utility.Random(100))
                {
                    m.PlaySound(0xF6);
                    m.PlaySound(0x1F7);
                    m.FixedParticles(0x3709, 1, 30, 9963, 13, 3, EffectLayer.Head);

                    IEntity from = new Entity(Serial.Zero, new Point3D(m.X, m.Y, m.Z - 10), Caster.Map);
                    IEntity to = new Entity(Serial.Zero, new Point3D(m.X, m.Y, m.Z + 50), Caster.Map);
                    Effects.SendMovingParticles(from, to, 0x2255, 1, 0, false, false, 13, 3, 9501, 1, 0, EffectLayer.Head, 0x100);

                    m.Paralyzed = false;

                    EvilOmenSpell.TryEndEffect(m);
                    StrangleSpell.RemoveCurse(m);
                    CorpseSkinSpell.RemoveCurse(m);
                    CurseSpell.RemoveEffect(m);
                    MortalStrike.EndWound(m);
                    WeakenSpell.RemoveEffects(m);
                    FeeblemindSpell.RemoveEffects(m);
                    ClumsySpell.RemoveEffects(m);

                    BloodOathSpell.RemoveCurse(m);

                    MindRotSpell.ClearMindRotScalar(m);
                    SpellPlagueSpell.RemoveFromList(m);

                    BuffInfo.RemoveBuff(m, BuffIcon.MassCurse);
                }
                else
                {
                    m.PlaySound(0x1DF);
                }
            }

            FinishSequence();
        }

        private class InternalTarget : Target
        {
            private readonly RemoveCurseSpell m_Owner;
            public InternalTarget(RemoveCurseSpell owner)
                : base(10, false, TargetFlags.Beneficial)
            {
                m_Owner = owner;
            }

            protected override void OnTarget(Mobile from, object o)
            {
                if (o is Mobile)
                    m_Owner.Target((Mobile)o);
            }

            protected override void OnTargetFinish(Mobile from)
            {
                m_Owner.FinishSequence();
            }
        }
    }
}