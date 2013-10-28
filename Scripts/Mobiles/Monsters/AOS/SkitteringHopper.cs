using System;

namespace Server.Mobiles
{
    [CorpseName("a skittering hopper corpse")]
    public class SkitteringHopper : BaseCreature
    {
        [Constructable]
        public SkitteringHopper()
            : base(AIType.AI_Melee, FightMode.Aggressor, 10, 1, 0.2, 0.4)
        {
            this.Name = "a skittering hopper";
            this.Body = 302;
            this.BaseSoundID = 959;

            this.SetStr(41, 65);
            this.SetDex(91, 115);
            this.SetInt(26, 50);

            this.SetHits(31, 45);

            this.SetDamage(3, 5);

            this.SetDamageType(ResistanceType.Physical, 100);

            this.SetResistance(ResistanceType.Physical, 5, 10);
            this.SetResistance(ResistanceType.Cold, 10, 20);
            this.SetResistance(ResistanceType.Energy, 5, 10);

            this.SetSkill(SkillName.MagicResist, 30.1, 45.0);
            this.SetSkill(SkillName.Tactics, 45.1, 70.0);
            this.SetSkill(SkillName.Wrestling, 40.1, 60.0);

            this.Fame = 300;
            this.Karma = 0;

            this.Tamable = true;
            this.ControlSlots = 1;
            this.MinTameSkill = -12.9;

            this.VirtualArmor = 12;
        }

        public SkitteringHopper(Serial serial)
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