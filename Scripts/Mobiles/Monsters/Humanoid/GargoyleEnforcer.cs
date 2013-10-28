using System;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a gargoyle corpse")]
    public class GargoyleEnforcer : BaseCreature
    {
        [Constructable]
        public GargoyleEnforcer()
            : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            this.Name = "Gargoyle Enforcer";
            this.Body = 0x2F2;
            this.BaseSoundID = 0x174;

            this.SetStr(760, 850);
            this.SetDex(102, 150);
            this.SetInt(152, 200);

            this.SetHits(482, 485);

            this.SetDamage(7, 14);

            this.SetResistance(ResistanceType.Physical, 40, 60);
            this.SetResistance(ResistanceType.Fire, 50, 60);
            this.SetResistance(ResistanceType.Cold, 20, 30);
            this.SetResistance(ResistanceType.Poison, 25, 35);
            this.SetResistance(ResistanceType.Energy, 15, 25);

            this.SetSkill(SkillName.MagicResist, 120.1, 130.0);
            this.SetSkill(SkillName.Tactics, 70.1, 80.0);
            this.SetSkill(SkillName.Wrestling, 80.1, 90.0);
            this.SetSkill(SkillName.Swords, 80.1, 90.0);
            this.SetSkill(SkillName.Anatomy, 70.1, 80.0);
            this.SetSkill(SkillName.Magery, 80.1, 90.0);
            this.SetSkill(SkillName.EvalInt, 70.3, 100.0);
            this.SetSkill(SkillName.Meditation, 70.3, 100.0);

            this.Fame = 5000;
            this.Karma = -5000;

            this.VirtualArmor = 50;

            if (0.2 > Utility.RandomDouble())
                this.PackItem(new GargoylesPickaxe());
        }

        public GargoyleEnforcer(Serial serial)
            : base(serial)
        {
        }

        public override int Meat
        {
            get
            {
                return 1;
            }
        }
        public override bool CanFly
        {
            get
            {
                return true;
            }
        }
        public override WeaponAbility GetWeaponAbility()
        {
            return WeaponAbility.WhirlwindAttack;
        }

        public override void GenerateLoot()
        {
            this.AddLoot(LootPack.Rich);
            this.AddLoot(LootPack.MedScrolls);
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