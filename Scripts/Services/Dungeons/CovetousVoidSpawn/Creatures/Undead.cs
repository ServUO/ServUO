
namespace Server.Mobiles
{
    [CorpseName("an angered spirit corpse")]
    public class AngeredSpirit : CovetousCreature
    {
        [Constructable]
        public AngeredSpirit() : base(AIType.AI_Mage)
        {
            Name = "an angered spirit";
            Body = 3;
            BaseSoundID = 471;
        }

        [Constructable]
        public AngeredSpirit(int level, bool voidSpawn) : base(AIType.AI_Mage, level, voidSpawn)
        {
            Name = "an angered spirit";
            Body = 3;
            BaseSoundID = 471;
        }

        public AngeredSpirit(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    [CorpseName("a bone swordslinger corpse")]
    public class BoneSwordSlinger : CovetousCreature
    {
        [Constructable]
        public BoneSwordSlinger() : base(AIType.AI_Melee)
        {
            Name = "a bone swordslinger";
            Body = 147;
            BaseSoundID = 451;
        }

        [Constructable]
        public BoneSwordSlinger(int level, bool voidSpawn) : base(AIType.AI_Melee, level, voidSpawn)
        {
            Name = "a bone sword slinger";
            Body = 147;
            BaseSoundID = 451;
        }

        public BoneSwordSlinger(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    [CorpseName("a vile cadaver")]
    public class VileCadaver : CovetousCreature
    {
        [Constructable]
        public VileCadaver() : base(AIType.AI_Melee)
        {
            Name = "a vile cadaver";
            Body = 154;
            BaseSoundID = 471;
        }

        [Constructable]
        public VileCadaver(int level, bool voidSpawn) : base(AIType.AI_Melee, level, voidSpawn)
        {
            Name = "a vile cadaver";
            Body = 154;
            BaseSoundID = 471;
        }

        public VileCadaver(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    [CorpseName("a liche's corpse")]
    public class DiseasedLich : CovetousCreature
    {
        [Constructable]
        public DiseasedLich() : base(AIType.AI_Mage)
        {
            Name = "a diseased lich";
            Body = 24;
            BaseSoundID = 0x3E9;
        }

        [Constructable]
        public DiseasedLich(int level, bool voidSpawn) : base(AIType.AI_Mage, level, voidSpawn)
        {
            Name = "a diseased lich";
            Body = 24;
            BaseSoundID = 0x3E9;
        }

        public DiseasedLich(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    [CorpseName("a revenant corpse")]
    public class CovetousRevenant : CovetousCreature
    {
        public override bool AlwaysMurderer => true;

        [Constructable]
        public CovetousRevenant() : base(AIType.AI_Mage)
        {
            Name = "a covetous revenant";
            Body = 400;
            Hue = 0x847E;

            Items.Robe shroud = new Items.Robe
            {
                ItemID = 0x2683,
                Hue = 0x4001,
                Movable = false
            };
            SetWearable(shroud);

            Items.Boots boots = new Items.Boots
            {
                Hue = 0x4001,
                Movable = false
            };
            SetWearable(boots);
        }

        [Constructable]
        public CovetousRevenant(int level, bool voidSpawn) : base(AIType.AI_Mage, level, voidSpawn)
        {
            Name = "a covetous revenant";
            Body = 400;
            //BaseSoundID = 609;
            //TODO: Soundid
        }

        public CovetousRevenant(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}
