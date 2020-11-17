using Server.Gumps;
using Server.Network;
using System;

namespace Server.Mobiles
{
    public class CapybaraStatue : Item, ICreatureStatuette
    {
        public override int LabelNumber => 1159492;  // Capybara

        public Type CreatureType => typeof(Capybara);

        [Constructable]
        public CapybaraStatue()
            : base(0xA57B)
        {
            LootType = LootType.Blessed;
        }

        public CapybaraStatue(Serial serial)
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
                PrivateOverheadMessage(MessageType.Regular, 0x3B2, 1010095, from.NetState); // This must be on your person to use.
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();
        }
    }

    [CorpseName("a capybara corpse")]
    public class Capybara : BaseMount
    {
        [Constructable]
        public Capybara()
            : this("capybara")
        {
        }

        [Constructable]
        public Capybara(string name)
            : base(name, 1527, 0x3ED3, AIType.AI_Melee, FightMode.Aggressor, 10, 1, 0.2, 0.4)
        {
            BaseSoundID = 0xCC;

            SetStr(400);
            SetDex(125);
            SetInt(50, 55);

            SetHits(240);
            SetStam(125);
            SetMana(0);

            SetDamage(1, 4);

            SetResistance(ResistanceType.Physical, 40, 50);
            SetResistance(ResistanceType.Fire, 30, 40);
            SetResistance(ResistanceType.Cold, 30, 40);
            SetResistance(ResistanceType.Poison, 30, 40);
            SetResistance(ResistanceType.Energy, 30, 40);

            SetDamageType(ResistanceType.Physical, 100);

            SetSkill(SkillName.MagicResist, 20.0, 30.0);
            SetSkill(SkillName.Tactics, 30.0, 45.0);
            SetSkill(SkillName.Wrestling, 20.0, 40.0);
            SetSkill(SkillName.DetectHidden, 30.0, 40.0);

            Fame = 300;
            Karma = 300;

            Tamable = true;
            ControlSlots = 1;
            MinTameSkill = 30.0;
        }

        public Capybara(Serial serial)
            : base(serial)
        {
        }

        public override bool DeleteOnRelease => true;
        public override int Meat => 3;
        public override int Hides => 10;
        public override FoodType FavoriteFood => FoodType.FruitsAndVegies;

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();
        }
    }
}
