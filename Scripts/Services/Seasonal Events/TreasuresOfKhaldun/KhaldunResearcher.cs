using Server.Engines.Points;
using Server.Items;
using Server.Mobiles;

namespace Server.Engines.Khaldun
{
    public class KhaldunResearcher : BaseTurnInMobile
    {
        public override int TitleLocalization => 1158744;  // Artifacts of the Cult
        public override int CancelLocalization => 1158675; 	// Bring me items of the Cult and I will reward you with valuable items.   
        public override int TurnInLocalization => 1157652;  // Turn In Artifacts of The Cult	
        public override int ClaimLocalization => 1155593;  // Claim Rewards

        public static KhaldunResearcher InstanceTram { get; set; }
        public static KhaldunResearcher InstanceFel { get; set; }

        [Constructable]
        public KhaldunResearcher() : base("the Researcher")
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

            // QuiverOfInfinityBase
            Item item = new SeekerOfTheFallenStarTitleDeed
            {
                Movable = false
            };
            PackItem(item);

            item = new ZealotOfKhalAnkurTitleDeed
            {
                Movable = false
            };
            PackItem(item);

            item = new ProphetTitleDeed
            {
                Movable = false
            };
            PackItem(item);

            item = new CultistsRitualTome
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

            list.Add(1158678); // Artifacts of the Cult Trader
        }

        public override void AwardPoints(PlayerMobile pm, Item item, int amount)
        {
            PointsSystem.Khaldun.AwardPoints(pm, amount);
        }

        public override bool IsRedeemableItem(Item item)
        {
            if (item is ICombatEquipment && ((ICombatEquipment)item).ReforgedSuffix == ReforgedSuffix.Khaldun)
                return true;

            return false;
        }

        public override void SendRewardGump(Mobile m)
        {
            if (m.Player && m.CheckAlive())
                m.SendGump(new KhaldunRewardGump(this, m as PlayerMobile));
        }

        public KhaldunResearcher(Serial serial)
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
