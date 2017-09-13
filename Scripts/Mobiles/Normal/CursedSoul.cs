using System;
using Server.Items;
using Server.Mobiles;

namespace Server.Engines.Quests.Samurai
{
    [CorpseName("a cursed soul corpse")]
    public class CursedSoul : BaseCreature
    {
        [Constructable]
        public CursedSoul()
            : base(AIType.AI_Melee, FightMode.Aggressor, 10, 1, 0.2, 0.4)
        {
            Name = "a cursed soul";
            Body = 3;
            BaseSoundID = 471;

            SetStr(20, 40);
            SetDex(40, 60);
            SetInt(15, 25);

            SetHits(10, 20);

            SetDamage(3, 7);

            SetDamageType(ResistanceType.Physical, 100);

            SetResistance(ResistanceType.Physical, 15, 20);
            SetResistance(ResistanceType.Fire, 8, 12);

            SetSkill(SkillName.Wrestling, 35.0, 39.0);
            SetSkill(SkillName.Tactics, 5.0, 15.0);
            SetSkill(SkillName.MagicResist, 10.0);

            Fame = 200;
            Karma = -200;

            PackBodyPartOrBones();
        }

        public CursedSoul(Serial serial)
            : base(serial)
        {
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