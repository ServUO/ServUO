using System;
using System.Collections.Generic;

using Server;
using Server.Mobiles;
using Server.Gumps;
using Server.Network;
using Server.Engines.Points;

namespace Server.Items
{
    public class DoomSign : Item, IRevealableItem
    {
        public List<Mobile> Revealed { get; set; }
        public Dictionary<Mobile, DateTime> NextMessage { get; set; }

        public override bool ForceShowProperties { get { return true; } }
        public bool CheckWhenHidden { get { return false; } }

        [Constructable]
        public DoomSign()
            : base(0xBD0)
        {
            Movable = false;
        }

        public override void OnDoubleClick(Mobile m)
        {
            if (PointsSystem.TreasuresOfDoom.InSeason && m.InRange(GetWorldLocation(), 2))
            {
                var gump = new Gump(150, 150);

                gump.AddImage(0, 0, HasRevealed(m) ? 0x7779 : 0x7724);

                m.SendGump(gump);
            }
        }

        public override bool HandlesOnMovement { get { return PointsSystem.TreasuresOfDoom.InSeason; } }

        public override void OnMovement(Mobile m, Point3D oldLocation)
        {
            if (m.Player && m.InRange(Location, 3) && m.InLOS(this))
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
                    m.PrivateOverheadMessage(MessageType.Regular, 0x3B2, 1155659, m.NetState); // *The wooden sign seems in oddly good condition for how old the sarcophagus is. Most of the inscription is worn away...*

                    NextMessage[m] = DateTime.UtcNow + TimeSpan.FromMinutes(10);
                }
            }
        }

        private bool HasRevealed(Mobile m)
        {
            return Revealed != null && Revealed.Contains(m);
        }

        public bool CheckReveal(Mobile m)
        {
            return Utility.Random((int)m.Skills[SkillName.DetectHidden].Value) < 100;
        }

        public void OnRevealed(Mobile m)
        {
            if (Revealed == null)
            {
                Revealed = new List<Mobile>();
            }

            if (!Revealed.Contains(m))
            {
                Revealed.Add(m);

                m.PrivateOverheadMessage(MessageType.Regular, 0x3B2, 1155658, m.NetState); // *You reveal something hidden about the object...*
            }

            OnDoubleClick(m);
        }

        public bool CheckPassiveDetect(Mobile m)
        {
            return false;
        }

        private void CheckList()
        {
            if (NextMessage == null)
            {
                return;
            }

            var list = new List<Mobile>(NextMessage.Keys);

            foreach (var m in list)
            {
                if (NextMessage[m] < DateTime.UtcNow)
                {
                    NextMessage.Remove(m);
                }
            }

            ColUtility.Free(list);
        }

        public DoomSign(Serial serial)
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