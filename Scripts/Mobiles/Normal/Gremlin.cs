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
            Name = "a gremlin";
            Body = 724; 

            SetStr(106);
            SetDex(130);
            SetInt(36);

            SetHits(70);

            SetDamage(5, 7);

            SetDamageType(ResistanceType.Physical, 100);

            SetResistance(ResistanceType.Physical, 26);
            SetResistance(ResistanceType.Fire, 36);
            SetResistance(ResistanceType.Cold, 22);
            SetResistance(ResistanceType.Poison, 17);
            SetResistance(ResistanceType.Energy, 30);

            SetSkill(SkillName.Anatomy, 78.5);
            SetSkill(SkillName.MagicResist, 82.5);
            SetSkill(SkillName.Tactics, 65.3);

            AddItem(new Bow());
            PackItem(new Arrow(Utility.RandomMinMax(60, 80)));
            PackItem(new Apple(5));
        }

        public Gremlin(Serial serial)
            : base(serial)
        {
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Rich);
        }

        public override void OnDeath(Container c)
        {
            base.OnDeath(c);

            if (Utility.RandomDouble() < 0.01)
                c.DropItem(new LuckyCoin());
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