using System;
using Server.Mobiles;

namespace Server.Engines.Quests.Samurai
{
    [CorpseName("an injured wolf corpse")]
    public class InjuredWolf : BaseCreature
    {
        [Constructable]
        public InjuredWolf()
            : base(AIType.AI_Animal, FightMode.Aggressor, 10, 1, 0.2, 0.4)
        {
            this.Body = 0xE1;
            this.Name = "an injured wolf";
            this.BaseSoundID = 0xE5;

            this.Hue = Utility.RandomAnimalHue();

            this.SetStr(10, 20);
            this.SetDex(45, 65);
            this.SetInt(10, 15);

            this.SetHits(1);

            this.SetDamage(1, 3);

            this.SetDamageType(ResistanceType.Physical, 100);

            this.SetResistance(ResistanceType.Physical, 15);
            this.SetResistance(ResistanceType.Fire, 5, 10);

            this.SetSkill(SkillName.MagicResist, 10.0);
            this.SetSkill(SkillName.Tactics, 0.0, 5.0);
            this.SetSkill(SkillName.Wrestling, 20.0, 30.0);
        }

        public InjuredWolf(Serial serial)
            : base(serial)
        {
        }

        public override int GetIdleSound()
        {
            return 0xE9;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();
        }
    }
}