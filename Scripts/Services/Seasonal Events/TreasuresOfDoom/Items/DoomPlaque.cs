using Server.Network;
using Server.Prompts;
using Server.Engines.TreasuresOfDoom;

using System;
using System.Collections.Generic;

namespace Server.Items
{
    public class DoomPlaque : Item
    {
        public override int LabelNumber => 1155662;  // Plaque
        public override bool ForceShowProperties => true;

        public Dictionary<Mobile, DateTime> NextMessage { get; set; }

        public static readonly Point3D TeleportDestination = new Point3D(76, 224, 4);

        [Constructable]
        public DoomPlaque()
            : base(0x4B20)
        {
            Movable = false;
            Hue = 2500;
        }

        public override void OnDoubleClick(Mobile m)
        {
            if (m.InRange(GetWorldLocation(), 2))
            {
                m.Prompt = new DoomPlaquePrompt();
            }
        }

        public override bool HandlesOnMovement => true;

        public override void OnMovement(Mobile m, Point3D oldLocation)
        {
            if (TreasuresOfDoomEvent.Instance.Running && m.Player && m.InRange(Location, 3) && m.InLOS(this))
            {
                if (NextMessage == null)
                {
                    NextMessage = new Dictionary<Mobile, DateTime>();
                }
                else
                {
                    CheckList();
                }

                if (!NextMessage.ContainsKey(m))
                {
                    m.PrivateOverheadMessage(MessageType.Regular, 0x3B2, 1155660, m.NetState); // *You examine the plaque...there appears to be a series of runic characters that are raised up...they look like buttons...*

                    NextMessage[m] = DateTime.UtcNow + TimeSpan.FromMinutes(10);
                }
            }
        }

        private void CheckList()
        {
            if (NextMessage == null)
            {
                return;
            }

            List<Mobile> list = new List<Mobile>(NextMessage.Keys);

            foreach (Mobile m in list)
            {
                if (NextMessage[m] < DateTime.UtcNow)
                {
                    NextMessage.Remove(m);
                }
            }

            ColUtility.Free(list);
        }

        private class DoomPlaquePrompt : Prompt
        {
            public override int MessageCliloc => 1155661;

            public override void OnResponse(Mobile from, string text)
            {
                if (!string.IsNullOrEmpty(text) && text.Trim().ToLower() == "wolfgang")
                {
                    Effects.SendLocationParticles(EffectItem.Create(from.Location, Map.Malas, EffectItem.DefaultDuration), 0x3728, 10, 10, 2023);
                    from.MoveToWorld(TeleportDestination, Map.Malas);
                    Effects.SendLocationParticles(EffectItem.Create(TeleportDestination, Map.Malas, EffectItem.DefaultDuration), 0x3728, 10, 10, 5023);
                }
                else
                {
                    from.SendLocalizedMessage(1155663); // Nothign Happens
                }
            }

            public override void OnCancel(Mobile from)
            {
            }
        }

        public DoomPlaque(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);

            CheckList();
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt(); // version
        }
    }
}
