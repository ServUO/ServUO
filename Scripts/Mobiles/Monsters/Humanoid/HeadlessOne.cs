using System;

namespace Server.Mobiles
{
    [CorpseName("a headless corpse")]
    public class HeadlessOne : BaseCreature
    {
        [Constructable]
        public HeadlessOne()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            this.Name = "a headless one";
            this.Body = 31;
            this.Hue = Utility.RandomSkinHue() & 0x7FFF;
            this.BaseSoundID = 0x39D;

            this.SetStr(26, 50);
            this.SetDex(36, 55);
            this.SetInt(16, 30);

            this.SetHits(16, 30);

            this.SetDamage(5, 10);

            this.SetDamageType(ResistanceType.Physical, 100);

            this.SetResistance(ResistanceType.Physical, 15, 20);

            this.SetSkill(SkillName.MagicResist, 15.1, 20.0);
            this.SetSkill(SkillName.Tactics, 25.1, 40.0);
            this.SetSkill(SkillName.Wrestling, 25.1, 40.0);

            this.Fame = 450;
            this.Karma = -450;

            this.VirtualArmor = 18;
        }

        public HeadlessOne(Serial serial)
            : base(serial)
        {
        }

        public override bool CanRummageCorpses
        {
            get
            {
                return true;
            }
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
            // TODO: body parts
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