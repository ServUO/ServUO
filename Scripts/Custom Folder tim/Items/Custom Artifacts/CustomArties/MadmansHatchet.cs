using System;
using Server;
namespace Server.Items
{
    public class MadmansHatchet : Hatchet, ITokunoDyable
    {
        public override int InitMinHits { get { return 255; } }
        public override int InitMaxHits { get { return 255; } }
public override int ArtifactRarity{ get{ return 15; } }
        
	[Constructable]
        public MadmansHatchet()
        {
            Name = "Madman's Hatchet";

            Hue = 1157;
            Attributes.WeaponDamage = 50;
            WeaponAttributes.HitLeechHits = 35;
            WeaponAttributes.UseBestSkill = 1;
            Attributes.WeaponSpeed = 10;
            WeaponAttributes.HitFireball = 20;
        }
        public override void GetDamageTypes( Mobile wielder, out int phys, out int fire, out int cold, out int pois, out int nrgy, out int chaos, out int direct )
        {
            phys = 100;
            cold = 0;
            fire = 0;
            nrgy = 0;
            pois = 0;
            chaos = 0;
            direct = 0;
        }
        public MadmansHatchet( Serial serial )
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
