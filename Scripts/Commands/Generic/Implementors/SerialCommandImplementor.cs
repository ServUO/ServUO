namespace Server.Commands.Generic
{
    public class SerialCommandImplementor : BaseCommandImplementor
    {
        public SerialCommandImplementor()
        {
            Accessors = new string[] { "Serial" };
            SupportRequirement = CommandSupport.Single;
            AccessLevel = AccessLevel.Counselor;
            Usage = "Serial <serial> <command>";
            Description = "Invokes the command on a single object by serial.";
        }

        public override void Execute(CommandEventArgs e)
        {
            if (e.Length >= 2)
            {
                Serial serial = e.GetInt32(0);

                object obj = null;

                if (serial.IsItem)
                    obj = World.FindItem(serial);
                else if (serial.IsMobile)
                    obj = World.FindMobile(serial);

                if (obj == null)
                {
                    e.Mobile.SendMessage("That is not a valid serial.");
                }
                else
                {
                    BaseCommand command = null;
                    Commands.TryGetValue(e.GetString(1), out command);

                    if (command == null)
                    {
                        e.Mobile.SendMessage("That is either an invalid command name or one that does not support this modifier.");
                    }
                    else if (e.Mobile.AccessLevel < command.AccessLevel)
                    {
                        e.Mobile.SendMessage("You do not have access to that command.");
                    }
                    else
                    {
                        switch (command.ObjectTypes)
                        {
                            case ObjectTypes.Both:
                                {
                                    if (!(obj is Item) && !(obj is Mobile))
                                    {
                                        e.Mobile.SendMessage("This command does not work on that.");
                                        return;
                                    }

                                    break;
                                }
                            case ObjectTypes.Items:
                                {
                                    if (!(obj is Item))
                                    {
                                        e.Mobile.SendMessage("This command only works on items.");
                                        return;
                                    }

                                    break;
                                }
                            case ObjectTypes.Mobiles:
                                {
                                    if (!(obj is Mobile))
                                    {
                                        e.Mobile.SendMessage("This command only works on mobiles.");
                                        return;
                                    }

                                    break;
                                }
                        }

                        string[] oldArgs = e.Arguments;
                        string[] args = new string[oldArgs.Length - 2];

                        for (int i = 0; i < args.Length; ++i)
                            args[i] = oldArgs[i + 2];

                        RunCommand(e.Mobile, obj, command, args);
                    }
                }
            }
            else
            {
                e.Mobile.SendMessage("You must supply an object serial and a command name.");
            }
        }
    }
}