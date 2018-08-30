using System;
using Server;
namespace Server.Items
{
    public class CriticalShot : Crossbow, ITokunoDyable
    {
public override int ArtifactRarity { get { return 19; } }
        public override int InitMinHits { get { return 255; } }
        public override int InitMaxHits { get { return 255; } }
        [Constructable]
        public CriticalShot()
        {
            Name = "Critical Shot";
            Hue = 2412;
            Attributes.WeaponDamage = 40;
            WeaponAttributes.HitLeechHits = 25;
            WeaponAttributes.HitLeechMana = 40;
            WeaponAttributes.HitFireball = 35;
            Attributes.SpellChanneling = 1;
            Attributes.WeaponSpeed = 20;
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
        public CriticalShot( Serial serial )
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
