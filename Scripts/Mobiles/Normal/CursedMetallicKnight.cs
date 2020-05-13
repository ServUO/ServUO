using Server.Items;

namespace Server.Mobiles
{
    public class CursedMetallicKnight : BaseCreature
    {
        [Constructable]
        public CursedMetallicKnight()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "cursed metallic knight";
            Body = 147;
            BaseSoundID = 451;

            Hue = Utility.RandomMetalHue();

            SetStr(201, 242);
            SetDex(76, 87);
            SetInt(36, 53);

            SetHits(118, 150);

            SetDamage(8, 18);

            SetDamageType(ResistanceType.Cold, 100);

            SetResistance(ResistanceType.Physical, 35, 45);
            SetResistance(ResistanceType.Fire, 20, 30);
            SetResistance(ResistanceType.Cold, 50, 60);
            SetResistance(ResistanceType.Poison, 20, 30);
            SetResistance(ResistanceType.Energy, 30, 40);

            SetSkill(SkillName.MagicResist, 67.4, 77.8);
            SetSkill(SkillName.Tactics, 89.5, 98.5);
            SetSkill(SkillName.Wrestling, 86.8, 93.1);
        }

        public override bool DeleteCorpseOnDeath => true;

        public override void GenerateLoot()
        {
            AddLoot(LootPack.LootGold(75, 200));
        }

        public override bool OnBeforeDeath()
        {
            if (!base.OnBeforeDeath())
                return false;

            new TreasureSand().MoveToWorld(this.Location, this.Map);

            return true;
        }

        public CursedMetallicKnight(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            /*int version = */
            reader.ReadInt();
        }
    }
}
