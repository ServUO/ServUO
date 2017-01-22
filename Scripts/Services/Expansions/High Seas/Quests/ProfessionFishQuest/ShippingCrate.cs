using System;
using Server;
using System.Collections.Generic;
using Server.Engines.Quests;
using Server.Gumps;
using Server.Network;

namespace Server.Items
{
    public class ShippingCrate : SmallCrate
    {
        private ProfessionalFisherQuest m_Quest;
        public ProfessionalFisherQuest Quest { get { return m_Quest; } set { m_Quest = value; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool QuestComplete
        {
            get
            {
                if (m_Quest == null)
                    return false;

                return m_Quest.Completed;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public Mobile TurnInMonger
        {
            get
            {
                if (m_Quest == null)
                    return null;

                return m_Quest.TurnIn;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public Mobile Quester
        {
            get
            {
                if (m_Quest == null || !(m_Quest.Quester is Mobile))
                    return null;

                return m_Quest.Quester as Mobile;
            }
        }

        public override int DefaultMaxWeight { get { return 2500; } }
        public override bool DisplaysContent { get { return true; } }

        public ShippingCrate(ProfessionalFisherQuest quest)
        {
            m_Quest = quest;
            Movable = false;

            LootType = LootType.Blessed;
        }

        public override void AddNameProperty(ObjectPropertyList list)
        {
            if (m_Quest != null && m_Quest.Owner != null)
                list.Add(1116515, m_Quest.Owner.Name);
            else
                list.Add("a shipping crate");
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            if (m_Quest == null)
                return;

            int loc = 1116453; //~1_val~: ~2_val~/~3_val~
            FishQuestObjective obj = m_Quest.GetObjective();

            if (obj == null)
                Delete();

            foreach (KeyValuePair<Type, int[]> kvp in obj.Line)
            {
                int idx = FishQuestHelper.GetIndexForType(kvp.Key);
                list.Add(loc, "#{0}\t{1}\t{2}", FishQuestHelper.Labels[idx], kvp.Value[0].ToString(), kvp.Value[1].ToString());
                loc++;
            }

            object delivery = GetDeliveryInfo();
            if (delivery is string)
                list.Add((string)delivery);
            else
                list.Add((int)delivery);

            list.Add(1076255); //NO-TRADE
        }

        public override bool OnDragLift(Mobile from)
        {
            if (m_Quest == null)
                return base.OnDragLift(from);

            if (m_Quest.Owner != from)
            {
                from.SendMessage("You cannot lift someone else's quest item.");
                return false;
            }

            return base.OnDragLift(from);
        }

        public override bool AllowSecureTrade(Mobile from, Mobile to, Mobile newOwner, bool accepted)
        {
            return false;
        }

        public override bool OnDroppedToMobile(Mobile from, Mobile target)
        {
            from.SendMessage("You cannot trade this item.");

            return false;
        }

        public override bool DropToItem(Mobile from, Item target, Point3D p)
        {
            if (target is GalleonHold || target is Hold)
                return base.DropToItem(from, target, p);

            return false;
        }

        public override bool OnDroppedToWorld(Mobile from, Point3D p)
        {
            return false;
        }

        public override bool OnStackAttempt(Mobile from, Item stack, Item dropped)
        {
            if (dropped is BaseHighseasFish && m_Quest != null)
            {
                FishQuestObjective obj = m_Quest.GetObjective();

                if (obj != null)
                {
                    if (obj.Update(dropped) && base.OnStackAttempt(from, stack, dropped))
                        return true;
                }
            }

            from.SendLocalizedMessage(1072355, null, 0x23); // That item does not match any of your quest criteria
            return false;
        }

        public override bool OnDragDrop(Mobile from, Item dropped)
        {
            if (dropped is BaseHighseasFish && m_Quest != null)
            {
                FishQuestObjective obj = m_Quest.GetObjective();

                if (obj != null && obj.Update(dropped))
                    return base.OnDragDrop(from, dropped);
            }

            from.SendLocalizedMessage(1072355, null, 0x23); // That item does not match any of your quest criteria
            return false;
        }

        public override bool OnDragDropInto(Mobile from, Item dropped, Point3D p)
        {
            if (dropped is BaseHighseasFish && m_Quest != null)
            {
                FishQuestObjective obj = m_Quest.GetObjective();

                if (obj != null && obj.Update(dropped))
                    return base.OnDragDropInto(from, dropped, p);
            }

            from.SendLocalizedMessage(1072355, null, 0x23); // That item does not match any of your quest criteria
            return false;
        }

        public override bool CheckLift(Mobile from, Item item, ref LRReason reject)
        {
            if (m_Quest == null)
                return base.CheckLift(from, item, ref reject);

            FishQuestObjective obj = m_Quest.GetObjective();

            if (obj != null && obj.CheckLift(item))
            {
                reject = LRReason.CannotLift;
                return false;
            }

            return true;
        }

        public bool CheckCarve(Item item)
        {
            if (m_Quest == null)
                return true;

            FishQuestObjective obj = m_Quest.GetObjective();

            if (obj.CheckLift(item))
                return false;

            return true;
        }

        public object GetDeliveryInfo()
        {
            if (m_Quest != null && m_Quest is ProfessionalFisherQuest && ((ProfessionalFisherQuest)m_Quest).TurnIn != null)
            {
                Region reg = ((ProfessionalFisherQuest)m_Quest).TurnIn.Region;

                if (reg == null || reg.Name == null)
                    return "Unknown Delivery";

                if (reg.Name == "Sea Market")
                    return 1116496;
                if (reg.Name == "Britain")
                    return 1116497;
                if (reg.Name == "Trinsic")
                    return 1116498;
                if (reg.Name == "Moonglow")
                    return 1116499;
                if (reg.Name == "Skara Brae")
                    return 1116500;
                if (reg.Name == "Vesper")
                    return 1116501;
                if (reg.Name == "Jhelom")
                    return 1116502;
                if (reg.Name == "Papua")
                    return 1116503;
            }
            return "Unknown Delivery";
        }

        public void AddQuest(ProfessionalFisherQuest quest)
        {
            m_Quest = quest;
        }

        public override void Delete()
        {
            if (m_Quest != null && !m_Quest.Completed)
                m_Quest.OnResign(false);

            base.Delete();
        }

        public ShippingCrate(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            if (Movable)
                Movable = false;
        }
    }
}