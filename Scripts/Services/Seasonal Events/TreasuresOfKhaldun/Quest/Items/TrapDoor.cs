using Server.Engines.Quests;
using Server.Items;
using Server.Mobiles;
using Server.Network;
using Server.Prompts;
using Server.SkillHandlers;
using System;

namespace Server.Engines.Khaldun
{
    public class TrapDoor : Item, IRevealableItem, IForensicTarget
    {
        //public static readonly Point3D TeleportDestination1 = new Point3D(6242, 2892, 17);

        public Timer HideTimer { get; set; }
        public bool CheckWhenHidden => true;

        [CommandProperty(AccessLevel.GameMaster)]
        public Map DestinationMap { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public Point3D Destination { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public string Keyword { get; set; }

        private bool _HasBeenExamined;

        [CommandProperty(AccessLevel.GameMaster)]
        public bool HasBeenExamined
        {
            get
            {
                return _HasBeenExamined;
            }
            set
            {
                bool current = _HasBeenExamined;
                _HasBeenExamined = value;

                if (!current && _HasBeenExamined)
                {
                    HideTimer = Timer.DelayCall(TimeSpan.FromMinutes(20), () => Hide());
                }
            }
        }

        public TrapDoor(string keyword, Point3D dest, Map destMap)
            : base(0xA1CD)
        {
            Keyword = keyword;
            Destination = dest;
            DestinationMap = destMap;

            Movable = false;
            Visible = false;
        }

        public override void Delete()
        {
            base.Delete();

            if (HideTimer != null)
            {
                HideTimer.Stop();
                HideTimer = null;
            }
        }

        private void Hide()
        {
            Visible = false;
            HasBeenExamined = false;
            ItemID = 0xA1CD;

            if (HideTimer != null)
            {
                HideTimer.Stop();
                HideTimer = null;
            }
        }

        public bool CheckReveal(Mobile m)
        {
            return true;
        }

        public bool CheckPassiveDetect(Mobile m)
        {
            if (m.InRange(Location, 4))
            {
                if (Utility.Random(100) < 10)
                    return true;
            }

            return false;
        }

        public void OnRevealed(Mobile m)
        {
            m.PrivateOverheadMessage(MessageType.Regular, 0x47E, 1158556, m.NetState); // *You notice something hidden in the floor...*
            Visible = true;
        }

        public void OnForensicEval(Mobile m)
        {
            if (!m.Player)
                return;

            GoingGumshoeQuest2 quest = QuestHelper.GetQuest<GoingGumshoeQuest2>((PlayerMobile)m);

            if (quest != null && CheckPrerequisite(quest))
            {
                m.PrivateOverheadMessage(MessageType.Regular, 0x47E, 1158559, m.NetState); // *You discover a hidden trap door!*
                m.SendLocalizedMessage(1158611, null, 0x23); // It seems a trap door has been hidden in some false pavers. The heavy wooden door is secured with a rotating combination lock that accepts alpha-numeric characters. You'll need to figure out the passcode to unlock it...

                m.SendSound(quest.UpdateSound);

                HasBeenExamined = true;
                ItemID = 0xA1CC;

                if (HideTimer != null)
                {
                    HideTimer.Stop();
                    HideTimer = null;

                    HideTimer = Timer.DelayCall(TimeSpan.FromMinutes(20), () => Hide());
                }
            }
        }

        private bool CheckPrerequisite(GoingGumshoeQuest2 quest)
        {
            switch (Keyword.ToLower())
            {
                case "boreas": return quest.VisitedHeastone1; // brit
                case "moriens": return quest.VisitedHeastone2; // vesper
                case "carthax": return quest.VisitedHeastone3; // moonglow
                case "tenebrae": return quest.VisitedHeastone4; // yew
            }

            return false;
        }

        public override void OnDoubleClick(Mobile m)
        {
            if (Destination == Point3D.Zero || DestinationMap == null || DestinationMap == Map.Internal || string.IsNullOrEmpty(Keyword))
                return;

            if (m.InRange(GetWorldLocation(), 2) && _HasBeenExamined)
            {
                m.Prompt = new TrapDoorPrompt(this);
            }
        }

        private class TrapDoorPrompt : Prompt
        {
            public override int MessageCliloc => 1158557;

            public TrapDoor Door { get; set; }

            public TrapDoorPrompt(TrapDoor door)
            {
                Door = door;
            }

            public override void OnResponse(Mobile from, string text)
            {
                if (Door.Destination == Point3D.Zero || Door.DestinationMap == null || Door.DestinationMap == Map.Internal || string.IsNullOrEmpty(Door.Keyword))
                    return;

                if (!string.IsNullOrEmpty(text) && text.Trim().ToLower() == Door.Keyword.ToLower())
                {
                    Effects.SendLocationParticles(EffectItem.Create(from.Location, from.Map, EffectItem.DefaultDuration), 0x3728, 10, 10, 2023);
                    from.MoveToWorld(Door.Destination, Door.DestinationMap);
                    Effects.SendLocationParticles(EffectItem.Create(Door.Destination, Door.DestinationMap, EffectItem.DefaultDuration), 0x3728, 10, 10, 5023);
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

        public TrapDoor(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version

            writer.Write(Keyword);
            writer.Write(Destination);
            writer.Write(DestinationMap);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            Keyword = reader.ReadString();
            Destination = reader.ReadPoint3D();
            DestinationMap = reader.ReadMap();

            Hide();
        }
    }
}
