using System;

namespace Server.Items
{
    public class TalismanofGoblinSlaying : BaseTalisman
    {
        [Constructable]
        public TalismanofGoblinSlaying()
            : base(0x2F58)
        { 
            this.Slayer = TalismanSlayerName.Goblin;
            this.MaxChargeTime = 1200;
        }

        public TalismanofGoblinSlaying(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1095011;
            }
        }//Talisman of Goblin Slaying
        public override bool ForceShowName
        {
            get
            {
                return true;
            }
        }
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();
        }
    }
}