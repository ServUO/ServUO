using System;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a gazer larva corpse")]
    public class GazerLarva : BaseCreature
    {
        [Constructable]
        public GazerLarva()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            this.Name = "a gazer larva";
            this.Body = 778;
            this.BaseSoundID = 377;

            this.SetStr(76, 100);
            this.SetDex(51, 75);
            this.SetInt(56, 80);

            this.SetHits(36, 47);

            this.SetDamage(2, 9);

            this.SetDamageType(ResistanceType.Physical, 100);

            this.SetResistance(ResistanceType.Physical, 15, 25);

            this.SetSkill(SkillName.MagicResist, 70.0);
            this.SetSkill(SkillName.Tactics, 70.0);
            this.SetSkill(SkillName.Wrestling, 70.0);

            this.Fame = 900;
            this.Karma = -900;

            this.VirtualArmor = 25;

            this.PackItem(new Nightshade(Utility.RandomMinMax(2, 3)));
        }

        public GazerLarva(Serial serial)
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
        public override void GenerateLoot()
        {
            this.AddLoot(LootPack.Poor);
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