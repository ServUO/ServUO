using System;
using Server.Gumps;

namespace Server.Mobiles
{
    public class WindrunnerStatue : Item, IMountStatuette
    {
        public override int LabelNumber { get { return 1124685; } } // Windrunner

        public Type MountType { get { return typeof(Windrunner); } }

        [Constructable]
        public WindrunnerStatue() 
            : base(0x9ED5)
        {
            LootType = LootType.Blessed;
        }
        public WindrunnerStatue(Serial serial)
            : base(serial)
        {
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (IsChildOf(from.Backpack))
                from.SendGump(new ConfirmMountStatuetteGump(this));
            else
                from.SendLocalizedMessage(1042001); // That must be in your pack for you to use it.
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

    [CorpseName("a Windrunner corpse")]
    public class Windrunner : BaseMount
    {
        [Constructable]
        public Windrunner()
            : this("Windrunner")
        {
        }

        [Constructable]
        public Windrunner(string name)
            : base(name, 1410, 16076, AIType.AI_Animal, FightMode.Aggressor, 10, 1, 0.2, 0.4)
        {
            BaseSoundID = 0xA8;

            SetStr(400);
            SetDex(125);
            SetInt(50, 55);

            SetHits(240);
            SetMana(0);

            SetDamage(1, 4);

            SetDamageType(ResistanceType.Physical, 100);

            SetResistance(ResistanceType.Physical, 40, 50);
            SetResistance(ResistanceType.Fire, 30, 40);
            SetResistance(ResistanceType.Cold, 30, 40);
            SetResistance(ResistanceType.Poison, 30, 40);
            SetResistance(ResistanceType.Energy, 30, 40);

            SetSkill(SkillName.MagicResist, 25.0, 30.0);
            SetSkill(SkillName.Tactics, 30.0, 40.0);
            SetSkill(SkillName.Wrestling, 30.0, 35.0);

            Fame = 300;
            Karma = 300;

            Tamable = true;
            ControlSlots = 1;
        }

        public Windrunner(Serial serial)
            : base(serial)
        {
        }

        public override int Meat { get { return 3; } }
        public override int Hides { get { return 10; } }
        public override FoodType FavoriteFood { get { return FoodType.Meat; } }

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
