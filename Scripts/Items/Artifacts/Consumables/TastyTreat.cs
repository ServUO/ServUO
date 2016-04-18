using System;
using Server.Mobiles;
using Server.Targeting;

namespace Server.Items
{
    public class TastyTreat : Item
    {
		public override bool IsArtifact { get { return true; } }
        private bool m_Used;
        [Constructable]
        public TastyTreat()
            : base(0xF7E)
        {
            this.Hue = 1745;

            this.m_Used = false;
        }

        public TastyTreat(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1113003;
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
            list.Add(1113214);
            list.Add(1070722, "Duration: 20 Min");
            list.Add(1042971, "Cooldown: 2 Min");
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!this.m_Used)
            {
                from.SendMessage("which animal you want to Targhet ?");

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
            private readonly TastyTreat m_Tasty;
            private int Change1;
            private int Change2;
            private int Change3;
            private int Change4;
            private int Change5;
            private int Change6;
            public InternalTarget(TastyTreat tasty)
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

                    this.Change1 = (int)((creature.RawStr) * 1.05);
                    this.Change2 = (int)((creature.RawDex) * 1.05);
                    this.Change3 = (int)((creature.RawInt) * 1.05);

                    this.Change4 = (int)(creature.RawStr);
                    this.Change5 = (int)(creature.RawDex);
                    this.Change6 = (int)(creature.RawInt);

                    if ((creature.Controlled || creature.Summoned) && (from == creature.ControlMaster) && !(creature.Asleep))
                    {
                        creature.FixedParticles(0x373A, 10, 15, 5018, EffectLayer.Waist);
                        creature.PlaySound(0x1EB);

                        creature.RawStr = this.Change1;
                        creature.RawDex = this.Change2;
                        creature.RawInt = this.Change3;

                        from.SendMessage("You have increased the Stats of your pet by 5% for 20 Minutes!!");
                        this.m_Tasty.m_Used = true;
                        creature.Asleep = true;

                        Timer.DelayCall(TimeSpan.FromMinutes(20.0), delegate()
                        {
                            creature.RawStr = this.Change4;
                            creature.RawDex = this.Change5;
                            creature.RawInt = this.Change6;
                            creature.PlaySound(0x1DF);

                            this.m_Tasty.m_Used = true;
                            creature.Asleep = false;
                            from.SendMessage("The effect of Tasty Treat is Finish !");

                            Timer.DelayCall(TimeSpan.FromMinutes(2.0), delegate()
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