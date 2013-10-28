using System;
using Server.Ethics;

namespace Server.Mobiles
{
    [CorpseName("an unholy corpse")]
    public class UnholySteed : BaseMount
    {
        [Constructable]
        public UnholySteed()
            : base("a dark steed", 0x74, 0x3EA7, AIType.AI_Melee, FightMode.Aggressor, 10, 1, 0.2, 0.4)
        {
            this.SetStr(496, 525);
            this.SetDex(86, 105);
            this.SetInt(86, 125);

            this.SetHits(298, 315);

            this.SetDamage(16, 22);

            this.SetDamageType(ResistanceType.Physical, 40);
            this.SetDamageType(ResistanceType.Fire, 40);
            this.SetDamageType(ResistanceType.Energy, 20);

            this.SetResistance(ResistanceType.Physical, 55, 65);
            this.SetResistance(ResistanceType.Fire, 30, 40);
            this.SetResistance(ResistanceType.Cold, 30, 40);
            this.SetResistance(ResistanceType.Poison, 30, 40);
            this.SetResistance(ResistanceType.Energy, 20, 30);

            this.SetSkill(SkillName.MagicResist, 25.1, 30.0);
            this.SetSkill(SkillName.Tactics, 97.6, 100.0);
            this.SetSkill(SkillName.Wrestling, 80.5, 92.5);

            this.Fame = 14000;
            this.Karma = -14000;

            this.VirtualArmor = 60;

            this.Tamable = false;
            this.ControlSlots = 1;
        }

        public UnholySteed(Serial serial)
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
        public override bool HasBreath
        {
            get
            {
                return true;
            }
        }
        public override bool CanBreath
        {
            get
            {
                return true;
            }
        }
        public override FoodType FavoriteFood
        {
            get
            {
                return FoodType.FruitsAndVegies | FoodType.GrainsAndHay;
            }
        }
        public override string ApplyNameSuffix(string suffix)
        {
            if (suffix.Length == 0)
                suffix = Ethic.Evil.Definition.Adjunct.String;
            else
                suffix = String.Concat(suffix, " ", Ethic.Evil.Definition.Adjunct.String);

            return base.ApplyNameSuffix(suffix);
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (Ethic.Find(from) != Ethic.Evil)
                from.SendMessage("You may not ride this steed.");
            else
                base.OnDoubleClick(from);
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