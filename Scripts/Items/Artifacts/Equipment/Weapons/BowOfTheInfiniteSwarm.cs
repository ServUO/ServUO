using System;

namespace Server.Items
{
    public class BowOfTheInfiniteSwarm : CompositeBow
	{
        public override int LabelNumber { get { return 1157347; } } // bow of the infinite swarm
        public override bool IsArtifact { get { return true; } }

        [Constructable]
        public BowOfTheInfiniteSwarm()
        {
            // Missing Swarm Props
            this.WeaponAttributes.HitLeechMana = 50;
            this.WeaponAttributes.HitLeechStam = 50;
            this.Attributes.BonusStam = 8;
            this.Attributes.RegenStam = 3;
            this.Attributes.WeaponSpeed = 30;
            this.Attributes.WeaponDamage = 50;
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

        public override int InitMinHits { get { return 255; } }
        public override int InitMaxHits { get { return 255; } }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();
        }
    }
}