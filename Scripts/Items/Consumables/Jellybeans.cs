using System;

namespace Server.Items
{
    public class JellyBeans : CandyCane
    {
        [Constructable]
        public JellyBeans()
            : this(1)
        {
        }

        public JellyBeans(int amount)
            : base(0x468C)
        {
            this.Stackable = true;
        }

        public JellyBeans(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1096932;
            }
        }/* jellybeans */
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