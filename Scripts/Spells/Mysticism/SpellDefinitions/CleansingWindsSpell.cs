using System;
using System.Collections.Generic;
using Server.Items;
using Server.Mobiles;
using Server.Spells.Fourth;
using Server.Spells.Necromancy;
using Server.Targeting;
using Server.Engines.PartySystem;

namespace Server.Spells.Mysticism
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
                        int level = m.Poison.RealLevel + 1;
                        int chanceToCure = (10000 + (int)(((prim + sec) / 2) * 75) - (level * 1750)) / 100;

                        if (chanceToCure > Utility.Random(100) && m.CurePoison(Caster))
                            toHealMod /= level;
                        else
                            toHealMod = 0;
                    }

                    if (MortalStrike.IsWounded(m))
                    {
                        MortalStrike.EndWound(m);
                        toHealMod = 0;
                    }

                    int curselevel = 0;

                    if (SleepSpell.IsUnderSleepEffects(m))
                    {
                        SleepSpell.EndSleep(m);
                        curselevel += 2;
                    }

                    if (EvilOmenSpell.TryEndEffect(m))
                    {
                        curselevel += 1;
                    }

                    if (StrangleSpell.RemoveCurse(m))
                    {
                        curselevel += 2;
                    }

                    if (CorpseSkinSpell.RemoveCurse(m))
                    {
                        curselevel += 3;
                    }

                    if (CurseSpell.UnderEffect(m))
                    {
                        CurseSpell.RemoveEffect(m);
                        curselevel += 4;
                    }

                    if (BloodOathSpell.RemoveCurse(m))
                    {
                        curselevel += 3;
                    }

                    if (MindRotSpell.HasMindRotScalar(m))
                    {
                        MindRotSpell.ClearMindRotScalar(m);
                        curselevel += 2;
                    }

                    if (SpellPlagueSpell.HasSpellPlague(m))
                    {
                        SpellPlagueSpell.RemoveFromList(m);
                        curselevel += 4;
                    }

                    if (m.GetStatMod("[Magic] Str Curse") != null)
                    {
                        m.RemoveStatMod("[Magic] Str Curse");
                        curselevel += 1;
                    }

                    if (m.GetStatMod("[Magic] Dex Curse") != null)
                    {
                        m.RemoveStatMod("[Magic] Dex Curse");
                        curselevel += 1;
                    }

                    if (m.GetStatMod("[Magic] Int Curse") != null)
                    {
                        m.RemoveStatMod("[Magic] Int Curse");
                        curselevel += 1;
                    }

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

                    if (toHealMod > 0 && curselevel > 0)
                    {
                        int toHealMod1 = toHealMod - (curselevel * 3);
                        int toHealMod2 = toHealMod - (int)((double)toHealMod * ((double)curselevel / 100));

                        toHealMod -= toHealMod1 + toHealMod2;
                    }

                    if (toHealMod > 0)
                        SpellHelper.Heal(toHealMod + Utility.RandomMinMax(1, 6), m, Caster);
                }
            }

            FinishSequence();
		}
	}
}