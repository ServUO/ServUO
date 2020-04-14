namespace Server.Items
{
    public class SolenWarriorCostume : BaseCostume
    {
        public override string CreatureName { get { return "solen warrior"; } }

        [Constructable]
        public SolenWarriorCostume() : base()
        {
            this.CostumeBody = 782;
        }

        public override int LabelNumber
        {
            get
            {
                return 1114231;
            }
        }// solen warrior costume

        public SolenWarriorCostume(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}
