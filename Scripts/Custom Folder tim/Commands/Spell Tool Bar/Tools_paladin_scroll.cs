using System;
using Server;
using Server.Network;
using Server.Gumps;

namespace Server.Items
{
	public class Tools_paladin_scroll : Item
	{
		public int mN01CleanseByFireSpell = 0;
		public int mN02CloseWoundsSpell = 0;
		public int mN03ConsecrateWeaponSpell = 0;
		public int mN04DispelEvilSpell = 0;
		public int mN05DivineFurySpell = 0;
		public int mN06EnemyOfOneSpell = 0;
		public int mN07HolyLightSpell = 0;
		public int mN08NobleSacrificeSpell = 0;
		public int mN09RemoveCurseSpell = 0;
		public int mN10SacredJourneySpell = 0;

		[CommandProperty(AccessLevel.GameMaster)]
		public int N01CleanseByFireSpell { get { return mN01CleanseByFireSpell; } set { mN01CleanseByFireSpell = value; } }
		[CommandProperty(AccessLevel.GameMaster)]
		public int N02CloseWoundsSpell { get { return mN02CloseWoundsSpell; } set { mN02CloseWoundsSpell = value; } }
		[CommandProperty(AccessLevel.GameMaster)]
		public int N03ConsecrateWeaponSpell { get { return mN03ConsecrateWeaponSpell; } set { mN03ConsecrateWeaponSpell = value; } }
		[CommandProperty(AccessLevel.GameMaster)]
		public int N04DispelEvilSpell { get { return mN04DispelEvilSpell; } set { mN04DispelEvilSpell = value; } }
		[CommandProperty(AccessLevel.GameMaster)]
		public int N05DivineFurySpell { get { return mN05DivineFurySpell; } set { mN05DivineFurySpell = value; } }
		[CommandProperty(AccessLevel.GameMaster)]
		public int N06EnemyOfOneSpell { get { return mN06EnemyOfOneSpell; } set { mN06EnemyOfOneSpell = value; } }
		[CommandProperty(AccessLevel.GameMaster)]
		public int N07HolyLightSpell { get { return mN07HolyLightSpell; } set { mN07HolyLightSpell = value; } }
		[CommandProperty(AccessLevel.GameMaster)]
		public int N08NobleSacrificeSpell { get { return mN08NobleSacrificeSpell; } set { mN08NobleSacrificeSpell = value; } }
		[CommandProperty(AccessLevel.GameMaster)]
		public int N09RemoveCurseSpell { get { return mN09RemoveCurseSpell; } set { mN09RemoveCurseSpell = value; } }
		[CommandProperty(AccessLevel.GameMaster)]
		public int N10SacredJourneySpell { get { return mN10SacredJourneySpell; } set { mN10SacredJourneySpell = value; } }

		[Constructable]
		public Tools_paladin_scroll() : base( 0x14F0 )
		{
			LootType = LootType.Blessed;
			Hue = 0xB8E;
			Name = "Paladin Creed";
		}

		public Tools_paladin_scroll( Serial serial ) : base( serial )
		{
		}

		public override void OnDoubleClick( Mobile from )
		{
			if (!IsChildOf(from.Backpack))
			{
				from.SendLocalizedMessage(1042001);
			}
			else
			{
				from.CloseGump( typeof( Tools_paladin_scrollGump ) );
				from.SendGump( new Tools_paladin_scrollGump( from, this ) );
			}
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 ); // version
			writer.Write(mN01CleanseByFireSpell);
			writer.Write(mN02CloseWoundsSpell);
			writer.Write(mN03ConsecrateWeaponSpell);
			writer.Write(mN04DispelEvilSpell);
			writer.Write(mN05DivineFurySpell);
			writer.Write(mN06EnemyOfOneSpell);
			writer.Write(mN07HolyLightSpell);
			writer.Write(mN08NobleSacrificeSpell);
			writer.Write(mN09RemoveCurseSpell);
			writer.Write(mN10SacredJourneySpell);
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
			mN01CleanseByFireSpell = reader.ReadInt();
			mN02CloseWoundsSpell = reader.ReadInt();
			mN03ConsecrateWeaponSpell = reader.ReadInt();
			mN04DispelEvilSpell = reader.ReadInt();
			mN05DivineFurySpell = reader.ReadInt();
			mN06EnemyOfOneSpell = reader.ReadInt();
			mN07HolyLightSpell = reader.ReadInt();
			mN08NobleSacrificeSpell = reader.ReadInt();
			mN09RemoveCurseSpell = reader.ReadInt();
			mN10SacredJourneySpell = reader.ReadInt();
		}
	}
}