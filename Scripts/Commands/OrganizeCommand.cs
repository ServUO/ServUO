using Server;
using Server.Commands;
using System.Collections.Generic;

public static class ItemListCommand
{
    public static void Initialize()
    {
        CommandSystem.Register("organize", AccessLevel.Player, new CommandEventHandler(OpenOrganizeCommand));
    }

    public static void OpenOrganizeCommand(CommandEventArgs e)
    {
        Mobile from = e.Mobile;

        // Create a sample list of items
        List<Item> items = new List<Item>();

        from.SendGump(new OrganizeGump(items, 0));
    }
}
