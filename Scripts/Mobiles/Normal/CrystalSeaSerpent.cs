using System;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a crystal sea serpent corpse")]
    public class CrystalSeaSerpent : SeaSerpent
    {
        [Constructable]
        public CrystalSeaSerpent()
        {
            Name = "a crystal sea serpent";
            Hue = 0x47E;

            SetStr(250, 450);
            SetDex(100, 150);
            SetInt(90, 190);

            SetHits(230, 330);

            SetDamage(10, 18);

            SetDamageType(ResistanceType.Physical, 10);
            SetDamageType(ResistanceType.Cold, 45);
            SetDamageType(ResistanceType.Energy, 45);

            SetResistance(ResistanceType.Physical, 50, 70);
            SetResistance(ResistanceType.Fire, 0);
            SetResistance(ResistanceType.Cold, 70, 90);
            SetResistance(ResistanceType.Poison, 20, 30);
            SetResistance(ResistanceType.Energy, 60, 80);
        }

        public override void OnDeath( Container c )
        {
        base.OnDeath( c );

            if ( Utility.RandomDouble() < 0.05 )
            c.DropItem( new CrushedCrystals() );

            if ( Utility.RandomDouble() < 0.1 )
            c.DropItem( new IcyHeart() );

            if ( Utility.RandomDouble() < 0.1 )
            c.DropItem( new LuckyDagger() );
        }

        public override int TreasureMapLevel { get { return 3; } }
        public override int Meat{ get{ return 10; } }
        public override int Hides{ get{ return 11; } }
        public override HideType HideType{ get{ return HideType.Horned; } }
        public override int Scales{ get{ return 8; } }
        public override ScaleType ScaleType{ get{ return ScaleType.Blue; } } 

        public CrystalSeaSerpent(Serial serial)
            : base(serial)
        {
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
