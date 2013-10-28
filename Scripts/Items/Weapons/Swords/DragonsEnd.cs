using System;

namespace Server.Items
{
    public class DragonsEnd : Longsword
    {
        [Constructable]
        public DragonsEnd()
            : base()
        {
            this.Hue = 0x554;

            this.Slayer = SlayerName.DragonSlaying;

            this.Attributes.AttackChance = 10;
            this.Attributes.WeaponDamage = 60;

            this.WeaponAttributes.ResistFireBonus = 20;
        }

        public DragonsEnd(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1079791;
            }
        }// Dragon's End
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
                return 120;
            }
        }
        public override int InitMaxHits
        {
            get
            {
                return 120;
            }
        }
        public override void GetDamageTypes(Mobile wielder, out int phys, out int fire, out int cold, out int pois, out int nrgy, out int chaos, out int direct)
        {
            phys = fire = nrgy = pois = direct = chaos = 0;
            cold = 100;
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