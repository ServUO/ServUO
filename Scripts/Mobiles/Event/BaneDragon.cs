using System;

namespace Server.Mobiles
{
    public class BaneDragon : BaseMount
    {
        [Constructable]
        public BaneDragon()
            : base("Bane Dragon", 0x31A, 0x3EBD, AIType.AI_Mage, FightMode.Aggressor, 10, 1, 0.2, 0.4)
        {
            BaseSoundID = 0x16A;
            Hue = 1175;

            SetStr(500, 555);
            SetDex(85, 125);
            SetInt(100, 165);

            SetHits(550, 650);

            SetDamage(20, 26);

            SetDamageType(ResistanceType.Physical, 50);
            SetDamageType(ResistanceType.Cold, 25);
            SetDamageType(ResistanceType.Poison, 25);

            SetResistance(ResistanceType.Physical, 60, 70);
            SetResistance(ResistanceType.Fire, 40, 50);
            SetResistance(ResistanceType.Cold, 30, 45);
            SetResistance(ResistanceType.Poison, 50, 60);
            SetResistance(ResistanceType.Energy, 20, 40);

            SetSkill(SkillName.Anatomy, 10.0);
            SetSkill(SkillName.MagicResist, 85.0);
            SetSkill(SkillName.Tactics, 110.0);
            SetSkill(SkillName.Wrestling, 90.0);
            SetSkill(SkillName.Magery, 45.0);
            SetSkill(SkillName.EvalInt, 35.0);
            SetSkill(SkillName.Meditation, 35.0);

            Fame = 18000;
            Karma = -18000;

            Tamable = true;
            ControlSlots = 3;
            MinTameSkill = 107.1;
        }

        public override Poison HitPoison { get { return Poison.Lethal; } }
        public override bool AlwaysMurderer { get { return true; } }

        /*public override bool OnDragDrop(Mobile m, Item dropped)
        {
            // todo: blackrock stew
        }*/

        public BaneDragon(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}