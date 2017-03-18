using Server;
using System;
using Server.Engines.Quests;
using Server.Gumps;
using Server.Mobiles;
using Server.Network;

namespace Server.Items
{
    public class KingBlackthornOrders : BaseQuestItem
    {
        public override Type[] Quests
        {
            get
            {
                return new Type[] 
			        { 
				        typeof( TimeIsOfTheEssenceQuest )
			        };
            }
        }

        public override int LabelNumber { get { return 1156516; } }  // Orders from King Blackthorn to Sir Geoffrey
        public override int Lifespan { get { return 360; } }

        [Constructable]
        public KingBlackthornOrders()
            : base(8792)
        {
        }

        public KingBlackthornOrders(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int v = reader.ReadInt();
        }
    }

    /*public class LavaStone : Item
    {
        public override int LabelNumber { get { return 1151166; } }  // lava rock

        [Constructable]
        public LavaStone()
            : base(39638)
        {
        }

        public LavaStone(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int v = reader.ReadInt();
        }
    }*/

    public class MosaicOfHeluzz : Item
	{
		public override int LabelNumber { get { return 1156467; } }  // Mosaic of Heluzz
		
		[Constructable]
		public MosaicOfHeluzz() : base(39638)
		{
            Hue = 2952;
		}

        public MosaicOfHeluzz(Serial serial)
            : base(serial)
		{
		}
		
		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);
			writer.Write(0);
		}
		
		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);
			int v = reader.ReadInt();
		}
	}
	
	public class TotemOfFabozz : Item
	{
		public override int LabelNumber { get { return 1156468; } }  // Totem Of Faboz
		
		[Constructable]
		public TotemOfFabozz() : base(40092)
		{
            Hue = 2576;
		}

        public TotemOfFabozz(Serial serial)
            : base(serial)
		{
		}
		
		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);
			writer.Write(0);
		}
		
		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);
			int v = reader.ReadInt();
		}
	}
	
	public class FiresOfKukuzz : Item
	{
		public override int LabelNumber { get { return 1156469; } }  // Fires of Kukuzz
		
		[Constructable]
		public FiresOfKukuzz() : base(40014)
		{
		}

        public FiresOfKukuzz(Serial serial)
            : base(serial)
		{
		}
		
		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);
			writer.Write(0);
		}
		
		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);
			int v = reader.ReadInt();
		}
	}
	
	public class SkullOfMotazz : Item
	{
		public override int LabelNumber { get { return 1156470; } }  // The Skull of Motazz
		
		[Constructable]
		public SkullOfMotazz() : base(40051)
		{
            Hue = 2500;
		}

        public SkullOfMotazz(Serial serial)
            : base(serial)
		{
		}
		
		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);
			writer.Write(0);
		}
		
		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);
			int v = reader.ReadInt();
		}
	}
	
	public class SkullOfAphazz : Item
	{
		public override int LabelNumber { get { return 1156471; } }  // The Skull of Aphazz
		
		[Constructable]
		public SkullOfAphazz() : base(8707)
		{
		}

        public SkullOfAphazz(Serial serial)
            : base(serial)
		{
		}
		
		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);
			writer.Write(0);
		}
		
		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);
			int v = reader.ReadInt();
		}
	}

    public enum RugHue
    {
        Regular,
        White,
        Black
    }

	public class TigerRugAddon : BaseAddon
	{
		public override BaseAddonDeed Deed { get { return new TigerRugAddonDeed(this.RugType); } }
		
		public RugHue RugType { get; set; }
		
		[Constructable]
		public TigerRugAddon() : this(RugHue.Regular, true)
		{
		}
		
		[Constructable]
		public TigerRugAddon(RugHue type, bool south)
		{
			RugType = type;

			int hue = 0;
			int cliloc;

            switch(type)
            {
                default:
                case RugHue.Regular:
                    cliloc = 1156481;
                    break;
                case RugHue.White:
                    hue = 2500;
                    cliloc = 1156483;
                    break;
                case RugHue.Black:
                    hue = 1175;
                    cliloc = 1156482;
                    break;
            }

			if(south)
			{
				AddComponent(new LocalizedAddonComponent(40057, cliloc), 0, 0, 0);
				AddComponent(new LocalizedAddonComponent(40058, cliloc), -1, 0, 0);
				AddComponent(new LocalizedAddonComponent(40059, cliloc), 0, -1, 0);
				AddComponent(new LocalizedAddonComponent(40060, cliloc), -1, -1, 0);
                AddComponent(new LocalizedAddonComponent(40061, cliloc), 0, -2, 0);
                AddComponent(new LocalizedAddonComponent(40062, cliloc), -1, -2, 0);
			}
			else
			{
				AddComponent(new LocalizedAddonComponent(40051, cliloc), 0, 0, 0);
				AddComponent(new LocalizedAddonComponent(40052, cliloc), 0, -1, 0);
				AddComponent(new LocalizedAddonComponent(40053, cliloc), -1, 0, 0);
				AddComponent(new LocalizedAddonComponent(40054, cliloc), -1, -1, 0);
                AddComponent(new LocalizedAddonComponent(40055, cliloc), -2, 0, 0);
                AddComponent(new LocalizedAddonComponent(40056, cliloc), -2, -1, 0);
			}

            Hue = hue;
		}
		
		public TigerRugAddon(Serial serial) : base(serial)
		{
		}
		
		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);
			writer.Write(0);
			
			writer.Write((int)RugType);
		}
		
		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);
			int v = reader.ReadInt();
			
			RugType = (RugHue)reader.ReadInt();
		}
	}
	
	public class TigerRugAddonDeed : BaseAddonDeed
	{
		public override int LabelNumber
		{
            get
            {
			    switch(this.RugType)
			    {
				    case RugHue.Regular: return 1156481;
				    case RugHue.White: return 1156483;
				    case RugHue.Black: return 1156482;
			    }
			
			    return 1156481;
            }
		}
		
		public override BaseAddon Addon { get { return new TigerRugAddon(this.RugType, SouthFacing); } }
		
		public RugHue RugType { get; set; }
		public bool SouthFacing { get; set; }
		
		[Constructable]
		public TigerRugAddonDeed() : this(RugHue.Regular)
		{
		}
		
		[Constructable]
        public TigerRugAddonDeed(RugHue type)
		{
			RugType = type;
		}
		
		public override void OnDoubleClick(Mobile from)
		{
			if(IsChildOf(from.Backpack))
			{
				from.SendGump(new SouthEastGump(s =>
				{
					SouthFacing = s;
					base.OnDoubleClick(from);
				}));
			}
		}

        public TigerRugAddonDeed(Serial serial)
            : base(serial)
		{
		}
		
		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);
			writer.Write(0);
			
			writer.Write((int)RugType);
		}
		
		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);
			int v = reader.ReadInt();
			
			RugType = (RugHue)reader.ReadInt();
		}
	}

	public enum BananaHoardSize
    {
        Small,
        Medium,
        Large
    }
	
	public class BananaHoardAddon : BaseAddon
	{
		public override BaseAddonDeed Deed { get { return new BananaHoardAddonDeed(); } }
		
		public BananaHoardSize BananaHoardSize { get; set; }
		
		[Constructable]
		public BananaHoardAddon()
		{
		}
		
		public BananaHoardAddon(BananaHoardSize bananaHoardSize)
		{
			BananaHoardSize = bananaHoardSize;
			
			switch(bananaHoardSize)
			{
				case BananaHoardSize.Small:
                    AddComponent(new LocalizedAddonComponent(40047, 1156484), 0, 0, 0);
                    AddComponent(new LocalizedAddonComponent(40046, 1156484), -1, 0, 0);
                    AddComponent(new LocalizedAddonComponent(40049, 1156484), 0, -1, 0);
                    AddComponent(new LocalizedAddonComponent(40050, 1156484), -1, -1, 0);
					break;
				case BananaHoardSize.Medium:
                    AddComponent(new LocalizedAddonComponent(40043, 1156485), 0, 0, 0);
                    AddComponent(new LocalizedAddonComponent(40044, 1156485), 0, -1, 0);
                    AddComponent(new LocalizedAddonComponent(40048, 1156485), -1, 0, 0);
                    AddComponent(new LocalizedAddonComponent(40045, 1156485), -1, -1, 0);
					break;
				case BananaHoardSize.Large:
                    AddComponent(new LocalizedAddonComponent(40042, 1156486), 0, 0, 0);
                    AddComponent(new LocalizedAddonComponent(40040, 1156486), -1, 0, 0);
                    AddComponent(new LocalizedAddonComponent(40041, 1156486), 0, -1, 0);
					break;
			}
		}

        public BananaHoardAddon(Serial serial)
            : base(serial)
		{
		}
		
		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);
			writer.Write(0);
			
			writer.Write((int)this.BananaHoardSize);
		}
		
		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);
			int v = reader.ReadInt();
			
			this.BananaHoardSize = (BananaHoardSize)reader.ReadInt();
		}
	}
	
	public class BananaHoardAddonDeed : BaseAddonDeed
	{
		public override BaseAddon Addon { get { return new BananaHoardAddon(BananaHoardSize); } }
        public override int LabelNumber { get { return 1156556; } } // Great Ape's Banana Hoard

		public BananaHoardSize BananaHoardSize { get; set; }
		
		[Constructable]
		public BananaHoardAddonDeed()
		{
		}
		
		public override void OnDoubleClick(Mobile from)
		{
			if(IsChildOf(from.Backpack))
			{
				from.SendGump(new InternalGump(from as PlayerMobile, s =>
				{
					this.BananaHoardSize = s;
					base.OnDoubleClick(from);
				}));
			}
		}
		
		private class InternalGump : Gump
		{
            public Action<BananaHoardSize> Callback { get; set; }
            public PlayerMobile User { get; set; }
			
			public InternalGump(PlayerMobile p, Action<BananaHoardSize> callback) : base(50, 50)
			{
				Callback = callback;
                User = p;

                AddGumpLayout();
			}
			
			public void AddGumpLayout()
			{
				AddBackground(0, 0, 200, 200, 5054);
				AddBackground(10, 10, 180, 180, 3000);
				
				AddHtml(55, 50, 150, 16, "Small", false, false);
				AddHtml(55, 80, 150, 16, "Medium", false, false);
				AddHtml(55, 110, 150, 16, "Large", false, false);
				
				for(int i = 0; i < 3; i++)
				{
                    AddButton(20, 50 + (i * 30), 4005, 4007, i + 1, GumpButtonType.Reply, 0);
				}
			}

            public override void OnResponse(NetState state, RelayInfo info)
            {
                if (info.ButtonID > 0 && Callback != null)
                {
                    Callback((BananaHoardSize)info.ButtonID - 1);
                }
            }
		}
		
		public BananaHoardAddonDeed(Serial serial) : base(serial)
		{
		}
		
		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);
			writer.Write(0);
		}
		
		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);
			int v = reader.ReadInt();
		}
	}
	
	public class DinosaurHunterRewardTitleDeed : BaseRewardTitleDeed
	{
		public override TextDefinition Title { get { return new TextDefinition("Dinosaur Hunter"); } }
		
		[Constructable]
		public DinosaurHunterRewardTitleDeed()
		{
		}

        public DinosaurHunterRewardTitleDeed(Serial serial)
            : base(serial)
		{
		}
		
		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);
			writer.Write(0);
		}
		
		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);
			int v = reader.ReadInt();
		}
	}
	
	public class LavaRockDisplay : Item
	{
        public override int LabelNumber { get { return 1124033; } }  // lava rock display
		
		[Constructable]
		public LavaRockDisplay() : base(40009)
		{
            Weight = 5.0;
		}

        public LavaRockDisplay(Serial serial)
            : base(serial)
		{
		}
		
		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);
			writer.Write(0);
		}
		
		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);
			int v = reader.ReadInt();
		}
	}
	
	public class DragonTurtleFountainAddon : BaseAddon
	{
		public override BaseAddonDeed Deed { get { return new DragonTurtleFountainAddonDeed(); } }

		[Constructable]
		public DragonTurtleFountainAddon(bool south)
		{
            if (south)
            {
                AddComponent(new AddonComponent(40087), 0, 0, 0);
                AddComponent(new AddonComponent(40089), 0, -1, 0);
                AddComponent(new AddonComponent(40088), 1, -1, 0);
                AddComponent(new AddonComponent(40090), 1, -2, 0);
            }
            else
            {
                AddComponent(new AddonComponent(40092), 0, 0, 0);
                AddComponent(new AddonComponent(40093), -1, 0, 0);
                AddComponent(new AddonComponent(40094), -1, 1, 0);
                AddComponent(new AddonComponent(40096), -2, 1, 0);
            }
		}

        public DragonTurtleFountainAddon(Serial serial)
            : base(serial)
		{
		}
		
		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);
			writer.Write(0);
		}
		
		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);
			int v = reader.ReadInt();
		}
	}
	
	public class DragonTurtleFountainAddonDeed : BaseAddonDeed
	{
		public override BaseAddon Addon { get { return new DragonTurtleFountainAddon(SouthFacing); } }
        public override int LabelNumber { get { return 1156488; } } // Dragon Turtle Fountain

        public bool SouthFacing { get; set; }

		[Constructable]
		public DragonTurtleFountainAddonDeed()
		{
		}

        public override void OnDoubleClick(Mobile from)
        {
            if (IsChildOf(from.Backpack))
            {
                from.SendGump(new SouthEastGump(s =>
                {
                    SouthFacing = s;
                    base.OnDoubleClick(from);
                }));
            }
        }

        public DragonTurtleFountainAddonDeed(Serial serial)
            : base(serial)
		{
		}
		
		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);
			writer.Write(0);
		}
		
		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);
			int v = reader.ReadInt();
		}
	}
	
	public class PetTigerCubStatuette : MonsterStatuette
	{
		[Constructable]
		public PetTigerCubStatuette() : base(MonsterStatuetteType.TigerCub)
		{
			IsRewardItem = false;
		}
		
		public override void OnDoubleClick(Mobile from)
		{
			if(IsLockedDown && from.Followers != from.FollowersMax)
			{
                BaseCreature cub = new TigerCub();
				Point3D p = from.Location;
				
				int x = p.X;
				int y = p.Y;
				
				Server.Movement.Movement.Offset(from.Direction, ref x, ref y);
				int z = from.Map.GetAverageZ(x, y);

                if (from.Map.CanSpawnMobile(x, y, z))
                    p = new Point3D(x, y, z);
				
				cub.MoveToWorld(p, from.Map);
				cub.SetControlMaster(from);
			}
		}

        public PetTigerCubStatuette(Serial serial)
            : base(serial)
		{
		}
		
		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);
			writer.Write(0);
		}
		
		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);
			int v = reader.ReadInt();
		}
	}
	
	public class UniqueTreasureBag : Pouch
	{
		public override int LabelNumber { get { return 1156581; } }  // A bag with a unique treasure
		
		[Constructable]
		public UniqueTreasureBag()
		{
            switch (Utility.Random(6))
            {
                case 0: DropItem(new LavaRockDisplay()); break;
                case 1:
                    double random = Utility.RandomDouble();

                    if (random <= .02)
                        DropItem(new TigerRugAddonDeed(RugHue.White));
                    else if (random < .15)
                        DropItem(new TigerRugAddonDeed(RugHue.Black));
                    else
                        DropItem(new TigerRugAddonDeed(RugHue.Regular));
                    break;
                case 2: DropItem(new BananaHoardAddonDeed()); break;
                case 3: DropItem(new DragonTurtleFountainAddonDeed()); break;
                case 4: DropItem(new DinosaurHunterRewardTitleDeed()); break;
                case 5: DropItem(new PetTigerCubStatuette()); break;
            }
		}

        public UniqueTreasureBag(Serial serial)
            : base(serial)
		{
		}
		
		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);
			writer.Write(0);
		}
		
		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);
			int v = reader.ReadInt();
		}
	}
}