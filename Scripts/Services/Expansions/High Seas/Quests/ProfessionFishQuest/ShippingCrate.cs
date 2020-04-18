using Server.Engines.Quests;
using Server.Network;
using System;
using System.Collections.Generic;

namespace Server.Items
{
    public class ShippingCrate : SmallCrate
    {
        public ProfessionalFisherQuest Quest { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool QuestComplete
        {
            get
            {
                if (Quest == null)
                    return false;

                return Quest.Completed;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public Mobile TurnInMonger
        {
            get
            {
                if (Quest == null)
                    return null;

                return Quest.TurnIn;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public Mobile Quester
        {
            get
            {
                if (Quest == null || !(Quest.Quester is Mobile))
                    return null;

                return Quest.Quester as Mobile;
            }
        }

        public override int DefaultMaxWeight => 1200;
        public override bool DisplaysContent => false;

        public ShippingCrate(ProfessionalFisherQuest quest)
        {
            Weight = 1.0;
            Quest = quest;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            if (Quest == null)
                return;

            int loc = 1116453; //~1_val~: ~2_val~/~3_val~
            FishQuestObjective obj = Quest.GetObjective();

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

            list.Add(1072241, "{0}\t{1}\t{2}\t{3}", TotalItems, MaxItems, TotalWeight, MaxWeight);
        }

        public override bool OnDragLift(Mobile from)
        {
            if (Quest == null)
                return base.OnDragLift(from);

            if (Quest.Owner != from)
            {
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
            from.SendLocalizedMessage(1076256); // That item cannot be traded.

            return false;
        }

        public override bool DropToItem(Mobile from, Item target, Point3D p)
        {
            from.SendLocalizedMessage(1076254); // That item cannot be dropped.

            return false;
        }

        public override bool OnDroppedToWorld(Mobile from, Point3D p)
        {
            from.SendLocalizedMessage(1076254); // That item cannot be dropped.

            return false;
        }

        public override bool OnStackAttempt(Mobile from, Item stack, Item dropped)
        {
            if (dropped is BaseHighseasFish && Quest != null)
            {
                FishQuestObjective obj = Quest.GetObjective();

                if (obj != null)
                {
                    if (obj.CheckLift(dropped) && base.OnStackAttempt(from, stack, dropped))
                    {
                        obj.Update(dropped);
                        InvalidateProperties();
                        return true;
                    }
                }
            }

            from.SendLocalizedMessage(1116461); // This is not needed to fill the order.
            return false;
        }

        public override bool OnDragDrop(Mobile from, Item dropped)
        {
            if (dropped is BaseHighseasFish && Quest != null)
            {
                FishQuestObjective obj = Quest.GetObjective();

                if (obj != null && obj.CheckLift(dropped) && base.OnDragDrop(from, dropped))
                {
                    obj.Update(dropped);
                    InvalidateProperties();
                    return true;
                }
            }

            from.SendLocalizedMessage(1116461); // This is not needed to fill the order.
            return false;
        }

        public override bool OnDragDropInto(Mobile from, Item dropped, Point3D p)
        {
            if (dropped is BaseHighseasFish && Quest != null)
            {
                FishQuestObjective obj = Quest.GetObjective();

                if (obj != null && obj.CheckLift(dropped) && base.OnDragDropInto(from, dropped, p))
                {
                    obj.Update(dropped);
                    InvalidateProperties();
                    return true;
                }
            }

            from.SendLocalizedMessage(1116461); // This is not needed to fill the order.
            return false;
        }

        public override bool CheckLift(Mobile from, Item item, ref LRReason reject)
        {
            if (Quest == null)
                return base.CheckLift(from, item, ref reject);

            FishQuestObjective obj = Quest.GetObjective();

            if (obj != null && obj.CheckLift(item))
            {
                reject = LRReason.CannotLift;
                return false;
            }

            return true;
        }

        public bool CheckCarve(Item item)
        {
            if (Quest == null)
                return true;

            FishQuestObjective obj = Quest.GetObjective();

            if (obj.CheckLift(item))
                return false;

            return true;
        }

        public object GetDeliveryInfo()
        {
            if (Quest != null && Quest is ProfessionalFisherQuest && Quest.TurnIn != null)
            {
                Region reg = Quest.TurnIn.Region;

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
            Quest = quest;
        }

        public override void Delete()
        {
            if (Quest != null && !Quest.Completed)
                Quest.OnResign(false);

            base.Delete();
        }

        public ShippingCrate(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}
