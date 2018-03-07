/* Based on Wailing Banshee, still no infos on Wight, including correct body ID */
using System;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a wight corpse")]
    public class Wight : BaseCreature
    {
        [Constructable]
        public Wight()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            this.Name = "a Wight";
            this.Body = 252;
            this.Hue = 1153;
            this.BaseSoundID = 0x482;

            this.SetStr(150, 200);
            this.SetDex(50, 60);
            this.SetInt(150, 200);

            this.SetHits(150, 250);

            this.SetDamage(13, 20);

            this.SetDamageType(ResistanceType.Physical, 20);
            this.SetDamageType(ResistanceType.Cold, 80);
			
            this.SetResistance(ResistanceType.Physical, 35, 45);
            this.SetResistance(ResistanceType.Fire, 30, 40);
            this.SetResistance(ResistanceType.Cold, 50, 60);
            this.SetResistance(ResistanceType.Poison, 40, 50);
            this.SetResistance(ResistanceType.Energy, 50, 60);

            this.SetSkill(SkillName.MagicResist, 40.0, 50.0);
            this.SetSkill(SkillName.Tactics, 45.0, 55.0);
            this.SetSkill(SkillName.Wrestling, 50.0, 60.0);
            this.SetSkill(SkillName.Magery, 60.0, 80.0);
            this.SetSkill(SkillName.Meditation, 50.0, 60.0);

            this.Fame = 1500;
            this.Karma = -1500;

            this.VirtualArmor = 19;
        }

        public Wight(Serial serial)
            : base(serial)
        {
        }

        public override bool BleedImmune
        {
            get
            {
                return true;
            }
        }
        public override WeaponAbility GetWeaponAbility()
        {
            return WeaponAbility.MortalStrike;
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