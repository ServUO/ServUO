using Server.Items;
using Server.Mobiles;
using Server.Spells.Necromancy;
using System;
using System.Linq;

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

        public override TimeSpan CastDelayBase => TimeSpan.FromSeconds(0.25);
        public override double RequiredSkill => 35.0;
        public override int RequiredMana => 10;
        public override int RequiredTithing => 10;
        public override int MantraNumber => 1060721;// Dispiro Malas
        public override bool BlocksMovement => false;
        public override bool DelayedDamage => false;
        public override void SendCastEffect()
        {
            Caster.FixedEffect(0x37C4, 10, 7, 4, 3); // At player
        }

        public override void OnCast()
        {
            if (CheckSequence())
            {
                Caster.PlaySound(0xF5);
                Caster.PlaySound(0x299);
                Caster.FixedParticles(0x37C4, 1, 25, 9922, 14, 3, EffectLayer.Head);

                int dispelSkill = ComputePowerValue(2);
                double chiv = Caster.Skills.Chivalry.Value;

                foreach (Mobile m in AcquireIndirectTargets(Caster.Location, 8).OfType<Mobile>())
                {
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
                                bc.ForceFleeUntil = DateTime.UtcNow + TimeSpan.FromSeconds(30.0);
                            }
                        }
                    }

                    TransformContext context = TransformationSpellHelper.GetContext(m);
                    if (context != null && context.Spell is NecromancerSpell)	//Trees are not evil!	TODO: OSI confirm?
                    {
                        // transformed ..
                        double drainChance = 0.5 * (Caster.Skills.Chivalry.Value / Math.Max(m.Skills.Necromancy.Value, 1));

                        if (drainChance > Utility.RandomDouble())
                        {
                            int drain = (5 * dispelSkill) / 100;

                            m.Stam -= drain;
                            m.Mana -= drain;
                        }
                    }
                }
            }

            FinishSequence();
        }
    }
}
