using System;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a patchwork skeletal corpse")]
    public class PatchworkSkeleton : BaseCreature
    {
        [Constructable]
        public PatchworkSkeleton()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            this.Name = "a patchwork skeleton";
            this.Body = 309;
            this.BaseSoundID = 0x48D;

            this.SetStr(96, 120);
            this.SetDex(71, 95);
            this.SetInt(16, 40);

            this.SetHits(58, 72);

            this.SetDamage(18, 22);

            this.SetDamageType(ResistanceType.Physical, 85);
            this.SetDamageType(ResistanceType.Cold, 15);

            this.SetResistance(ResistanceType.Physical, 55, 65);
            this.SetResistance(ResistanceType.Fire, 50, 60);
            this.SetResistance(ResistanceType.Cold, 70, 80);
            this.SetResistance(ResistanceType.Poison, 100);
            this.SetResistance(ResistanceType.Energy, 40, 50);

            this.SetSkill(SkillName.MagicResist, 70.1, 95.0);
            this.SetSkill(SkillName.Tactics, 55.1, 80.0);
            this.SetSkill(SkillName.Wrestling, 50.1, 70.0);

            this.Fame = 500;
            this.Karma = -500;

            this.VirtualArmor = 54;
        }

        public PatchworkSkeleton(Serial serial)
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
        public override Poison PoisonImmune
        {
            get
            {
                return Poison.Lethal;
            }
        }
        public override int TreasureMapLevel
        {
            get
            {
                return 1;
            }
        }
        public override WeaponAbility GetWeaponAbility()
        {
            return WeaponAbility.Dismount;
        }

        public override void GenerateLoot()
        {
            this.AddLoot(LootPack.Meager);
        }
        public override void OnDeath(Container c)
        {

            base.OnDeath(c);
            Region reg = Region.Find(c.GetWorldLocation(), c.Map);
            if (1.0 > Utility.RandomDouble() && reg.Name == "Skeletal Dragon")
            {
                if (Utility.RandomDouble() < 0.6)
                    c.DropItem(new EssencePersistence());

            }
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