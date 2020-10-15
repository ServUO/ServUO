using Server.Network;
using System;

namespace Server.Mobiles
{
    [CorpseName("a gooey maggots corpse")]
    public class GooeyMaggots : BaseCreature
    {
        [Constructable]
        public GooeyMaggots()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "Gooey Maggots";
            Body = 319;
            BaseSoundID = 898;

            SetStr(738, 763);
            SetDex(61, 70);
            SetInt(10);

            SetMana(0);

            SetDamage(3, 9);

            SetDamageType(ResistanceType.Physical, 50);
            SetDamageType(ResistanceType.Poison, 50);

            SetResistance(ResistanceType.Physical, 60, 70);
            SetResistance(ResistanceType.Fire, 60, 70);
            SetResistance(ResistanceType.Cold, 50, 60);
            SetResistance(ResistanceType.Poison, 60, 70);
            SetResistance(ResistanceType.Energy, 60, 70);

            SetSkill(SkillName.Tactics, 80.2, 89.7);
            SetSkill(SkillName.Wrestling, 80.2, 87.5);

            Fame = 1000;
            Karma = -1000;

            Timer selfDeleteTimer = new InternalSelfDeleteTimer(this);
            selfDeleteTimer.Start();
        }

        public GooeyMaggots(Serial serial)
            : base(serial)
        {
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Meager, 2);
        }

        protected override void OnLocationChange(Point3D oldLocation)
        {
            new GooeyMaggotSlime().MoveToWorld(oldLocation, Map);

            base.OnLocationChange(oldLocation);
        }

        public class InternalSelfDeleteTimer : Timer
        {
            private readonly GooeyMaggots creature;

            public InternalSelfDeleteTimer(Mobile p) : base(TimeSpan.FromMinutes(3))
            {
                Priority = TimerPriority.FiveSeconds;
                creature = ((GooeyMaggots)p);
            }
            protected override void OnTick()
            {
                if (creature.Map != Map.Internal)
                {
                    creature.Delete();
                    Stop();
                }
            }
        }

        public override Poison PoisonImmune => Poison.Lethal;

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            Timer SelfDeleteTimer = new InternalSelfDeleteTimer(this);
            SelfDeleteTimer.Start();
        }
    }

    public class GooeyMaggotSlime : Item
    {
        public override int LabelNumber => 1015246;  // Slime

        [Constructable]
        public GooeyMaggotSlime()
            : this(Utility.RandomList(0x1645, 0x122A, 0x122B, 0x122C, 0x122D, 0x122E, 0x122F))
        {
        }

        [Constructable]
        public GooeyMaggotSlime(int itemID)
            : base(itemID)
        {
            Movable = false;
            Hue = 363;

            new InternalTimer(this).Start();
        }

        public override bool OnMoveOver(Mobile from)
        {
            if (!from.IsStaff() && from.Alive && from.Player)
            {
                from.SendSpeedControl(SpeedControlType.WalkSpeed);
                from.SendLocalizedMessage(1152144); // You suddenly find yourself unable to run.

                Timer.DelayCall(TimeSpan.FromSeconds(1), delegate
                {
                    from.SendSpeedControl(SpeedControlType.Disable);
                    from.SendLocalizedMessage(1152145); // You are are free to move again.
                });

                Delete();
            }

            return base.OnMoveOver(from);
        }

        public GooeyMaggotSlime(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            new InternalTimer(this).Start();
        }

        private class InternalTimer : Timer
        {
            private readonly Item m_Slime;
            public InternalTimer(Item slime)
                : base(TimeSpan.FromSeconds(10.0))
            {
                Priority = TimerPriority.OneSecond;

                m_Slime = slime;
            }

            protected override void OnTick()
            {
                m_Slime.Delete();
            }
        }
    }
}
