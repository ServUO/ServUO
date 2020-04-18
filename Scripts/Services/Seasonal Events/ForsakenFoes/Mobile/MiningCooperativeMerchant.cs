using Server.Gumps;
using Server.Items;
using Server.Mobiles;
using Server.Network;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Server.Engines.Quests
{
    public class CooperativeArray
    {
        public string Account { get; set; }
        public int Purchase { get; set; }
    }

    public class MiningCooperative
    {
        public static string FilePath = Path.Combine("Saves/Misc", "MiningCooperative.bin");
        private static List<CooperativeArray> PurchaseList = new List<CooperativeArray>();

        public static void Configure()
        {
            EventSink.WorldSave += OnSave;
            EventSink.WorldLoad += OnLoad;
        }

        public static void AddPurchase(Mobile from, int amount)
        {
            string acc = from.Account.ToString();

            if (CheckList(from))
            {
                PurchaseList.Find(x => x.Account == acc).Purchase += amount;
            }
            else
            {
                PurchaseList.Add(new CooperativeArray { Account = from.Account.ToString(), Purchase = amount });
            }
        }

        public static bool CheckList(Mobile from)
        {
            string acc = from.Account.ToString();

            return PurchaseList.Any(x => x.Account == acc);
        }

        public static int PurchaseAmount(Mobile from)
        {
            int amount = 0;

            if (CheckList(from))
            {
                amount = PurchaseList.FirstOrDefault(x => x.Account == from.Account.ToString()).Purchase;
            }

            return amount;
        }

        public static void DefragTables()
        {
            PurchaseList = new List<CooperativeArray>();
        }

        public static void OnSave(WorldSaveEventArgs e)
        {
            Persistence.Serialize(
                FilePath,
                writer =>
                {
                    writer.Write(0);

                    writer.Write(PurchaseList.Count);

                    PurchaseList.ForEach(s =>
                    {
                        writer.Write(s.Account);
                        writer.Write(s.Purchase);
                    });
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
                        string acc = reader.ReadString();
                        int purchase = reader.ReadInt();

                        if (acc != null)
                        {
                            PurchaseList.Add(new CooperativeArray { Account = acc, Purchase = purchase });
                        }
                    }
                });
        }
    }

    public class MiningCooperativeMerchant : BaseVendor
    {
        protected readonly List<SBInfo> m_SBInfos = new List<SBInfo>();
        protected override List<SBInfo> SBInfos => m_SBInfos;

        public override bool IsActiveVendor => false;
        public override bool IsInvulnerable => true;

        public int MaxAmount => 5000;
        public int Price => 112;
        public int Quantity => 500;

        public static MiningCooperativeMerchant InstanceTram { get; set; }
        public static MiningCooperativeMerchant InstanceFel { get; set; }

        public override void InitSBInfo()
        {
        }

        [Constructable]
        public MiningCooperativeMerchant()
            : base("the Mining Cooperative Merchant")
        {
        }

        public MiningCooperativeMerchant(Serial serial)
            : base(serial)
        {
        }

        public override void InitOutfit()
        {
            AddItem(new FancyShirt(0x3E4));
            AddItem(new LongPants(0x192));
            AddItem(new Pickaxe());
            AddItem(new ThighBoots(0x283));
        }

        public override void OnDoubleClick(Mobile from)
        {
            from.CloseGump(typeof(MiningCooperativeGump));
            from.SendGump(new MiningCooperativeGump(this, from));
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            if (Map == Map.Trammel)
            {
                InstanceTram = this;
            }

            if (Map == Map.Felucca)
            {
                InstanceFel = this;
            }
        }

        public class MiningCooperativeGump : Gump
        {
            private readonly MiningCooperativeMerchant Vendor;

            public MiningCooperativeGump(MiningCooperativeMerchant vendor, Mobile from)
                : base(100, 100)
            {
                Vendor = vendor;

                AddPage(0);

                int available = vendor.MaxAmount - MiningCooperative.PurchaseAmount(from);

                AddBackground(0, 0, 310, 350, 0x6DB);
                AddImage(54, 0, 0x6E4);
                AddHtmlLocalized(10, 10, 290, 18, 1114513, "#1154040", 0x0, false, false); // <DIV ALIGN=CENTER>~1_TOKEN~</DIV>
                AddItem(20, 80, 0xA3E8);
                AddHtmlLocalized(120, 73, 180, 18, 1159190, 0x43FF, false, false); // Ethereal Sand
                AddHtmlLocalized(120, 100, 180, 18, 1159191, vendor.Price.ToString(), 0x43FF, false, false); // GP: ~1_VALUE~
                AddItem(20, 140, 0x14F0);
                AddHtmlLocalized(120, 143, 180, 18, 1159193, string.Format("{0}@{1}", vendor.Quantity, vendor.Quantity * vendor.Price), 0x5FF0, false, false); // x~1_QUANT~ GP: ~2_COST~
                AddHtmlLocalized(25, 203, 275, 18, 1159192, string.Format("{0}@{1}", available, vendor.MaxAmount), 0x7FF0, false, false); // Available For Purchase: ~1_PART~ / ~2_WHOLE~
                AddHtmlLocalized(20, 243, 160, 72, 1159194, string.Format("{0}@#1159190@{1}", vendor.Quantity, vendor.Quantity * vendor.Price), 0x7FFF, false, false); // Purchase a Commodity Deed filled with ~1_QUANT~ ~2_NAME~ for ~3_COST~ GP?
                AddButton(220, 260, 0x81C, 0x81B, 1, GumpButtonType.Reply, 0);
            }

            public override void OnResponse(NetState sender, RelayInfo info)
            {
                Mobile from = sender.Mobile;

                int available = Vendor.MaxAmount - MiningCooperative.PurchaseAmount(from);
                int payment = Vendor.Quantity * Vendor.Price;

                if (info.ButtonID == 1)
                {
                    if (available > 0)
                    {
                        if (Banker.Withdraw(from, payment, true))
                        {
                            CommodityDeed deed = new CommodityDeed();
                            deed.SetCommodity(new EtherealSand(Vendor.Quantity));
                            from.AddToBackpack(deed);

                            MiningCooperative.AddPurchase(from, Vendor.Quantity);
                            from.SendGump(new MiningCooperativeGump(Vendor, from));
                        }
                        else
                        {
                            Vendor.Say(500192); // Begging thy pardon, but thou canst not afford that.
                        }
                    }
                    else
                    {
                        Vendor.Say(1159195); // Begging thy pardon, but your family has purchased the maximum amount of that commodity.  I cannot sell you more until a new shipment arrives!
                    }
                }
            }
        }
    }
}
