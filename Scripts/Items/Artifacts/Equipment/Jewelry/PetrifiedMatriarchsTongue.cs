using System;
using Server;

namespace Server.Items
{
    public class PetrifiedMatriarchsTongue : GoldRing
    {
        public override bool IsArtifact { get { return true; } }
        public override int LabelNumber { get { return 1115776; } } // Petrified Matriarch's Tongue

        public override int InitMinHits { get { return 255; } }
        public override int InitMaxHits { get { return 255; } }

        [Constructable]
        public PetrifiedMatriarchsTongue()
        {
            Hue = 2006; //TODO: get proper hue, this is a guess
            Attributes.RegenMana = 2;
            Attributes.AttackChance = 10;
            Attributes.CastSpeed = 1;
            Attributes.CastRecovery = 2;
            Attributes.LowerManaCost = 4;
            Resistances.Poison = 5;
        }

        public PetrifiedMatriarchsTongue(Serial serial)
            : base(serial)
        {
        }

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