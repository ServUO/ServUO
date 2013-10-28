using System;

namespace Server.Items
{
    public class TheDragonSlayer : Lance
    {
        [Constructable]
        public TheDragonSlayer()
        {
            this.Hue = 0x530;
            this.Slayer = SlayerName.DragonSlaying;
            this.Attributes.Luck = 110;
            this.Attributes.WeaponDamage = 50;
            this.WeaponAttributes.ResistFireBonus = 20;
            this.WeaponAttributes.UseBestSkill = 1;
        }

        public TheDragonSlayer(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1061248;
            }
        }// The Dragon Slayer
        public override int ArtifactRarity
        {
            get
            {
                return 11;
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
            phys = fire = cold = pois = chaos = direct = 0;
            nrgy = 100;
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
                this.Slayer = SlayerName.DragonSlaying;
        }
    }
}