/* Based on Gargoyle, still no infos on Undead Gargoyle... Have to get also the correct body ID */

namespace Server.Mobiles
{
    [CorpseName("an  effete undead gargoyle corpse")]
    public class EffeteUndeadGargoyle : BaseCreature
    {
        [Constructable]
        public EffeteUndeadGargoyle()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "an effete undead gargoyle";
            Body = 722;
            BaseSoundID = 372;

            SetStr(60, 65);
            SetDex(60, 65);
            SetInt(30, 35);

            SetHits(65, 70);

            SetDamage(3, 7);

            SetDamageType(ResistanceType.Physical, 100);

            SetResistance(ResistanceType.Physical, 20);
            SetResistance(ResistanceType.Fire, 5, 10);
            SetResistance(ResistanceType.Cold, 25, 30);
            SetResistance(ResistanceType.Poison, 25);
            SetResistance(ResistanceType.Energy, 14, 15);

            SetSkill(SkillName.MagicResist, 50.0, 55.0);
            SetSkill(SkillName.Tactics, 50.0);
            SetSkill(SkillName.Wrestling, 50.0);

            Fame = 3500;
            Karma = -3500;
        }

        public EffeteUndeadGargoyle(Serial serial)
            : base(serial)
        {
        }

        public override int TreasureMapLevel => 1;
        public override int Meat => 1;
        public override void GenerateLoot()
        {
            AddLoot(LootPack.Meager);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}