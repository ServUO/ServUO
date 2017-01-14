using System;
using Server;
using Server.Items;
using Server.Mobiles;
using Server.Spells;
using Server.Targeting;
using System.Collections.Generic;
using Server.Network;

namespace Server.Spells.Mystic
{
	public class MassSleepSpell : MysticSpell
	{
        public override SpellCircle Circle { get { return SpellCircle.Fifth; } }

		private static SpellInfo m_Info = new SpellInfo(
				"Mass Sleep", "Vas Zu",
				230,
				9022,
				Reagent.Ginseng,
				Reagent.Nightshade,
				Reagent.SpidersSilk
			);

		public MassSleepSpell( Mobile caster, Item scroll ) : base( caster, scroll, m_Info )
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
                        if (Caster != m && target.InLOS(m) && SpellHelper.ValidIndirectTarget(Caster, m) && Caster.CanBeHarmful(m, false) && !SleepSpell.IsUnderSleepEffects(m) && !m.Paralyzed)
                            targets.Add(m);
                    }
                    eable.Free();

					for ( int i = 0; i < targets.Count; ++i )
					{
						Mobile m = targets[i];

                        double duration = ((Caster.Skills[CastSkill].Value + Caster.Skills[DamageSkill].Value) / 20) + 3;
                        duration -= target.Skills[SkillName.MagicResist].Value / 10;

                        if (duration > 0)
                        {
                            Caster.DoHarmful(m);

                            SleepSpell.DoSleep(Caster, m, TimeSpan.FromSeconds(duration));
                        }
					}
				}
			}

			FinishSequence();
		}
	}
}
