using System;
using Server;
namespace Server.Items
{
    public class NordicVikingSword : VikingSword, ITokunoDyable
    {
        public override int InitMinHits { get { return 255; } }
        public override int InitMaxHits { get { return 255; } }
public override int ArtifactRarity{ get{ return 15; } }
        [Constructable]
        public NordicVikingSword()
        {
            Name = "Nordic Viking Sword";

            Hue = 741;
            Attributes.WeaponDamage = 50;
            Attributes.WeaponSpeed = 20;
            WeaponAttributes.HitLightning = 50;
            Attributes.BonusHits = 30;
            Slayer = SlayerName.DragonSlaying;
        }
        public override void GetDamageTypes( Mobile wielder, out int phys, out int fire, out int cold, out int pois, out int nrgy, out int chaos, out int direct )
        {
            phys = 40;
            cold = 0;
            fire = 20;
            nrgy = 40;
            pois = 0;
            chaos = 0;
            direct = 0;
        }
        public NordicVikingSword( Serial serial )
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
