using Server.ContextMenus;
using Server.Gumps;
using Server.Mobiles;
using Server.Multis;
using Server.Network;
using Server.Targeting;
using System.Collections.Generic;

namespace Server.Items
{
    [Flipable(0x4513, 0x4514)]
    public class ChickenCoop : Item, ISecurable, IChopable
    {
        public static readonly int MaxStables = 3;

        public override int LabelNumber => 1112570;  // a chicken coop

        private SecureLevel m_Level;
        private readonly Dictionary<Mobile, List<BaseCreature>> m_Stored = new Dictionary<Mobile, List<BaseCreature>>();

        [CommandProperty(AccessLevel.GameMaster)]
        public SecureLevel Level
        {
            get { return m_Level; }
            set { m_Level = value; }
        }

        public Dictionary<Mobile, List<BaseCreature>> Stored => m_Stored;

        [Constructable]
        public ChickenCoop()
            : base(0x4513)
        {
            Weight = 20;
            m_Level = SecureLevel.CoOwners;
        }

        public void OnChop(Mobile from)
        {
            if (CheckAccess(from))
            {
                Effects.PlaySound(GetWorldLocation(), Map, 0x3B3);
                from.SendLocalizedMessage(500461); // You destroy the item.

                Delete();
            }
        }

        public override void Delete()
        {
            if (m_Stored != null && m_Stored.Count > 0)
            {
                List<List<BaseCreature>> masterList = new List<List<BaseCreature>>(m_Stored.Values);

                for (int i = 0; i < masterList.Count; i++)
                {
                    for (int j = 0; j < masterList[i].Count; j++)
                    {
                        if (masterList[i][j] != null && !masterList[i][j].Deleted)
                            masterList[i][j].Delete();
                    }
                }

                m_Stored.Clear();
            }

            base.Delete();
        }

        public override void GetContextMenuEntries(Mobile from, List<ContextMenuEntry> list)
        {
            base.GetContextMenuEntries(from, list);

            if (CheckAccess(from))
            {
                SetSecureLevelEntry.AddTo(from, this, list);

                list.Add(new StableEntry(this, from));

                if (m_Stored.ContainsKey(from) && m_Stored[from].Count > 0)
                    list.Add(new ClaimAllEntry(this, from));
            }
        }

        private class StableEntry : ContextMenuEntry
        {
            private readonly ChickenCoop m_Coop;
            private readonly Mobile m_From;

            public StableEntry(ChickenCoop coop, Mobile from)
                : base(6126, 12)
            {
                m_Coop = coop;
                m_From = from;
            }

            public override void OnClick()
            {
                m_Coop.BeginStable(m_From);
            }
        }

        private class ClaimAllEntry : ContextMenuEntry
        {
            private readonly ChickenCoop m_Coop;
            private readonly Mobile m_From;

            public ClaimAllEntry(ChickenCoop coop, Mobile from)
                : base(6127, 12)
            {
                m_Coop = coop;
                m_From = from;
            }

            public override void OnClick()
            {
                m_Coop.Claim(m_From);
            }
        }

        public ChickenCoop(Serial serial)
            : base(serial)
        {
        }

        public override bool ForceShowProperties => true;

        public override void GetProperties(ObjectPropertyList list)
        {
            base.AddNameProperties(list);
        }

        private class ClaimListGump : Gump
        {
            private readonly ChickenCoop m_Post;
            private readonly Mobile m_From;
            private readonly List<BaseCreature> m_List;

            public ClaimListGump(ChickenCoop post, Mobile from, List<BaseCreature> list)
                : base(50, 50)
            {
                m_Post = post;
                m_From = from;
                m_List = list;

                from.CloseGump(typeof(ClaimListGump));

                AddPage(0);

                AddBackground(0, 0, 325, 50 + (list.Count * 20), 9250);
                AddAlphaRegion(5, 5, 315, 40 + (list.Count * 20));

                AddHtml(15, 15, 275, 20, "<BASEFONT COLOR=#FFFFFF>Select a pet to retrieve from the stables:</BASEFONT>", false, false);

                for (int i = 0; i < list.Count; ++i)
                {
                    BaseCreature pet = list[i];

                    if (pet == null || pet.Deleted)
                        continue;

                    AddButton(15, 39 + (i * 20), 10006, 10006, i + 1, GumpButtonType.Reply, 0);
                    AddHtml(32, 35 + (i * 20), 275, 18, string.Format("<BASEFONT COLOR=#C0C0EE>{0}</BASEFONT>", pet.Name), false, false);
                }
            }

            public override void OnResponse(NetState sender, RelayInfo info)
            {
                int index = info.ButtonID - 1;

                if (index >= 0 && index < m_List.Count)
                    m_Post.EndClaimList(m_From, m_List[index]);
            }
        }

        public static int GetMaxStabled(Mobile from)
        {
            int max = 3;
            return max;
        }

        private class StableTarget : Target
        {
            private readonly ChickenCoop m_Post;

            public StableTarget(ChickenCoop post)
                : base(12, false, TargetFlags.None)
            {
                m_Post = post;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (targeted is ChickenLizard || targeted is Chicken || targeted is BattleChickenLizard)
                    m_Post.EndStable(from, (BaseCreature)targeted);
                else if (targeted == from)
                    from.SendLocalizedMessage(502672); // HA HA HA! Sorry, I am not an inn.
                else
                    from.SendLocalizedMessage(1112558); // You may only stable chickens in the chicken coop.
            }
        }

        public void BeginClaimList(Mobile from)
        {
            if (Deleted || !from.CheckAlive() || !m_Stored.ContainsKey(from) || m_Stored[from] == null)
                return;

            List<BaseCreature> stabled = m_Stored[from];

            for (int i = 0; i < stabled.Count; i++)
            {
                BaseCreature pet = stabled[i] as BaseCreature;

                if (pet == null || pet.Deleted)
                {
                    if (pet != null)
                    {
                        pet.IsStabled = false;
                    }

                    stabled.RemoveAt(i);
                    --i;
                    continue;
                }
            }

            if (stabled.Count > 0)
                from.SendGump(new ClaimListGump(this, from, stabled));
            else
                from.SendLocalizedMessage(502671); // But I have no animals stabled with me at the moment!
        }

        public void EndClaimList(Mobile from, BaseCreature pet)
        {
            if (Deleted || !from.CheckAlive() || !m_Stored.ContainsKey(from))
                return;

            if ((from.Followers + pet.ControlSlots) <= from.FollowersMax)
            {
                pet.SetControlMaster(from);

                if (pet.Summoned)
                    pet.SummonMaster = from;

                pet.ControlTarget = from;
                pet.ControlOrder = OrderType.Follow;

                pet.MoveToWorld(from.Location, from.Map);

                pet.IsStabled = false;

                if (m_Stored[from].Contains(pet))
                    m_Stored[from].Remove(pet);

                from.SendLocalizedMessage(1042559); // Here you go... and good day to you!
            }
            else
            {
                from.SendLocalizedMessage(1049612, pet.Name); // ~1_NAME~ remained in the stables because you have too many followers.
            }
        }

        public void BeginStable(Mobile from)
        {
            if (Deleted || !from.CheckAlive() || !CanUse() || !CheckAccess(from))
                return;

            else if (GetCount() >= MaxStables)
            {
                from.SendLocalizedMessage(1114325); // There is no more room in your chicken coop!
            }
            else
            {
                /*from.SendLocalizedMessage(1042558);  I charge 30 gold per pet for a real week's stable time.
										 * I will withdraw it from thy bank account.
										 * Which animal wouldst thou like to stable here?
										 */

                from.Target = new StableTarget(this);
                from.SendLocalizedMessage(1112559); // Which chicken do you wish to stable?
            }
        }

        private int GetCount()
        {
            int count = 0;

            foreach (List<BaseCreature> bcList in m_Stored.Values)
            {
                if (bcList != null)
                    count += bcList.Count;
            }

            return count;
        }

        public void EndStable(Mobile from, BaseCreature pet)
        {
            if (Deleted || !from.CheckAlive() || !CanUse() || !CheckAccess(from))
                return;

            else if (!pet.Controlled || pet.ControlMaster != from)
            {
                from.SendLocalizedMessage(1042562); // You do not own that pet!
            }
            else if (pet.IsDeadPet)
            {
                from.SendLocalizedMessage(1049668); // Living pets only, please.
            }
            else if (pet.Summoned)
            {
                from.SendLocalizedMessage(502673); // I can not stable summoned creatures.
            }
            else if (pet.Allured)
            {
                from.SendLocalizedMessage(1048053); // You can't stable that!
            }
            else if (pet.Body.IsHuman)
            {
                from.SendLocalizedMessage(502672); // HA HA HA! Sorry, I am not an inn.
            }
            else if (pet.Combatant != null && pet.InRange(pet.Combatant, 12) && pet.Map == pet.Combatant.Map)
            {
                from.SendLocalizedMessage(1042564); // I'm sorry.  Your pet seems to be busy.
            }
            else if (GetCount() >= MaxStables)
            {
                from.SendLocalizedMessage(1114325); // There is no more room in your chicken coop!
            }
            else
            {
                pet.ControlTarget = null;
                pet.ControlOrder = OrderType.Stay;
                pet.Internalize();

                pet.SetControlMaster(null);
                pet.SummonMaster = null;

                pet.IsStabled = true;

                pet.Loyalty = BaseCreature.MaxLoyalty; // Wonderfully happy

                if (!m_Stored.ContainsKey(from))
                    m_Stored.Add(from, new List<BaseCreature>());

                if (!m_Stored[from].Contains(pet))
                    m_Stored[from].Add(pet);

                from.SendMessage("Your chicken has been stabled.");
            }
        }

        public void Claim(Mobile from)
        {
            if (Deleted || !from.CheckAlive() || !m_Stored.ContainsKey(from))
                return;

            bool claimed = false;
            int stabledCount = 0;

            List<BaseCreature> stabled = m_Stored[from];

            for (int i = 0; i < stabled.Count; ++i)
            {
                BaseCreature pet = stabled[i] as BaseCreature;

                if (pet == null || pet.Deleted)
                {
                    if (pet != null)
                    {
                        pet.IsStabled = false;
                    }

                    stabled.RemoveAt(i);
                    --i;
                    continue;
                }

                ++stabledCount;

                if ((from.Followers + pet.ControlSlots) <= from.FollowersMax)
                {
                    pet.SetControlMaster(from);

                    if (pet.Summoned)
                        pet.SummonMaster = from;

                    pet.ControlTarget = from;
                    pet.ControlOrder = OrderType.Follow;

                    pet.MoveToWorld(from.Location, from.Map);

                    pet.IsStabled = false;

                    pet.Loyalty = BaseCreature.MaxLoyalty; // Wonderfully Happy

                    stabled.RemoveAt(i);
                    --i;

                    claimed = true;
                }
                else
                {
                    from.SendLocalizedMessage(1049612, pet.Name); // ~1_NAME~ remained in the stables because you have too many followers.
                }
            }

            if (claimed)
            {
                from.SendLocalizedMessage(1042559); // Here you go... and good day to you!
            }

            else if (stabledCount == 0)
                from.SendLocalizedMessage(502671); // But I have no animals stabled with me at the moment!
        }

        public bool CheckAccess(Mobile m)
        {
            BaseHouse h = BaseHouse.FindHouseAt(this);

            return h != null && h.HasSecureAccess(m, m_Level);
        }

        public bool CanUse()
        {
            return IsLockedDown;
        }

        public override bool HandlesOnSpeech => true;

        public override void OnSpeech(SpeechEventArgs e)
        {
            if (CheckAccess(e.Mobile) && IsLockedDown)
            {
                if (!e.Handled && e.HasKeyword(0x0008))
                {
                    e.Handled = true;
                    BeginStable(e.Mobile);
                }
                else if (!e.Handled && e.HasKeyword(0x0009))
                {
                    e.Handled = true;

                    if (!Insensitive.Equals(e.Speech, "claim"))
                        BeginClaimList(e.Mobile);
                    else
                        Claim(e.Mobile);
                }
                else
                {
                    base.OnSpeech(e);
                }
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(2); // version

            writer.Write((int)m_Level);
            writer.Write(m_Stored.Count);
            foreach (KeyValuePair<Mobile, List<BaseCreature>> kvp in m_Stored)
            {
                writer.Write(kvp.Key);
                writer.Write(kvp.Value.Count);

                foreach (BaseCreature bc in kvp.Value)
                    writer.Write(bc);
            }
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            m_Level = (SecureLevel)reader.ReadInt();

            if (version == 1)
                return;

            int c = reader.ReadInt();

            for (int i = 0; i < c; i++)
            {
                Mobile owner = reader.ReadMobile();
                int count = reader.ReadInt();
                List<BaseCreature> list = new List<BaseCreature>();

                for (int j = 0; j < count; j++)
                {
                    Mobile chicken = reader.ReadMobile();

                    if (chicken != null && chicken is BaseCreature)
                    {
                        BaseCreature bc = chicken as BaseCreature;
                        bc.IsStabled = true;
                        list.Add(bc);
                    }
                }

                if (owner != null && list.Count > 0)
                    m_Stored.Add(owner, list);
            }
        }
    }
}
