
using System;
using Server.Items;

namespace Server.Engines.Quests
{
    public class FreedomQuest : BaseQuest
    {
        public override bool DoneOnce { get { return true; } }

        public FreedomQuest()
            : base()
        {
            AddObjective(new EscortObjective("Sanctuary Entrance"));

            AddReward(new BaseReward(typeof(StolenRing), "Lenley's Favorite Sparkly"));
        }

        /* Freedom! */
        public override object Title { get { return 1072367; } }

        /*
         * Lenley isn't seen.  Why you see me? Lenley is sneaking.  Lenley runs away.
         * You help Lenley to not get dead?  We go out past pig-men orcs?  Yes? Yes? You say yes?
        */
        public override object Description { get { return 1072552; } }

        /* You no like Lenley? No hurt Lenley!  No see Lenley.  Go 'way. */
        public override object Refuse { get { return 1072553; } }

        /* Lenley not run away yet.  Go, go, Lenley not past pig-men orcs.  You go, Lenley go after you.  Go! */
        public override object Uncomplete { get { return 1072554; } }

        /* Lenley so happy!  Lenley not get dead.  You have best Lenley shiny! */
        public override object Complete { get { return 1072556; } }

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
        public override Type[] Quests { get { return new Type[] { typeof(FreedomQuest) }; } }

        private static bool Talked { get; set; }

        [Constructable]
        public Lenley()
            : base()
        {
            Name = "Lenley";
            Title = "the snitch";
            Body = 0x2A;
            Hidden = true;

            SetStr(96, 120);
            SetDex(81, 100);
            SetInt(36, 60);

            SetHits(58, 72);

            SetDamage(4, 5);

            SetDamageType(ResistanceType.Physical, 100);

            SetResistance(ResistanceType.Physical, 25, 30);
            SetResistance(ResistanceType.Fire, 10, 20);
            SetResistance(ResistanceType.Cold, 10, 20);
            SetResistance(ResistanceType.Poison, 10, 20);
            SetResistance(ResistanceType.Energy, 10, 20);

            SetSkill(SkillName.MagicResist, 35.1, 60.0);
            SetSkill(SkillName.Tactics, 50.1, 75.0);
            SetSkill(SkillName.Wrestling, 50.1, 75.0);
            SetSkill(SkillName.Hiding, 75.0);

            Fame = 1500;
            Karma = 1500;

            VirtualArmor = 28;
        }

        public Lenley(Serial serial)
            : base(serial)
        {
        }
        
        public override void OnMovement(Mobile m, Point3D oldLocation)
        {
            if (!Talked)
            {
                if (m.InRange(this, 2))
                {
                    Talked = true;
                    Say(1075014); // Psst!  Lenley isn't seen.  You help Lenley?
                    Move(GetDirectionTo(m.Location));
                    Hidden = false;
                    SpamTimer t = new SpamTimer();
                    t.Start();
                }
                else
                    Hidden = true;
                UseSkill(SkillName.Stealth);
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
                Talked = false;
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
}
