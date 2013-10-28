using Server;

namespace CustomsFramework.Systems.VIPSystem
{
    public class BronzeVIPDeed : BaseVIPDeed
    {
        [Constructable(AccessLevel.Developer)]
        public BronzeVIPDeed()
            : base()
        {
            this.Hue = 1055;
            this.Tier = VIPTier.Bronze;
            this.Bonuses.ResProtection.Enabled = true;
            this.Bonuses.ToolbarAccess.Enabled = true;
            this.Bonuses.BasicCommands.Enabled = true;
            this.Bonuses.GainIncrease.Enabled = true;
            this.Bonuses.FreeCorpseReturn.Enabled = true;
        }

        public BronzeVIPDeed(Serial serial)
            : base(serial)
        {
        }

        public override string DefaultName
        {
            get
            {
                return "A Bronze VIP Deed";
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