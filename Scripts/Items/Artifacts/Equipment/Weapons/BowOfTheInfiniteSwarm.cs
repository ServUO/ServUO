namespace Server.Items
{
    public class BowOfTheInfiniteSwarm : CompositeBow
    {
        public override int LabelNumber => 1157347;  // bow of the infinite swarm
        public override bool IsArtifact => true;

        [Constructable]
        public BowOfTheInfiniteSwarm()
        {
            ExtendedWeaponAttributes.HitSwarm = 20;
            WeaponAttributes.HitLeechMana = 50;
            WeaponAttributes.HitLeechStam = 50;
            Attributes.BonusStam = 8;
            Attributes.RegenStam = 3;
            Attributes.WeaponSpeed = 30;
            Attributes.WeaponDamage = 50;
        }

        public override void GetDamageTypes(Mobile wielder, out int phys, out int fire, out int cold, out int pois, out int nrgy, out int chaos, out int direct)
        {
            phys = 100;
            fire = cold = nrgy = chaos = direct = pois = 0;
        }

        public BowOfTheInfiniteSwarm(Serial serial)
            : base(serial)
        {
        }

        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(1); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();

            if (version == 0 && WeaponAttributes.HitLeechMana != 50)
                WeaponAttributes.HitLeechMana = 50;
        }
    }

    public class GlaiveOfTheInfiniteSwarm : SoulGlaive
    {
        public override int LabelNumber => 1157366;  // Glaive of the Infinite Swarm
        public override bool IsArtifact => true;

        [Constructable]
        public GlaiveOfTheInfiniteSwarm()
        {
            ExtendedWeaponAttributes.HitSwarm = 20;

            WeaponAttributes.HitLeechMana = 50;
            WeaponAttributes.HitLeechStam = 50;
            Attributes.BonusStam = 8;
            Attributes.RegenStam = 3;
            Attributes.WeaponSpeed = 30;
            Attributes.WeaponDamage = 50;
        }

        public override void GetDamageTypes(Mobile wielder, out int phys, out int fire, out int cold, out int pois, out int nrgy, out int chaos, out int direct)
        {
            phys = 100;
            fire = cold = nrgy = chaos = direct = pois = 0;
        }

        public GlaiveOfTheInfiniteSwarm(Serial serial)
            : base(serial)
        {
        }

        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(1); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();

            if (version == 0 && WeaponAttributes.HitLeechMana != 50)
                WeaponAttributes.HitLeechMana = 50;
        }
    }
}