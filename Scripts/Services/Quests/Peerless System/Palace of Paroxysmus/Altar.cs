using System;
using Server.Mobiles;

namespace Server.Items
{
    public class ParoxysmusAltar : PeerlessAltar
    { 
        [Constructable]
        public ParoxysmusAltar()
            : base(0x207A)
        { 
            this.Hue = 0x465;
			
            this.BossLocation = new Point3D(6517, 357, 0);
            this.TeleportDest = new Point3D(6519, 381, 0);
            this.ExitDest = new Point3D(5623, 3038, 15);			
        }

        public ParoxysmusAltar(Serial serial)
            : base(serial)
        {
        }

        public override int KeyCount
        {
            get
            {
                return 3;
            }
        }
        public override MasterKey MasterKey
        {
            get
            {
                return new ParoxysmusKey();
            }
        }
        public override Type[] Keys
        {
            get
            {
                return new Type[]
                {
                    typeof(CoagulatedLegs), typeof(PartiallyDigestedTorso),
                    typeof(GelatanousSkull), typeof(SpleenOfThePutrefier)
                };
            }
        }
        public override BasePeerless Boss
        {
            get
            {
                return new ChiefParoxysmus();
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