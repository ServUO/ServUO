using System;
using Server;
namespace Server.Items
{
    public class Bloodlust : DoubleBladedStaff, ITokunoDyable
    {
public override int ArtifactRarity { get { return 19; } }
        public override int InitMinHits { get { return 255; } }
        public override int InitMaxHits { get { return 255; } }
        [Constructable]
        public Bloodlust()
        {
            Name = "Bloodlust";
            Attributes.AttackChance = 10;
            Attributes.DefendChance = 10;
            WeaponAttributes.HitLeechHits = 25;
            WeaponAttributes.HitFireball = 25;
	    WeaponAttributes.HitMagicArrow = 25;
            WeaponAttributes.SelfRepair = 5;
            Attributes.BonusDex = 5;
            Attributes.WeaponSpeed = 25;
            Hue = 1627;
        }
        public override void GetDamageTypes( Mobile wielder, out int phys, out int fire, out int cold, out int pois, out int nrgy, out int chaos, out int direct )
        {
            phys = 50;
            cold = 10;
            fire = 20;
            nrgy = 10;
            pois = 10;
            chaos = 0;
            direct = 0;
        }
        public Bloodlust( Serial serial )
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
