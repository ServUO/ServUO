using Server.Items;

namespace Server.Mobiles
{
    public class CursedMetallicMage : BaseCreature
    {
        [Constructable]
        public CursedMetallicMage()
            : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "cursed metallic mage";
            Body = 148;
            BaseSoundID = 451;

            Hue = Utility.RandomMetalHue();

            SetStr(78, 107);
            SetDex(66, 75);
            SetInt(191, 210);

            SetHits(46, 66);

            SetDamage(3, 7);

            SetDamageType(ResistanceType.Cold, 100);

            SetResistance(ResistanceType.Physical, 35, 40);
            SetResistance(ResistanceType.Fire, 20, 30);
            SetResistance(ResistanceType.Cold, 50, 60);
            SetResistance(ResistanceType.Poison, 20, 30);
            SetResistance(ResistanceType.Energy, 30, 40);

            SetSkill(SkillName.MagicResist, 59.7, 69.4);
            SetSkill(SkillName.Tactics, 46.2, 57.6);
            SetSkill(SkillName.Wrestling, 45.9, 54);
            SetSkill(SkillName.EvalInt, 60.6, 68.6);
            SetSkill(SkillName.Magery, 60.9, 68.5);
            SetSkill(SkillName.SpiritSpeak, 61.9, 69.1);
            SetSkill(SkillName.Necromancy, 62.2, 68.8);
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

        public CursedMetallicMage(Serial serial)
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
