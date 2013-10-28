/*using System;
using System.Collections.Generic;
using Server;
using Server.ContextMenus;
using Server.Mobiles;

namespace CustomsFramework.Systems.VIPSystem
{
[CorpseName("VIP Dealer's Corpse")]
public class VIPVendor : Mobile
{
[Constructable]
public VIPVendor()
{
this.Name = "VIP Dealer";
this.Direction = Direction.East;
this.Blessed = true;
}

public VIPVendor(Serial serial) : base(serial)
{
}

public override bool HandlesOnSpeech(Mobile from)
{
if (from.InRange(this.Location, 12))
return true;

return base.HandlesOnSpeech(from);
}

public override void OnSpeech(SpeechEventArgs e)
{
if (!e.Handled && e.Mobile.InRange(this.Location, 12))
{
}
base.OnSpeech(e);
}

public override void GetContextMenuEntries(Mobile from, List<Server.ContextMenus.ContextMenuEntry> list)
{
base.GetContextMenuEntries(from, list);

list.Add(new ViewShopEntry(from));
}

public override void OnDoubleClick(Mobile from)
{
SendGump(from);
}

public static void SendGump(Mobile from)
{
if (from is PlayerMobile)
{
PlayerMobile player = from as PlayerMobile;
//player.CloseGump(typeof(VIPShop));
//player.SendGump(new VIPShop(player));
}
}

public class ViewShopEntry : ContextMenuEntry
{
private readonly Mobile _Player;

public ViewShopEntry(Mobile from) : base(1062219)
{
this._Player = from;
}

public override void OnClick()
{
SendGump(this._Player);
}
}
}
}*/