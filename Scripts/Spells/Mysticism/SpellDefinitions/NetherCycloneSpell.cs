using System;
using System.Collections.Generic;
using Server.Items;
using Server.Targeting;

namespace Server.Spells.Mystic
{
    public class NetherCycloneSpell : MysticSpell
    {
        private static readonly SpellInfo m_Info = new SpellInfo(
            "Nether Cyclone", "Grav Hur",
            230,
            9022,
            Reagent.MandrakeRoot,
            Reagent.Nightshade,
            Reagent.SulfurousAsh,
            Reagent.Bloodmoss);
        public NetherCycloneSpell(Mobile caster, Item scroll)
            : base(caster, scroll, m_Info)
        {
        }

        // Hurls a magical boulder at the Target, dealing physical damage. 
        // This spell also has a chance to knockback and stun a player Target. 
        public override int RequiredMana
        {
            get
            {
                return 50;
            }
        }
        public override double RequiredSkill
        {
            get
            {
                return 83;
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
                Map map = this.Caster.Map;

                if (map != null)
                {
                    List<Mobile> targets = new List<Mobile>();

                    foreach (Mobile m in target.GetMobilesInRange(3))
                        if (this.Caster != m && target.InLOS(m) && SpellHelper.ValidIndirectTarget(this.Caster, m) && this.Caster.CanBeHarmful(m, false))
                            targets.Add(m);

                    Effects.PlaySound(target.Location, map, 0x655);
                    Effects.PlaySound(target.Location, map, 0x655);
                    Effects.SendLocationParticles(EffectItem.Create(target.Location, map, EffectItem.DefaultDuration), 0x37CC, 1, 40, 97, 3, 9917, 0);

                    for (int i = 0; i < targets.Count; ++i)
                    {
                        Mobile m = targets[i];

                        this.Caster.DoHarmful(m);
                        m.FixedParticles(0x374A, 1, 15, 9502, 97, 3, (EffectLayer)255);

                        double damage = Utility.RandomMinMax(30, 35);

                        //damage *= (300 + (m.Karma / 100) + (GetDamageSkill( Caster ) * 10));
                        //damage /= 1000;

                        // TODO: cap?
                        //if ( damage > 40 )
                        //	damage = 40;

                        SpellHelper.Damage(this, m, damage, 0, 0, 0, 0, 100);
                    }
                }
            }

            this.FinishSequence();
        }
    }
}
/*




*/