using System;
using Server.Gumps;

namespace Server.Items
{
    [Flipable(0x14ED, 0x14EE)]
    public class SOS : Item
    {
        public override int LabelNumber
        {
            get
            {
                if (this.IsAncient)
                    return 1063450; // an ancient SOS

                return 1041081; // a waterstained SOS
            }
        }

        private int m_Level;
        private Map m_TargetMap;
        private Point3D m_TargetLocation;
        private int m_MessageIndex;

        [CommandProperty(AccessLevel.GameMaster)]
        public bool IsAncient
        {
            get
            {
                return (this.m_Level >= 4);
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int Level
        {
            get
            {
                return this.m_Level;
            }
            set
            {
                this.m_Level = Math.Max(1, Math.Min(value, 4));
                this.UpdateHue();
                this.InvalidateProperties();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public Map TargetMap
        {
            get
            {
                return this.m_TargetMap;
            }
            set
            {
                this.m_TargetMap = value;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public Point3D TargetLocation
        {
            get
            {
                return this.m_TargetLocation;
            }
            set
            {
                this.m_TargetLocation = value;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int MessageIndex
        {
            get
            {
                return this.m_MessageIndex;
            }
            set
            {
                this.m_MessageIndex = value;
            }
        }

        public void UpdateHue()
        {
            if (this.IsAncient)
                this.Hue = 0x481;
            else
                this.Hue = 0;
        }

        [Constructable]
        public SOS()
            : this(Map.Trammel)
        {
        }

        [Constructable]
        public SOS(Map map)
            : this(map, MessageInABottle.GetRandomLevel())
        {
        }

        [Constructable]
        public SOS(Map map, int level)
            : base(0x14EE)
        {
            this.Weight = 1.0;

            this.m_Level = level;
            this.m_MessageIndex = Utility.Random(MessageEntry.Entries.Length);
            this.m_TargetMap = map;
            this.m_TargetLocation = FindLocation(this.m_TargetMap);

            this.UpdateHue();
        }

        public SOS(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)4); // version

            writer.Write(this.m_Level);

            writer.Write(this.m_TargetMap);
            writer.Write(this.m_TargetLocation);
            writer.Write(this.m_MessageIndex);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch ( version )
            {
                case 4:
                case 3:
                case 2:
                    {
                        this.m_Level = reader.ReadInt();
                        goto case 1;
                    }
                case 1:
                    {
                        this.m_TargetMap = reader.ReadMap();
                        this.m_TargetLocation = reader.ReadPoint3D();
                        this.m_MessageIndex = reader.ReadInt();

                        break;
                    }
                case 0:
                    {
                        this.m_TargetMap = this.Map;

                        if (this.m_TargetMap == null || this.m_TargetMap == Map.Internal)
                            this.m_TargetMap = Map.Trammel;

                        this.m_TargetLocation = FindLocation(this.m_TargetMap);
                        this.m_MessageIndex = Utility.Random(MessageEntry.Entries.Length);

                        break;
                    }
            }

            if (version < 2)
                this.m_Level = MessageInABottle.GetRandomLevel();

            if (version < 3)
                this.UpdateHue();

            if (version < 4 && this.m_TargetMap == Map.Tokuno)
                this.m_TargetMap = Map.Trammel;
        }
		
        public override void OnDoubleClick(Mobile from)
        {
            if (this.IsChildOf(from.Backpack))
            {
                MessageEntry entry;

                if (this.m_MessageIndex >= 0 && this.m_MessageIndex < MessageEntry.Entries.Length)
                    entry = MessageEntry.Entries[this.m_MessageIndex];
                else
                    entry = MessageEntry.Entries[this.m_MessageIndex = Utility.Random(MessageEntry.Entries.Length)];

                //from.CloseGump( typeof( MessageGump ) );
                from.SendGump(new MessageGump(entry, this.m_TargetMap, this.m_TargetLocation));
            }
            else
            {
                from.SendLocalizedMessage(1042001); // That must be in your pack for you to use it.
            }
        }

        private static readonly int[] m_WaterTiles = new int[]
        {
            0x00A8, 0x00AB,
            0x0136, 0x0137
        };

        private static readonly Rectangle2D[] m_BritRegions = new Rectangle2D[] { new Rectangle2D(0, 0, 5120, 4096) };
        private static readonly Rectangle2D[] m_IlshRegions = new Rectangle2D[] { new Rectangle2D(1472, 272, 304, 240), new Rectangle2D(1240, 1000, 312, 160) };
        private static readonly Rectangle2D[] m_MalasRegions = new Rectangle2D[] { new Rectangle2D(1376, 1520, 464, 280) };
        private static readonly Rectangle2D[] m_TokunoRegions = new Rectangle2D[] { new Rectangle2D(10, 10, 1440, 1440) };

        public static Point3D FindLocation(Map map)
        {
            if (map == null || map == Map.Internal)
                return Point3D.Zero;

            Rectangle2D[] regions;

            if (map == Map.Felucca || map == Map.Trammel)
                regions = m_BritRegions;
            else if (map == Map.Ilshenar)
                regions = m_IlshRegions;
            else if (map == Map.Malas)
                regions = m_MalasRegions;
            else if (map == Map.Tokuno)
                regions = m_TokunoRegions;
            else
                regions = new Rectangle2D[] { new Rectangle2D(0, 0, map.Width, map.Height) };

            if (regions.Length == 0)
                return Point3D.Zero;

            for (int i = 0; i < 50; ++i)
            {
                Rectangle2D reg = regions[Utility.Random(regions.Length)];
                int x = Utility.Random(reg.X, reg.Width);
                int y = Utility.Random(reg.Y, reg.Height);

                if (!ValidateDeepWater(map, x, y))
                    continue;

                bool valid = true;

                for (int j = 1, offset = 5; valid && j <= 5; ++j, offset += 5)
                {
                    if (!ValidateDeepWater(map, x + offset, y + offset))
                        valid = false;
                    else if (!ValidateDeepWater(map, x + offset, y - offset))
                        valid = false;
                    else if (!ValidateDeepWater(map, x - offset, y + offset))
                        valid = false;
                    else if (!ValidateDeepWater(map, x - offset, y - offset))
                        valid = false;
                }

                if (valid)
                    return new Point3D(x, y, 0);
            }

            return Point3D.Zero;
        }

        public static bool ValidateDeepWater(Map map, int x, int y)
        {
            int tileID = map.Tiles.GetLandTile(x, y).ID;
            bool water = false;

            for (int i = 0; !water && i < m_WaterTiles.Length; i += 2)
                water = (tileID >= m_WaterTiles[i] && tileID <= m_WaterTiles[i + 1]);

            return water;
        }

        #if false
		private class MessageGump : Gump
		{
			public MessageGump( MessageEntry entry, Map map, Point3D loc ) : base( (640 - entry.Width) / 2, (480 - entry.Height) / 2 )
			{
				int xLong = 0, yLat = 0;
				int xMins = 0, yMins = 0;
				bool xEast = false, ySouth = false;
				string fmt;

				if ( Sextant.Format( loc, map, ref xLong, ref yLat, ref xMins, ref yMins, ref xEast, ref ySouth ) )
					fmt = String.Format( "{0}°{1}'{2},{3}°{4}'{5}", yLat, yMins, ySouth ? "S" : "N", xLong, xMins, xEast ? "E" : "W" );
				else
					fmt = "?????";

				AddPage( 0 );
				AddBackground( 0, 0, entry.Width, entry.Height, 2520 );
				AddHtml( 38, 38, entry.Width - 83, entry.Height - 86, String.Format( entry.Message, fmt ), false, false );
			}
		}
        #else
        private class MessageGump : Gump
        {
            public MessageGump(MessageEntry entry, Map map, Point3D loc)
                : base(150, 50)
            {
                int xLong = 0, yLat = 0;
                int xMins = 0, yMins = 0;
                bool xEast = false, ySouth = false;
                string fmt;

                if (Sextant.Format(loc, map, ref xLong, ref yLat, ref xMins, ref yMins, ref xEast, ref ySouth))
                    fmt = String.Format("{0}°{1}'{2},{3}°{4}'{5}", yLat, yMins, ySouth ? "S" : "N", xLong, xMins, xEast ? "E" : "W");
                else
                    fmt = "?????";

                this.AddPage(0);

                this.AddBackground(0, 40, 350, 300, 2520);

                this.AddHtmlLocalized(30, 80, 285, 160, 1018326, true, true); /* This is a message hastily scribbled by a passenger aboard a sinking ship.
                * While it is probably too late to save the passengers and crew,
                * perhaps some treasure went down with the ship!
                * The message gives the ship's last known sextant co-ordinates.
                */

                this.AddHtml(35, 240, 230, 20, fmt, false, false);

                this.AddButton(35, 265, 4005, 4007, 0, GumpButtonType.Reply, 0);
                this.AddHtmlLocalized(70, 265, 100, 20, 1011036, false, false); // OKAY
            }
        }
        #endif

        private class MessageEntry
        {
            private readonly int m_Width;

            private readonly int m_Height;

            private readonly string m_Message;

            public int Width
            {
                get
                {
                    return this.m_Width;
                }
            }
            public int Height
            {
                get
                {
                    return this.m_Height;
                }
            }
            public string Message
            {
                get
                {
                    return this.m_Message;
                }
            }

            public MessageEntry(int width, int height, string message)
            {
                this.m_Width = width;
                this.m_Height = height;
                this.m_Message = message;
            }

            private static readonly MessageEntry[] m_Entries = new MessageEntry[]
            {
                new MessageEntry(280, 180, "...Ar! {0} and a fair wind! No chance... storms, though--ar! Is that a sea serp...<br><br>uh oh."),
                new MessageEntry(280, 215, "...been inside this whale for three days now. I've run out of food I can pick out of his teeth. I took a sextant reading through the blowhole: {0}. I'll never see my treasure again..."),
                new MessageEntry(280, 285, "...grand adventure! Captain Quacklebush had me swab down the decks daily...<br>  ...pirates came, I was in the rigging practicing with my sextant. {0} if I am not mistaken...<br>  ....scuttled the ship, and our precious cargo went with her and the screaming pirates, down to the bottom of the sea..."),
                new MessageEntry(280, 180, "Help! Ship going dow...n heavy storms...precious cargo...st reach dest...current coordinates {0}...ve any survivors... ease!"),
                new MessageEntry(280, 215, "...know that the wreck is near {0} but have not found it. Could the message passed down in my family for generations be wrong? No... I swear on the soul of my grandfather, I will find..."),
                new MessageEntry(280, 195, "...never expected an iceberg...silly woman on bow crushed instantly...send help to {0}...ey'll never forget the tragedy of the sinking of the Miniscule..."),
                new MessageEntry(280, 265, "...nobody knew I was a girl. They just assumed I was another sailor...then we met the undine. {0}. It was demanded sacrifice...I was youngset, they figured...<br>  ...grabbed the captain's treasure, screamed, 'It'll go down with me!'<br>  ...they took me up on it."),
                new MessageEntry(280, 230, "...so I threw the treasure overboard, before the curse could get me too. But I was too late. Now I am doomed to wander these seas, a ghost forever. Join me: seek ye at {0} if thou wishest my company..."),
                new MessageEntry(280, 285, "...then the ship exploded. A dragon swooped by. The slime swallowed Bertie whole--he screamed, it was amazing. The sky glowed orange. A sextant reading put us at {0}. Norma was chattering about sailing over the edge of the world. I looked at my hands and saw through them..."),
                new MessageEntry(280, 285, "...trapped on a deserted island, with a magic fountain supplying wood, fresh water springs, gorgeous scenery, and my lovely young wife. I know the ship with all our life's earnings sank at {0} but I don't know what our coordinates are... someone has GOT to rescue me before Sunday's finals game or I'll go mad..."),
                new MessageEntry(280, 160, "WANTED: divers exp...d in shipwre...overy. Must have own vess...pply at {0}<br>...good benefits, flexible hours..."),
                new MessageEntry(280, 250, "...was a cad and a boor, no matter what momma s...rew him overboard! Oh, Anna, 'twas so exciting!<br>  Unfort...y he grabbe...est, and all his riches went with him!<br>  ...sked the captain, and he says we're at {0}<br>...so maybe...")
            };

            public static MessageEntry[] Entries
            {
                get
                {
                    return m_Entries;
                }
            }
        }
    }
}