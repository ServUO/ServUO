using System;
using Server.Ethics;

namespace Server.Mobiles
{
    [CorpseName("a holy corpse")]
    public class HolyFamiliar : BaseCreature
    {
        [Constructable]
        public HolyFamiliar()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            this.Name = "a silver wolf";
            this.Body = 100;
            this.BaseSoundID = 0xE5;

            this.SetStr(96, 120);
            this.SetDex(81, 105);
            this.SetInt(36, 60);

            this.SetHits(58, 72);
            this.SetMana(0);

            this.SetDamage(11, 17);

            this.SetDamageType(ResistanceType.Physical, 100);

            this.SetResistance(ResistanceType.Physical, 20, 25);
            this.SetResistance(ResistanceType.Fire, 10, 20);
            this.SetResistance(ResistanceType.Cold, 5, 10);
            this.SetResistance(ResistanceType.Poison, 5, 10);
            this.SetResistance(ResistanceType.Energy, 10, 15);

            this.SetSkill(SkillName.MagicResist, 57.6, 75.0);
            this.SetSkill(SkillName.Tactics, 50.1, 70.0);
            this.SetSkill(SkillName.Wrestling, 60.1, 80.0);

            this.Fame = 2500;
            this.Karma = 2500;

            this.VirtualArmor = 22;

            this.Tamable = false;
            this.ControlSlots = 1;
        }

        public HolyFamiliar(Serial serial)
            : base(serial)
        {
        }

        public override bool IsDispellable
        {
            get
            {
                return false;
            }
        }
        public override bool IsBondable
        {
            get
            {
                return false;
            }
        }
        public override int Meat
        {
            get
            {
                return 1;
            }
        }
        public override int Hides
        {
            get
            {
                return 7;
            }
        }
        public override FoodType FavoriteFood
        {
            get
            {
                return FoodType.Meat;
            }
        }
        public override PackInstinct PackInstinct
        {
            get
            {
                return PackInstinct.Canine;
            }
        }
        public override string ApplyNameSuffix(string suffix)
        {
            if (suffix.Length == 0)
                suffix = Ethic.Hero.Definition.Adjunct.String;
            else
                suffix = String.Concat(suffix, " ", Ethic.Hero.Definition.Adjunct.String);

            return base.ApplyNameSuffix(suffix);
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