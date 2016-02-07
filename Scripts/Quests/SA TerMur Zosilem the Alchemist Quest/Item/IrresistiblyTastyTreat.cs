using System;
using Server.Mobiles;
using Server.Targeting;

namespace Server.Items
{
    public class IrresistiblyTastyTreat : Item
    {
        private bool m_Used;
        [Constructable]
        public IrresistiblyTastyTreat()
            : base(0xF7E)
        {
            this.m_Used = false;
        }

        public IrresistiblyTastyTreat(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1113005;
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
            list.Add(1113216);
            list.Add(1113217); 
            list.Add(1070722, "Duration: 10 Min"); 
            list.Add(1042971, "Cooldown: 2 Hours"); 
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
            private readonly IrresistiblyTastyTreat m_Tasty;
            private int Base1;
            private int Base2;
            private int Change1;
            private int Change2;
            private int Change3;
            private int Change4;
            private int Change5;
            private int Change6;
            private int Change7;
            private int Change8;
            public InternalTarget(IrresistiblyTastyTreat tasty)
                : base(10, false, TargetFlags.None)
            {
                this.m_Tasty = tasty;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                PlayerMobile pm = from as PlayerMobile;

                if (this.m_Tasty.Deleted)
                    return;
                if (targeted is Mobile)
                { 
                    if (targeted is BaseCreature)
                    { 
                        BaseCreature creature = (BaseCreature)targeted;

                        this.Base1 = (creature.DamageMin); 
                        this.Base2 = (creature.DamageMax); 

                        this.Change1 = (int)((creature.DamageMin) * 1.10); 
                        this.Change2 = (int)((creature.DamageMax) * 1.10);   

                        this.Change6 = (creature.RawStr); 
                        this.Change7 = (creature.RawDex);
                        this.Change8 = (creature.RawInt);
  
                        this.Change3 = (int)((creature.RawStr) * 1.15); 
                        this.Change4 = (int)((creature.RawDex) * 1.15);
                        this.Change5 = (int)((creature.RawInt) * 1.15);
              
                        if ((creature.Controlled || creature.Summoned) && (from == creature.ControlMaster) && !(creature.Asleep))
                        {
                            creature.FixedParticles(0x376A, 9, 32, 5005, EffectLayer.Waist);
                            creature.PlaySound(0x1E9);

                            creature.SetDamage(this.Change1, this.Change2);
 
                            creature.RawStr = this.Change3;
                            creature.RawDex = this.Change4;
                            creature.RawInt = this.Change5;

                            from.SendMessage("You have increased the Stats of your pet by 15% and the Damage by 10% for 10 Minutes !!");  
                            this.m_Tasty.m_Used = true;
                            creature.Asleep = true; 

                            Timer.DelayCall(TimeSpan.FromMinutes(10.0), delegate()
                            {
                                creature.SetDamage(this.Base1, this.Base2);                     
                      
                                creature.RawStr = this.Change6;
                                creature.RawDex = this.Change7;
                                creature.RawInt = this.Change8;
                       
                                this.m_Tasty.m_Used = true; 
                                creature.Asleep = false;
                                from.SendMessage("The effect of Irresistibly Tasty Treat is Finish !");   

                                Timer.DelayCall(TimeSpan.FromMinutes(120.0), delegate()
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
}