using System;

namespace Server.Items
{
    public class ZyronicClaw : ExecutionersAxe
    {
        [Constructable]
        public ZyronicClaw()
        {
            this.Hue = 0x485;
            this.Slayer = SlayerName.ElementalBan;
            this.WeaponAttributes.HitLeechMana = 50;
            this.Attributes.AttackChance = 30;
            this.Attributes.WeaponDamage = 50;
        }

        public ZyronicClaw(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1061593;
            }
        }// Zyronic Claw
        public override int ArtifactRarity
        {
            get
            {
                return 10;
            }
        }
        public override int InitMinHits
        {
            get
            {
                return 255;
            }
        }
        public override int InitMaxHits
        {
            get
            {
                return 255;
            }
        }
        public override void GetDamageTypes(Mobile wielder, out int phys, out int fire, out int cold, out int pois, out int nrgy, out int chaos, out int direct)
        {
            chaos = direct = 0;
            phys = fire = cold = pois = nrgy = 20;
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

            if (this.Slayer == SlayerName.None)
                this.Slayer = SlayerName.ElementalBan;
        }
    }
}