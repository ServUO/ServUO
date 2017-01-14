using System;
using System.Collections;
using System.Collections.Generic;
using Server.Network;
using Server.Items;
using Server.Targeting;
using Server.Mobiles;

namespace Server.Spells.Mystic
{
	public class StoneFormSpell : MysticTransformationSpell
	{
		private static HashSet<Mobile> m_Effected = new HashSet<Mobile>();
		public static bool IsEffected(Mobile m)
		{
			return m_Effected.Contains(m);
		}

		private static SpellInfo m_Info = new SpellInfo(
				"Stone Form", "In Rel Ylem",
				230,
				9022,
				Reagent.Bloodmoss,
				Reagent.FertileDirt,
				Reagent.Garlic
			);

        private int m_ResisMod;

        public override SpellCircle Circle { get { return SpellCircle.Fourth; } }

		public override TimeSpan CastDelayBase { get { return TimeSpan.FromSeconds( 2.0 ); } }

		public override int Body{ get{ return 705; } }
        public override int PhysResistOffset { get { return m_ResisMod; } }
        public override int FireResistOffset { get { return m_ResisMod; } }
        public override int ColdResistOffset { get { return m_ResisMod; } }
        public override int PoisResistOffset { get { return m_ResisMod; } }
        public override int NrgyResistOffset { get { return m_ResisMod; } }

		public StoneFormSpell( Mobile caster, Item scroll ) : base( caster, scroll, m_Info )
		{
		}

        public override bool CheckCast()
        {
            bool doCast = base.CheckCast();
			if (doCast && Caster.Flying)
			{
				Caster.SendLocalizedMessage(1112567); // You are flying.
				doCast = false;
			}

            if(doCast)
                m_ResisMod = (int)(Caster.Skills[CastSkill].Value + Caster.Skills[DamageSkill].Value) / 24;

            return doCast;
        }

		public override void DoEffect( Mobile m )
		{
			m.PlaySound( 0x65A );
			m.FixedParticles( 0x3728, 1, 13, 9918, 92, 3, EffectLayer.Head );

            Timer.DelayCall(new TimerCallback(MobileDelta_Callback));
			m_Effected.Add(m);

            string args = String.Format("{0}\t{1}\t{2}\t{3}\t{4}", "-10", "-2", m_ResisMod.ToString(), m_ResisMod.ToString(), "-10");
            BuffInfo.AddBuff(m, new BuffInfo(BuffIcon.StoneForm, 1080145, 1080146, args));
		}

        public void MobileDelta_Callback()
        {
            Caster.Delta( MobileDelta.WeaponDamage );
        }

		public override void RemoveEffect( Mobile m )
		{
			m.Delta( MobileDelta.WeaponDamage );
			m_Effected.Remove(m);
            BuffInfo.RemoveBuff(m, BuffIcon.StoneForm);
		}

        public static int GetMaxResistMod(PlayerMobile pm)
        {
            if (TransformationSpellHelper.UnderTransformation(pm, typeof(Spells.Mystic.StoneFormSpell)))
            {
                int prim = (int)pm.Skills[SkillName.Mysticism].Value;
                int sec = (int)pm.Skills[SkillName.Imbuing].Value;

                if (pm.Skills[SkillName.Focus].Value > sec)
                    sec = (int)pm.Skills[SkillName.Focus].Value;

                return Math.Max(2, (prim + sec) / 48);
            }
            return 0;
        }

        public static bool CheckImmunity(Mobile from)
        {
            if (TransformationSpellHelper.UnderTransformation(from, typeof(Spells.Mystic.StoneFormSpell)))
            {
                int prim = (int)from.Skills[SkillName.Mysticism].Value;
                int sec = (int)from.Skills[SkillName.Imbuing].Value;
                if (from.Skills[SkillName.Focus].Value > sec)
                    sec = (int)from.Skills[SkillName.Focus].Value;

                int immunity = ((prim + sec) / 240) * 100;

                if (Server.Spells.Necromancy.EvilOmenSpell.TryEndEffect(from))
                    immunity -= 30;

                return immunity > Utility.Random(100);
            }

            return false;
        }
	}
}