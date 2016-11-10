using Server;
using System;

namespace Server.Items
{
    public class EpauletteBearingTheCrestOfBlackthorn6 : Cloak
    {
        public override bool IsArtifact { get { return true; } }

        public override int LabelNumber { get { return 1123325; } } // Epaulette

        [Constructable]
        public EpauletteBearingTheCrestOfBlackthorn6()
        {
            ReforgedSuffix = ReforgedSuffix.Blackthorn;
            ItemID = 0x9985;            
            Attributes.AttackChance = 3;
            Attributes.DefendChance = 3;
            Attributes.SpellDamage = 3;
            Hue = 2019;            
        }

        public override int InitMinHits { get { return 255; } }
        public override int InitMaxHits { get { return 255; } }       

        public EpauletteBearingTheCrestOfBlackthorn6(Serial serial)
            : base(serial)
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