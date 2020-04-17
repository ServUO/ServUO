using System.Collections.Generic;

namespace Server.Items
{
    public class AutomatonStatue : Item
    {
        [CommandProperty(AccessLevel.GameMaster)]
        public KotlAutomaton Automaton { get; set; }

        public override int LabelNumber => 1124395;  // Automaton
        public override bool HandlesOnMovement => Automaton == null;

        [Constructable]
        public AutomatonStatue()
            : base(Utility.RandomBool() ? 0x9DB3 : 0x9DB4)
        {
            Movable = false;
        }

        public override void OnMovement(Mobile m, Point3D lastLocation)
        {
            base.OnMovement(m, lastLocation);

            if (m.Player && m.InRange(Location, 5) && m.AccessLevel == AccessLevel.Player && 0.5 > Utility.RandomDouble())
            {
                KotlAutomaton automaton = new KotlAutomaton();
                automaton.MoveToWorld(Location, Map);

                OnBirth(automaton, this);

                Visible = false;
                Automaton = automaton;

                Timer.DelayCall(() => automaton.Combatant = m);
            }
        }

        public static Dictionary<KotlAutomaton, AutomatonStatue> Statues { get; set; }

        public static void OnBirth(KotlAutomaton automaton, AutomatonStatue statue)
        {
            if (Statues == null)
                Statues = new Dictionary<KotlAutomaton, AutomatonStatue>();

            if (!Statues.ContainsKey(automaton))
                Statues[automaton] = statue;

        }

        public static void OnDeath(KotlAutomaton automaton)
        {
            if (Statues == null)
                return;

            if (Statues.ContainsKey(automaton) && Statues[automaton] != null && !Statues[automaton].Deleted)
            {
                Statues[automaton].Delete();
                Statues.Remove(automaton);

                if (Statues.Count == 0)
                    Statues = null;
            }
        }

        public AutomatonStatue(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version

            if (Automaton != null)
            {
                writer.Write(0);
                writer.Write(Automaton);
            }
            else
            {
                writer.Write(1);
            }
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            if (reader.ReadInt() == 0)
            {
                Automaton = reader.ReadMobile() as KotlAutomaton;

                if (Automaton == null)
                {
                    Delete();
                }
            }
        }
    }
}
