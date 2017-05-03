using System;

namespace Server.Mobiles
{
    public class WindrunnerStatue : Item
    {
        public override int LabelNumber { get { return 1124685; } } // Windrunner

        [Constructable]
        public WindrunnerStatue() 
            : base(0x9ED5)
        {
        }
        public WindrunnerStatue(Serial serial)
            : base(serial)
        {
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (this.IsChildOf(from.Backpack))
            {
                BaseMount m = new Windrunner();
                m.SetControlMaster(from);
                m.MoveToWorld(from.Location, from.Map);
                Delete();
            }
            else
                from.SendLocalizedMessage(1062334); // This item must be in your backpack to be used.
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
            this.BaseSoundID = 0xA8;

            this.SetStr(400);
            this.SetDex(125);
            this.SetInt(50, 55);

            this.SetHits(240);
            this.SetMana(0);

            this.SetDamage(1, 4);

            this.SetDamageType(ResistanceType.Physical, 100);

            this.SetResistance(ResistanceType.Physical, 40, 50);
            this.SetResistance(ResistanceType.Fire, 30, 40);
            this.SetResistance(ResistanceType.Cold, 30, 40);
            this.SetResistance(ResistanceType.Poison, 30, 40);
            this.SetResistance(ResistanceType.Energy, 30, 40);

            this.SetSkill(SkillName.MagicResist, 25.0, 30.0);
            this.SetSkill(SkillName.Tactics, 30.0, 40.0);
            this.SetSkill(SkillName.Wrestling, 30.0, 35.0);

            this.Fame = 300;
            this.Karma = 300;

            this.Tamable = true;
            this.ControlSlots = 1;
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