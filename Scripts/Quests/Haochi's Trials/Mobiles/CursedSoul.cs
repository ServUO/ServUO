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
            this.Name = "a cursed soul";
            this.Body = 3;
            this.BaseSoundID = 471;

            this.SetStr(20, 40);
            this.SetDex(40, 60);
            this.SetInt(15, 25);

            this.SetHits(10, 20);

            this.SetDamage(3, 7);

            this.SetDamageType(ResistanceType.Physical, 100);

            this.SetResistance(ResistanceType.Physical, 15, 20);
            this.SetResistance(ResistanceType.Fire, 8, 12);

            this.SetSkill(SkillName.Wrestling, 35.0, 39.0);
            this.SetSkill(SkillName.Tactics, 5.0, 15.0);
            this.SetSkill(SkillName.MagicResist, 10.0);

            this.Fame = 200;
            this.Karma = -200;

            switch ( Utility.Random(10) )
            {
                case 0:
                    this.PackItem(new LeftArm());
                    break;
                case 1:
                    this.PackItem(new RightArm());
                    break;
                case 2:
                    this.PackItem(new Torso());
                    break;
                case 3:
                    this.PackItem(new Bone());
                    break;
                case 4:
                    this.PackItem(new RibCage());
                    break;
                case 5:
                    this.PackItem(new RibCage());
                    break;
                case 6:
                    this.PackItem(new BonePile());
                    break;
                case 7:
                    this.PackItem(new BonePile());
                    break;
                case 8:
                    this.PackItem(new BonePile());
                    break;
                case 9:
                    this.PackItem(new BonePile());
                    break;
            }
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