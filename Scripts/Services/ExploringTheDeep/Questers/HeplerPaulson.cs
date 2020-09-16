using Server.Commands;
using Server.Engines.Quests;
using Server.Gumps;
using Server.Items;
using Server.Network;
using System.Collections.Generic;

namespace Server.Mobiles
{
    public class HeplerPaulson : BaseVendor
    {
        private readonly List<SBInfo> m_SBInfos = new List<SBInfo>();
        protected override List<SBInfo> SBInfos => m_SBInfos;

        public override void InitSBInfo()
        {
            m_SBInfos.Add(new SBHepler());
        }

        [Constructable]
        public HeplerPaulson()
            : base("The Salvage Master")
        {
            Name = "Hepler Paulson";
            Race = Race.Human;
            CantWalk = true;
            Hue = Utility.RandomSkinHue();
            Blessed = true;

            Utility.AssignRandomHair(this);
        }

        public override void InitOutfit()
        {
            AddItem(new Backpack());
            AddItem(new Shoes(0x737));
            AddItem(new LongPants(0x1BB));
            AddItem(new FancyShirt(0x535));
        }

        public HeplerPaulson(Serial serial) : base(serial)
        {
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            list.Add(1072269); // Quest Giver
        }

        public override void OnDoubleClick(Mobile m)
        {
            if (!(m is PlayerMobile))
                return;

            PlayerMobile pm = (PlayerMobile)m;

            if (pm.Young)
            {
                m.SendLocalizedMessage(502593); // Thou art too young to choose this fate.
                return;
            }

            Item boots = m.Backpack.FindItemByType(typeof(BootsOfBallast));
            Item robe = m.Backpack.FindItemByType(typeof(CanvassRobe));
            Item neck = m.Backpack.FindItemByType(typeof(AquaPendant));
            Item lens = m.Backpack.FindItemByType(m.Race == Race.Gargoyle ? typeof(GargishNictitatingLens) : typeof(NictitatingLens));

            if (pm.ExploringTheDeepQuest == ExploringTheDeepQuestChain.None)
            {
                if (!m.HasGump(typeof(HeplerPaulsonGump)))
                {
                    BaseGump.SendGump(new HeplerPaulsonGump(m as PlayerMobile));
                    pm.ExploringTheDeepQuest = ExploringTheDeepQuestChain.HeplerPaulson;
                }
            }
            else if (pm.ExploringTheDeepQuest == ExploringTheDeepQuestChain.CollectTheComponent && boots != null && robe != null && neck != null && lens != null)
            {
                pm.ExploringTheDeepQuest = ExploringTheDeepQuestChain.CollectTheComponentComplete;
                m.AddToBackpack(new UnknownShipwreck());
            }
            else if (pm.ExploringTheDeepQuest == ExploringTheDeepQuestChain.CollectTheComponentComplete)
            {
                BaseGump.SendGump(new HeplerPaulsonCollectCompleteGump(m as PlayerMobile));
            }
            else
            {
                if (!m.HasGump(typeof(HeplerPaulsonCompleteGump)))
                {
                    BaseGump.SendGump(new HeplerPaulsonCompleteGump(m as PlayerMobile));
                }
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }

        public override bool OnDragDrop(Mobile from, Item dropped)
        {
            PlayerMobile m = from as PlayerMobile;

            if (m != null)
            {
                if (dropped is BrokenShipwreckRemains)
                {
                    if (m.ExploringTheDeepQuest == ExploringTheDeepQuestChain.HeplerPaulson)
                    {
                        dropped.Delete();
                        BaseGump.SendGump(new HeplerPaulsonCompleteGump(m as PlayerMobile));
                        m.ExploringTheDeepQuest = ExploringTheDeepQuestChain.HeplerPaulsonComplete;
                    }
                    else if (m.ExploringTheDeepQuest >= ExploringTheDeepQuestChain.HeplerPaulsonComplete)
                    {
                        m.SendLocalizedMessage(1154320); // You've already completed this task.
                    }
                    else
                    {
                        m.SendLocalizedMessage(1154325); // You feel as though by doing this you are missing out on an important part of your journey...
                    }
                }
                else
                {
                    PublicOverheadMessage(MessageType.Regular, 0x3B2, 501550); // I am not interested in 
                }
            }
            return false;
        }
    }

    public class HeplerPaulsonGump : StoryGump
    {
        public static void Initialize()
        {
            CommandSystem.Register("HeplerPaulson", AccessLevel.GameMaster, HeplerPaulsonGump_OnCommand);
        }

        private static void HeplerPaulsonGump_OnCommand(CommandEventArgs e)
        {
            SendGump(new HeplerPaulsonGump(e.Mobile as PlayerMobile));
        }

        private static readonly PageData[] GumpInfo =
        {
                new PageData(1, 1154279, new SelectionEntry(1154280, 2), new SelectionEntry(1154282, 3)),
                new PageData(2, 1154281, new SelectionEntry(1154282, 4)),
                new PageData(3, 1154283, new SelectionEntry(1154280, 5)),
                new PageData(4, 1154283),
                new PageData(5, 1154281)
        };

        public HeplerPaulsonGump(PlayerMobile pm)
            : base(pm, 1154327, GumpInfo)
        {

        }
    }

    public class HeplerPaulsonCompleteGump : StoryGump
    {
        public static void Initialize()
        {
            CommandSystem.Register("HeplerPaulsonComplete", AccessLevel.GameMaster, HeplerPaulsonCompleteGump_OnCommand);
        }

        private static readonly PageData[] GumpInfo =
        {
            new PageData(1, 1154284,  new SelectionEntry(1154285, 2)),
            new PageData(2, 1154286)
        };

        private static void HeplerPaulsonCompleteGump_OnCommand(CommandEventArgs e)
        {
            SendGump(new HeplerPaulsonCompleteGump(e.Mobile as PlayerMobile));
        }

        public HeplerPaulsonCompleteGump(PlayerMobile pm)
            : base(pm, 1154327, GumpInfo)
        {
        }
    }

    public class HeplerPaulsonCollectCompleteGump : StoryGump
    {
        public static void Initialize()
        {
            CommandSystem.Register("HeplerPaulsonCollectComplete", AccessLevel.GameMaster, HeplerPaulsonCollectCompleteGump_OnCommand);
        }

        private static void HeplerPaulsonCollectCompleteGump_OnCommand(CommandEventArgs e)
        {
            SendGump(new HeplerPaulsonCollectCompleteGump(e.Mobile as PlayerMobile));
        }

        private static readonly PageData[] GumpInfo =
        {
            new PageData(1, 1154319),
        };

        public HeplerPaulsonCollectCompleteGump(PlayerMobile pm)
            : base(pm, 1154327, GumpInfo)
        {
        }
    }
}
