using System;
using System.Collections;
using System.Reflection;
using Server.Commands.Generic;
using Server.Gumps;
using Server.Network;

namespace Server.Commands
{
    public class Batch : BaseCommand
    {
        private readonly ArrayList m_BatchCommands;
        private BaseCommandImplementor m_Scope;
        private string m_Condition;
        public Batch()
        {
            this.Commands = new string[] { "Batch" };
            this.ListOptimized = true;

            this.m_BatchCommands = new ArrayList();
            this.m_Condition = "";
        }

        public BaseCommandImplementor Scope
        {
            get
            {
                return this.m_Scope;
            }
            set
            {
                this.m_Scope = value;
            }
        }
        public string Condition
        {
            get
            {
                return this.m_Condition;
            }
            set
            {
                this.m_Condition = value;
            }
        }
        public ArrayList BatchCommands
        {
            get
            {
                return this.m_BatchCommands;
            }
        }
        public static void Initialize()
        {
            CommandSystem.Register("Batch", AccessLevel.Counselor, new CommandEventHandler(Batch_OnCommand));
        }

        [Usage("Batch")]
        [Description("Allows multiple commands to be run at the same time.")]
        public static void Batch_OnCommand(CommandEventArgs e)
        {
            Batch batch = new Batch();

            e.Mobile.SendGump(new BatchGump(e.Mobile, batch));
        }

        public override void ExecuteList(CommandEventArgs e, ArrayList list)
        {
            if (list.Count == 0)
            {
                this.LogFailure("Nothing was found to use this command on.");
                return;
            }

            try
            {
                BaseCommand[] commands = new BaseCommand[this.m_BatchCommands.Count];
                CommandEventArgs[] eventArgs = new CommandEventArgs[this.m_BatchCommands.Count];

                for (int i = 0; i < this.m_BatchCommands.Count; ++i)
                {
                    BatchCommand bc = (BatchCommand)this.m_BatchCommands[i];

                    string commandString, argString;
                    string[] args;

                    bc.GetDetails(out commandString, out argString, out args);

                    BaseCommand command = this.m_Scope.Commands[commandString];

                    commands[i] = command;
                    eventArgs[i] = new CommandEventArgs(e.Mobile, commandString, argString, args);

                    if (command == null)
                    {
                        e.Mobile.SendMessage("That is either an invalid command name or one that does not support this modifier: {0}.", commandString);
                        return;
                    }
                    else if (e.Mobile.AccessLevel < command.AccessLevel)
                    {
                        e.Mobile.SendMessage("You do not have access to that command: {0}.", commandString);
                        return;
                    }
                    else if (!command.ValidateArgs(this.m_Scope, eventArgs[i]))
                    {
                        return;
                    }
                }

                for (int i = 0; i < commands.Length; ++i)
                {
                    BaseCommand command = commands[i];
                    BatchCommand bc = (BatchCommand)this.m_BatchCommands[i];

                    if (list.Count > 20)
                        CommandLogging.Enabled = false;

                    ArrayList usedList;

                    if (Utility.InsensitiveCompare(bc.Object, "Current") == 0)
                    {
                        usedList = list;
                    }
                    else
                    {
                        Hashtable propertyChains = new Hashtable();

                        usedList = new ArrayList(list.Count);

                        for (int j = 0; j < list.Count; ++j)
                        {
                            object obj = list[j];

                            if (obj == null)
                                continue;

                            Type type = obj.GetType();

                            PropertyInfo[] chain = (PropertyInfo[])propertyChains[type];

                            string failReason = "";

                            if (chain == null && !propertyChains.Contains(type))
                                propertyChains[type] = chain = Properties.GetPropertyInfoChain(e.Mobile, type, bc.Object, PropertyAccess.Read, ref failReason);

                            if (chain == null)
                                continue;

                            PropertyInfo endProp = Properties.GetPropertyInfo(ref obj, chain, ref failReason);

                            if (endProp == null)
                                continue;

                            try
                            {
                                obj = endProp.GetValue(obj, null);

                                if (obj != null)
                                    usedList.Add(obj);
                            }
                            catch
                            {
                            }
                        }
                    }

                    command.ExecuteList(eventArgs[i], usedList);

                    if (list.Count > 20)
                        CommandLogging.Enabled = true;

                    command.Flush(e.Mobile, list.Count > 20);
                }
            }
            catch (Exception ex)
            {
                e.Mobile.SendMessage(ex.Message);
            }
        }

        public bool Run(Mobile from)
        {
            if (this.m_Scope == null)
            {
                from.SendMessage("You must select the batch command scope.");
                return false;
            }
            else if (this.m_Condition.Length > 0 && !this.m_Scope.SupportsConditionals)
            {
                from.SendMessage("This command scope does not support conditionals.");
                return false;
            }
            else if (this.m_Condition.Length > 0 && !Utility.InsensitiveStartsWith(this.m_Condition, "where"))
            {
                from.SendMessage("The condition field must start with \"where\".");
                return false;
            }

            string[] args = CommandSystem.Split(this.m_Condition);

            this.m_Scope.Process(from, this, args);

            return true;
        }
    }

    public class BatchCommand
    {
        private string m_Command;
        private string m_Object;
        public BatchCommand(string command, string obj)
        {
            this.m_Command = command;
            this.m_Object = obj;
        }

        public string Command
        {
            get
            {
                return this.m_Command;
            }
            set
            {
                this.m_Command = value;
            }
        }
        public string Object
        {
            get
            {
                return this.m_Object;
            }
            set
            {
                this.m_Object = value;
            }
        }
        public void GetDetails(out string command, out string argString, out string[] args)
        {
            int indexOf = this.m_Command.IndexOf(' ');

            if (indexOf >= 0)
            {
                argString = this.m_Command.Substring(indexOf + 1);

                command = this.m_Command.Substring(0, indexOf);
                args = CommandSystem.Split(argString);
            }
            else
            {
                argString = "";
                command = this.m_Command.ToLower();
                args = new string[0];
            }
        }
    }

    public class BatchGump : BaseGridGump
    {
        private readonly Mobile m_From;
        private readonly Batch m_Batch;
        public BatchGump(Mobile from, Batch batch)
            : base(30, 30)
        {
            this.m_From = from;
            this.m_Batch = batch;

            this.Render();
        }

        public void Render()
        {
            this.AddNewPage();

            /* Header */
            this.AddEntryHeader(20);
            this.AddEntryHtml(180, this.Center("Batch Commands"));
            this.AddEntryHeader(20);
            this.AddNewLine();

            this.AddEntryHeader(9);
            this.AddEntryLabel(191, "Run Batch");
            this.AddEntryButton(20, ArrowRightID1, ArrowRightID2, this.GetButtonID(1, 0, 0), ArrowRightWidth, ArrowRightHeight);
            this.AddNewLine();

            this.AddBlankLine();

            /* Scope */
            this.AddEntryHeader(20);
            this.AddEntryHtml(180, this.Center("Scope"));
            this.AddEntryHeader(20);
            this.AddNewLine();

            this.AddEntryHeader(9);
            this.AddEntryLabel(191, this.m_Batch.Scope == null ? "Select Scope" : this.m_Batch.Scope.Accessors[0]);
            this.AddEntryButton(20, ArrowRightID1, ArrowRightID2, this.GetButtonID(1, 0, 1), ArrowRightWidth, ArrowRightHeight);
            this.AddNewLine();

            this.AddBlankLine();

            /* Condition */
            this.AddEntryHeader(20);
            this.AddEntryHtml(180, this.Center("Condition"));
            this.AddEntryHeader(20);
            this.AddNewLine();

            this.AddEntryHeader(9);
            this.AddEntryText(202, 0, this.m_Batch.Condition);
            this.AddEntryHeader(9);
            this.AddNewLine();

            this.AddBlankLine();

            /* Commands */
            this.AddEntryHeader(20);
            this.AddEntryHtml(180, this.Center("Commands"));
            this.AddEntryHeader(20);

            for (int i = 0; i < this.m_Batch.BatchCommands.Count; ++i)
            {
                BatchCommand bc = (BatchCommand)this.m_Batch.BatchCommands[i];

                this.AddNewLine();

                this.AddImageTiled(this.CurrentX, this.CurrentY, 9, 2, 0x24A8);
                this.AddImageTiled(this.CurrentX, this.CurrentY + 2, 2, this.EntryHeight + this.OffsetSize + this.EntryHeight - 4, 0x24A8);
                this.AddImageTiled(this.CurrentX, this.CurrentY + this.EntryHeight + this.OffsetSize + this.EntryHeight - 2, 9, 2, 0x24A8);
                this.AddImageTiled(this.CurrentX + 3, this.CurrentY + 3, 6, this.EntryHeight + this.EntryHeight - 4 - this.OffsetSize, this.HeaderGumpID);

                this.IncreaseX(9);
                this.AddEntryText(202, 1 + (i * 2), bc.Command);
                this.AddEntryHeader(9, 2);

                this.AddNewLine();

                this.IncreaseX(9);
                this.AddEntryText(202, 2 + (i * 2), bc.Object);
            }

            this.AddNewLine();

            this.AddEntryHeader(9);
            this.AddEntryLabel(191, "Add New Command");
            this.AddEntryButton(20, ArrowRightID1, ArrowRightID2, this.GetButtonID(1, 0, 2), ArrowRightWidth, ArrowRightHeight);

            this.FinishPage();
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            int type, index;

            if (!this.SplitButtonID(info.ButtonID, 1, out type, out index))
                return;

            TextRelay entry = info.GetTextEntry(0);

            if (entry != null)
                this.m_Batch.Condition = entry.Text;

            for (int i = this.m_Batch.BatchCommands.Count - 1; i >= 0; --i)
            {
                BatchCommand sc = (BatchCommand)this.m_Batch.BatchCommands[i];

                entry = info.GetTextEntry(1 + (i * 2));

                if (entry != null)
                    sc.Command = entry.Text;

                entry = info.GetTextEntry(2 + (i * 2));

                if (entry != null)
                    sc.Object = entry.Text;

                if (sc.Command.Length == 0 && sc.Object.Length == 0)
                    this.m_Batch.BatchCommands.RemoveAt(i);
            }

            switch ( type )
            {
                case 0: // main
                    {
                        switch ( index )
                        {
                            case 0: // run
                                {
                                    this.m_Batch.Run(this.m_From);
                                    break;
                                }
                            case 1: // set scope
                                {
                                    this.m_From.SendGump(new BatchScopeGump(this.m_From, this.m_Batch));
                                    return;
                                }
                            case 2: // add command
                                {
                                    this.m_Batch.BatchCommands.Add(new BatchCommand("", ""));
                                    break;
                                }
                        }

                        break;
                    }
            }

            this.m_From.SendGump(new BatchGump(this.m_From, this.m_Batch));
        }
    }

    public class BatchScopeGump : BaseGridGump
    {
        private readonly Mobile m_From;
        private readonly Batch m_Batch;
        public BatchScopeGump(Mobile from, Batch batch)
            : base(30, 30)
        {
            this.m_From = from;
            this.m_Batch = batch;

            this.Render();
        }

        public void Render()
        {
            this.AddNewPage();

            /* Header */
            this.AddEntryHeader(20);
            this.AddEntryHtml(140, this.Center("Change Scope"));
            this.AddEntryHeader(20);

            /* Options */
            for (int i = 0; i < BaseCommandImplementor.Implementors.Count; ++i)
            {
                BaseCommandImplementor impl = BaseCommandImplementor.Implementors[i];

                if (this.m_From.AccessLevel < impl.AccessLevel)
                    continue;

                this.AddNewLine();

                this.AddEntryLabel(20 + this.OffsetSize + 140, impl.Accessors[0]);
                this.AddEntryButton(20, ArrowRightID1, ArrowRightID2, this.GetButtonID(1, 0, i), ArrowRightWidth, ArrowRightHeight);
            }

            this.FinishPage();
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            int type, index;

            if (this.SplitButtonID(info.ButtonID, 1, out type, out index))
            {
                switch ( type )
                {
                    case 0:
                        {
                            if (index < BaseCommandImplementor.Implementors.Count)
                            {
                                BaseCommandImplementor impl = BaseCommandImplementor.Implementors[index];

                                if (this.m_From.AccessLevel >= impl.AccessLevel)
                                    this.m_Batch.Scope = impl;
                            }

                            break;
                        }
                }
            }

            this.m_From.SendGump(new BatchGump(this.m_From, this.m_Batch));
        }
    }
}