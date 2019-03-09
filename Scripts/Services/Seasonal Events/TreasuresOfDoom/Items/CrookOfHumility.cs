using Server;

namespace Server.Items
{
    public class CrookOfHumility : ShepherdsCrook
    {
        public override bool IsArtifact { get { return true; } }
        public override int LabelNumber { get { return 1155624; } } // Crook of Humilty

        public override int InitMinHits { get { return 255; } }
        public override int InitMaxHits { get { return 255; } }

        [Constructable]
        public CrookOfHumility()
        {
            Slayer3 = TalismanSlayerName.Wolf;
            Attributes.SpellChanneling = 1;
            Attributes.BonusInt = 10;
            Attributes.WeaponDamage = 20;
        }

        public CrookOfHumility(Serial serial)
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
            reader.ReadInt(); // version
        }
    }
}