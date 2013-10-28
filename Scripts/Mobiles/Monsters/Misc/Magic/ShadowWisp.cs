using System;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a wisp corpse")]
    public class ShadowWisp : BaseCreature
    {
        [Constructable]
        public ShadowWisp()
            : base(AIType.AI_Mage, FightMode.Aggressor, 10, 1, 0.3, 0.6)
        {
            this.Name = "a shadow wisp";
            this.Body = 165;
            this.BaseSoundID = 466;

            this.SetStr(16, 40);
            this.SetDex(16, 45);
            this.SetInt(11, 25);

            this.SetHits(10, 24);

            this.SetDamage(5, 10);

            this.SetDamageType(ResistanceType.Physical, 100);

            this.SetResistance(ResistanceType.Physical, 15, 20);
            this.SetResistance(ResistanceType.Fire, 5, 10);
            this.SetResistance(ResistanceType.Poison, 5, 10);
            this.SetResistance(ResistanceType.Energy, 15, 20);

            this.SetSkill(SkillName.EvalInt, 40.0);
            this.SetSkill(SkillName.Magery, 50.0);
            this.SetSkill(SkillName.Meditation, 40.0);
            this.SetSkill(SkillName.MagicResist, 10.0);
            this.SetSkill(SkillName.Tactics, 0.1, 15.0);
            this.SetSkill(SkillName.Wrestling, 25.1, 40.0);

            this.Fame = 500;

            this.VirtualArmor = 18;

            this.AddItem(new LightSource());

            switch ( Utility.Random(10))
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

        public ShadowWisp(Serial serial)
            : base(serial)
        {
        }

        public override OppositionGroup OppositionGroup
        {
            get
            {
                return OppositionGroup.FeyAndUndead;
            }
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