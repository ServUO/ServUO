using System;
using Server;
namespace Server.Items
{
    public class Fury : Katana, ITokunoDyable
    {
        public override int InitMinHits { get { return 255; } }
        public override int InitMaxHits { get { return 255; } }
public override int ArtifactRarity{ get{ return 13; } }

        [Constructable]
        public Fury()
        {
            Name = "Fury";
            WeaponAttributes.HitFireball = 25;
            WeaponAttributes.HitLightning = 25;
            WeaponAttributes.SelfRepair = 5;
            Attributes.CastSpeed = 1;
            Attributes.Luck = 200;
            Attributes.ReflectPhysical = 5;
            Attributes.WeaponSpeed = 20;
            Hue = 1357;
        }
        public override void GetDamageTypes( Mobile wielder, out int phys, out int fire, out int cold, out int pois, out int nrgy, out int chaos, out int direct )
        {
            phys = 40;
            cold = 15;
            fire = 15;
            nrgy = 15;
            pois = 15;
            chaos = 0;
            direct = 0;
        }
        public Fury( Serial serial )
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
