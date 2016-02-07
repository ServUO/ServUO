using System;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a gremlin corpse")]
    public class Gremlin : BaseCreature
    {
        [Constructable]
        public Gremlin()
            : base(AIType.AI_Archer, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            this.Name = "a gremlin";
            this.Body = 724; 

            this.SetStr(106);
            this.SetDex(130);
            this.SetInt(36);

            this.SetHits(70);

            this.SetDamage(5, 7);

            this.SetDamageType(ResistanceType.Physical, 100);

            this.SetResistance(ResistanceType.Physical, 26);
            this.SetResistance(ResistanceType.Fire, 36);
            this.SetResistance(ResistanceType.Cold, 22);
            this.SetResistance(ResistanceType.Poison, 17);
            this.SetResistance(ResistanceType.Energy, 30);

            this.SetSkill(SkillName.Anatomy, 78.5);
            this.SetSkill(SkillName.MagicResist, 82.5);
            this.SetSkill(SkillName.Tactics, 65.3);

            this.AddItem(new Bow());
            this.PackItem(new Arrow(Utility.RandomMinMax(60, 80)));
            this.PackItem(new Apple(5));
        }

        public Gremlin(Serial serial)
            : base(serial)
        {
        }

        public override void GenerateLoot()
        {
            this.AddLoot(LootPack.Rich);
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