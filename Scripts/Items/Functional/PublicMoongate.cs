#region References
using Server.Commands;
using Server.Engines.CityLoyalty;
using Server.Gumps;
using Server.Mobiles;
using Server.Network;
using Server.Spells;
using System;
using System.Collections.Generic;
using System.Linq;
#endregion

namespace Server.Items
{
    public class PublicMoongate : Item
    {
        public static List<PublicMoongate> Moongates { get; private set; }

        static PublicMoongate()
        {
            Moongates = new List<PublicMoongate>();
        }

        public static void Initialize()
        {
            CommandSystem.Register("MoonGen", AccessLevel.Administrator, MoonGen_OnCommand);
            CommandSystem.Register("MoonGenDelete", AccessLevel.Administrator, MoonGenDelete_OnCommand);
        }

        [Usage("MoonGen")]
        [Description("Generates public moongates. Removes all old moongates.")]
        public static void MoonGen_OnCommand(CommandEventArgs e)
        {
            DeleteAll();

            int count = 0;

            if (!Siege.SiegeShard)
            {
                count += MoonGen(PMList.Trammel);
            }

            count += MoonGen(PMList.Felucca);
            count += MoonGen(PMList.Ilshenar);
            count += MoonGen(PMList.Malas);
            count += MoonGen(PMList.Tokuno);
            count += MoonGen(PMList.TerMur);

            World.Broadcast(0x35, true, "{0} moongates generated.", count);
        }

        private static int MoonGen(PMList list)
        {
            foreach (PMEntry entry in list.Entries)
            {
                PublicMoongate o = new PublicMoongate();

                o.MoveToWorld(entry.Location, list.Map);

                if (entry.Number == 1060642) // Umbra
                {
                    o.Hue = 0x497;
                }
            }

            return list.Entries.Length;
        }

        [Usage("MoonGenDelete")]
        [Description("Removes all public moongates.")]
        public static void MoonGenDelete_OnCommand(CommandEventArgs e)
        {
            DeleteAll();
        }

        public static void DeleteAll()
        {
            int count = Moongates.Count;

            int index = count;

            while (--index >= 0)
            {
                if (index < Moongates.Count)
                    Moongates[index].Delete();
            }

            Moongates.Clear();

            if (count > 0)
            {
                World.Broadcast(0x35, true, "{0:#,0} moongates removed.", count);
            }
        }

        public static IEnumerable<PublicMoongate> FindGates(Map map)
        {
            PublicMoongate o;

            int i = Moongates.Count;

            while (--i >= 0)
            {
                o = Moongates[i];

                if (o == null || o.Deleted)
                {
                    Moongates.RemoveAt(i);
                }
                else if (o.Map == map)
                {
                    yield return o;
                }
            }
        }

        public override int LabelNumber => 1023952;  // Blue Moongate

        public override bool HandlesOnMovement => true;
        public override bool ForceShowProperties => true;

        [Constructable]
        public PublicMoongate()
            : base(0xF6C)
        {
            Movable = false;
            Light = LightType.Circle300;

            Moongates.Add(this);
        }

        public PublicMoongate(Serial serial)
            : base(serial)
        {
            Moongates.Add(this);
        }

        public override void OnDelete()
        {
            base.OnDelete();

            Moongates.Remove(this);
        }

        public override void OnAfterDelete()
        {
            base.OnAfterDelete();

            Moongates.Remove(this);
        }

        public override void OnDoubleClick(Mobile m)
        {
            if (m.InRange(GetWorldLocation(), 1))
            {
                UseGate(m);
            }
        }

        public override bool OnMoveOver(Mobile m)
        {
            if (m.Player && m.CanSee(this))
            {
                UseGate(m);
            }

            return m.Map == Map && m.InRange(this, 1);
        }

        public override void OnMovement(Mobile m, Point3D oldLocation)
        {
            if (m.Player && !Utility.InRange(m.Location, Location, 1) && Utility.InRange(oldLocation, Location, 1))
            {
                m.CloseGump(typeof(MoongateGump));
            }
        }

        public virtual bool CanUseGate(Mobile m, bool message)
        {
            if (m.IsStaff())
            {
                //Staff can always use a gate!
                return true;
            }

            if (m.Criminal)
            {
                // Thou'rt a criminal and cannot escape so easily.
                m.SendLocalizedMessage(1005561, "", 0x22);
                return false;
            }

            if (SpellHelper.CheckCombat(m))
            {
                // Wouldst thou flee during the heat of battle??
                m.SendLocalizedMessage(1005564, "", 0x22);
                return false;
            }

            if (m.Spell != null)
            {
                // You are too busy to do that at the moment.
                m.SendLocalizedMessage(1049616);
                return false;
            }

            if (m.Holding != null)
            {
                // You cannot teleport while dragging an object.
                m.SendLocalizedMessage(1071955);
                return false;
            }

            return true;
        }

        public bool UseGate(Mobile m)
        {
            if (!CanUseGate(m, true))
            {
                return false;
            }

            m.CloseGump(typeof(MoongateGump));
            m.SendGump(new MoongateGump(m, this));

            PlaySound(m);

            return true;
        }

        public virtual void PlaySound(Mobile m)
        {
            if (!m.Hidden || m.IsPlayer())
            {
                Effects.PlaySound(m.Location, m.Map, 0x20E);
            }
            else
            {
                m.SendSound(0x20E);
            }
        }

        protected PMEntry FindEntry()
        {
            return FindEntry(PMList.GetList(Map));
        }

        protected PMEntry FindEntry(PMList list)
        {
            if (list != null)
            {
                return PMList.FindEntry(list, Location);
            }

            return null;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            reader.ReadInt();
        }
    }

    public class PMEntry
    {
        public Point3D Location { get; private set; }
        public Map Map { get; private set; }
        public TextDefinition Number { get; private set; }
        public TextDefinition Desc { get; private set; }

        public PMEntry(Point3D loc, TextDefinition number)
            : this(loc, number, string.Empty, null)
        { }

        public PMEntry(Point3D loc, TextDefinition number, Map map)
            : this(loc, number, string.Empty, map)
        { }

        public PMEntry(Point3D loc, TextDefinition number, TextDefinition desc)
            : this(loc, number, desc, null)
        { }

        public PMEntry(Point3D loc, TextDefinition number, TextDefinition desc, Map map)
        {
            Location = loc;
            Map = map;
            Number = number;
            Desc = desc;
        }
    }

    public class PMList
    {
        public static readonly PMList Trammel = new PMList(
            1012000,
            1012012,
            Map.Trammel,
            new[]
            {
                new PMEntry(new Point3D(4467, 1283, 5), 1012003), // Moonglow
				new PMEntry(new Point3D(1336, 1997, 5), 1012004), // Britain
				new PMEntry(new Point3D(1499, 3771, 5), 1012005), // Jhelom
				new PMEntry(new Point3D(771, 752, 5), 1012006), // Yew
				new PMEntry(new Point3D(2701, 692, 5), 1012007), // Minoc
				new PMEntry(new Point3D(1828, 2948, -20), 1012008), // Trinsic
				new PMEntry(new Point3D(643, 2067, 5), 1012009), // Skara Brae
				/* Dynamic Z for Magincia to support both old and new maps. */
				new PMEntry(new Point3D(3563, 2139, Map.Trammel.GetAverageZ(3563, 2139)), 1012010), // (New) Magincia
				new PMEntry(new Point3D(3450, 2677, 25), 1078098) // New Haven
			});

        public static readonly PMList Felucca = new PMList(
            1012001,
            1012013,
            Map.Felucca,
            new[]
            {
                new PMEntry(new Point3D(4467, 1283, 5), 1012003), // Moonglow
				new PMEntry(new Point3D(1336, 1997, 5), 1012004), // Britain
				new PMEntry(new Point3D(1499, 3771, 5), 1012005), // Jhelom
				new PMEntry(new Point3D(771, 752, 5), 1012006), // Yew
				new PMEntry(new Point3D(2701, 692, 5), 1012007), // Minoc
				new PMEntry(new Point3D(1828, 2948, -20), 1012008), // Trinsic
				new PMEntry(new Point3D(643, 2067, 5), 1012009), // Skara Brae
				/* Dynamic Z for Magincia to support both old and new maps. */
				new PMEntry(new Point3D(3563, 2139, Map.Felucca.GetAverageZ(3563, 2139)), 1012010), // (New) Magincia
				new PMEntry(new Point3D(2711, 2234, 0), 1019001) // Buccaneer's Den
			});

        public static readonly PMList Ilshenar = new PMList(
            1012002,
            1012014,
            Map.Ilshenar,
            new[]
            {
                new PMEntry(new Point3D(1215, 467, -13), 1012015), // Compassion
				new PMEntry(new Point3D(722, 1366, -60), 1012016), // Honesty
				new PMEntry(new Point3D(744, 724, -28), 1012017), // Honor
				new PMEntry(new Point3D(281, 1016, 0), 1012018), // Humility
				new PMEntry(new Point3D(987, 1011, -32), 1012019), // Justice
				new PMEntry(new Point3D(1174, 1286, -30), 1012020), // Sacrifice
				new PMEntry(new Point3D(1532, 1340, -3), 1012021), // Spirituality
				new PMEntry(new Point3D(528, 216, -45), 1012022), // Valor
				new PMEntry(new Point3D(1721, 218, 96), 1019000) // Chaos
			});

        public static readonly PMList Malas = new PMList(
            1060643,
            1062039,
            Map.Malas,
            new[]
            {
                new PMEntry(new Point3D(1015, 527, -65), 1060641), // Luna
				new PMEntry(new Point3D(1997, 1386, -85), 1060642) // Umbra
			});

        public static readonly PMList Tokuno = new PMList(
            1063258,
            1063415,
            Map.Tokuno,
            new[]
            {
                new PMEntry(new Point3D(1169, 998, 41), 1063412), // Isamu-Jima
				new PMEntry(new Point3D(802, 1204, 25), 1063413), // Makoto-Jima
				new PMEntry(new Point3D(270, 628, 15), 1063414) // Homare-Jima
			});

        public static readonly PMList TerMur = new PMList(
            1113602,
            1113604,
            Map.TerMur,
            new[]
            {
                new PMEntry(new Point3D(850, 3525, -38), 1113603), // Royal City
				new PMEntry(new Point3D(719, 1863, 40), 1156262) // Valley of Eodon
            });

        public static readonly PMList[] Lists = { Trammel, Felucca, Ilshenar, Malas, Tokuno, TerMur };
        public static readonly PMList[] ListsYoung = { Trammel, Ilshenar, Malas, Tokuno, TerMur };
        public static readonly PMList[] RedLists = { Felucca };
        public static readonly PMList[] SigilLists = { Felucca };

        public static readonly PMList[] AllLists = { Trammel, Felucca, Ilshenar, Malas, Tokuno, TerMur };

        public static PMList GetList(Map map)
        {
            if (map == null || map == Map.Internal)
            {
                return null;
            }

            if (map == Map.Trammel)
            {
                return Trammel;
            }

            if (map == Map.Felucca)
            {
                return Felucca;
            }

            if (map == Map.Ilshenar)
            {
                return Ilshenar;
            }

            if (map == Map.Malas)
            {
                return Malas;
            }

            if (map == Map.Tokuno)
            {
                return Tokuno;
            }

            if (map == Map.TerMur)
            {
                return TerMur;
            }

            return null;
        }

        public static int IndexOfEntry(PMEntry entry)
        {
            PMList list = AllLists.FirstOrDefault(o => o.Entries.Contains(entry));

            return IndexOfEntry(list, entry);
        }

        public static int IndexOfEntry(PMList list, PMEntry entry)
        {
            if (list != null && entry != null)
            {
                return Array.IndexOf(list.Entries, entry);
            }

            return -1;
        }

        public static PMEntry FindEntry(PMList list, Point3D loc)
        {
            if (list != null)
            {
                return list.Entries.FirstOrDefault(o => o.Location == loc);
            }

            return null;
        }

        public static PMEntry FindEntry(Map map, Point3D loc)
        {
            PMList list = GetList(map);

            if (list != null)
            {
                return FindEntry(list, loc);
            }

            return null;
        }

        private readonly TextDefinition m_Number;
        private readonly TextDefinition m_SelNumber;
        private readonly Map m_Map;
        private readonly PMEntry[] m_Entries;

        public PMList(TextDefinition number, TextDefinition selNumber, Map map, PMEntry[] entries)
        {
            m_Number = number;
            m_SelNumber = selNumber;
            m_Map = map;
            m_Entries = entries;
        }

        public TextDefinition Number => m_Number;
        public TextDefinition SelNumber => m_SelNumber;
        public Map Map => m_Map;
        public PMEntry[] Entries => m_Entries;
    }

    public class MoongateGump : Gump
    {
        private readonly Mobile m_Mobile;
        private readonly Item m_Moongate;
        private readonly PMList[] m_Lists;

        public MoongateGump(Mobile mobile, Item moongate)
            : base(100, 100)
        {
            m_Mobile = mobile;
            m_Moongate = moongate;

            PMList[] checkLists;

            if (mobile.Player)
            {
                if (mobile.IsStaff())
                {
                    checkLists = PMList.Lists;
                }
                else if (Engines.VvV.VvVSigil.ExistsOn(mobile))
                {
                    checkLists = PMList.SigilLists;
                }
                else if (SpellHelper.RestrictRedTravel && mobile.Murderer && !Siege.SiegeShard)
                {
                    checkLists = PMList.RedLists;
                }
                else
                {
                    bool young = mobile is PlayerMobile && ((PlayerMobile)mobile).Young;

                    checkLists = young ? PMList.ListsYoung : PMList.Lists;
                }
            }
            else
            {
                checkLists = PMList.Lists;
            }

            m_Lists = new PMList[checkLists.Length];

            for (int i = 0; i < m_Lists.Length; ++i)
            {
                m_Lists[i] = checkLists[i];
            }

            for (int i = 0; i < m_Lists.Length; ++i)
            {
                if (m_Lists[i].Map == mobile.Map)
                {
                    PMList temp = m_Lists[i];

                    m_Lists[i] = m_Lists[0];
                    m_Lists[0] = temp;

                    break;
                }
            }

            AddPage(0);

            AddBackground(0, 0, 380, 280, 5054);

            AddButton(10, 210, 4005, 4007, 1, GumpButtonType.Reply, 0);
            AddHtmlLocalized(45, 210, 140, 25, 1011036, false, false); // OKAY

            AddButton(10, 235, 4005, 4007, 0, GumpButtonType.Reply, 0);
            AddHtmlLocalized(45, 235, 140, 25, 1011012, false, false); // CANCEL

            AddHtmlLocalized(5, 5, 200, 20, 1012011, false, false); // Pick your destination:

            for (int i = 0; i < checkLists.Length; ++i)
            {
                if (Siege.SiegeShard && checkLists[i].Number == 1012000) // Trammel
                {
                    continue;
                }

                AddButton(10, 35 + (i * 25), 2117, 2118, 0, GumpButtonType.Page, Array.IndexOf(m_Lists, checkLists[i]) + 1);

                if (checkLists[i].Number.Number > 0)
                {
                    AddHtmlLocalized(30, 35 + (i * 25), 150, 20, checkLists[i].Number.Number, false, false);
                }
                else if (!string.IsNullOrEmpty(checkLists[i].Number.String))
                {
                    AddHtml(30, 35 + (i * 25), 150, 20, checkLists[i].Number.String, false, false);
                }
            }

            for (int i = 0; i < m_Lists.Length; ++i)
            {
                RenderPage(i, Array.IndexOf(checkLists, m_Lists[i]));
            }
        }

        public override void OnResponse(NetState state, RelayInfo info)
        {
            if (info.ButtonID == 0) // Cancel
            {
                return;
            }
            if (m_Mobile.Deleted || m_Moongate.Deleted || m_Mobile.Map == null)
            {
                return;
            }

            int[] switches = info.Switches;

            if (switches.Length == 0)
            {
                return;
            }

            int switchID = switches[0];
            int listIndex = switchID / 100;
            int listEntry = switchID % 100;

            if (listIndex < 0 || listIndex >= m_Lists.Length)
            {
                return;
            }

            var list = m_Lists[listIndex];

            if (listEntry < 0 || listEntry >= list.Entries.Length)
            {
                return;
            }

            var entry = list.Entries[listEntry];
            var map = entry.Map ?? list.Map;

            if (m_Mobile.Map == map && m_Mobile.InRange(entry.Location, 1))
            {
                m_Mobile.SendLocalizedMessage(1019003); // You are already there.
                return;
            }
            if (m_Mobile.IsStaff())
            {
                //Staff can always use a gate!
            }
            else if (!m_Mobile.InRange(m_Moongate.GetWorldLocation(), 1) || m_Mobile.Map != m_Moongate.Map)
            {
                m_Mobile.SendLocalizedMessage(1019002); // You are too far away to use the gate.
                return;
            }
            else if (m_Mobile.Player && SpellHelper.RestrictRedTravel && m_Mobile.Murderer && map != Map.Felucca && !Siege.SiegeShard)
            {
                m_Mobile.SendLocalizedMessage(1019004); // You are not allowed to travel there.
                return;
            }
            else if (Engines.VvV.VvVSigil.ExistsOn(m_Mobile) && map != Engines.VvV.ViceVsVirtueSystem.Facet)
            {
                m_Mobile.SendLocalizedMessage(1019004); // You are not allowed to travel there.
                return;
            }
            else if (m_Mobile.Criminal)
            {
                m_Mobile.SendLocalizedMessage(1005561, "", 0x22); // Thou'rt a criminal and cannot escape so easily.
                return;
            }
            else if (SpellHelper.CheckCombat(m_Mobile))
            {
                m_Mobile.SendLocalizedMessage(1005564, "", 0x22); // Wouldst thou flee during the heat of battle??
                return;
            }
            else if (m_Mobile.Spell != null)
            {
                m_Mobile.SendLocalizedMessage(1049616); // You are too busy to do that at the moment.
                return;
            }

            BaseCreature.TeleportPets(m_Mobile, entry.Location, map);

            m_Mobile.Combatant = null;
            m_Mobile.Warmode = false;
            m_Mobile.Hidden = true;

            m_Mobile.MoveToWorld(entry.Location, map);

            Effects.PlaySound(entry.Location, map, 0x1FE);

            CityTradeSystem.OnQuickTravelUsed(m_Mobile);
        }

        private void RenderPage(int index, int offset)
        {
            PMList list = m_Lists[index];

            if (Siege.SiegeShard && list.Number == 1012000) // Trammel
                return;

            AddPage(index + 1);

            AddButton(10, 35 + (offset * 25), 2117, 2118, 0, GumpButtonType.Page, index + 1);

            if (list.SelNumber.Number > 0)
            {
                AddHtmlLocalized(30, 35 + (offset * 25), 150, 20, list.SelNumber.Number, false, false);
            }
            else if (!string.IsNullOrEmpty(list.SelNumber.String))
            {
                AddHtml(30, 35 + (offset * 25), 150, 20, list.SelNumber.String, false, false);
            }

            PMEntry[] entries = list.Entries;

            for (int i = 0; i < entries.Length; ++i)
            {
                AddRadio(200, 35 + (i * 25), 210, 211, false, (index * 100) + i);

                if (entries[i].Number.Number > 0)
                {
                    AddHtmlLocalized(225, 35 + (i * 25), 150, 20, entries[i].Number.Number, false, false);
                }
                else if (!string.IsNullOrEmpty(entries[i].Number.String))
                {
                    AddHtml(225, 35 + (i * 25), 150, 20, entries[i].Number.String, false, false);
                }
            }
        }
    }
}
