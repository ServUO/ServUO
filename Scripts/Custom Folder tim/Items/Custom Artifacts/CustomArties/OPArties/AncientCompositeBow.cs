using System;
using Server;
namespace Server.Items
{
    public class AncientCompositeBow : CompositeBow, ITokunoDyable
    {
public override int ArtifactRarity { get { return 19; } }
        public override int InitMinHits { get { return 255; } }
        public override int InitMaxHits { get { return 255; } }
        [Constructable]
        public AncientCompositeBow()
        {
            Name = "Ancient Composite Bow";
            Hue = 2968;
            Attributes.WeaponDamage = 45;
            WeaponAttributes.HitLeechHits = 25;
            WeaponAttributes.HitLeechStam = 25;
            WeaponAttributes.HitLightning = 45;
		Attributes.BonusDex = 5;
            Attributes.SpellChanneling = 1;
            Attributes.WeaponSpeed = 20;
		Velocity = 40;
        }
        public override void GetDamageTypes( Mobile wielder, out int phys, out int fire, out int cold, out int pois, out int nrgy, out int chaos, out int direct )
        {
            phys = 45;
            cold = 15;
            fire = 10;
            nrgy = 15;
            pois = 15;
            chaos = 0;
            direct = 0;
        }
        public AncientCompositeBow( Serial serial )
            : base( serial )
        {
        }
        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );
            writer.Write( (int)0 );
        }
        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );
            int version = reader.ReadInt();
        }
    }
}
