using Server.Gumps;
using System;

namespace Server.Mobiles
{
    public class HungryCoconutCrabStatue : Item, ICreatureStatuette
    {
        public override int LabelNumber => 1159221;  // Hungry Coconut Crab Statuette

        public Type CreatureType => typeof(HungryCoconutCrab);

        [Constructable]
        public HungryCoconutCrabStatue()
            : base(0xA336)
        {
            Hue = 2713;
        }

        public HungryCoconutCrabStatue(Serial serial)
            : base(serial)
        {
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (IsChildOf(from.Backpack))
            {
                from.SendGump(new ConfirmMountStatuetteGump(this));
            }
            else
            {
                SendLocalizedMessageTo(from, 1010095); // This must be on your person to use.
            }
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            list.Add(1159222); // *Redeemable for a pet*<br>*Requires High Seas Booster Pack*
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    [CorpseName("a coconut crab corpse")]
    public class HungryCoconutCrab : BaseCreature
    {
        [Constructable]
        public HungryCoconutCrab()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "hungry coconut crab";
            Body = 0x5E7;
            Hue = 2713;
            BaseSoundID = 0x4F2;

            SetStr(19);
            SetDex(15);
            SetInt(5);

            SetHits(12);

            SetDamage(3, 4);

            SetDamageType(ResistanceType.Physical, 100);

            SetResistance(ResistanceType.Physical, 10, 20);

            SetSkill(SkillName.MagicResist, 5.0);
            SetSkill(SkillName.Tactics, 5.0);
            SetSkill(SkillName.Wrestling, 5.0);

            Fame = 0;
            Karma = 0;

            Tamable = true;
            ControlSlots = 1;
            MinTameSkill = 0.0;
        }

        public HungryCoconutCrab(Serial serial)
            : base(serial)
        {
        }

        public override bool DeleteOnRelease => true;
        public override FoodType FavoriteFood => FoodType.FruitsAndVegies;

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}
