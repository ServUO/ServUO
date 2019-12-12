using System;

namespace Server.Items
{
    [FlipableAttribute(0xA511, 0xA512)]
    public class HorseStatue : MonsterStatuette
    {
        public override int LabelNumber { get { return 1018263; } } // horse

        [Constructable]
        public HorseStatue()
            : base(MonsterStatuetteType.Horse)
        {
            LootType = LootType.Blessed;
            Weight = 1.0;			
        }

        public HorseStatue(Serial serial)
            : base(serial)
        {
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
