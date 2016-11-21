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
            this.Hue = 2006; //TODO: get proper hue, this is a guess

            this.Attributes.RegenMana = 2;
            this.Attributes.AttackChance = 10;
            this.Attributes.CastSpeed = 1;
            this.Attributes.CastRecovery = 2;
            this.Attributes.LowerManaCost = 4;
            this.Resistances.Poison = 5;
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