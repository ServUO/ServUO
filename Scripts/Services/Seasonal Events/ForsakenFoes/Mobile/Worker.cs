using Server.Engines.Quests;
using Server.Gumps;
using Server.Items;
using Server.Mobiles;
using System.Collections.Generic;
using System.IO;

namespace Server.Engines.Fellowship
{
    public enum FellowshipChain
    {
        None,
        One,
        Two,
        Three,
        Four,
        Five,
        Six,
        Seven,
        Eight
    }

    public class Worker : BaseQuester
    {
        public static string FilePath = Path.Combine("Saves/Misc", "FellowshipChain.bin");
        public static Dictionary<Mobile, FellowshipChain> FellowshipChainList = new Dictionary<Mobile, FellowshipChain>();

        public static void Configure()
        {
            EventSink.WorldSave += OnSave;
            EventSink.WorldLoad += OnLoad;
        }

        public static void OnSave(WorldSaveEventArgs e)
        {
            Persistence.Serialize(
                FilePath,
                writer =>
                {
                    writer.Write(0);

                    writer.Write(FellowshipChainList.Count);

                    foreach (KeyValuePair<Mobile, FellowshipChain> chain in FellowshipChainList)
                    {
                        writer.Write(chain.Key);
                        writer.Write((int)chain.Value);
                    }
                });
        }

        public static void OnLoad()
        {
            Persistence.Deserialize(
                FilePath,
                reader =>
                {
                    int version = reader.ReadInt();
                    int count = reader.ReadInt();

                    for (int i = count; i > 0; i--)
                    {
                        Mobile m = reader.ReadMobile();
                        FellowshipChain chain = (FellowshipChain)reader.ReadInt();

                        if (m != null)
                        {
                            FellowshipChainList[m] = chain;
                        }
                    }
                });
        }

        public static Worker InstanceTram { get; set; }
        public static Worker InstanceFel { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public FellowshipChain Chain { get; set; }

        [Constructable]
        public Worker(FellowshipChain chain)
            : base("the Worker")
        {
            Chain = chain;
        }

        public override void InitBody()
        {
            base.InitBody();

            Name = NameList.RandomName("male");

            SpeechHue = 0x3B2;
            Hue = Utility.RandomSkinHue();
            Body = 0x190;
        }

        public override void InitOutfit()
        {
            SetWearable(new FullApron(), 902);
            SetWearable(new HalfApron(), 946);
            SetWearable(new Shirt(), 1513);
            SetWearable(new LeatherGloves());
            SetWearable(new Boots(), 2013);
            SetWearable(new ShortPants());
            SetWearable(new SmithHammer());
        }

        public override bool OnDragDrop(Mobile from, Item item)
        {
            if (item is FellowshipCoin)
            {
                if (FellowshipChainList.ContainsKey(from))
                {
                    if (Chain > FellowshipChainList[from])
                    {
                        FellowshipChainList[from] = Chain;
                    }
                    else
                    {
                        SayTo(from, 500607, 0x3B2); // I'm not interested in that.
                        return false;
                    }

                    if (FellowshipChainList[from] == FellowshipChain.Eight)
                    {
                        Item medallion;

                        if (from.Race == Race.Gargoyle)
                        {
                            medallion = new GargishFellowshipMedallion();
                        }
                        else
                        {
                            medallion = new FellowshipMedallion();
                        }

                        from.AddToBackpack(medallion);
                    }
                }
                else
                {
                    FellowshipChainList.Add(from, Chain);
                }

                item.Delete();

                return true;
            }
            else
            {
                SayTo(from, 500607, 0x3B2); // I'm not interested in that.

                return false;
            }
        }

        public override void OnDoubleClick(Mobile m)
        {
            if (m != null && InRange(m.Location, 5))
            {
                if (!m.HasGump(typeof(WorkerGump)))
                {
                    m.SendGump(new WorkerGump(m, Chain));
                }
            }
        }

        public override void OnTalk(PlayerMobile player, bool contextMenu)
        {
            if (!player.HasGump(typeof(WorkerGump)))
            {
                player.SendGump(new WorkerGump(player, Chain));
            }
        }

        public class WorkerGump : Gump
        {
            private static readonly int[,] clilocs = new int[,]
            {
                {1159238, 1159239},
                {1159236, 1159240},
                {1159236, 1159241},
                {1159236, 1159242},
                {1159236, 1159243},
                {1159236, 1159244},
                {1159236, 1159245},
                {1159236, 1159246},
            };

            public WorkerGump(Mobile from, FellowshipChain chain)
                : base(100, 100)
            {
                int cliloc;

                if (FellowshipChainList.ContainsKey(from))
                {
                    if (chain > FellowshipChainList[from])
                        cliloc = clilocs[(int)(chain - 1), 0];
                    else
                        cliloc = clilocs[(int)(chain - 1), 1];
                }
                else
                {
                    cliloc = clilocs[(int)(chain - 1), 0];
                }

                AddPage(0);

                AddBackground(0, 0, 620, 328, 0x2454);
                AddImage(0, 0, 0x61A);
                AddHtmlLocalized(335, 14, 273, 18, 1114513, "#1159237", 0xC63, false, false); // <DIV ALIGN=CENTER>~1_TOKEN~</DIV>
                AddHtmlLocalized(335, 51, 273, 267, cliloc, 0xC63, false, true);
            }
        }

        public Worker(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);

            writer.Write((int)Chain);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            Chain = (FellowshipChain)reader.ReadInt();

            if (Map == Map.Trammel)
            {
                InstanceTram = this;
            }

            if (Map == Map.Felucca)
            {
                InstanceFel = this;
            }
        }
    }
}
