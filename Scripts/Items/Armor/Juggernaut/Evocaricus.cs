using System;

namespace Server.Items
{
    public class Evocaricus : VikingSword
    {
        [Constructable]
        public Evocaricus()
            : base()
        {
            this.SetHue = 0x76D;
			
            this.Attributes.WeaponDamage = 50;
			
            this.SetSelfRepair = 3;			
            this.SetAttributes.DefendChance = 10;
            this.SetAttributes.BonusStr = 10;
            this.SetAttributes.WeaponSpeed = 35;
        }

        public Evocaricus(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1074309;
            }
        }// Evocaricus (Juggernaut Set)
        public override SetItem SetID
        {
            get
            {
                return SetItem.Juggernaut;
            }
        }
        public override int Pieces
        {
            get
            {
                return 2;
            }
        }
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}