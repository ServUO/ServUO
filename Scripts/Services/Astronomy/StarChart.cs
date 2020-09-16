using Server.Engines.Astronomy;
using Server.Engines.Craft;
using Server.Gumps;
using Server.Mobiles;
using Server.Targeting;
using System;

namespace Server.Items
{
    [Flipable(0xA1E4, 0xA1E5)]
    public class StarChart : Item, ICraftable
    {
        private int _Constellation;
        private string _Name;
        private Mobile _ChartedBy;
        private DateTime _ChartedOn;

        [CommandProperty(AccessLevel.GameMaster)]
        public int Constellation
        {
            get { return _Constellation; }
            set
            {
                _Constellation = value;

                if (_Constellation < 0)
                {
                    Hue = 2500;
                }
                else
                {
                    Hue = 0;
                }

                InvalidateProperties();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public string ConstellationName { get { return _Name; } set { _Name = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public Mobile ChartedBy { get { return _ChartedBy; } set { _ChartedBy = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime ChartedOn { get { return _ChartedOn; } set { _ChartedOn = value; } }

        public override int LabelNumber => _Constellation == -1 ? 1158743 : 1158493;  // An Indecipherable Star Chart : Star Chart

        [Constructable]
        public StarChart()
            : base(0xA1E4)
        {
            _Constellation = -1;
            Hue = 2500;
        }

        public int OnCraft(int quality, bool makersMark, Mobile from, CraftSystem craftSystem, Type typeRes, ITool tool, CraftItem craftItem, int resHue)
        {
            Timer.DelayCall(() => SendTarget(from));

            return quality;
        }

        public void SendTarget(Mobile m)
        {
            m.SendLocalizedMessage(1158494); // Which telescope do you wish to create the star chart from?
            m.BeginTarget(10, false, TargetFlags.None, (from, targeted) =>
                {
                    if (!Deleted && IsChildOf(from.Backpack) && targeted is PersonalTelescope)
                    {
                        PersonalTelescope tele = (PersonalTelescope)targeted;

                        ConstellationInfo constellation = AstronomySystem.GetConstellation(tele.TimeCoordinate, tele.RA, tele.DEC);

                        if (constellation != null)
                        {
                            from.SendLocalizedMessage(1158496); // You successfully map the time-coordinate of the constellation.

                            ChartedBy = from;
                            ChartedOn = DateTime.Now;
                            Constellation = constellation.Identifier;
                            from.PlaySound(0x249);
                        }
                        else
                        {
                            from.SendLocalizedMessage(1158495); // There is nothing to chart at these coordinates at this time.
                        }
                    }
                });
        }

        public override void OnDoubleClick(Mobile m)
        {
            if (m is PlayerMobile && IsChildOf(m.Backpack) && _Constellation > -1)
            {
                BaseGump.SendGump(new InternalGump((PlayerMobile)m, this));
            }
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            if (_Constellation > -1)
            {
                if (_ChartedBy != null)
                {
                    list.Add(1158500, _ChartedBy.Name); // Charted By: ~1_NAME~
                }

                list.Add(1158501, _Name ?? "A Constellation With No Name"); // TODO: Get if null
            }
        }

        public class InternalGump : BaseGump
        {
            public StarChart Chart { get; set; }

            public InternalGump(PlayerMobile pm, StarChart chart)
                : base(pm, 50, 50)
            {
                pm.CloseGump(typeof(InternalGump));

                Chart = chart;
            }

            public override void AddGumpLayout()
            {
                ConstellationInfo info = AstronomySystem.GetConstellation(Chart.Constellation);

                AddPage(0);

                AddBackground(0, 0, 454, 350, 0x24AE);
                AddHtmlLocalized(32, 68, 112, 36, 1158505, false, false); // Constellation Name:
                AddHtml(154, 68, 300, 36, Color("#0040FF", string.IsNullOrEmpty(Chart.ConstellationName) ? "This constellation has not yet been named" : Chart.ConstellationName), false, false);

                AddHtmlLocalized(32, 104, 75, 36, 1158502, false, false); // Charted By:
                AddHtml(112, 104, 50, 36, Color("#0040FF", Chart.ChartedBy == null ? string.Empty : Chart.ChartedBy.Name), false, false);

                AddHtmlLocalized(32, 140, 75, 36, 1158503, false, false); // Charted On:
                AddHtml(112, 140, 80, 36, Color("#0040FF", Chart.ChartedOn.ToShortDateString()), false, false);

                AddHtmlLocalized(32, 176, 125, 18, 1158504, false, false); // Time-Coordinate:
                AddHtmlLocalized(47, 199, 60, 36, AstronomySystem.TimeCoordinateLocalization(info.TimeCoordinate), 0x1F, false, false);

                AddHtmlLocalized(157, 199, 20, 36, 1158489, false, false); // RA
                AddHtml(182, 199, 20, 36, Color("#0040FF", info.CoordRA.ToString()), false, false);

                AddHtmlLocalized(242, 199, 25, 36, 1158490, false, false); // DEC
                AddHtml(272, 199, 50, 36, Color("#0040FF", info.CoordDEC.ToString()), false, false);

                AddBackground(32, 253, 343, 22, 0x2486);
                AddTextEntry(34, 255, 339, 18, 0, 1, string.Empty, 34);

                AddButton(375, 245, 0x232C, 0x232D, 1, GumpButtonType.Reply, 0);
            }

            public override void OnResponse(RelayInfo info)
            {
                if (info.ButtonID == 1 && Chart != null && !Chart.Deleted && Chart.Constellation >= 0)
                {
                    TextRelay relay = info.GetTextEntry(1);

                    if (relay != null && relay.Text != null)
                    {
                        string text = relay.Text;

                        if (Guilds.BaseGuildGump.CheckProfanity(text) &&
                            !AstronomySystem.CheckNameExists(text) &&
                            text.Length > 0 &&
                            text.Length < 37)
                        {
                            Chart.ConstellationName = text;
                            User.SendLocalizedMessage(1158512); // You record the name of the constellation.
                        }
                        else
                        {
                            User.SendLocalizedMessage(1158511); // You have entered an invalid name. Please try again.
                        }
                    }
                }
            }
        }

        public StarChart(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);

            writer.Write(_Constellation);
            writer.Write(_Name);
            writer.Write(_ChartedBy);
            writer.Write(_ChartedOn);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();

            _Constellation = reader.ReadInt();
            _Name = reader.ReadString();
            _ChartedBy = reader.ReadMobile();
            _ChartedOn = reader.ReadDateTime();
        }
    }
}