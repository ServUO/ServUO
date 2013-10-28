using Server;

namespace CustomsFramework.Systems.VIPSystem
{
    public class SilverVIPDeed : BaseVIPDeed
    {
        [Constructable(AccessLevel.Developer)]
        public SilverVIPDeed()
            : base()
        {
            this.Hue = 2407;
            this.Tier = VIPTier.Silver;
            this.Bonuses.FullLRC.Enabled = true;
            this.Bonuses.BankIncrease.Enabled = true;
            this.Bonuses.LifeStoneNoUses.Enabled = true;
            this.Bonuses.LootGoldFromGround.Enabled = true;
            this.Bonuses.DoubleResources.Enabled = true;
        }

        public SilverVIPDeed(Serial serial)
            : base(serial)
        {
        }

        public override string DefaultName
        {
            get
            {
                return "A Silver VIP Deed";
            }
        }
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            Utilities.WriteVersion(writer, 0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch (version)
            {
                case 0:
                    {
                        break;
                    }
            }
        }
    }
}