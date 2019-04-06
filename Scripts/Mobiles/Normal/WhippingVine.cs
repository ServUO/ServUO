using System;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a whipping vine corpse")]
    public class WhippingVine : BaseCreature
    {
        [Constructable]
        public WhippingVine()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            this.Name = "a whipping vine";
            this.Body = 8;
            this.Hue = 0x851;
            this.BaseSoundID = 352;

            this.SetStr(251, 300);
            this.SetDex(76, 100);
            this.SetInt(26, 40);

            this.SetMana(0);

            this.SetDamage(7, 25);

            this.SetDamageType(ResistanceType.Physical, 70);
            this.SetDamageType(ResistanceType.Poison, 30);

            this.SetResistance(ResistanceType.Physical, 75, 85);
            this.SetResistance(ResistanceType.Fire, 15, 25);
            this.SetResistance(ResistanceType.Cold, 15, 25);
            this.SetResistance(ResistanceType.Poison, 75, 85);
            this.SetResistance(ResistanceType.Energy, 35, 45);

            this.SetSkill(SkillName.MagicResist, 70.0);
            this.SetSkill(SkillName.Tactics, 70.0);
            this.SetSkill(SkillName.Wrestling, 70.0);

            this.Fame = 1000;
            this.Karma = -1000;

            this.VirtualArmor = 45;

            this.PackReg(3);
            this.PackItem(new FertileDirt(Utility.RandomMinMax(1, 10)));

            if (0.2 >= Utility.RandomDouble())
                this.PackItem(new ExecutionersCap());

            PackItem(new Vines());  //this is correct
            PackItem(new FertileDirt(Utility.RandomMinMax(1, 10)));

            if (Utility.RandomDouble() < 0.10)
            {
                PackItem(new DecorativeVines());
            }
        }

        public WhippingVine(Serial serial)
            : base(serial)
        {
        }

        public override bool BardImmune
        {
            get
            {
                return !Core.AOS;
            }
        }
        public override Poison PoisonImmune
        {
            get
            {
                return Poison.Lethal;
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