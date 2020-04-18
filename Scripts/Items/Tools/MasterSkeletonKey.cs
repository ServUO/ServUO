namespace Server.Items
{
    public class MasterSkeletonKey : Skeletonkey
    {
        public override int LabelNumber => 1095523;

        public override bool IsSkeletonKey => true;
        public override int SkillBonus => 100;

        [Constructable]
        public MasterSkeletonKey()
        {
            Uses = 10;
        }

        public MasterSkeletonKey(Serial serial) : base(serial)
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
        }
    }
}