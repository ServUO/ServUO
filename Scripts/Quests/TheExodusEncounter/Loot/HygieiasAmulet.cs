using System;
using Server;
using Server.Spells;

namespace Server.Items
{
    public class HygieiasAmulet : GoldNecklace
    {
        public override bool IsArtifact { get { return true; } }

        [Constructable]
        public HygieiasAmulet()
        {
            this.SkillBonuses.SetValues(0, SkillName.Alchemy, 10);
        }        

        public HygieiasAmulet(Serial serial)  : base(serial)
        {
        }

        public override bool CanFortify { get { return false; } }

        public override int LabelNumber
        {
            get
            {
                return 1153524;
            }
        } // Hygieia's Amulet [Replica]

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