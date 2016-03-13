using Server;
using System;

namespace Server.Items
{
    public class MasterSkeletonKey : Skeletonkey
	{
        public override int LabelNumber { get { return 1095523; } }

        public override bool IsSkeletonKey { get { return true; } }
        public override int SkillBonus { get { return 100; } }

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
			writer.Write((int)0);	
		}
	
		public override void Deserialize(GenericReader reader)
		{
		 	base.Deserialize(reader);
			int version = reader.ReadInt();
		}
	}
}