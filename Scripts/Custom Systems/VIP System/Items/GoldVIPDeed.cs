using Server;

namespace CustomsFramework.Systems.VIPSystem
{
    public class GoldVIPDeed : BaseVIPDeed
    {
        [Constructable(AccessLevel.Developer)]
        public GoldVIPDeed()
            : base()
        {
            this.Hue = 2213;
            this.Tier = VIPTier.Gold;

            foreach (Bonus bonus in this.Bonuses)
            {
                bonus.Enabled = true;
            }
        }

        public GoldVIPDeed(Serial serial)
            : base(serial)
        {
        }

        public override string DefaultName
        {
            get
            {
                return "A Gold VIP Deed";
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