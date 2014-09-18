using System;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a lava elemental corpse")]
    public class LavaElemental : BaseCreature
    {
        [Constructable]
        public LavaElemental()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            this.Name = "a lava elemental";
            this.Body = 720; 

            this.SetStr(446, 510);
            this.SetDex(173, 191);
            this.SetInt(369, 397);

            this.SetHits(260, 266);

            this.SetDamage(12, 18);

            this.SetDamageType(ResistanceType.Physical, 10);
            this.SetDamageType(ResistanceType.Fire, 90);

            this.SetResistance(ResistanceType.Physical, 60, 70);
            this.SetResistance(ResistanceType.Fire, 20, 30);
            this.SetResistance(ResistanceType.Cold, 20, 30);
            this.SetResistance(ResistanceType.Poison, 100);
            this.SetResistance(ResistanceType.Energy, 40, 50);

            this.SetSkill(SkillName.Anatomy, 0.0, 12.8);
            this.SetSkill(SkillName.EvalInt, 84.8, 92.6);
            this.SetSkill(SkillName.Magery, 90.1, 92.7);
            this.SetSkill(SkillName.Meditation, 97.8, 102.8);
            this.SetSkill(SkillName.MagicResist, 101.9, 106.2);
            this.SetSkill(SkillName.Tactics, 80.3, 94.0);
            this.SetSkill(SkillName.Wrestling, 71.7, 85.4);

            this.PackItem(new LesserPoisonPotion());
            this.PackReg(9);
        }

        public LavaElemental(Serial serial)
            : base(serial)
        {
        }

        public override bool HasBreath
        {
            get
            {
                return true;
            }
        }// fire breath enabled
        public override void GenerateLoot()
        {
            this.AddLoot(LootPack.FilthyRich, 3);
            this.AddLoot(LootPack.Gems, 2);
            this.AddLoot(LootPack.MedScrolls);
        }
        public override void OnDeath(Container c)
        {

            base.OnDeath(c);
            Region reg = Region.Find(c.GetWorldLocation(), c.Map);
            if (0.25> Utility.RandomDouble() && reg.Name == "Crimson Veins")
            {
                if (Utility.RandomDouble() < 0.6)
                    c.DropItem(new EssencePrecision());
            }
           
            if (0.25 > Utility.RandomDouble() && reg.Name == "Fire Temple Ruins")
            {
                if (Utility.RandomDouble() < 0.6)
                    c.DropItem(new EssenceOrder());
            }
            if (0.25 > Utility.RandomDouble() && reg.Name == "Lava Caldera")
            {
                if (Utility.RandomDouble() < 0.6)
                    c.DropItem(new EssencePassion());
            }
        }
        public override int GetIdleSound()
        {
            return 1549;
        }

        public override int GetAngerSound()
        {
            return 1546;
        }

        public override int GetHurtSound()
        {
            return 1548;
        }

        public override int GetDeathSound()
        {
            return 1547;
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