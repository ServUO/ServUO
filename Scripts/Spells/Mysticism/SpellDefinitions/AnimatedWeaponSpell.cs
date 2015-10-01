using System;
using Server.Mobiles;

namespace Server.Spells.Mystic
{
	public class AnimatedWeaponSpell : MysticSpell
	{
        public override SpellCircle Circle { get { return SpellCircle.Fourth; } }

		private static SpellInfo m_Info = new SpellInfo(
				"Animated Weapon", "In Jux Por Ylem",
				230,
				9022,
				Reagent.BlackPearl,
				Reagent.MandrakeRoot,
				Reagent.Nightshade,
				Reagent.Bone
			);

		public AnimatedWeaponSpell( Mobile caster, Item scroll ) : base( caster, scroll, m_Info )
		{
		}

		public override bool CheckCast()
		{
			if ( !base.CheckCast() )
				return false;

			if ( Caster.Followers + 4 > Caster.FollowersMax )
			{
				Caster.SendLocalizedMessage( 1049645 ); // You have too many followers to summon that creature.
				return false;
			}

			return true;
		}

		public override void OnCast()
		{
			if ( CheckSequence() )
			{
                TimeSpan duration = TimeSpan.FromSeconds((Caster.Skills[CastSkill].Fixed + Caster.Skills[DamageSkill].Fixed) / 5);

                AnimatedWeapon anim = new AnimatedWeapon();

                anim.Skills[SkillName.Anatomy].Base = Caster.Skills[SkillName.Anatomy].Base;
                anim.Skills[SkillName.Tactics].Base = Caster.Skills[SkillName.Tactics].Base;

				SpellHelper.Summon( anim, Caster, 0x216, duration, true, true, false, SkillName.Mysticism );
			}

			FinishSequence();
		}
	}
}