using System;
using Server.Engines.Craft;

namespace Server.Items
{
    public class Skillet : BaseTool
    {
        [Constructable]
        public Skillet()
            : base(0x97F)
        {
            this.Weight = 1.0;
        }

        [Constructable]
        public Skillet(int uses)
            : base(uses, 0x97F)
        {
            this.Weight = 1.0;
        }

        public Skillet(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1044567;
            }
        }// skillet
        public override CraftSystem CraftSystem
        {
            get
            {
                return DefCooking.CraftSystem;
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