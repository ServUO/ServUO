using System;
using Server.Gumps;

namespace Server.Mobiles
{
    public class LasherStatue : Item, IMountStatuette
    {
        public override int LabelNumber { get { return 1157214; } } // Lasher

        public Type MountType { get { return typeof(Lasher); } }

        [Constructable]
        public LasherStatue() 
            : base(0x9E35)
        {
            LootType = LootType.Blessed;
        }
        public LasherStatue(Serial serial)
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

    [CorpseName("a Lasher corpse")]
    public class Lasher : BaseMount
    {
        [Constructable]
        public Lasher()
            : this("Lasher")
        {
        }

        [Constructable]
        public Lasher(string name)
            : base(name, 1407, 16075, AIType.AI_Animal, FightMode.Aggressor, 10, 1, 0.2, 0.4)
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

        public Lasher(Serial serial)
            : base(serial)
        {
        }

        public override int Meat { get { return 3; } }
        public override int Hides { get { return 10; } }
        public override FoodType FavoriteFood { get { return FoodType.FruitsAndVegies | FoodType.GrainsAndHay; } }

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
