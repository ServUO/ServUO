using System;

namespace Server.Items
{
    public class MantleOfTheArchlich : Robe
    {
        public override bool IsArtifact { get { return true; } }

        [Constructable]
        public MantleOfTheArchlich()
        {
            Name = "Mantle of the Archlich";
            Hue = 1150;

            SkillBonuses.SetValues(0, SkillName.MagicResist, 10.0);
            Attributes.BonusStr = 10;
            Attributes.LowerManaCost = 5;
            Attributes.SpellDamage = 8;
            Attributes.CastSpeed = 1;
        }

        public MantleOfTheArchlich(Serial serial)
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
