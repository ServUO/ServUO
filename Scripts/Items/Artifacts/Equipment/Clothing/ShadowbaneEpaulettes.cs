using System;

namespace Server.Items
{
    public class ShadowbaneEpaulettes : Epaulette
    {
        public override bool IsArtifact { get { return true; } }

        [Constructable]
        public ShadowbaneEpaulettes()
        {
            Name = "Shadowbane Epaulettes";
            Hue = 1109;

            Attributes.AttackChance = 10;
            Attributes.DefendChance = 10;
            Attributes.LowerManaCost = 8;
        }

        public ShadowbaneEpaulettes(Serial serial)
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
            reader.ReadInt();
        }
    }

    public class GargishShadowbaneEpaulettes : GargishEpaulette
    {
        public override bool IsArtifact { get { return true; } }

        [Constructable]
        public GargishShadowbaneEpaulettes()
        {
            Name = "Gargish Shadowbane Epaulettes";
            Hue = 1109;

            Attributes.AttackChance = 10;
            Attributes.DefendChance = 10;
            Attributes.LowerManaCost = 8;
        }

        public GargishShadowbaneEpaulettes(Serial serial)
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
            reader.ReadInt();
        }
    }
}
