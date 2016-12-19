using System;
using System.Collections.Generic;
using Server.Commands;
using Server.Gumps;
using Server.Mobiles;
using Server.Network;
using Server.Spells;

namespace Server.Items
{
    public class PublicMoongate : Item
    {
        private static List<PublicMoongate> _Moongates = new List<PublicMoongate>();
        public static List<PublicMoongate> Moongates { get { return _Moongates; } }

        [Constructable]
        public PublicMoongate()
            : base(0xF6C)
        {
            this.Movable = false;
            this.Light = LightType.Circle300;

            _Moongates.Add(this);
        }

        public PublicMoongate(Serial serial)
            : base(serial)
        {
        }

        public override bool ForceShowProperties
        {
            get
            {
                return ObjectPropertyList.Enabled;
            }
        }
        public override bool HandlesOnMovement
        {
            get
            {
                return true;
            }
        }
        public static void Initialize()
        {
			CommandSystem.Register("MoonGen", AccessLevel.Administrator, new CommandEventHandler(MoonGen_OnCommand));
			CommandSystem.Register("MoonGenDelete", AccessLevel.Administrator, new CommandEventHandler(MoonGenDelete_OnCommand));
		}

		[Usage("MoonGen")]
        [Description("Generates public moongates. Removes all old moongates.")]
        public static void MoonGen_OnCommand(CommandEventArgs e)
        {
            DeleteAll();

            int count = 0;

            count += MoonGen(PMList.Trammel);
            count += MoonGen(PMList.Felucca);
            count += MoonGen(PMList.Ilshenar);
            count += MoonGen(PMList.Malas);
            count += MoonGen(PMList.Tokuno);
            count += MoonGen(PMList.TerMur);

            World.Broadcast(0x35, true, "{0} moongates generated.", count);
        }

		[Usage("MoonGenDelete")]
		[Description("Removes all public moongates.")]
		public static void MoonGenDelete_OnCommand(CommandEventArgs e)
		{
			DeleteAll();
		}

		public override void OnDoubleClick(Mobile from)
        {
            if (!from.Player)
                return;

            if (from.InRange(this.GetWorldLocation(), 1))
                this.UseGate(from);
            else
                from.SendLocalizedMessage(500446); // That is too far away.
        }

        public override bool OnMoveOver(Mobile m)
        {
            // Changed so criminals are not blocked by it.
            if (m.Player)
                this.UseGate(m);

            return true;
        }

        public override void OnMovement(Mobile m, Point3D oldLocation)
        {
            if (m is PlayerMobile)
            {
                if (!Utility.InRange(m.Location, this.Location, 1) && Utility.InRange(oldLocation, this.Location, 1))
                    m.CloseGump(typeof(MoongateGump));
            }
        }

        public bool UseGate(Mobile m)
        {
            if (m.Criminal)
            {
                m.SendLocalizedMessage(1005561, "", 0x22); // Thou'rt a criminal and cannot escape so easily.
                return false;
            }
            else if (SpellHelper.CheckCombat(m))
            {
                m.SendLocalizedMessage(1005564, "", 0x22); // Wouldst thou flee during the heat of battle??
                return false;
            }
            else if (m.Spell != null)
            {
                m.SendLocalizedMessage(1049616); // You are too busy to do that at the moment.
                return false;
            }
            else if (m.Holding != null)
            {
                m.SendLocalizedMessage(1071955); // You cannot teleport while dragging an object.
                return false;
            }
            else
            {
                m.CloseGump(typeof(MoongateGump));
                m.SendGump(new MoongateGump(m, this));

                if (!m.Hidden || m.IsPlayer())
                    Effects.PlaySound(m.Location, m.Map, 0x20E);

                return true;
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            _Moongates.Add(this);
        }

        public override void Delete()
        {
            base.Delete();

            if (_Moongates.Contains(this))
                _Moongates.Remove(this);
        }

        private static void DeleteAll()
        {
            List<PublicMoongate> list = new List<PublicMoongate>();

            foreach (PublicMoongate item in _Moongates)
            {
                list.Add(item);
            }

            foreach (PublicMoongate gate in list)
                gate.Delete();

            if (list.Count > 0)
                World.Broadcast(0x35, true, "{0} moongates removed.", list.Count);
        }

        private static int MoonGen(PMList list)
        {
            foreach (PMEntry entry in list.Entries)
            {
                Item item = new PublicMoongate();

                item.MoveToWorld(entry.Location, list.Map);

                if (entry.Number == 1060642) // Umbra
                    item.Hue = 0x497;
            }

            return list.Entries.Length;
        }
    }

    public class PMEntry
    {
        private readonly Point3D m_Location;
        private readonly int m_Number;
        public PMEntry(Point3D loc, int number)
        {
            this.m_Location = loc;
            this.m_Number = number;
        }

        public Point3D Location
        {
            get
            {
                return this.m_Location;
            }
        }
        public int Number
        {
            get
            {
                return this.m_Number;
            }
        }
    }

    public class PMList
    {
        public static readonly PMList Trammel =
            new PMList(1012000, 1012012, Map.Trammel, new PMEntry[]
            {
                new PMEntry(new Point3D(4467, 1283, 5), 1012003), // Moonglow
                new PMEntry(new Point3D(1336, 1997, 5), 1012004), // Britain
                new PMEntry(new Point3D(1499, 3771, 5), 1012005), // Jhelom
                new PMEntry(new Point3D(771, 752, 5), 1012006), // Yew
                new PMEntry(new Point3D(2701, 692, 5), 1012007), // Minoc
                new PMEntry(new Point3D(1828, 2948,-20), 1012008), // Trinsic
                new PMEntry(new Point3D(643, 2067, 5), 1012009), // Skara Brae
                /* Dynamic Z for Magincia to support both old and new maps. */
                new PMEntry(new Point3D(3563, 2139, Map.Trammel.GetAverageZ(3563, 2139)), 1012010), // (New) Magincia
                new PMEntry(new Point3D(3450, 2677, 25), 1078098)// New Haven
            });
        public static readonly PMList Felucca =
            new PMList(1012001, 1012013, Map.Felucca, new PMEntry[]
            {
                new PMEntry(new Point3D(4467, 1283, 5), 1012003), // Moonglow
                new PMEntry(new Point3D(1336, 1997, 5), 1012004), // Britain
                new PMEntry(new Point3D(1499, 3771, 5), 1012005), // Jhelom
                new PMEntry(new Point3D(771, 752, 5), 1012006), // Yew
                new PMEntry(new Point3D(2701, 692, 5), 1012007), // Minoc
                new PMEntry(new Point3D(1828, 2948,-20), 1012008), // Trinsic
                new PMEntry(new Point3D(643, 2067, 5), 1012009), // Skara Brae
                /* Dynamic Z for Magincia to support both old and new maps. */
                new PMEntry(new Point3D(3563, 2139, Map.Felucca.GetAverageZ(3563, 2139)), 1012010), // (New) Magincia
                new PMEntry(new Point3D(2711, 2234, 0), 1019001)// Buccaneer's Den
            });
        public static readonly PMList Ilshenar =
            new PMList(1012002, 1012014, Map.Ilshenar, new PMEntry[]
            {
                new PMEntry(new Point3D(1215, 467, -13), 1012015), // Compassion
                new PMEntry(new Point3D(722, 1366, -60), 1012016), // Honesty
                new PMEntry(new Point3D(744, 724, -28), 1012017), // Honor
                new PMEntry(new Point3D(281, 1016, 0), 1012018), // Humility
                new PMEntry(new Point3D(987, 1011, -32), 1012019), // Justice
                new PMEntry(new Point3D(1174, 1286, -30), 1012020), // Sacrifice
                new PMEntry(new Point3D(1532, 1340, - 3), 1012021), // Spirituality
                new PMEntry(new Point3D(528, 216, -45), 1012022), // Valor
                new PMEntry(new Point3D(1721, 218, 96), 1019000)// Chaos
            });
        public static readonly PMList Malas =
            new PMList(1060643, 1062039, Map.Malas, new PMEntry[]
            {
                new PMEntry(new Point3D(1015, 527, -65), 1060641), // Luna
                new PMEntry(new Point3D(1997, 1386, -85), 1060642)// Umbra
            });
        public static readonly PMList Tokuno =
            new PMList(1063258, 1063415, Map.Tokuno, new PMEntry[]
            {
                new PMEntry(new Point3D(1169, 998, 41), 1063412), // Isamu-Jima
                new PMEntry(new Point3D(802, 1204, 25), 1063413), // Makoto-Jima
                new PMEntry(new Point3D(270, 628, 15), 1063414)// Homare-Jima
            });
        public static readonly PMList TerMur =
            new PMList(1113602, 1113602, Map.TerMur, new PMEntry[]
			{
				new PMEntry(new Point3D(852, 3526, -43), 1113603), // Royal City
				new PMEntry(new Point3D(926, 3989, -36), 1112572), // Holy City
			});
        public static readonly PMList[] UORLists = new PMList[] { Trammel, Felucca };
        public static readonly PMList[] UORListsYoung = new PMList[] { Trammel };
        public static readonly PMList[] LBRLists = new PMList[] { Trammel, Felucca, Ilshenar };
        public static readonly PMList[] LBRListsYoung = new PMList[] { Trammel, Ilshenar };
        public static readonly PMList[] AOSLists = new PMList[] { Trammel, Felucca, Ilshenar, Malas };
        public static readonly PMList[] AOSListsYoung = new PMList[] { Trammel, Ilshenar, Malas };
        public static readonly PMList[] SELists = new PMList[] { Trammel, Felucca, Ilshenar, Malas, Tokuno };
        public static readonly PMList[] SEListsYoung = new PMList[] { Trammel, Ilshenar, Malas, Tokuno };
		public static readonly PMList[] SALists	= new PMList[] { Trammel, Felucca, Ilshenar, Malas, Tokuno, TerMur };
		public static readonly PMList[] SAListsYoung = new PMList[] { Trammel, Ilshenar, Malas, Tokuno, TerMur };
        public static readonly PMList[] RedLists = new PMList[] { Felucca };
        public static readonly PMList[] SigilLists = new PMList[] { Felucca };
        private readonly int m_Number;
        private readonly int m_SelNumber;
        private readonly Map m_Map;
        private readonly PMEntry[] m_Entries;
        public PMList(int number, int selNumber, Map map, PMEntry[] entries)
        {
            this.m_Number = number;
            this.m_SelNumber = selNumber;
            this.m_Map = map;
            this.m_Entries = entries;
        }

        public int Number
        {
            get
            {
                return this.m_Number;
            }
        }
        public int SelNumber
        {
            get
            {
                return this.m_SelNumber;
            }
        }
        public Map Map
        {
            get
            {
                return this.m_Map;
            }
        }
        public PMEntry[] Entries
        {
            get
            {
                return this.m_Entries;
            }
        }
    }

    public class MoongateGump : Gump
    {
        private readonly Mobile m_Mobile;
        private readonly Item m_Moongate;
        private readonly PMList[] m_Lists;
        public MoongateGump(Mobile mobile, Item moongate)
            : base(100, 100)
        {
            this.m_Mobile = mobile;
            this.m_Moongate = moongate;

            PMList[] checkLists;

            if (mobile.Player)
            {
                if (Factions.Sigil.ExistsOn(mobile))
                {
                    checkLists = PMList.SigilLists;
                }
                else if (mobile.Kills >= 5)
                {
                    checkLists = PMList.RedLists;
                }
                else
                {
                    ClientFlags flags = mobile.NetState == null ? ClientFlags.None : mobile.NetState.Flags;
                    bool young = mobile is PlayerMobile ? ((PlayerMobile)mobile).Young : false;

                    if (Core.SA && (flags & ClientFlags.TerMur) != 0)
                        checkLists = young ? PMList.SAListsYoung : PMList.SALists;
                    else if (Core.SE && (flags & ClientFlags.Tokuno) != 0)
                        checkLists = young ? PMList.SEListsYoung : PMList.SELists;
                    else if (Core.AOS && (flags & ClientFlags.Malas) != 0)
                        checkLists = young ? PMList.AOSListsYoung : PMList.AOSLists;
                    else if ((flags & ClientFlags.Ilshenar) != 0)
                        checkLists = young ? PMList.LBRListsYoung : PMList.LBRLists;
                    else
                        checkLists = young ? PMList.UORListsYoung : PMList.UORLists;
                }
            }
            else
            {
                checkLists = PMList.SELists;
            }

            this.m_Lists = new PMList[checkLists.Length];

            for (int i = 0; i < this.m_Lists.Length; ++i)
                this.m_Lists[i] = checkLists[i];

            for (int i = 0; i < this.m_Lists.Length; ++i)
            {
                if (this.m_Lists[i].Map == mobile.Map)
                {
                    PMList temp = this.m_Lists[i];

                    this.m_Lists[i] = this.m_Lists[0];
                    this.m_Lists[0] = temp;

                    break;
                }
            }

            this.AddPage(0);

            this.AddBackground(0, 0, 380, 280, 5054);

            this.AddButton(10, 210, 4005, 4007, 1, GumpButtonType.Reply, 0);
            this.AddHtmlLocalized(45, 210, 140, 25, 1011036, false, false); // OKAY

            this.AddButton(10, 235, 4005, 4007, 0, GumpButtonType.Reply, 0);
            this.AddHtmlLocalized(45, 235, 140, 25, 1011012, false, false); // CANCEL

            this.AddHtmlLocalized(5, 5, 200, 20, 1012011, false, false); // Pick your destination:

            for (int i = 0; i < checkLists.Length; ++i)
            {
                this.AddButton(10, 35 + (i * 25), 2117, 2118, 0, GumpButtonType.Page, Array.IndexOf(this.m_Lists, checkLists[i]) + 1);
                this.AddHtmlLocalized(30, 35 + (i * 25), 150, 20, checkLists[i].Number, false, false);
            }

            for (int i = 0; i < this.m_Lists.Length; ++i)
                this.RenderPage(i, Array.IndexOf(checkLists, this.m_Lists[i]));
        }

        public override void OnResponse(NetState state, RelayInfo info)
        {
            if (info.ButtonID == 0) // Cancel
                return;
            else if (this.m_Mobile.Deleted || this.m_Moongate.Deleted || this.m_Mobile.Map == null)
                return;

            int[] switches = info.Switches;

            if (switches.Length == 0)
                return;

            int switchID = switches[0];
            int listIndex = switchID / 100;
            int listEntry = switchID % 100;

            if (listIndex < 0 || listIndex >= this.m_Lists.Length)
                return;

            PMList list = this.m_Lists[listIndex];

            if (listEntry < 0 || listEntry >= list.Entries.Length)
                return;

            PMEntry entry = list.Entries[listEntry];

            if (!this.m_Mobile.InRange(this.m_Moongate.GetWorldLocation(), 1) || this.m_Mobile.Map != this.m_Moongate.Map)
            {
                this.m_Mobile.SendLocalizedMessage(1019002); // You are too far away to use the gate.
            }
            else if (this.m_Mobile.Player && this.m_Mobile.Kills >= 5 && list.Map != Map.Felucca)
            {
                this.m_Mobile.SendLocalizedMessage(1019004); // You are not allowed to travel there.
            }
            else if (Factions.Sigil.ExistsOn(this.m_Mobile) && list.Map != Factions.Faction.Facet)
            {
                this.m_Mobile.SendLocalizedMessage(1019004); // You are not allowed to travel there.
            }
            else if (this.m_Mobile.Criminal)
            {
                this.m_Mobile.SendLocalizedMessage(1005561, "", 0x22); // Thou'rt a criminal and cannot escape so easily.
            }
            else if (SpellHelper.CheckCombat(this.m_Mobile))
            {
                this.m_Mobile.SendLocalizedMessage(1005564, "", 0x22); // Wouldst thou flee during the heat of battle??
            }
            else if (this.m_Mobile.Spell != null)
            {
                this.m_Mobile.SendLocalizedMessage(1049616); // You are too busy to do that at the moment.
            }
            else if (this.m_Mobile.Map == list.Map && this.m_Mobile.InRange(entry.Location, 1))
            {
                this.m_Mobile.SendLocalizedMessage(1019003); // You are already there.
            }
            else
            {
                BaseCreature.TeleportPets(this.m_Mobile, entry.Location, list.Map);

                this.m_Mobile.Combatant = null;
                this.m_Mobile.Warmode = false;
                this.m_Mobile.Hidden = true;

                this.m_Mobile.MoveToWorld(entry.Location, list.Map);

                Effects.PlaySound(entry.Location, list.Map, 0x1FE);

                Server.Engines.CityLoyalty.CityTradeSystem.OnPublicMoongateUsed(m_Mobile);
            }
        }

        private void RenderPage(int index, int offset)
        {
            PMList list = this.m_Lists[index];

            this.AddPage(index + 1);

            this.AddButton(10, 35 + (offset * 25), 2117, 2118, 0, GumpButtonType.Page, index + 1);
            this.AddHtmlLocalized(30, 35 + (offset * 25), 150, 20, list.SelNumber, false, false);

            PMEntry[] entries = list.Entries;

            for (int i = 0; i < entries.Length; ++i)
            {
                this.AddRadio(200, 35 + (i * 25), 210, 211, false, (index * 100) + i);
                this.AddHtmlLocalized(225, 35 + (i * 25), 150, 20, entries[i].Number, false, false);
            }
        }
    }
}