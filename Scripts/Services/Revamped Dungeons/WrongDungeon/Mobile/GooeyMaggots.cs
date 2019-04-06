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
            this.Name = "Gooey Maggots";
            this.Body = 319;
            this.BaseSoundID = 898;

            this.SetStr(738, 763);
            this.SetDex(61, 70);
            this.SetInt(10);

            this.SetMana(0);

            this.SetDamage(3, 9);

            this.SetDamageType(ResistanceType.Physical, 50);
            this.SetDamageType(ResistanceType.Poison, 50);

            this.SetResistance(ResistanceType.Physical, 60, 70);
            this.SetResistance(ResistanceType.Fire, 60, 70);
            this.SetResistance(ResistanceType.Cold, 50, 60);
            this.SetResistance(ResistanceType.Poison, 60, 70);
            this.SetResistance(ResistanceType.Energy, 60, 70);

            this.SetSkill(SkillName.Tactics, 80.2, 89.7);
            this.SetSkill(SkillName.Wrestling, 80.2, 87.5);

            this.Fame = 1000;
            this.Karma = -1000;

            this.VirtualArmor = 24;

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
            new GooeyMaggotSlime().MoveToWorld(oldLocation, this.Map);

            base.OnLocationChange(oldLocation);
        }

        public class InternalSelfDeleteTimer : Timer
        {
            private GooeyMaggots creature;

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
                    this.Stop();
                }
            }
        }

        public override Poison PoisonImmune { get { return Poison.Lethal; } }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
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
        public override int LabelNumber { get { return 1015246; } } // Slime

        [Constructable]
        public GooeyMaggotSlime()
            : this(Utility.RandomList(0x1645, 0x122A, 0x122B, 0x122C, 0x122D, 0x122E, 0x122F))
        {
        }

        [Constructable]
        public GooeyMaggotSlime(int itemID)
            : base(itemID)
        {
            this.Movable = false;
            this.Hue = 363;

            new InternalTimer(this).Start();
        }

        public override bool OnMoveOver(Mobile from)
        {
            if (!from.IsStaff() && from.Alive && from.Player)
            {
                from.SendSpeedControl(SpeedControlType.WalkSpeed);
                from.SendLocalizedMessage(1152144); // You suddenly find yourself unable to run.

                Timer.DelayCall(TimeSpan.FromSeconds(1), new TimerCallback(
                    delegate
                    {
                        from.SendSpeedControl(SpeedControlType.Disable);
                        from.SendLocalizedMessage(1152145); // You are are free to move again.
                    }));

                this.Delete();
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
            writer.Write((int)0); // version
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
                this.Priority = TimerPriority.OneSecond;

                this.m_Slime = slime;
            }

            protected override void OnTick()
            {
                this.m_Slime.Delete();
            }
        }
    }
}