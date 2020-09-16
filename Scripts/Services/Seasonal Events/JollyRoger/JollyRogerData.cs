using Server.Engines.Points;
using Server.Gumps;
using Server.Mobiles;
using Server.Network;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Server.Engines.JollyRoger
{
    public class JollyRogerData : PointsSystem
    {
        private static List<RewardArray> _List = new List<RewardArray>();

        public override PointsType Loyalty => PointsType.JollyRogerData;
        public override TextDefinition Name => "Jolly Roger";
        public override bool AutoAdd => true;
        public override double MaxPoints => double.MaxValue;
        public override bool ShowOnLoyaltyGump => false;

        public static void Initialize()
        {
            EventSink.Speech += EventSink_Speech;
        }

        public static void EventSink_Speech(SpeechEventArgs e)
        {
            string speech = e.Speech;
            Mobile m = e.Mobile;

            if (m.Region.Name == "Chaos Shrine" && !m.HasGump(typeof(RenounceVirtueGump)) && m is PlayerMobile pm && ShrineTitles.ContainsKey(pm) &&
                speech.IndexOf("I renounce virtue", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                m.SendGump(new RenounceVirtueGump());
            }
        }

        public static Dictionary<PlayerMobile, int> ShrineTitles { get; set; } = new Dictionary<PlayerMobile, int>();

        private static readonly List<ShrineDef> ShrineDef = new List<ShrineDef>
        {
            new ShrineDef(Shrine.Spirituality, 2500, 1159321),
            new ShrineDef(Shrine.Compassion, 1912, 1159327),
            new ShrineDef(Shrine.Honor, 1918, 1159325),
            new ShrineDef(Shrine.Honesty, 1916, 1159326),
            new ShrineDef(Shrine.Humility, 1910, 1159324),
            new ShrineDef(Shrine.Justice, 1914 , 1159323),
            new ShrineDef(Shrine.Valor, 1920, 1159320),
            new ShrineDef(Shrine.Sacrifice, 1922, 1159322)
        };

        public static void DisplayTitle(PlayerMobile pm, ObjectPropertyList list)
        {
            var title = GetShrineTitle(pm);

            if (title > 0)
            {
                list.Add(title);
            }
        }

        public static int GetShrineTitle(PlayerMobile pm)
        {
            if (ShrineTitles.ContainsKey(pm))
            {
                return ShrineTitles[pm];
            }

            return 0;
        }

        public static void SetShrineTitle(PlayerMobile pm, int title)
        {
            ShrineTitles[pm] = title;
            pm.InvalidateProperties();
        }

        public static RewardArray GetList(Mobile m)
        {
            return _List.FirstOrDefault(x => x.Mobile == m);
        }

        public static void SetCloak(Mobile m, bool b)
        {
            GetList(m).Cloak = b;
        }

        public static void SetTabard(Mobile m, bool t)
        {
            GetList(m).Tabard = t;
        }

        public static int FragmentRandomHue()
        {
            return Utility.RandomList(ShrineDef.Select(x => x.Hue).ToArray());
        }

        public static int GetShrineHue(Shrine shrine)
        {
            return ShrineDef.FirstOrDefault(x => x.Shrine == shrine).Hue;
        }

        public static Shrine GetShrine(int cliloc)
        {
            return ShrineDef.Find(x => x.TitleCliloc == cliloc).Shrine;
        }

        public static Shrine GetShrine(Item item)
        {
            return ShrineDef.FirstOrDefault(x => x.Hue == item.Hue).Shrine;
        }

        public static int GetTitle(Shrine shrine)
        {
            return ShrineDef.FirstOrDefault(x => x.Shrine == shrine).TitleCliloc;
        }

        public static void AddMasterKill(Mobile m, Shrine shrine)
        {
            var list = _List.FirstOrDefault(x => x.Mobile == m);

            if (list != null && list.Shrine != null)
            {
                if (list.Shrine.Any(y => y.Shrine == shrine))
                {
                    _List.FirstOrDefault(x => x.Mobile == m).Shrine.FirstOrDefault(y => y.Shrine == shrine).MasterDeath++;
                }
                else
                {
                    _List.FirstOrDefault(x => x.Mobile == m).Shrine.Add(new ShrineArray { Shrine = shrine, MasterDeath = 1 });
                }
            }
            else
            {
                var sa = new List<ShrineArray>
                {
                    new ShrineArray {Shrine = shrine, MasterDeath = 1 }
                };

                var ra = new RewardArray(m, sa);

                _List.Add(ra);
            }
        }

        public static void FragmentIncrease(Mobile m, Shrine shrine)
        {
            if (m == null)
            {
                return;
            }

            var list = _List.FirstOrDefault(x => x.Mobile == m);

            if (list != null && list.Shrine != null)
            {
                if (list.Shrine.Any(y => y.Shrine == shrine))
                {
                    _List.FirstOrDefault(x => x.Mobile == m).Shrine.FirstOrDefault(y => y.Shrine == shrine).FragmentCount++;
                }
                else
                {
                    _List.FirstOrDefault(x => x.Mobile == m).Shrine.Add(new ShrineArray { Shrine = shrine, FragmentCount = 1 });
                }
            }
            else
            {
                var sa = new List<ShrineArray>
                {
                    new ShrineArray {Shrine = shrine, FragmentCount = 1 }
                };

                var ra = new RewardArray(m, sa);

                _List.Add(ra);
            }

            TitleCheck(m, shrine);
        }

        public static void TitleCheck(Mobile m, Shrine shrine)
        {
            var list = _List.FirstOrDefault(x => x.Mobile == m);
            var pm = m as PlayerMobile;

            if (pm != null && list != null && list.Shrine != null)
            {
                var count = list.Shrine.FirstOrDefault(x => x.Shrine == shrine).FragmentCount;
                var playerTitle = GetShrineTitle(pm);
                var shrineTitle = GetTitle(shrine);

                if (playerTitle == 0 || (playerTitle != shrineTitle && list.Shrine.Any(x => x.FragmentCount < count && x.Shrine != shrine)))
                {
                    SetShrineTitle(pm, shrineTitle);
                }
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(1);

            writer.Write(_List.Count);

            _List.ForEach(l =>
            {
                writer.Write(l.Mobile);
                writer.Write(l.Tabard);
                writer.Write(l.Cloak);

                writer.Write(l.Shrine.Count);

                l.Shrine.ForEach(s =>
                {
                    writer.Write((int)s.Shrine);
                    writer.Write(s.FragmentCount);
                    writer.Write(s.MasterDeath);
                });
            });

            writer.Write(ShrineTitles.Count);

            foreach (var kvp in ShrineTitles)
            {
                writer.WriteMobile(kvp.Key);
                writer.Write(kvp.Value);
            }
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            var version = reader.ReadInt();

            switch (version)
            {
                case 1:
                case 0:
                    if (version == 0)
                    {
                        reader.ReadBool();
                        var questGenerated = reader.ReadBool();

                        Timer.DelayCall(() =>
                        {
                            var jolly = SeasonalEvents.SeasonalEventSystem.GetEvent<JollyRogerEvent>();

                            if (jolly != null)
                            {
                                jolly.QuestContentGenerated = questGenerated;
                            }
                        });
                    }

                    var count = reader.ReadInt();

                    for (var i = count; i > 0; i--)
                    {
                        var m = reader.ReadMobile();
                        var t = reader.ReadBool();
                        var c = reader.ReadBool();

                        var temp = new List<ShrineArray>();

                        var scount = reader.ReadInt();

                        for (var s = scount; s > 0; s--)
                        {
                            var sh = (Shrine)reader.ReadInt();
                            var fc = reader.ReadInt();
                            var md = reader.ReadInt();

                            temp.Add(new ShrineArray { Shrine = sh, FragmentCount = fc, MasterDeath = md });
                        }

                        if (m != null)
                        {
                            _List.Add(new RewardArray(m, temp, t, c));
                        }
                    }

                    count = reader.ReadInt();

                    for (int i = 0; i < count; i++)
                    {
                        var pm = reader.ReadMobile<PlayerMobile>();
                        var title = reader.ReadInt();

                        if (pm != null)
                        {
                            ShrineTitles[pm] = title;
                        }
                    }
                    break;
            }
        }
    }

    public class ShrineDef
    {
        public Shrine Shrine { get; set; }
        public int Hue { get; set; }
        public int TitleCliloc { get; set; }

        public ShrineDef(Shrine s, int h, int tc)
        {
            Shrine = s;
            Hue = h;
            TitleCliloc = tc;
        }
    }

    public class RewardArray
    {
        public Mobile Mobile { get; set; }
        public List<ShrineArray> Shrine { get; set; }
        public bool Tabard { get; set; }
        public bool Cloak { get; set; }

        public RewardArray(Mobile m, List<ShrineArray> s)
        {
            Mobile = m;
            Shrine = s;
        }

        public RewardArray(Mobile m, List<ShrineArray> s, bool tabard, bool cloak)
        {
            Mobile = m;
            Shrine = s;
            Tabard = tabard;
            Cloak = cloak;
        }
    }

    public class ShrineArray
    {
        public Shrine Shrine { get; set; }
        public int FragmentCount { get; set; }
        public int MasterDeath { get; set; }
    }

    public class RenounceVirtueGump : Gump
    {
        public RenounceVirtueGump()
            : base(100, 100)
        {
            AddPage(0);

            AddBackground(0, 0, 320, 245, 0x6DB);
            AddHtmlLocalized(65, 10, 200, 20, 1114513, "#1159452", 0x67D5, false, false); // <DIV ALIGN=CENTER>~1_TOKEN~</DIV>
            AddHtmlLocalized(15, 50, 295, 100, 1159453, 0x72ED, false, false); // You are about to renounce your Shrine Battle virtue title. You may recover the title by placing a mysterious fragment at any shrine. Do you wish to proceed?
            AddButton(30, 200, 0x867, 0x869, 1, GumpButtonType.Reply, 0);
            AddButton(265, 200, 0x867, 0x869, 0, GumpButtonType.Reply, 0);
            AddHtmlLocalized(33, 180, 100, 50, 1046362, 0x7FFF, false, false); // Yes
            AddHtmlLocalized(273, 180, 100, 50, 1046363, 0x7FFF, false, false); // No
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            switch (info.ButtonID)
            {
                case 0:
                    {
                        break;
                    }
                case 1:
                    {

                        if (sender.Mobile is PlayerMobile pm && JollyRogerData.ShrineTitles.ContainsKey(pm))
                        {
                            JollyRogerData.ShrineTitles.Remove(pm);
                            pm.InvalidateProperties();
                        }

                        break;
                    }
            }
        }
    }
}
