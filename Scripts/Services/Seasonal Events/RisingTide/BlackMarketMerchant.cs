using Server.Engines.Points;
using Server.Items;
using Server.Mobiles;

namespace Server.Engines.RisingTide
{
    public class BlackMarketMerchant : BaseTurnInMobile
    {
        public static BlackMarketMerchant InstanceTram { get; set; }
        public static BlackMarketMerchant InstanceFel { get; set; }
        public static Point3D SpawnLocation => new Point3D(2719, 2187, 0);

        public override int TitleLocalization => 1158918;  // Maritime Black Market
        public override int CancelLocalization => 1158911; 	// Bring me maritime trade cargo and I will pay you in doubloons!
        public override int TurnInLocalization => 1158912;  // Sell Maritime Trade Cargo
        public override int ClaimLocalization => 1158913;  // Buy Pirate Loot

        public override bool IsShrineHealer => false;

        [Constructable]
        public BlackMarketMerchant() : base("the Pirate")
        {
        }

        public override void InitBody()
        {
            base.InitBody();

            Name = NameList.RandomName("male");

            SpeechHue = 0x3B2;
            Hue = Utility.RandomSkinHue();
            Body = 0x190;

            PackItem(new QuartermasterRewardDeed());
            PackItem(new SailingMasterRewardDeed());
            PackItem(new BotswainRewardDeed());
            PackItem(new PowderMonkeyRewardDeed());
            PackItem(new SpikedWhipOfPlundering());
            PackItem(new BladedWhipOfPlundering());
            PackItem(new BarbedWhipOfPlundering());
            PackItem(new TritonStatue());
        }

        public override void InitOutfit()
        {
            SetWearable(new ElvenPants(), 1307);
            SetWearable(new Shirt(), 1209);
            SetWearable(new JinBaori(), 2017);
            SetWearable(new Sandals(), 0);
            SetWearable(new Bandana(), 2051);
            SetWearable(new BarbedWhip(), 2721);
            SetWearable(new ShoulderParrot(), 18);
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1158914); // Maritime Black Market Merchant
        }

        public override void AwardPoints(PlayerMobile pm, Item item, int amount)
        {
            if (item is MaritimeCargo)
            {
                PointsSystem.RisingTide.AwardPoints(pm, ((MaritimeCargo)item).GetAwardAmount());
            }
        }

        public override bool IsRedeemableItem(Item item)
        {
            return item is MaritimeCargo;
        }

        public override void SendRewardGump(Mobile m)
        {
            if (m.Player && m.CheckAlive())
                m.SendGump(new RisingTideRewardGump(this, m as PlayerMobile));
        }

        public override void Delete()
        {
            base.Delete();

            if (InstanceTram == this)
            {
                InstanceTram = null;
            }
            else if (InstanceFel == this)
            {
                InstanceFel = null;
            }
        }

        public BlackMarketMerchant(Serial serial) : base(serial)
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
            int version = reader.ReadInt();

            if (Map == Map.Trammel)
            {
                InstanceTram = this;
            }
            else if (Map == Map.Felucca)
            {
                InstanceFel = this;
            }
        }
    }
}
