using Server.Gumps;
using Server.Items;
using Server.Mobiles;
using System.Collections.Generic;

namespace Server.Engines.Astronomy
{
    public class Willebrord : BaseVendor
    {
        public static Willebrord TramInstance { get; set; }

        protected readonly List<SBInfo> m_SBInfos = new List<SBInfo>();
        protected override List<SBInfo> SBInfos => m_SBInfos;
        public override bool IsActiveVendor => false;

        public override void InitSBInfo()
        {
        }

        public static void Initialize()
        {
            if (TramInstance == null)
            {
                TramInstance = new Willebrord();
                TramInstance.MoveToWorld(new Point3D(4706, 1128, 6), Map.Trammel);
            }

            if (Map.Trammel.FindItem<AstronomyTent>(new Point3D(4707, 1127, 0)) == null)
            {
                AstronomyTent tent = new AstronomyTent();
                tent.MoveToWorld(new Point3D(4707, 1127, 0), Map.Trammel);
            }

            if (Map.Trammel.FindItem<PersonalTelescope>(new Point3D(4705, 1128, 0)) == null)
            {
                PersonalTelescope tele = new PersonalTelescope
                {
                    Movable = false
                };
                tele.MoveToWorld(new Point3D(4705, 1128, 0), Map.Trammel);
            }

            if (Map.Trammel.FindItem<BrassOrrery>(new Point3D(4705, 1126, 0)) == null)
            {
                BrassOrrery orrery = new BrassOrrery
                {
                    Movable = false
                };
                orrery.MoveToWorld(new Point3D(4705, 1126, 0), Map.Trammel);
            }

            if (Map.Trammel.FindItem<ConstellationLedger>(new Point3D(4709, 1127, 0)) == null)
            {
                ConstellationLedger ledger = new ConstellationLedger
                {
                    Movable = false
                };
                ledger.MoveToWorld(new Point3D(4709, 1127, 4), Map.Trammel);
            }

            if (Map.Trammel.FindItem<PrimerOnBritannianAstronomy>(new Point3D(4709, 1126, 0)) == null)
            {
                PrimerOnBritannianAstronomy book = new PrimerOnBritannianAstronomy
                {
                    Movable = false
                };
                book.MoveToWorld(new Point3D(4709, 1126, 4), Map.Trammel);
            }
        }

        public Willebrord()
            : base("the Astronomer")
        {
        }

        public override void InitBody()
        {
            InitStats(100, 100, 25);
            CantWalk = true;
            Name = "Willebrord";

            Race = Race.Human;
            Body = Race.MaleBody;

            HairItemID = Race.RandomHair(false);
            HairHue = Race.RandomHairHue();
        }

        public override void InitOutfit()
        {
            SetWearable(new Kamishimo());
            SetWearable(new ThighBoots(), 1908);
            SetWearable(new FancyShirt(), 1255);
        }

        public override void OnDoubleClick(Mobile m)
        {
            if (m.InRange(Location, 3) && InLOS(m))
            {
                Gump gump = new Gump(100, 100);
                gump.AddBackground(0, 0, 720, 270, 0x2454);
                gump.AddImage(0, 0, 0x69D);

                gump.AddHtmlLocalized(290, 14, 418, 18, 1114513, "#1158517", 0xC63, false, false);
                gump.AddHtmlLocalized(290, 51, 418, 209, 1158518, 0xC63, false, true);

                m.SendGump(gump);
            }
        }

        public override bool OnDragDrop(Mobile m, Item dropped)
        {
            if (dropped is StarChart)
            {
                StarChart chart = (StarChart)dropped;

                if (chart.Constellation >= 0 && chart.Constellation < AstronomySystem.MaxConstellations)
                {
                    if (string.IsNullOrEmpty(chart.ConstellationName))
                    {
                        m.SendLocalizedMessage(1158751); // You must name your constellation before submitting it.
                    }
                    else
                    {
                        ConstellationInfo info = AstronomySystem.GetConstellation(chart.Constellation);

                        if (info != null)
                        {
                            Gump gump = new Gump(100, 100);
                            gump.AddBackground(0, 0, 720, 270, 0x2454);
                            gump.AddImage(0, 0, 0x69D);
                            gump.AddHtmlLocalized(290, 14, 418, 18, 1114513, "#1158517", 0xC63, false, false); // Willebrord the Astronomer

                            if (info.HasBeenDiscovered)
                            {
                                m.SendLocalizedMessage(1158764); // That constellation name has already been chosen, please choose another and resubmit your star chart.
                                gump.AddHtmlLocalized(290, 51, 418, 209, 1158530, 0xC63, false, true);
                                // Sorry to say that constellation has already been discovered! Fix your eyes to the heavens and keep up the searc
                            }
                            else
                            {
                                gump.AddHtmlLocalized(290, 51, 418, 209, 1158519, 0xC63, false, true);
                                // Wow! Would you look at that! Always amazes me how even an amateur can make such profound discoveries!
                                // I've recorded your discovery in the ledger. Here's some items I think you have more than earned! Well done!

                                info.DiscoveredBy = chart.ChartedBy;
                                info.Name = chart.ConstellationName;
                                info.DiscoveredOn = chart.ChartedOn;
                                AstronomySystem.AddDiscovery(info);

                                m.AddToBackpack(new RecipeScroll(465));
                                m.AddToBackpack(new AstronomerTitleDeed());
                            }

                            m.SendGump(gump);
                        }
                    }
                }
            }
            else
            {
                SayTo(m, 1158529, 1163); // What's this? I haven't time for this! Star Charts only please!
            }

            return false;
        }

        public Willebrord(Serial serial)
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
            reader.ReadInt();

            if (Map == Map.Trammel)
            {
                TramInstance = this;
            }
        }
    }
}
