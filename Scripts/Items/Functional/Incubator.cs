using Server.Commands;
using Server.Gumps;
using Server.Multis;
using System;

namespace Server.Items
{
    [Flipable(0x407C, 0x407D)]
    public class Incubator : Container, ISecurable
    {
        public static readonly int MaxEggs = 6;

        public override int LabelNumber => 1112479;  //an incubator

        private SecureLevel m_Level;

        [CommandProperty(AccessLevel.GameMaster)]
        public SecureLevel Level
        {
            get { return m_Level; }
            set { m_Level = value; }
        }

        public override int DefaultGumpID => 1156;
        public override int DefaultDropSound => 66;

        [Constructable]
        public Incubator()
            : base(0x407C)
        {
            m_Level = SecureLevel.CoOwners;
        }

        public override bool OnDragDropInto(Mobile from, Item item, Point3D p)
        {
            bool canDrop = base.OnDragDropInto(from, item, p);

            if (canDrop && item is ChickenLizardEgg)
            {
                ChickenLizardEgg egg = (ChickenLizardEgg)item;

                if (egg.TotalIncubationTime > TimeSpan.FromHours(120))
                    egg.BurnEgg();
                else
                {
                    egg.IncubationStart = DateTime.UtcNow;
                    egg.Incubating = true;
                }
            }

            return canDrop;
        }

        public override bool OnDragDrop(Mobile from, Item item)
        {
            bool canDrop = base.OnDragDrop(from, item);

            if (canDrop && item is ChickenLizardEgg)
            {
                ChickenLizardEgg egg = (ChickenLizardEgg)item;

                if (egg.TotalIncubationTime > TimeSpan.FromHours(120))
                    egg.BurnEgg();
                else
                {
                    egg.IncubationStart = DateTime.UtcNow;
                    egg.Incubating = true;
                }
            }

            return canDrop;
        }

        public override bool CheckHold(Mobile m, Item item, bool message, bool checkItems, int plusItems, int plusWeight)
        {
            if (!BaseHouse.CheckSecured(this))
            {
                m.SendLocalizedMessage(1113711); //The incubator must be secured for the egg to grow, not locked down.
                return false;
            }
            if (!(item is ChickenLizardEgg))
            {
                m.SendMessage("This will only accept chicken eggs.");
                return false;
            }

            if (MaxEggs > -1 && Items.Count >= MaxEggs)
            {
                m.SendMessage("You can only put {0} chicken eggs in the incubator at a time.", MaxEggs.ToString()); //TODO: Get Message
                return false;
            }

            return true;
        }

        public void CheckEggs_Callback()
        {
            if (!BaseHouse.CheckSecured(this))
                return;

            foreach (Item item in Items)
            {
                if (item is ChickenLizardEgg)
                    ((ChickenLizardEgg)item).CheckStatus();
            }
        }

        public static void Initialize()
        {
            CommandSystem.Register("IncreaseStage", AccessLevel.Counselor, IncreaseStage_OnCommand);
        }

        public static void IncreaseStage_OnCommand(CommandEventArgs e)
        {
            e.Mobile.SendMessage("Target the egg.");
            e.Mobile.BeginTarget(12, false, Targeting.TargetFlags.None, IncreaseStage_OnTarget);
        }

        public static void IncreaseStage_OnTarget(Mobile from, object targeted)
        {
            if (targeted is ChickenLizardEgg)
            {
                ((ChickenLizardEgg)targeted).TotalIncubationTime += TimeSpan.FromHours(24);
                ((ChickenLizardEgg)targeted).CheckStatus();
            }
        }

        public Incubator(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version

            writer.Write((int)m_Level);

            if (Items.Count > 0)
                Timer.DelayCall(TimeSpan.FromSeconds(10), CheckEggs_Callback);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            m_Level = (SecureLevel)reader.ReadInt();

            if (Items.Count > 0)
                Timer.DelayCall(TimeSpan.FromSeconds(60), CheckEggs_Callback);
        }
    }
}
