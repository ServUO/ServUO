using System;

namespace Server.Mobiles
{
    [CorpseName("a dolphin corpse")]
    public class Dolphin : BaseCreature
    {
        [Constructable]
        public Dolphin()
            : base(AIType.AI_Animal, FightMode.Aggressor, 10, 1, 0.2, 0.4)
        {
            this.Name = "a dolphin";
            this.Body = 0x97;
            this.BaseSoundID = 0x8A;

            this.SetStr(21, 49);
            this.SetDex(66, 85);
            this.SetInt(96, 110);

            this.SetHits(15, 27);

            this.SetDamage(3, 6);

            this.SetDamageType(ResistanceType.Physical, 100);

            this.SetResistance(ResistanceType.Physical, 15, 20);
            this.SetResistance(ResistanceType.Fire, 70, 80);
            this.SetResistance(ResistanceType.Cold, 25, 30);
            this.SetResistance(ResistanceType.Poison, 10, 15);
            this.SetResistance(ResistanceType.Energy, 10, 15);

            this.SetSkill(SkillName.MagicResist, 15.1, 20.0);
            this.SetSkill(SkillName.Tactics, 19.2, 29.0);
            this.SetSkill(SkillName.Wrestling, 19.2, 29.0);

            this.Fame = 500;
            this.Karma = 2000;

            this.VirtualArmor = 16;
            this.CanSwim = true;
            this.CantWalk = true;
        }

        public Dolphin(Serial serial)
            : base(serial)
        {
        }

        public override int Meat
        {
            get
            {
                return 1;
            }
        }
        public override void OnDoubleClick(Mobile from)
        {
            if (from.AccessLevel >= AccessLevel.GameMaster)
                this.Jump();
        }

        public virtual void Jump()
        {
            if (Utility.RandomBool())
                this.Animate(3, 16, 1, true, false, 0);
            else
                this.Animate(4, 20, 1, true, false, 0);
        }

        public override void OnThink()
        {
            if (Utility.RandomDouble() < .005) // slim chance to jump
                this.Jump();

            base.OnThink();
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