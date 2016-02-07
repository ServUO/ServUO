using System;

namespace Server.Items
{
    public class RoseInAVase : Item /* TODO: when dye tub changes are implemented, furny dyable this */
    {
        [Constructable]
        public RoseInAVase()
            : base(0x0EB0)
        {
            this.Hue = 0x20;
            this.LootType = LootType.Blessed;
        }

        public RoseInAVase(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1023760;
            }
        }// A Rose in a Vase	1023760
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