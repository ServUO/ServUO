using System;
using Server;
namespace Server.Items
{
    public class MauloftheBeast : Maul, ITokunoDyable
    {
        public override int InitMinHits { get { return 225; } }
        public override int InitMaxHits { get { return 225; } }
public override int ArtifactRarity{ get{ return 15; } }
        [Constructable]
        public MauloftheBeast()
        {
            Name = "Maul of the Beast";
            Hue = 1779;
            Attributes.WeaponDamage = 60;
            WeaponAttributes.HitLeechHits = 35;
            WeaponAttributes.HitLeechMana = 35;
            WeaponAttributes.HitLeechStam = 35;
            WeaponAttributes.SelfRepair = 2;
             Attributes.SpellChanneling = 1;
            Attributes.WeaponSpeed = -30;
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
        public MauloftheBeast( Serial serial )
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
