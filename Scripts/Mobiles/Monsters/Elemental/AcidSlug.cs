using System;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("an acid slug corpse")]
    public class AcidSlug : BaseCreature
    {
        [Constructable]
        public AcidSlug()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            this.Name = "an acid slug";
            this.Body = 51;
            this.Hue = 44;

            this.SetStr(250, 290);
            this.SetDex(70, 105);
            this.SetInt(10, 30);

            this.SetHits(300, 350);

            this.SetDamage(11, 17);

            this.SetDamageType(ResistanceType.Physical, 60);
            this.SetDamageType(ResistanceType.Poison, 40);

            this.SetResistance(ResistanceType.Physical, 15);
            this.SetResistance(ResistanceType.Fire, 0);
            this.SetResistance(ResistanceType.Cold, 15);
            this.SetResistance(ResistanceType.Poison, 70);
            this.SetResistance(ResistanceType.Energy, 15);

            this.SetSkill(SkillName.MagicResist, 25.0);
            this.SetSkill(SkillName.Tactics, 30.0, 50.0);
            this.SetSkill(SkillName.Wrestling, 30.0, 80.0);

            if (0.1 > Utility.RandomDouble())
                this.PackItem(new VialOfVitriol());

            if (0.75 > Utility.RandomDouble())
                this.PackItem(new AcidSac());

            this.PackItem(new CongealedSlugAcid());
        }

        public AcidSlug(Serial serial)
            : base(serial)
        {
        }

        public override void GenerateLoot()
        {
            this.AddLoot(LootPack.Average);
        }

        public override int GetIdleSound()
        {
            return 1499;
        }

        public override int GetAngerSound()
        {
            return 1496;
        }

        public override int GetHurtSound()
        {
            return 1498;
        }

        public override int GetDeathSound()
        {
            return 1497;
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