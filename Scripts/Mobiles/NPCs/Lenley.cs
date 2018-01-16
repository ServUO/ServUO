
using System;
using Server.Items;
using Server.Network;
using Server.Targeting;
using Server.Misc;

namespace Server.Engines.Quests
{
    public class FreedomQuest : BaseQuest
    {
        public override bool DoneOnce { get { return true; } }

        public FreedomQuest()
            : base()
        {
            this.AddObjective(new EscortObjective("Sanctuary Entrance"));

            this.AddReward(new BaseReward(typeof(StolenRing), "Lenley's Favorite Sparkly"));
        }

        /*Freedom!*/
        public override object Title
        {
            get
            {
                return 1072367;
            }
        }
        /*Lenley isn't seen.  Why you see me? Lenley is sneaking.  Lenley runs away.
         You help Lenley to not get dead?  We go out past pig-men orcs?  Yes? Yes? You say yes?*/
        public override object Description
        {
            get
            {
                return 1072552;
            }
        }
        /*Well, if you change your mind, I’ll be here.*/
        public override object Refuse
        {
            get
            {
                return 1113784;
            }
        }
        /*Lenley not run away yet.  Go, go, Lenley not past pig-men orcs.  You go, Lenley go after you.  Go!*/
        public override object Uncomplete
        {
            get
            {
                return 1072554;
            }
        }
        /*Lenley so happy!  Lenley not get dead.  You have best Lenley shiny!*/
        public override object Complete
        {
            get
            {
                return 1072556;
            }
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

    public class Lenley : BaseEscort
    {
        private static bool m_Talked;
      
         string[] Lenleysay = new string[]  
		      {  
               "Psst! Lenley isn't seen. You help Lenley?",
		};


        [Constructable]
        public Lenley()
            : base()
        {
            this.Name = "Lenley";
            this.Title = "the snitch";
            this.Body = 45;
            this.Hidden = true;

            this.SetStr(96, 120);
            this.SetDex(81, 100);
            this.SetInt(36, 60);

            this.SetHits(58, 72);

            this.SetDamage(4, 5);

            this.SetDamageType(ResistanceType.Physical, 100);

            this.SetResistance(ResistanceType.Physical, 25, 30);
            this.SetResistance(ResistanceType.Fire, 10, 20);
            this.SetResistance(ResistanceType.Cold, 10, 20);
            this.SetResistance(ResistanceType.Poison, 10, 20);
            this.SetResistance(ResistanceType.Energy, 10, 20);

            this.SetSkill(SkillName.MagicResist, 35.1, 60.0);
            this.SetSkill(SkillName.Tactics, 50.1, 75.0);
            this.SetSkill(SkillName.Wrestling, 50.1, 75.0);
            this.SetSkill(SkillName.Hiding, 75.0);

            this.Fame = 1500;
            this.Karma = 1500;

            this.VirtualArmor = 28;
        }

        public Lenley(Serial serial)
            : base(serial)
        {
        }

        public override Type[] Quests
        {
            get
            {
                return new Type[] 
                {
                    typeof(FreedomQuest)
                };
            }
        }

        public override void OnMovement(Mobile m, Point3D oldLocation)
        {

             if (m_Talked == false)
            {

                if (m.InRange(this, 2))
                {
                    m_Talked = true;
                    //m.SendMessage("Psst! Lenley isn't seen. You help Lenley?");
                    SayRandom(Lenleysay, this);
                    this.Move(GetDirectionTo(m.Location));
                    this.Hidden = false;
                    SpamTimer t = new SpamTimer();
                    t.Start();
                }
                else
                    this.Hidden = true;
                    this.UseSkill(SkillName.Stealth);
            }
        }

        private class SpamTimer : Timer
        {
            public SpamTimer()
                : base(TimeSpan.FromSeconds(30))
            {
                Priority = TimerPriority.OneSecond;
            }

            protected override void OnTick()
            {
                m_Talked = false;
            
            }
        }

        private static void SayRandom(string[] say, Mobile m)
        {
            m.Say(say[Utility.Random(say.Length)]);
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
