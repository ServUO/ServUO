using System;
using Server.Items;

namespace Server.Mobiles
{
    public class ParoxysmusSwampDragon : SwampDragon
    {
        [Constructable]
        public ParoxysmusSwampDragon()
            : base()
        {
            this.BardingResource = CraftResource.Iron;
            this.BardingExceptional = true;
            this.BardingHP = this.BardingMaxHP;
            this.HasBarding = true;
            this.Hue = 0x851;
        }

        public ParoxysmusSwampDragon(Serial serial)
            : base(serial)
        {
        }

        public override bool DeleteOnRelease
        {
            get
            {
                return true;
            }
        }
        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            list.Add(1049646); // (summoned)
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}