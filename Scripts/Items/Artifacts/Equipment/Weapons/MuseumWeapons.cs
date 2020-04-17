namespace Server.Items
{
    public class BlackthornsKryss : Kryss
    {
        public override bool IsArtifact => true;
        [Constructable]
        public BlackthornsKryss()
            : base()
        {
            Hue = 0x5E5;
            Slayer = SlayerGroup.RandomSuperSlayerAOS();
            Attributes.WeaponSpeed = 25;
            Attributes.WeaponDamage = 50;
            WeaponAttributes.UseBestSkill = 1;
            WeaponAttributes.HitLeechHits = 22;
        }

        public BlackthornsKryss(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1073260;// Blackthorn's Kryss - Museum of Vesper Replica	
        public override int InitMinHits => 80;
        public override int InitMaxHits => 80;
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

    public class SwordOfJustice : VikingSword
    {
        public override bool IsArtifact => true;
        [Constructable]
        public SwordOfJustice()
            : base()
        {
            Hue = 0x47E;
            Slayer = SlayerGroup.RandomSuperSlayerAOS();
            Attributes.SpellChanneling = 1;
            Attributes.CastSpeed = -1;
            Attributes.WeaponDamage = 50;
            Attributes.Luck = 100;
            WeaponAttributes.UseBestSkill = 1;
            WeaponAttributes.HitLowerAttack = 60;
        }

        public SwordOfJustice(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1073261;// Sword of Justice - Museum of Vesper Replica
        public override int InitMinHits => 80;
        public override int InitMaxHits => 80;
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

    public class GeoffreysAxe : ExecutionersAxe
    {
        public override bool IsArtifact => true;
        [Constructable]
        public GeoffreysAxe()
            : base()
        {
            Hue = 0x21;
            Slayer = SlayerGroup.RandomSuperSlayerAOS();
            Attributes.BonusStr = 10;
            Attributes.AttackChance = 15;
            Attributes.WeaponDamage = 40;
            Attributes.Luck = 150;
            WeaponAttributes.ResistFireBonus = 10;
            WeaponAttributes.UseBestSkill = 1;
            WeaponAttributes.HitLowerAttack = 60;
        }

        public GeoffreysAxe(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1073263;// Geoffrey's Axe - Museum of Vesper Replica
        public override int InitMinHits => 80;
        public override int InitMaxHits => 80;
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