using System;
using Server.Items;

namespace Server.Mobiles 
{ 
    [CorpseName("an evil mage corpse")] 
    public class EvilMage : BaseCreature 
    { 
        [Constructable] 
        public EvilMage()
            : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        { 
            Name = NameList.RandomName("evil mage");
            Title = "the evil mage";

            var robe = new Robe(Utility.RandomNeutralHue());
            var sandals = new Sandals();

            if (!Core.UOTD)
            {
                Body = Race.Human.MaleBody;

                AddItem(robe);
                AddItem(sandals);
            }
            else
            {
                Body = 124;

                PackItem(robe);
                PackItem(sandals);
            }

            SetStr(81, 105);
            SetDex(91, 115);
            SetInt(96, 120);

            SetHits(49, 63);

            SetDamage(5, 10);

            SetDamageType(ResistanceType.Physical, 100);

            SetResistance(ResistanceType.Physical, 15, 20);
            SetResistance(ResistanceType.Fire, 5, 10);
            SetResistance(ResistanceType.Poison, 5, 10);
            SetResistance(ResistanceType.Energy, 5, 10);

            SetSkill(SkillName.EvalInt, 75.1, 100.0);
            SetSkill(SkillName.Magery, 75.1, 100.0);
            SetSkill(SkillName.MagicResist, 75.0, 97.5);
            SetSkill(SkillName.Tactics, 65.0, 87.5);
            SetSkill(SkillName.Wrestling, 20.2, 60.0);

            Fame = 2500;
            Karma = -2500;

            VirtualArmor = 16;
            PackReg(6);

            if (Core.AOS)
            {
                switch (Utility.Random(18))
                {
                    case 0: PackItem(new BloodOathScroll()); break;
                    case 1: PackItem(new CurseWeaponScroll()); break;
                    case 2: PackItem(new StrangleScroll()); break;
                }
            }
        }

        public override int GetDeathSound()
        {
            return 0x423;
        }

        public override int GetHurtSound()
        {
            return 0x436;
        }

        public EvilMage(Serial serial)
            : base(serial)
        {
        }

        public override bool CanRummageCorpses
        {
            get
            {
                return true;
            }
        }
        public override bool AlwaysMurderer
        {
            get
            {
                return true;
            }
        }
        public override int Meat
        {
            get
            {
                return 1;
            }
        }
        public override int TreasureMapLevel
        {
            get
            {
                return Core.AOS ? 1 : 0;
            }
        }
        public override void GenerateLoot()
        {
            AddLoot(LootPack.Average);
            AddLoot(LootPack.MedScrolls);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}
