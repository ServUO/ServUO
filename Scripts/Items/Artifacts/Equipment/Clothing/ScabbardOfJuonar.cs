using System;

namespace Server.Items
{
    public class ScabbardOfJuonar : SwordBelt
    {
        public override bool IsArtifact { get { return true; } }

        [Constructable]
        public ScabbardOfJuonar()
        {
            Name = "Scabbard of Juo'nar";
            Hue = 1109;

            SkillBonuses.SetValues(0, SkillName.MagicResist, 10.0);
            Attributes.BonusHits = 5;
            Attributes.BonusInt = 10;
            Attributes.SpellDamage = 5;
            Attributes.CastSpeed = 1;
        }

        public ScabbardOfJuonar(Serial serial)
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

    public class GargishScabbardOfJuonar : GargoyleHalfApron
    {
        public override bool IsArtifact { get { return true; } }

        [Constructable]
        public GargishScabbardOfJuonar()
        {
            Name = "Gargish Scabbard of Juo'nar";
            Hue = 1109;

            SkillBonuses.SetValues(0, SkillName.MagicResist, 10.0);
            Attributes.BonusHits = 5;
            Attributes.BonusInt = 10;
            Attributes.SpellDamage = 5;
            Attributes.CastSpeed = 1;
        }

        public GargishScabbardOfJuonar(Serial serial)
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
