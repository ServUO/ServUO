namespace Server.Items
{
    public class Pincer : BattleAxe
    {
        public override bool IsArtifact => true;
        public override int LabelNumber => 1154476;  // Pincer

        [Constructable]
        public Pincer()
        {
            Hue = 2500;
            Attributes.BalancedWeapon = 1;
            Slayer2 = BaseRunicTool.GetRandomSlayer();
            SetSkillBonuses.SetValues(0, SkillName.Lumberjacking, 10);
            WeaponAttributes.HitLeechHits = 87;
            Attributes.RegenStam = 6;
            Attributes.RegenMana = 6;
            Attributes.WeaponDamage = 40;
        }

        public override void GetDamageTypes(Mobile wielder, out int phys, out int fire, out int cold, out int pois, out int nrgy, out int chaos, out int direct)
        {
            phys = 30; fire = 70;
            cold = nrgy = chaos = direct = pois = 0;
        }

        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        public Pincer(Serial serial)
            : base(serial)
        {
        }
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

    public class GargishPincer : GargishBattleAxe
    {
        public override bool IsArtifact => true;
        public override int LabelNumber => 1154476;  // Pincer

        [Constructable]
        public GargishPincer()
        {
            Hue = 2500;
            Attributes.BalancedWeapon = 1;
            Slayer2 = BaseRunicTool.GetRandomSlayer();
            SetSkillBonuses.SetValues(0, SkillName.Lumberjacking, 10);
            WeaponAttributes.HitLeechHits = 87;
            Attributes.RegenStam = 6;
            Attributes.RegenMana = 6;
            Attributes.WeaponDamage = 40;
        }

        public override void GetDamageTypes(Mobile wielder, out int phys, out int fire, out int cold, out int pois, out int nrgy, out int chaos, out int direct)
        {
            phys = 30; fire = 70;
            cold = nrgy = chaos = direct = pois = 0;
        }

        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        public GargishPincer(Serial serial)
            : base(serial)
        {
        }
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