using System;
using System.Collections.Generic;
using Server.Items;
using Server.Mobiles;
using Server.Spells.Fourth;
using Server.Spells.Necromancy;
using Server.Targeting;
using Server.Engines.PartySystem;

namespace Server.Spells.Mystic
{
	public class CleansingWindsSpell : MysticSpell
	{
        public override SpellCircle Circle { get { return SpellCircle.Sixth; } }

		private static SpellInfo m_Info = new SpellInfo(
				"Cleansing Winds", "In Vas Mani Hur",
				230,
				9022,
				Reagent.Garlic,
				Reagent.Ginseng,
				Reagent.MandrakeRoot,
				Reagent.DragonBlood
			);

		public CleansingWindsSpell( Mobile caster, Item scroll ) : base( caster, scroll, m_Info )
		{
		}

		public override void OnCast()
		{
			Caster.Target = new MysticSpellTarget( this, TargetFlags.Beneficial );
		}

		public override void OnTarget( Object o )
		{
			IPoint3D p = o as IPoint3D;

			if ( p == null )
				return;

            if (CheckSequence())
            {
                List<Mobile> targets = new List<Mobile>();
                Party party = Party.Get(Caster);

                double prim = Caster.Skills[CastSkill].Value;
                double sec = Caster.Skills[DamageSkill].Value;

                IPooledEnumerable eable = Caster.Map.GetMobilesInRange(new Point3D(p), 3);
                foreach (Mobile mob in eable)
                {
                    if (mob == null)
                        continue;

                    if (mob == Caster)
                        targets.Add(mob);

                    if (Caster.CanBeBeneficial(mob, false) && party != null && party.Contains(mob))
                        targets.Add(mob);
                }
                eable.Free();

                Mobile m;
                int toheal = (int)(((prim + sec) / 2) * 0.3) - 6;
                Caster.PlaySound(0x64C);

                for (int i = 0; i < targets.Count; i++)
                {
                    m = targets[i];
                    int toHealMod = toheal;

                    if (!m.Alive)
                        continue;

                    if (m.Poisoned)
                    {
                        int chanceToCure = (10000 + (int)(((prim + sec) / 2) * 75) - ((m.Poison.RealLevel + 1) * 1750)) / 100;

                        if (chanceToCure > Utility.Random(100) && m.CurePoison(Caster))
                            toHealMod /= 3;
                        else
                            toHealMod = 0;
                    }

                    if (MortalStrike.IsWounded(m))
                    {
                        MortalStrike.EndWound(m);
                        toHealMod = 0;
                    }

                    if (toHealMod > 0)
                        m.Heal(toHealMod + Utility.RandomMinMax(1, 6));

                    m.RemoveStatMod("[Magic] Str Curse");
					m.RemoveStatMod("[Magic] Dex Curse");
					m.RemoveStatMod("[Magic] Int Curse");

                    SleepSpell.EndSleep(m);
                    EvilOmenSpell.TryEndEffect(m);
                    StrangleSpell.RemoveCurse(m);
                    CorpseSkinSpell.RemoveCurse(m);
                    CurseSpell.RemoveEffect(m);
                    BloodOathSpell.RemoveCurse(m);
                    MindRotSpell.ClearMindRotScalar(m);

                    BuffInfo.RemoveBuff(m, BuffIcon.Clumsy);
                    BuffInfo.RemoveBuff(m, BuffIcon.FeebleMind);
                    BuffInfo.RemoveBuff(m, BuffIcon.Weaken);
                    BuffInfo.RemoveBuff(m, BuffIcon.Curse);
                    BuffInfo.RemoveBuff(m, BuffIcon.MassCurse);
                    BuffInfo.RemoveBuff( m, BuffIcon.MortalStrike );
                    BuffInfo.RemoveBuff ( m, BuffIcon.Mindrot );
                    BuffInfo.RemoveBuff( m, BuffIcon.CorpseSkin );
                    BuffInfo.RemoveBuff( m, BuffIcon.Strangle );
                    BuffInfo.RemoveBuff( m, BuffIcon.EvilOmen );

                    //TODO: Message/Effects???
                }
            }

            FinishSequence();
		}
	}
}