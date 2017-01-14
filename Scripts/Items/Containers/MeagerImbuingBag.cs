using System;

namespace Server.Items
{
    public class MeagerImbuingBag : BaseRewardBag
    {
        [Constructable]
        public MeagerImbuingBag()
        {
            switch (Utility.Random(4))
            {
                case 0:
                    this.AddItem(new SlithTongue());
                    break;
                case 1:
                    this.AddItem(new GoblinBlood());
                    break;
                case 2:
                    this.AddItem(new ReflectiveWolfEye());
                    break;
                case 3:
                    this.AddItem(new RaptorTeeth());
                    break;
            }
        }

        public MeagerImbuingBag(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1112994;
            }
        }
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version 
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}