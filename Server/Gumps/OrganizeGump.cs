using Server;
using Server.Gumps;
using Server.Items;
using Server.Network;
using Server.Targeting;
using System;
using System.Collections.Generic;

public class OrganizeGump : Gump
{
    private List<Item> _items; // The list of items to display.
    private int _currentPage; // Current page number
    private const int ItemsPerPage = 10; // Number of items per page
    private Container _selectedBag; // The selected bag

    public OrganizeGump(List<Item> items, int currentPage = 0, Container selectedBag = null) : base(50, 50)
    {
        _items = items;
        _currentPage = currentPage;
        _selectedBag = selectedBag;

        Closable = true;
        Dragable = true;
        Resizable = false;

        AddPage(0);
        AddBackground(0, 0, 300, 400, 9270);
        AddLabel(20, 20, 1152, "Item List:");

        int y = 50;
        int start = _currentPage * ItemsPerPage;
        int end = Math.Min(start + ItemsPerPage, _items.Count);

        for (int i = start; i < end; i++)
        {
            Item item = _items[i];
            item = World.FindItem(item.Serial);
            AddLabel(20, y, 1152, item.ItemID + " - " + item.ItemData.Name ?? "Unnamed Item");
            AddButton(200, y, 4017, 4019, i + 1, GumpButtonType.Reply, 0);
            y += 25;
        }

        // Navigation Buttons
        if (_currentPage > 0)
        {
            AddButton(20, 370, 4014, 4016, 1001, GumpButtonType.Reply, 0); // Previous Page
            AddLabel(60, 370, 1152, "Previous");
        }

        if (end < _items.Count)
        {
            AddButton(150, 370, 4005, 4007, 1002, GumpButtonType.Reply, 0); // Next Page
            AddLabel(190, 370, 1152, "Next");
        }

        // Add Item Button
        AddButton(20, 340, 4005, 4007, 1003, GumpButtonType.Reply, 0); // Add Item
        AddLabel(60, 340, 1152, "Add Item");

        // Target Bag Button
        AddButton(20, 310, 4005, 4007, 1004, GumpButtonType.Reply, 0); // Target Bag
        AddLabel(60, 310, 1152, "Target Bag");

        // Selected Bag Label
        AddLabel(20, 280, 1152, "Selected Bag: " + (_selectedBag != null ? _selectedBag.Name : "None"));
    }

    public override void OnResponse(NetState sender, RelayInfo info)
    {
        Mobile from = sender.Mobile;

        if (info.ButtonID == 1001) // Previous Page
        {
            from.SendGump(new OrganizeGump(_items, _currentPage - 1, _selectedBag));
        }
        else if (info.ButtonID == 1002) // Next Page
        {
            from.SendGump(new OrganizeGump(_items, _currentPage + 1, _selectedBag));
        }
        else if (info.ButtonID == 1003) // Add Item
        {
            from.SendMessage("Target an item in your bag to add to the list.");
            from.Target = new ItemTarget(this, _items);
        }
        else if (info.ButtonID == 1004) // Target Bag
        {
            from.SendMessage("Target a bag to select it.");
            from.Target = new BagTarget(this, _items, _selectedBag);
        }
        else if (info.ButtonID > 0 && info.ButtonID <= _items.Count) // Remove item
        {
            int indexToRemove = info.ButtonID - 1;
            _items.RemoveAt(indexToRemove);
            from.SendGump(new OrganizeGump(_items, _currentPage, _selectedBag));
        }
    }

    private class ItemTarget : Target
    {
        private OrganizeGump _gump;
        private List<Item> _items;

        public ItemTarget(OrganizeGump gump, List<Item> items) : base(18, false, TargetFlags.None)
        {
            _gump = gump;
            _items = items;
        }

        protected override void OnTarget(Mobile from, object targeted)
        {
            if (targeted is Item item && item.IsChildOf(from.Backpack))
            {
                _items.Add(item);
                from.SendGump(new OrganizeGump(_items, _gump._currentPage));
            }
            else
            {
                from.SendMessage("You can only target items in your backpack.");
                from.SendGump(new OrganizeGump(_items, _gump._currentPage));
            }
        }
    }

    private class BagTarget : Target
    {
        private OrganizeGump _gump;
        private List<Item> _items;
        private Container _selectedBag;

        public BagTarget(OrganizeGump gump, List<Item> items, Container selectedBag) : base(18, false, TargetFlags.None)
        {
            _gump = gump;
            _items = items;
            _selectedBag = selectedBag;
        }

        protected override void OnTarget(Mobile from, object targeted)
        {
            if (targeted is Container bag && bag.IsChildOf(from.Backpack))
            {
                _selectedBag = bag;
                from.SendGump(new OrganizeGump(_items, _gump._currentPage, _selectedBag));
            }
            else
            {
                from.SendMessage("You can only target bags in your backpack.");
                from.SendGump(new OrganizeGump(_items, _gump._currentPage, _selectedBag));
            }
        }
    }
}
