using System;
using System.Collections.Generic;
using Server.Items;
using Server.Mobiles;
using Server.Spells.Necromancy;

namespace Server.Spells.Chivalry
{
    public class DispelEvilSpell : PaladinSpell
    {
        private static readonly SpellInfo m_Info = new SpellInfo(
            "Dispel Evil", "Dispiro Malas",
            -1,
            9002);
        public DispelEvilSpell(Mobile caster, Item scroll)
            : base(caster, scroll, m_Info)
        {
        }

        public override TimeSpan CastDelayBase
        {
            get
            {
                return TimeSpan.FromSeconds(0.25);
            }
        }
        public override double RequiredSkill
        {
            get
            {
                return 35.0;
            }
        }
        public override int RequiredMana
        {
            get
            {
                return 10;
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
                return 1060721;
            }
        }// Dispiro Malas
        public override bool BlocksMovement
        {
            get
            {
                return false;
            }
        }
        public override bool DelayedDamage
        {
            get
            {
                return false;
            }
        }
        public override void SendCastEffect()
        {
            this.Caster.FixedEffect(0x37C4, 10, 7, 4, 3); // At player
        }

        public override void OnCast()
        {
            if (this.CheckSequence())
            {
                List<Mobile> targets = new List<Mobile>();
                IPooledEnumerable eable = Caster.GetMobilesInRange(8);

                foreach (Mobile m in eable)
                {
                    if (this.Caster != m && SpellHelper.ValidIndirectTarget(this.Caster, m) && this.Caster.CanBeHarmful(m, false))
                        targets.Add(m);
                }
                eable.Free();
				
                this.Caster.PlaySound(0xF5);
                this.Caster.PlaySound(0x299);
                this.Caster.FixedParticles(0x37C4, 1, 25, 9922, 14, 3, EffectLayer.Head);

                int dispelSkill = this.ComputePowerValue(2);

                double chiv = this.Caster.Skills.Chivalry.Value;

                for (int i = 0; i < targets.Count; ++i)
                {
                    Mobile m = targets[i];
                    BaseCreature bc = m as BaseCreature;

                    if (bc != null)
                    {
                        bool dispellable = bc.Summoned && !bc.IsAnimatedDead;

                        if (dispellable)
                        {
                            double dispelChance = (50.0 + ((100 * (chiv - bc.GetDispelDifficulty())) / (bc.DispelFocus * 2))) / 100;
                            dispelChance *= dispelSkill / 100.0;

                            if (dispelChance > Utility.RandomDouble())
                            {
                                Effects.SendLocationParticles(EffectItem.Create(m.Location, m.Map, EffectItem.DefaultDuration), 0x3728, 8, 20, 5042);
                                Effects.PlaySound(m, m.Map, 0x201);

                                m.Delete();
                                continue;
                            }
                        }

                        bool evil = !bc.Controlled && bc.Karma < 0;

                        if (evil)
                        {
                            // TODO: Is this right?
                            double fleeChance = (100 - Math.Sqrt(m.Fame / 2)) * chiv * dispelSkill;
                            fleeChance /= 1000000;

                            if (fleeChance > Utility.RandomDouble())
                            {
                                // guide says 2 seconds, it's longer
                                bc.BeginFlee(TimeSpan.FromSeconds(30.0));
                            }
                        }
                    }

                    TransformContext context = TransformationSpellHelper.GetContext(m);
                    if (context != null && context.Spell is NecromancerSpell)	//Trees are not evil!	TODO: OSI confirm?
                    {
                        // transformed ..
                        double drainChance = 0.5 * (this.Caster.Skills.Chivalry.Value / Math.Max(m.Skills.Necromancy.Value, 1));

                        if (drainChance > Utility.RandomDouble())
                        {
                            int drain = (5 * dispelSkill) / 100;

                            m.Stam -= drain;
                            m.Mana -= drain;
                        }
                    }
                }
            }

            this.FinishSequence();
        }
    }
}