using Server.Engines.Points;
using Server.Items;
using Server.Mobiles;

namespace Server.Engines.SorcerersDungeon
{
    public class SorcerersDungeonResearcher : BaseTurnInMobile
    {
        public override int TitleLocalization => 1157657;  // Artifacts of Enchanted Origin
        public override int CancelLocalization => 1157616; 	// Bring me items of Enchanted Origin and I will reward you with valuable items.
        public override int TurnInLocalization => 1158763;  // Turn In Artifacts of Enchanted Origin
        public override int ClaimLocalization => 1155593;  // Claim Rewards

        public static SorcerersDungeonResearcher Instance { get; set; }

        [Constructable]
        public SorcerersDungeonResearcher() : base("the Researcher")
        {
        }

        public override void InitBody()
        {
            base.InitBody();

            Name = NameList.RandomName("male");

            Hue = Utility.RandomSkinHue();
            Body = 0x190;
            HairItemID = 0x2047;
            HairHue = 0x46D;
        }

        public override void InitOutfit()
        {
            SetWearable(new FancyShirt(), 1255);
            SetWearable(new JinBaori(), 2722);
            SetWearable(new Kilt(), 2012);
            SetWearable(new LongPants(), 1775);
            SetWearable(new ThighBoots(1908));
            SetWearable(new Necklace());

            Item item = new HeroOfTheUnlovedTitleDeed
            {
                Movable = false
            };
            PackItem(item);

            item = new SaviorOfTheDementedTitleDeed
            {
                Movable = false
            };
            PackItem(item);

            item = new SlayerOfThePumpkinKingTitleDeed
            {
                Movable = false
            };
            PackItem(item);

            item = new SterlingSilverRing
            {
                Movable = false
            };
            PackItem(item);

            item = new TalonsOfEscaping
            {
                Movable = false
            };
            PackItem(item);

            item = new BootsOfEscaping
            {
                Movable = false
            };
            PackItem(item);
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            list.Add(1157617); // Artifacts of Enchanted Origin Trader
        }

        public override void AwardPoints(PlayerMobile pm, Item item, int amount)
        {
            PointsSystem.SorcerersDungeon.AwardPoints(pm, amount);
        }

        public override bool IsRedeemableItem(Item item)
        {
            if (item is ICombatEquipment && ((ICombatEquipment)item).ReforgedSuffix == ReforgedSuffix.EnchantedOrigin)
                return true;

            return false;
        }

        public override void SendRewardGump(Mobile m)
        {
            if (m.Player && m.CheckAlive())
                m.SendGump(new SorcerersDungeonRewardGump(this, m as PlayerMobile));
        }

        public SorcerersDungeonResearcher(Serial serial)
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
            int version = reader.ReadInt();

            if (Map == Map.Ilshenar)
            {
                Instance = this;
            }
        }
    }
}
