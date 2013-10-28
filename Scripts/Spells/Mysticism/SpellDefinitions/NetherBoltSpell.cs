using System;
using Server.Targeting;

namespace Server.Spells.Mystic
{
    public class NetherBoltSpell : MysticSpell
    {
        private static readonly SpellInfo m_Info = new SpellInfo(
            "Nether Bolt", "In Corp Ylem",
            230,
            9022,
            Reagent.BlackPearl,
            Reagent.SulfurousAsh);
        public NetherBoltSpell(Mobile caster, Item scroll)
            : base(caster, scroll, m_Info)
        {
        }

        // Fires a bolt of nether energy at the Target, dealing chaos damage.
        public override int RequiredMana
        {
            get
            {
                return 4;
            }
        }
        public override double RequiredSkill
        {
            get
            {
                return 0;
            }
        }
        public override bool DelayedDamage
        {
            get
            {
                return true;
            }
        }
        public override void OnCast()
        {
            this.Caster.Target = new MysticSpellTarget(this, TargetFlags.Harmful);
        }

        public override void OnTarget(Object o)
        {
            Mobile target = o as Mobile;

            if (target == null)
            {
                return;
            }
            else if (this.CheckHSequence(target))
            {
                double damage = this.GetNewAosDamage(10, 1, 4, target);
                int hue = 0;

                switch( Utility.Random(5) )
                {
                    case 0:
                        {
                            SpellHelper.Damage(this, target, damage, 100, 0, 0, 0, 0);
                            hue = 1908;
                        }
                        break;
                    case 1:
                        {
                            SpellHelper.Damage(this, target, damage, 0, 100, 0, 0, 0);
                            hue = 1355;
                        }
                        break;
                    case 2:
                        {
                            SpellHelper.Damage(this, target, damage, 0, 0, 100, 0, 0);
                            hue = 1361;
                        }
                        break;
                    case 3:
                        {
                            SpellHelper.Damage(this, target, damage, 0, 0, 0, 100, 0);
                            hue = 1367;
                        }
                        break;
                    default:
                        {
                            SpellHelper.Damage(this, target, damage, 0, 0, 0, 0, 100);
                            hue = 1373;
                        }
                        break;
                }

                target.BoltEffect(hue);
                this.Caster.PlaySound(0x654);
            }

            this.FinishSequence();
        }
    }
}