using System;
using Server.Mobiles;
using Server.Targeting;

namespace Server.Items
{
    public class DeliciouslyTastyTreat : Item
    {
		public override bool IsArtifact { get { return true; } }
        private bool m_Used;
        [Constructable]
        public DeliciouslyTastyTreat()
            : base(0xF7E)
        {
            this.Hue = 1745;

            this.m_Used = false;
        }

        public DeliciouslyTastyTreat(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1113004;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public bool Used
        {
            get
            {
                return this.m_Used;
            }
            set
            {
                this.m_Used = value;
            }
        }
        public override void AddNameProperties(ObjectPropertyList list)
        {
            base.AddNameProperties(list);

            list.Add(1113213);
            list.Add(1113215);
            list.Add(1070722, "Duration: 10 Min");
            list.Add(1042971, "Cooldown: 60 Min");
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!this.m_Used)
            {
                from.SendMessage("Which animal you want to Targhet ?");

                from.Target = new InternalTarget(this);
            }
            else
            {
                from.SendLocalizedMessage(1113051);
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version

            writer.Write((bool)this.m_Used);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            this.m_Used = reader.ReadBool();
        }

        private class InternalTarget : Target
        {
            private readonly DeliciouslyTastyTreat m_Tasty;
            private int Change1;
            private int Change2;
            private int Change3;
            private int Change4;
            private int Change5;
            private int Change6;
            public InternalTarget(DeliciouslyTastyTreat tasty)
                : base(10, false, TargetFlags.None)
            {
                this.m_Tasty = tasty;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                PlayerMobile pm = from as PlayerMobile;

                if (this.m_Tasty.Deleted)
                    return;

                if (targeted is BaseCreature)
                {
                    BaseCreature creature = (BaseCreature)targeted;

                    this.Change1 = (int)((creature.RawStr) * 1.10);
                    this.Change2 = (int)((creature.RawDex) * 1.10);
                    this.Change3 = (int)((creature.RawInt) * 1.10);

                    this.Change4 = (int)(creature.RawStr);
                    this.Change5 = (int)(creature.RawDex);
                    this.Change6 = (int)(creature.RawInt);

                    if ((creature.Controlled || creature.Summoned) && (from == creature.ControlMaster) && !(creature.Asleep))
                    {
                        creature.FixedParticles(0x373A, 10, 15, 5018, EffectLayer.Waist);
                        creature.PlaySound(0x1EA);

                        creature.RawStr = this.Change1;
                        creature.RawDex = this.Change2;
                        creature.RawInt = this.Change3;

                        from.SendMessage("You have increased the Stats of your pet by 10% for 10 Minutes !!");
                        this.m_Tasty.m_Used = true;
                        creature.Asleep = true;

                        Timer.DelayCall(TimeSpan.FromMinutes(10.0), delegate()
                        {
                            creature.RawStr = this.Change4;
                            creature.RawDex = this.Change5;
                            creature.RawInt = this.Change6;
                            creature.PlaySound(0x1EB);

                            this.m_Tasty.m_Used = true;
                            creature.Asleep = false;

                            from.SendMessage("The effect of Deliciously Tasty Treat is Finish !");

                            Timer.DelayCall(TimeSpan.FromMinutes(60.0), delegate()
                            {
                                this.m_Tasty.m_Used = false;
                            });
                        });
                    }
                    else if ((creature.Controlled || creature.Summoned) && (from == creature.ControlMaster) && (creature.Asleep))
                    {
                        from.SendLocalizedMessage(502676);
                    }
                    else
                    {
                        from.SendLocalizedMessage(1113049);
                    }
                }
                else
                {
                    from.SendLocalizedMessage(500329);
                }
            }
        }
    }
}