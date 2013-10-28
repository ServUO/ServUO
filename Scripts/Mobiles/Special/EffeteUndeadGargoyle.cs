/* Based on Gargoyle, still no infos on Undead Gargoyle... Have to get also the correct body ID */
using System;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("an  effete undead gargoyle corpse")]
    public class EffeteUndeadGargoyle : BaseCreature
    {
        [Constructable]
        public EffeteUndeadGargoyle()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            this.Name = "an Effete Undead Gargoyle";
            this.Body = 722;
            this.BaseSoundID = 372;

            this.SetStr(60, 65);
            this.SetDex(60, 65);
            this.SetInt(30, 35);

            this.SetHits(65, 70);

            this.SetDamage(3, 7);

            this.SetDamageType(ResistanceType.Physical, 100);
           
            this.SetResistance(ResistanceType.Physical, 20);
            this.SetResistance(ResistanceType.Fire, 5, 10);
            this.SetResistance(ResistanceType.Cold, 25, 30);
            this.SetResistance(ResistanceType.Poison, 25);
            this.SetResistance(ResistanceType.Energy, 14, 15);

            this.SetSkill(SkillName.MagicResist, 50.0, 55.0);
            this.SetSkill(SkillName.Tactics, 50.0);
            this.SetSkill(SkillName.Wrestling, 50.0);

            this.Fame = 3500;
            this.Karma = -3500;

            this.VirtualArmor = 32;

            if (0.05 > Utility.RandomDouble())
                this.PackItem(new UndyingFlesh());
        }

        public EffeteUndeadGargoyle(Serial serial)
            : base(serial)
        {
        }

        public override int TreasureMapLevel
        {
            get
            {
                return 1;
            }
        }
        public override int Meat
        {
            get
            {
                return 1;
            }
        }
        public override void GenerateLoot()
        {
            this.AddLoot(LootPack.Meager);
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