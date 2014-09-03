using Server;

namespace CustomsFramework.Systems.VIPSystem
{
    public class VIPFreeHouseDecorationDeed : BaseVIPDeed
    {
        [Constructable(AccessLevel.Developer)]
        public VIPFreeHouseDecorationDeed()
            : base()
        {
            this.Hue = 2213;
            this.Bonuses.FreeHouseDecoration.Enabled = true;
        }

        public VIPFreeHouseDecorationDeed(Serial serial)
            : base(serial)
        {
        }

        public override string DefaultName
        {
            get
            {
                return "VIP Deed - Free House Decoration";
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