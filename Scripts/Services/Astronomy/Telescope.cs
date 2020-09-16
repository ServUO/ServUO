using Server.ContextMenus;
using Server.Engines.Astronomy;
using Server.Gumps;
using Server.Mobiles;
using Server.Multis;
using System;
using System.Collections.Generic;

namespace Server.Items
{
    [Flipable(0xA12C, 0xA12D)]
    public class PersonalTelescope : Item, ISecurable
    {
        private string _DisplayName;
        private double _DEC;

        //[CommandProperty(AccessLevel.GameMaster)]
        //public static TimeCoordinate ForceTimeCoordinate { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public string DisplayName { get { return _DisplayName; } set { _DisplayName = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public SecureLevel Level { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public int RA { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public double DEC
        {
            get { return _DEC; }
            set
            {
                _DEC = value;

                _DEC = (double)decimal.Round((decimal)_DEC, 2);
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public TimeCoordinate TimeCoordinate => AstronomySystem.GetTimeCoordinate(this);

        public DateTime LastUse { get; set; }

        public override int LabelNumber => 1125284;

        [Constructable]
        public PersonalTelescope()
            : base(0xA12C)
        {
            Level = SecureLevel.Owner;

            _DisplayName = _Names[Utility.Random(_Names.Length)];
        }

        public override void OnDoubleClick(Mobile m)
        {
            if (!IsLockedDown)
            {
                m.SendLocalizedMessage(1114298); // This must be locked down in order to use it.
                return;
            }

            if (m.InRange(Location, 2))
            {
                BaseHouse house = BaseHouse.FindHouseAt(this);

                if (house != null && house.HasSecureAccess(m, Level))
                {
                    if (DateTime.UtcNow - LastUse > TimeSpan.FromMinutes(10))
                    {
                        m.SendLocalizedMessage(1158643); // The telescope is calibrating, try again in a moment.

                        LastUse = DateTime.UtcNow;
                    }
                    else
                    {
                        BaseGump.SendGump(new TelescopeGump((PlayerMobile)m, this));
                    }
                }
            }
            else
            {
                m.LocalOverheadMessage(Network.MessageType.Regular, 0x3B2, 1019045); // I can't reach that.
            }
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            if (!string.IsNullOrEmpty(_DisplayName))
            {
                list.Add(1158477, _DisplayName); // <BASEFONT COLOR=#FFD24D>From the personal study of ~1_NAME~<BASEFONT COLOR=#FFFFFF>
            }
        }

        public override void GetContextMenuEntries(Mobile from, List<ContextMenuEntry> list)
        {
            base.GetContextMenuEntries(from, list);

            SetSecureLevelEntry.AddTo(from, this, list);
        }

        public PersonalTelescope(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(1);

            writer.Write(_DisplayName);

            writer.Write((int)Level);
            writer.Write(RA);
            writer.Write(DEC);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            switch (version)
            {
                case 1:
                    _DisplayName = reader.ReadString();
                    goto case 0;
                case 0:
                    Level = (SecureLevel)reader.ReadInt();
                    RA = reader.ReadInt();
                    DEC = reader.ReadDouble();
                    break;
            }

            if (version == 0)
            {
                _DisplayName = _Names[Utility.Random(_Names.Length)];
            }
        }

        private static readonly string[] _Names =
        {
            "Adranath", "Aeluva the Arcanist", "Aesthyron", "Anon", "Balaki", "Clanin", "Dexter", "Doctor Spector", "Dryus Doost",
            "Gilform", "Grizelda the Hag", "Hawkwind", "Heigel of Moonglow", "Intanya", "Juo'Nar", "King Blackthorn", "Koole the Arcanist",
            "Kronos", "Kyrnia", "Lathiari", "Leoric Gathenwale", "Lysander Gathenwale", "Malabelle", "Mariah", "Melissa", "Minax",
            "Mondain", "Mordra", "Mythran", "Neira the Necromancer", "Nystul", "Queen Zhah", "Relvinian", "Selsius the Astronomer",
            "Sutek", "Uzeraan", "Wexton the Apprentice"
        };
    }

    public class TelescopeGump : BaseGump
    {
        public Tuple<int, int> InterstellarObject { get; set; }
        public ConstellationInfo Constellation { get; set; }

        public PersonalTelescope Tele { get; set; }
        public int ImageID { get; set; }

        public TelescopeGump(PlayerMobile pm, PersonalTelescope tele)
            : base(pm, 200, 200)
        {
            Tele = tele;

            pm.CloseGump(typeof(TelescopeGump));
        }

        public override void AddGumpLayout()
        {
            AddPage(0);

            if (ImageID == 0)
            {
                ImageID = AstronomySystem.RandomSkyImage(User);
            }

            AddImage(0, 0, ImageID);

            AddImage(222, 597, 0x694);
            AddImage(229, 600, GetGumpNumber(GetPlace(Tele.RA, 10)));

            AddButton(222, 584, 0x697, 0x698, 60000, GumpButtonType.Reply, 0);
            AddButton(222, 631, 0x699, 0x69A, 60001, GumpButtonType.Reply, 0);

            AddImage(256, 597, 0x694);
            AddImage(263, 600, GetGumpNumber(GetPlace(Tele.RA, 1)));

            AddButton(256, 584, 0x697, 0x698, 60002, GumpButtonType.Reply, 0);
            AddButton(256, 631, 0x699, 0x69A, 60003, GumpButtonType.Reply, 0);

            AddButton(291, 597, 0x69B, 0x69C, 70000, GumpButtonType.Reply, 0);
            AddTooltip(1158499); // View Coordinate

            AddImage(332, 597, 0x694);
            AddImage(339, 600, GetGumpNumber(GetPlace((int)Math.Truncate(Tele.DEC), 10)));

            AddButton(332, 584, 0x697, 0x698, 60004, GumpButtonType.Reply, 0);
            AddButton(332, 631, 0x699, 0x69A, 60005, GumpButtonType.Reply, 0);

            AddImage(366, 597, 0x694);
            AddImage(373, 600, GetGumpNumber(GetPlace((int)Math.Truncate(Tele.DEC), 1)));

            AddButton(366, 584, 0x697, 0x698, 60006, GumpButtonType.Reply, 0);
            AddButton(366, 631, 0x699, 0x69A, 60007, GumpButtonType.Reply, 0);

            AddImage(400, 597, 0x694);
            AddImage(407, 600, GetGumpNumber(GetDecimalPlace(Tele.DEC)));

            AddButton(400, 584, 0x697, 0x698, 60008, GumpButtonType.Reply, 0);
            AddButton(400, 631, 0x699, 0x69A, 60009, GumpButtonType.Reply, 0);

            AddImage(397, 623, 0x696);

            AddHtmlLocalized(251, 651, 100, 50, 1158489, 0x6B55, false, false); // RA
            AddTooltip(1158497); // Right Ascension

            AddHtmlLocalized(371, 651, 100, 50, 1158490, 0x6B55, false, false); // DEC
            AddTooltip(1158498); // Declination

            if (Constellation != null)
            {
                RenderConstellation();
            }
            else if (InterstellarObject != null)
            {
                RenderInterstellarObject();
            }
        }

        private void RenderInterstellarObject()
        {
            AddImage(180, 150, InterstellarObject.Item1);
        }

        private void RenderConstellation()
        {
            foreach (ConstellationInfo.StarPosition pos in Constellation.StarPositions)
            {
                AddImage(pos.X, pos.Y, pos.ImageID);
            }
        }

        public override void OnResponse(RelayInfo info)
        {
            if (!User.InRange(Tele.Location, 2) || User.Map != Tele.Map)
            {
                return;
            }

            Tele.LastUse = DateTime.UtcNow;

            switch (info.ButtonID)
            {
                case 60000: // RA 10's Up
                    if (Tele.RA >= 20)
                        Tele.RA -= 20;
                    else
                        Tele.RA += 10;
                    User.SendSound(0x4A);
                    break;
                case 60001: // RA 10's Down
                    if (Tele.RA < 10)
                        Tele.RA += 20;
                    else
                        Tele.RA -= 10;
                    User.SendSound(0x4A);
                    break;
                case 60002: // RA 1's Up
                    int raOnes = GetPlace(Tele.RA, 1);

                    if (raOnes >= 9)
                        Tele.RA -= 9;
                    else
                        Tele.RA++;
                    User.SendSound(0x4A);
                    break;
                case 60003: // RA 1's Down
                    int raOnes1 = GetPlace(Tele.RA, 1);

                    if (raOnes1 == 0)
                        Tele.RA += 9;
                    else
                        Tele.RA--;
                    User.SendSound(0x4A);
                    break;
                case 60004: // DEC 10's Up
                    if (Tele.DEC >= 90)
                        Tele.DEC -= 90;
                    else
                        Tele.DEC += 10;
                    User.SendSound(0x4A);
                    break;
                case 60005: // DEC 10's Down
                    if (Tele.DEC < 10)
                        Tele.DEC += 90;
                    else
                        Tele.DEC -= 10;
                    User.SendSound(0x4A);
                    break;
                case 60006: // DEC 1's Up
                    int decOnes = GetPlace((int)Math.Truncate(Tele.DEC), 1);

                    if (decOnes >= 9)
                        Tele.DEC -= 9;
                    else
                        Tele.DEC++;
                    User.SendSound(0x4A);
                    break;
                case 60007: // DEC 1's Down
                    int decOnes1 = GetPlace((int)Math.Truncate(Tele.DEC), 1);

                    if (decOnes1 <= 0)
                        Tele.DEC += 9;
                    else
                        Tele.DEC--;
                    User.SendSound(0x4A);
                    break;
                case 60008: // DEC .2 Up
                    int dec = GetDecimalPlace(Tele.DEC);

                    if (dec >= 8)
                        Tele.DEC = Math.Truncate(Tele.DEC);
                    else
                        Tele.DEC += .2;
                    User.SendSound(0x4A);
                    break;
                case 60009: // DEC .2 Down
                    int dec1 = GetDecimalPlace(Tele.DEC);

                    if (dec1 < 2)
                        Tele.DEC += 0.8;
                    else if (dec1 == 2)
                        Tele.DEC = Math.Truncate(Tele.DEC);
                    else
                        Tele.DEC -= 0.2;

                    User.SendSound(0x4A);
                    break;
                case 70000: // View Coord
                    if (Tele.RA > AstronomySystem.MaxRA || Tele.DEC > AstronomySystem.MaxDEC)
                    {
                        User.SendLocalizedMessage(1158488); // You have entered invalid coordinates.
                        User.SendSound(81);
                    }
                    else
                    {
                        InterstellarObject = null;
                        Constellation = null;
                        ImageID = AstronomySystem.RandomSkyImage(User);

                        TimeCoordinate timeCoord = Tele.TimeCoordinate;

                        if (timeCoord == TimeCoordinate.Day)
                        {
                            User.SendLocalizedMessage(1158513); // You won't have much luck seeing the night sky during the day...
                        }
                        else
                        {
                            ConstellationInfo constellation = AstronomySystem.GetConstellation(timeCoord, Tele.RA, Tele.DEC);

                            if (constellation != null)
                            {
                                Constellation = constellation;

                                User.SendLocalizedMessage(1158492, "", 0xBF); // You peer into the heavens and see...a constellation!
                                User.SendSound(User.Female ? 0x32B : 0x43D);
                            }
                            else if (0.2 > Utility.RandomDouble())
                            {
                                InterstellarObject = AstronomySystem.GetRandomInterstellarObject();

                                User.SendLocalizedMessage(InterstellarObject.Item2, "", 0xBF); // 
                                User.SendSound(User.Female ? 0x32B : 0x43D);
                            }
                            else
                            {
                                User.SendLocalizedMessage(1158491, "", 0xBF); // You peer into the heavens and see...only empty space...
                            }
                        }
                    }

                    Refresh();
                    return;
            }

            if (info.ButtonID != 0)
            {
                Refresh();
            }
        }

        private int GetPlace(int value, int place)
        {
            return ((value % (place * 10)) - (value % place)) / place;
        }

        private int GetDecimalPlace(double value)
        {
            decimal dec = decimal.Round((decimal)(value - Math.Truncate(value)), 2);

            return (int)(dec * 10);
        }

        private int GetGumpNumber(int v)
        {
            return 0x58F + v;
        }
    }
}
