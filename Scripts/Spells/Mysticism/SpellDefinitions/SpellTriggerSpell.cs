using System;
using Server;
using Server.Gumps;
using Server.Items;
using Server.Mobiles;
using Server.Spells;
using Server.Targeting;

namespace Server.Spells.Mystic
{
	public class SpellTriggerSpell : MysticSpell
	{
        public override SpellCircle Circle { get { return SpellCircle.Fifth; } }

		private static SpellInfo m_Info = new SpellInfo(
				"Spell Trigger", "In Vas Ort Ex ",
				230,
				9022,
				Reagent.Garlic,
				Reagent.MandrakeRoot,
				Reagent.SpidersSilk,
				Reagent.DragonBlood
			);

		public SpellTriggerSpell( Mobile caster, Item scroll ) : base( caster, scroll, m_Info )
		{
		}

		public override void OnCast()
		{
			if ( Caster.HasGump( typeof( SpellTriggerGump ) ) )
					Caster.CloseGump( typeof( SpellTriggerGump ) );

			Caster.SendGump( new SpellTriggerGump( Caster, this ) );
		}
	}
}