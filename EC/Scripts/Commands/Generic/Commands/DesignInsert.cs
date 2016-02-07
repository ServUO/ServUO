using System;
using System.Collections;
using System.Collections.Generic;
using Server.Gumps;
using Server.Items;
using Server.Multis;
using Server.Targeting;

namespace Server.Commands.Generic
{
    public class DesignInsertCommand : BaseCommand
    {
        public static void Initialize()
        {
            TargetCommands.Register(new DesignInsertCommand());
        }

        public DesignInsertCommand()
        {
            this.AccessLevel = AccessLevel.GameMaster;
            this.Supports = CommandSupport.Single | CommandSupport.Area;
            this.Commands = new string[] { "DesignInsert" };
            this.ObjectTypes = ObjectTypes.Items;
            this.Usage = "DesignInsert [allItems=false]";
            this.Description = "Inserts multiple targeted items into a customizable house's design.";
        }

        #region Single targeting mode
        public override void Execute(CommandEventArgs e, object obj)
        {
            Target t = new DesignInsertTarget(new List<HouseFoundation>(), (e.Length < 1 || !e.GetBoolean(0)));
            t.Invoke(e.Mobile, obj);
        }

        private class DesignInsertTarget : Target
        {
            private readonly List<HouseFoundation> m_Foundations;
            private readonly bool m_StaticsOnly;

            public DesignInsertTarget(List<HouseFoundation> foundations, bool staticsOnly)
                : base(-1, false, TargetFlags.None)
            {
                this.m_Foundations = foundations;
                this.m_StaticsOnly = staticsOnly;
            }

            protected override void OnTargetCancel(Mobile from, TargetCancelType cancelType)
            {
                if (this.m_Foundations.Count != 0)
                {
                    from.SendMessage("Your changes have been committed. Updating...");

                    foreach (HouseFoundation house in this.m_Foundations)
                        house.Delta(ItemDelta.Update);
                }
            }

            protected override void OnTarget(Mobile from, object obj)
            {
                HouseFoundation house;
                DesignInsertResult result = ProcessInsert(obj as Item, this.m_StaticsOnly, out house);

                switch ( result )
                {
                    case DesignInsertResult.Valid:
                        {
                            if (this.m_Foundations.Count == 0)
                                from.SendMessage("The item has been inserted into the house design. Press ESC when you are finished.");
                            else
                                from.SendMessage("The item has been inserted into the house design.");

                            if (!this.m_Foundations.Contains(house))
                                this.m_Foundations.Add(house);

                            break;
                        }
                    case DesignInsertResult.InvalidItem:
                        {
                            from.SendMessage("That cannot be inserted. Try again.");
                            break;
                        }
                    case DesignInsertResult.NotInHouse:
                    case DesignInsertResult.OutsideHouseBounds:
                        {
                            from.SendMessage("That item is not inside a customizable house. Try again.");
                            break;
                        }
                }

                from.Target = new DesignInsertTarget(this.m_Foundations, this.m_StaticsOnly);
            }
        }
        #endregion

        #region Area targeting mode
        public override void ExecuteList(CommandEventArgs e, ArrayList list)
        {
            e.Mobile.SendGump(new WarningGump(1060637, 30720, String.Format("You are about to insert {0} objects. This cannot be undone without a full server revert.<br><br>Continue?", list.Count), 0xFFC000, 420, 280, new WarningGumpCallback(OnConfirmCallback), new object[] { e, list, (e.Length < 1 || !e.GetBoolean(0)) }));
            this.AddResponse("Awaiting confirmation...");
        }

        private void OnConfirmCallback(Mobile from, bool okay, object state)
        {
            object[] states = (object[])state;
            CommandEventArgs e = (CommandEventArgs)states[0];
            ArrayList list = (ArrayList)states[1];
            bool staticsOnly = (bool)states[2];

            bool flushToLog = false;

            if (okay)
            {
                List<HouseFoundation> foundations = new List<HouseFoundation>();
                flushToLog = (list.Count > 20);

                for (int i = 0; i < list.Count; ++i)
                {
                    HouseFoundation house;
                    DesignInsertResult result = ProcessInsert(list[i] as Item, staticsOnly, out house);

                    switch ( result )
                    {
                        case DesignInsertResult.Valid:
                            {
                                this.AddResponse("The item has been inserted into the house design.");

                                if (!foundations.Contains(house))
                                    foundations.Add(house);

                                break;
                            }
                        case DesignInsertResult.InvalidItem:
                            {
                                this.LogFailure("That cannot be inserted.");
                                break;
                            }
                        case DesignInsertResult.NotInHouse:
                        case DesignInsertResult.OutsideHouseBounds:
                            {
                                this.LogFailure("That item is not inside a customizable house.");
                                break;
                            }
                    }
                }

                foreach (HouseFoundation house in foundations)
                    house.Delta(ItemDelta.Update);
            }
            else
            {
                this.AddResponse("Command aborted.");
            }

            this.Flush(from, flushToLog);
        }

        #endregion

        public enum DesignInsertResult
        {
            Valid,
            InvalidItem,
            NotInHouse,
            OutsideHouseBounds
        }

        public static DesignInsertResult ProcessInsert(Item item, bool staticsOnly, out HouseFoundation house)
        {
            house = null;

            if (item == null || item is BaseMulti || item is HouseSign || (staticsOnly && !(item is Static)))
                return DesignInsertResult.InvalidItem;

            house = BaseHouse.FindHouseAt(item) as HouseFoundation;

            if (house == null)
                return DesignInsertResult.NotInHouse;

            int x = item.X - house.X;
            int y = item.Y - house.Y;
            int z = item.Z - house.Z;

            if (!TryInsertIntoState(house.CurrentState, item.ItemID, x, y, z))
                return DesignInsertResult.OutsideHouseBounds;

            TryInsertIntoState(house.DesignState, item.ItemID, x, y, z);
            item.Delete();

            return DesignInsertResult.Valid;
        }

        private static bool TryInsertIntoState(DesignState state, int itemID, int x, int y, int z)
        {
            MultiComponentList mcl = state.Components;

            if (x < mcl.Min.X || y < mcl.Min.Y || x > mcl.Max.X || y > mcl.Max.Y)
                return false;

            mcl.Add(itemID, x, y, z);
            state.OnRevised();

            return true;
        }
    }
}