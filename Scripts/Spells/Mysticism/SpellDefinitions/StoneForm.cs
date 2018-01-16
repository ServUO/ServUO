using System;
using System.Collections.Generic;

namespace Server.Spells.Mysticism
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

            if (doCast)
                m_ResisMod = GetResBonus(Caster);

            return doCast;
        }

		public override void DoEffect( Mobile m )
		{
			m.PlaySound( 0x65A );
			m.FixedParticles( 0x3728, 1, 13, 9918, 92, 3, EffectLayer.Head );

            Timer.DelayCall(new TimerCallback(MobileDelta_Callback));
			m_Effected.Add(m);

            string args = String.Format("{0}\t{1}\t{2}\t{3}\t{4}", "-10", "-2", GetResBonus(m).ToString(), GetMaxResistance(m).ToString(), GetDamBonus(m).ToString());
            BuffInfo.AddBuff(m, new BuffInfo(BuffIcon.StoneForm, 1080145, 1080146, args));
            BuffInfo.AddBuff(m, new BuffInfo(BuffIcon.PoisonImmunity, 1153785, 1153814));
		}

        public void MobileDelta_Callback()
        {
            Caster.Delta(MobileDelta.WeaponDamage);
            Caster.UpdateResistances();
        }

		public override void RemoveEffect( Mobile m )
		{
			m.Delta( MobileDelta.WeaponDamage );
			m_Effected.Remove(m);
            BuffInfo.RemoveBuff(m, BuffIcon.StoneForm);
            BuffInfo.RemoveBuff(m, BuffIcon.PoisonImmunity);
		}

        public static int GetMaxResistBonus(Mobile m)
        {
            if (TransformationSpellHelper.UnderTransformation(m, typeof(StoneFormSpell)))
            {
                return GetMaxResistance(m);
            }

            return 0;
        }

        public static int GetResistanceBonus(Mobile m)
        {
            if (TransformationSpellHelper.UnderTransformation(m, typeof(StoneFormSpell)))
            {
                return GetResBonus(m);
            }

            return 0;
        }

        public static int GetDamageBonus(Mobile m)
        {
            if (TransformationSpellHelper.UnderTransformation(m, typeof(StoneFormSpell)))
            {
                return GetDamBonus(m);
            }

            return 0;
        }

        public static double StatOffsetReduction(Mobile caster, Mobile m)
        {
            if (TransformationSpellHelper.UnderTransformation(m, typeof(StoneFormSpell)))
            {
                int prim = (int)m.Skills[SkillName.Mysticism].Value;
                int sec = (int)m.Skills[SkillName.Imbuing].Value;

                if (m.Skills[SkillName.Focus].Value > sec)
                    sec = (int)m.Skills[SkillName.Focus].Value;

                caster.SendLocalizedMessage(1080192); // Your target resists your ability reduction magic.

                return (double)(prim + sec) / 480;
            }

            return 1.0;
        }

        private static int GetResBonus(Mobile m)
        {
            int prim = (int)m.Skills[SkillName.Mysticism].Value;
            int sec = (int)m.Skills[SkillName.Imbuing].Value;

            if (m.Skills[SkillName.Focus].Value > sec)
                sec = (int)m.Skills[SkillName.Focus].Value;

            return Math.Max(2, (prim + sec) / 24);
        }

        private static int GetMaxResistance(Mobile m)
        {
            int prim = (int)m.Skills[SkillName.Mysticism].Value;
            int sec = (int)m.Skills[SkillName.Imbuing].Value;

            if (m.Skills[SkillName.Focus].Value > sec)
                sec = (int)m.Skills[SkillName.Focus].Value;

            return Math.Max(2, (prim + sec) / 48);
        }

        private static int GetDamBonus(Mobile m)
        {
            int prim = (int)m.Skills[SkillName.Mysticism].Value;
            int sec = (int)m.Skills[SkillName.Imbuing].Value;

            if (m.Skills[SkillName.Focus].Value > sec)
                sec = (int)m.Skills[SkillName.Focus].Value;

            return Math.Max(0, (prim + sec) / 12);
        }

        public static bool CheckImmunity(Mobile from)
        {
            if (TransformationSpellHelper.UnderTransformation(from, typeof(StoneFormSpell)))
            {
                int prim = (int)from.Skills[SkillName.Mysticism].Value;
                int sec = (int)from.Skills[SkillName.Imbuing].Value;

                if (from.Skills[SkillName.Focus].Value > sec)
                    sec = (int)from.Skills[SkillName.Focus].Value;

                int immunity = (int)(((double)(prim + sec) / 480) * 100);

                if (Server.Spells.Necromancy.EvilOmenSpell.TryEndEffect(from))
                    immunity -= 30;

                return immunity > Utility.Random(100);
            }

            return false;
        }
	}
}
