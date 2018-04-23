using System;
using Server;
using Server.Network;
using Server.Gumps;

namespace Server.Items
{
	public class Tools_necro_scroll : Item
	{
		public int mN01AnimateDeadSpell = 0;
		public int mN02BloodOathSpell = 0;
		public int mN03CorpseSkinSpell = 0;
		public int mN04CurseWeaponSpell = 0;
		public int mN05EvilOmenSpell = 0;
		public int mN06HorrificBeastSpell = 0;
		public int mN07LichFormSpell = 0;
		public int mN08MindRotSpell = 0;
		public int mN09PainSpikeSpell = 0;
		public int mN10PoisonStrikeSpell = 0;
		public int mN11StrangleSpell = 0;
		public int mN12SummonFamiliarSpell = 0;
		public int mN13VampiricEmbraceSpell = 0;
		public int mN14VengefulSpiritSpell = 0;
		public int mN15WitherSpell = 0;
		public int mN16WraithFormSpell = 0;

		[CommandProperty(AccessLevel.GameMaster)]
		public int N01AnimateDeadSpell { get { return mN01AnimateDeadSpell; } set { mN01AnimateDeadSpell = value; } }
		[CommandProperty(AccessLevel.GameMaster)]
		public int N02BloodOathSpell { get { return mN02BloodOathSpell; } set { mN02BloodOathSpell = value; } }
		[CommandProperty(AccessLevel.GameMaster)]
		public int N03CorpseSkinSpell { get { return mN03CorpseSkinSpell; } set { mN03CorpseSkinSpell = value; } }
		[CommandProperty(AccessLevel.GameMaster)]
		public int N04CurseWeaponSpell { get { return mN04CurseWeaponSpell; } set { mN04CurseWeaponSpell = value; } }
		[CommandProperty(AccessLevel.GameMaster)]
		public int N05EvilOmenSpell { get { return mN05EvilOmenSpell; } set { mN05EvilOmenSpell = value; } }
		[CommandProperty(AccessLevel.GameMaster)]
		public int N06HorrificBeastSpell { get { return mN06HorrificBeastSpell; } set { mN06HorrificBeastSpell = value; } }
		[CommandProperty(AccessLevel.GameMaster)]
		public int N07LichFormSpell { get { return mN07LichFormSpell; } set { mN07LichFormSpell = value; } }
		[CommandProperty(AccessLevel.GameMaster)]
		public int N08MindRotSpell { get { return mN08MindRotSpell; } set { mN08MindRotSpell = value; } }
		[CommandProperty(AccessLevel.GameMaster)]
		public int N09PainSpikeSpell { get { return mN09PainSpikeSpell; } set { mN09PainSpikeSpell = value; } }
		[CommandProperty(AccessLevel.GameMaster)]
		public int N10PoisonStrikeSpell { get { return mN10PoisonStrikeSpell; } set { mN10PoisonStrikeSpell = value; } }
		[CommandProperty(AccessLevel.GameMaster)]
		public int N11StrangleSpell { get { return mN11StrangleSpell; } set { mN11StrangleSpell = value; } }
		[CommandProperty(AccessLevel.GameMaster)]
		public int N12SummonFamiliarSpell { get { return mN12SummonFamiliarSpell; } set { mN12SummonFamiliarSpell = value; } }
		[CommandProperty(AccessLevel.GameMaster)]
		public int N13VampiricEmbraceSpell { get { return mN13VampiricEmbraceSpell; } set { mN13VampiricEmbraceSpell = value; } }
		[CommandProperty(AccessLevel.GameMaster)]
		public int N14VengefulSpiritSpell { get { return mN14VengefulSpiritSpell; } set { mN14VengefulSpiritSpell = value; } }
		[CommandProperty(AccessLevel.GameMaster)]
		public int N15WitherSpell { get { return mN15WitherSpell; } set { mN15WitherSpell = value; } }
		[CommandProperty(AccessLevel.GameMaster)]
		public int N16WraithFormSpell { get { return mN16WraithFormSpell; } set { mN16WraithFormSpell = value; } }

		[Constructable]
		public Tools_necro_scroll() : base( 0x14F0 )
		{
			LootType = LootType.Blessed;
			Hue = 0xB85;
			Name = "Necromancer Notes";
		}

		public Tools_necro_scroll( Serial serial ) : base( serial )
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
				from.CloseGump( typeof( Tools_necro_scrollGump ) );
				from.SendGump( new Tools_necro_scrollGump( from, this ) );
			}
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 ); // version
			writer.Write(mN01AnimateDeadSpell);
			writer.Write(mN02BloodOathSpell);
			writer.Write(mN03CorpseSkinSpell);
			writer.Write(mN04CurseWeaponSpell);
			writer.Write(mN05EvilOmenSpell);
			writer.Write(mN06HorrificBeastSpell);
			writer.Write(mN07LichFormSpell);
			writer.Write(mN08MindRotSpell);
			writer.Write(mN09PainSpikeSpell);
			writer.Write(mN10PoisonStrikeSpell);
			writer.Write(mN11StrangleSpell);
			writer.Write(mN12SummonFamiliarSpell);
			writer.Write(mN13VampiricEmbraceSpell);
			writer.Write(mN14VengefulSpiritSpell);
			writer.Write(mN15WitherSpell);
			writer.Write(mN16WraithFormSpell);
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
			mN01AnimateDeadSpell = reader.ReadInt();
			mN02BloodOathSpell = reader.ReadInt();
			mN03CorpseSkinSpell = reader.ReadInt();
			mN04CurseWeaponSpell = reader.ReadInt();
			mN05EvilOmenSpell = reader.ReadInt();
			mN06HorrificBeastSpell = reader.ReadInt();
			mN07LichFormSpell = reader.ReadInt();
			mN08MindRotSpell = reader.ReadInt();
			mN09PainSpikeSpell = reader.ReadInt();
			mN10PoisonStrikeSpell = reader.ReadInt();
			mN11StrangleSpell = reader.ReadInt();
			mN12SummonFamiliarSpell = reader.ReadInt();
			mN13VampiricEmbraceSpell = reader.ReadInt();
			mN14VengefulSpiritSpell = reader.ReadInt();
			mN15WitherSpell = reader.ReadInt();
			mN16WraithFormSpell = reader.ReadInt();
		}
	}
}
