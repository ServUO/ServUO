using System;
using Server.Gumps;
using Server.Items;

namespace Server.Mobiles
{
    public class TritonStatue : Item, ICreatureStatuette
    {
        public override int LabelNumber { get { return 1158929; } } // Triton

        public Type CreatureType { get { return typeof(Triton); } }

        [Constructable]
        public TritonStatue() 
            : base(0xA2D8)
        {
            Hue = 2713;
        }

        public TritonStatue(Serial serial)
            : base(serial)
        {
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (IsChildOf(from.Backpack))
            {
                if (from.Skills[SkillName.AnimalTaming].Value >= 100)
                {
                    from.SendGump(new ConfirmMountStatuetteGump(this));
                }
                else
                {
                    from.SendLocalizedMessage(1158959, "100"); // ~1_SKILL~ Animal Taming skill is required to redeem this pet.
                }
            }
            else
            {
                SendLocalizedMessageTo(from, 1010095); // This must be on your person to use.
            }
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            
            list.Add(1158954); // *Redeemable for a pet*<br>*Requires Grandmaster Taming to Claim Pet*
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

    [CorpseName("a triton corpse")]
    public class Triton : BaseCreature
    {        
        [Constructable]
        public Triton()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "Triton";
            Body = 0x2D0;
            Hue = 2713;
            BaseSoundID = 0x5A;

            SetStr(103, 250);
            SetDex(151, 220);
            SetInt(101, 121);

            SetHits(651, 700);
            SetStam(150);

            SetDamage(13, 24);

            SetDamageType(ResistanceType.Physical, 50);
            SetDamageType(ResistanceType.Energy, 50);

            SetResistance(ResistanceType.Physical, 45, 55);
            SetResistance(ResistanceType.Fire, 50, 60);
            SetResistance(ResistanceType.Cold, 45, 55);
            SetResistance(ResistanceType.Poison, 35, 45);
            SetResistance(ResistanceType.Energy, 85, 90);

            SetSkill(SkillName.MagicResist, 150.0, 190.0);
            SetSkill(SkillName.Tactics, 80.0, 95.0);
            SetSkill(SkillName.Wrestling, 110.0, 130.0);
            SetSkill(SkillName.Healing, 70.0, 99.0);
            SetSkill(SkillName.DetectHidden, 50.1);
            SetSkill(SkillName.Parry, 55.0, 70.0);

            Fame = 300;
            Karma = 300;

            Tamable = true;
            ControlSlots = 2;
            MinTameSkill = 96.0;

            PackItem(new Bandage(11));

            SetSpecialAbility(SpecialAbility.Heal);
        }

        public Triton(Serial serial)
            : base(serial)
        {
        }

        public override bool DeleteOnRelease { get { return true; } }
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
