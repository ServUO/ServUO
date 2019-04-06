using System;
using Server;
using Server.Items;
using Server.Engines.MyrmidexInvasion;

namespace Server.Mobiles
{
    [CorpseName("a myrmidex corpse")]
    public class MyrmidexDrone : BaseCreature
    {
        [Constructable]
        public MyrmidexDrone()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, .2, .4)
        {
            Name = "a myrmidex drone";

            Body = 1402;
            BaseSoundID = 959;
            //Hue = 2676;

            SetStr(76, 105);
            SetDex(96, 136);
            SetInt(25, 44);

            SetDamage(6, 12);

            SetHits(460, 597);
            SetMana(0);

            SetResistance(ResistanceType.Physical, 1, 5);
            SetResistance(ResistanceType.Fire, 1, 5);
            SetResistance(ResistanceType.Cold, 1, 5);
            SetResistance(ResistanceType.Poison, 1, 5);
            SetResistance(ResistanceType.Energy, 1, 5);

            SetDamageType(ResistanceType.Physical, 50);
            SetDamageType(ResistanceType.Poison, 50);

            SetSkill(SkillName.MagicResist, 30.1, 43.5);
            SetSkill(SkillName.Tactics, 30.1, 49.0);
            SetSkill(SkillName.Wrestling, 41.1, 49.8);

            PackGold(50, 70);

            Fame = 2500;
            Karma = -2500;
        }

        public override bool OnBeforeDeath()
        {
            if (Region.IsPartOf("MyrmidexBattleground") && 0.25 > Utility.RandomDouble())
                PackItem(new MyrmidexEggsac(Utility.RandomMinMax(1, 5)));

            return base.OnBeforeDeath();
        }

        public override int Meat { get { return 4; } }
        public override Poison HitPoison { get { return Poison.Regular; } }
        public override Poison PoisonImmune { get { return Poison.Regular; } }

        public override bool IsEnemy(Mobile m)
        {
            if (MyrmidexInvasionSystem.Active && MyrmidexInvasionSystem.IsAlliedWithEodonTribes(m))
                return true;

            if (MyrmidexInvasionSystem.Active && MyrmidexInvasionSystem.IsAlliedWithMyrmidex(m))
                return false;

            return base.IsEnemy(m);
        }

        public MyrmidexDrone(Serial serial)
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
            int version = reader.ReadInt();
        }
    }
}