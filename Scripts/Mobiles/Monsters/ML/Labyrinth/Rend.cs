using System;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a Rend corpse")]
    public class Rend : Reptalon
    {
        [Constructable]
        public Rend()
        {

            this.Name = "Rend";
            this.Hue = 0x455;

            this.SetStr(1261, 1284);
            this.SetDex(363, 384);
            this.SetInt(601, 642);

            this.SetHits(5176, 6100);

            this.SetDamage(26, 33);

            this.SetDamageType(ResistanceType.Physical, 100);
            this.SetDamageType(ResistanceType.Poison, 0);
            this.SetDamageType(ResistanceType.Energy, 0);

            this.SetResistance(ResistanceType.Physical, 75, 85);
            this.SetResistance(ResistanceType.Fire, 81, 94);
            this.SetResistance(ResistanceType.Cold, 46, 55);
            this.SetResistance(ResistanceType.Poison, 35, 44);
            this.SetResistance(ResistanceType.Energy, 45, 52);

            this.SetSkill(SkillName.Wrestling, 136.3, 150.3);
            this.SetSkill(SkillName.Tactics, 133.4, 141.4);
            this.SetSkill(SkillName.MagicResist, 90.9, 110.0);
            this.SetSkill(SkillName.Anatomy, 66.6, 72.0);

            this.Fame = 21000;
            this.Karma = -21000;

            Tamable = false;            
        }

        public Rend(Serial serial)
            : base(serial)
        {
        }

        public override bool HasBreath{ get{ return true; } } // fire breath enabled
        public override double BreathDamageScalar{ get{ return 0.06; } }
        public override bool GivesMLMinorArtifact{get{ return true; } }
        
        public override void GenerateLoot()
        {
            this.AddLoot(LootPack.UltraRich, 3);
        }

        public override WeaponAbility GetWeaponAbility()
        {
            if (Utility.RandomBool())
                return WeaponAbility.ParalyzingBlow;
            else
                return WeaponAbility.BleedAttack;
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