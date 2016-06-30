using System;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("Fire Daemon [Renowned] corpse")]  
    public class FireDaemonRenowned : BaseRenowned
    {
        [Constructable]
        public FireDaemonRenowned()
            : base(AIType.AI_Mage)
        {
            this.Name = "Fire Daemon";
            this.Title = "[Renowned]";
            this.Body = 40;
            this.BaseSoundID = 357;

            this.Hue = 243;

            this.SetStr(800, 1199);
            this.SetDex(200, 250);
            this.SetInt(202, 336);

            this.SetHits(1111, 1478);

            this.SetDamage(22, 29);

            this.SetDamageType(ResistanceType.Physical, 50);
            this.SetDamageType(ResistanceType.Fire, 25);
            this.SetDamageType(ResistanceType.Energy, 25);

            this.SetResistance(ResistanceType.Physical, 60, 93);
            this.SetResistance(ResistanceType.Fire, 60, 100);
            this.SetResistance(ResistanceType.Cold, 40, 70);
            this.SetResistance(ResistanceType.Poison, 100);
            this.SetResistance(ResistanceType.Energy, 37, 50);

            this.SetSkill(SkillName.MagicResist, 110.1, 132.6);
            this.SetSkill(SkillName.Tactics, 86.9, 95.5);
            this.SetSkill(SkillName.Wrestling, 42.2, 98.8);
            this.SetSkill(SkillName.Magery, 97.1, 100.8);
            this.SetSkill(SkillName.EvalInt, 91.1, 91.8);
            this.SetSkill(SkillName.Meditation, 45.4, 94.1);
            this.SetSkill(SkillName.Anatomy, 45.4, 74.1);

            this.Fame = 7000;
            this.Karma = -10000;

            this.VirtualArmor = 55;
            this.QLPoints = 50;
                        
            this.PackItem(new EssencePassion());
        }

        public FireDaemonRenowned(Serial serial)
            : base(serial)
        {
        }

        public override Type[] UniqueSAList
        {
            get
            {
                return new Type[] { typeof(ResonantStaffofEnlightenment), typeof(MantleOfTheFallen) };
            }
        }
        public override Type[] SharedSAList
        {
            get
            {
                return new Type[] { typeof(LegacyOfDespair) };
            }
        }
        public override WeaponAbility GetWeaponAbility()
        {
            return WeaponAbility.ConcussionBlow;
        }

        public override void GenerateLoot()
        {
            this.AddLoot(LootPack.FilthyRich, 2);
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