using System;
using System.Collections.Generic;
using Server.Items;
using Server.Targeting;

namespace Server.Spells.Mystic
{
	public class NetherCycloneSpell : MysticSpell
	{
        public override SpellCircle Circle { get { return SpellCircle.Eighth; } }

		private static SpellInfo m_Info = new SpellInfo(
				"Nether Cyclone", "Grav Hur",
				230,
				9022,
				Reagent.MandrakeRoot,
				Reagent.Nightshade,
				Reagent.SulfurousAsh,
				Reagent.Bloodmoss
			);

		public NetherCycloneSpell( Mobile caster, Item scroll ) : base( caster, scroll, m_Info )
		{
		}

		public override void OnCast()
		{
			Caster.Target = new MysticSpellTarget( this, TargetFlags.Harmful );
		}

		public override void OnTarget( Object o )
		{
			Mobile target = o as Mobile;

			if ( target == null )
			{
				return;
			}
			else if ( CheckHSequence( target ) )
			{

				Map map = Caster.Map;

				if ( map != null )
				{
					List<Mobile> targets = new List<Mobile>();

                    IPooledEnumerable eable = target.GetMobilesInRange(3);
                    foreach (Mobile m in eable)
                    {
                        if (Caster != m && target.InLOS(m) && SpellHelper.ValidIndirectTarget(Caster, m) && Caster.CanBeHarmful(m, false))
                            targets.Add(m);
                    }
                    eable.Free();

					for ( int i = 0; i < targets.Count; ++i )
					{
						Mobile m = targets[i];

						m.FixedParticles( 0x374A, 1, 15, 9502, 97, 3, (EffectLayer)255 );

                        double damage = (((Caster.Skills[CastSkill].Value + (Caster.Skills[DamageSkill].Value / 2)) * .66) + Utility.RandomMinMax(1, 6));

                        SpellHelper.Damage(this, m, damage, 0, 0, 0, 0, 0, 100, 0);

                        double stamSap = (damage / 3);
                        double manaSap = (damage / 3);
                        double mod = m.Skills[SkillName.MagicResist].Value - ((Caster.Skills[CastSkill].Value + Caster.Skills[DamageSkill].Value) / 2);

                        if(mod > 0)
                        {
                            mod /= 100;

                            stamSap *= mod;
                            manaSap *= mod;
                        }

                        m.Stam -= (int)stamSap;
                        m.Mana -= (int)manaSap;

                        Timer.DelayCall(TimeSpan.FromSeconds(10), () =>
                        {
                            if (m.Alive)
                            {
                                m.Stam += (int)stamSap;
                                m.Mana += (int)manaSap;
                            }
                        });

                        Effects.PlaySound(target.Location, map, 0x654);
                        Effects.PlaySound(target.Location, map, 0x654);
                        Effects.SendLocationParticles(EffectItem.Create(target.Location, map, EffectItem.DefaultDuration), 0x37CC, 1, 40, 97, 3, 9917, 0);
					}
				}
			}

			FinishSequence();
		}
	}
}