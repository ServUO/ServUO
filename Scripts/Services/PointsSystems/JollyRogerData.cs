#region

using Server.Engines.JollyRoger;
using Server.Engines.SeasonalEvents;
using Server.Mobiles;
using System.Collections.Generic;
using System.Linq;

#endregion

namespace Server.Engines.Points
{
    public class JollyRogerData : PointsSystem
    {
        private static List<RewardArray> _List = new List<RewardArray>();

        public override PointsType Loyalty => PointsType.JollyRogerData;
        public override TextDefinition Name => "Jolly Roger";
        public override bool AutoAdd => true;
        public override double MaxPoints => double.MaxValue;
        public override bool ShowOnLoyaltyGump => false;

        public bool InSeason => SeasonalEventSystem.IsActive(EventType.JollyRoger);

        public bool Enabled { get; set; }
        public bool QuestContentGenerated { get; set; }

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

            if (list != null && list.Shrine != null)
            {
                var count = list.Shrine.FirstOrDefault(x => x.Shrine == shrine).FragmentCount;
                var title = GetTitle(shrine);

                if (m is PlayerMobile pm && (pm.ShrineTitle == 0 || pm.ShrineTitle != title && list.Shrine.Any(x => x.FragmentCount < count && x.Shrine != shrine)))
                {
                    pm.ShrineTitle = title;
                }
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);

            writer.Write(Enabled);
            writer.Write(QuestContentGenerated);

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
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            var version = reader.ReadInt();

            switch (version)
            {
                case 0:
                    Enabled = reader.ReadBool();
                    QuestContentGenerated = reader.ReadBool();

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
}
