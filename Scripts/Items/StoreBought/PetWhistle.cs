using Server.Accounting;
using Server.ContextMenus;
using Server.Gumps;
using Server.Mobiles;
using Server.Network;
using Server.Targeting;
using System;
using System.Collections.Generic;

namespace Server.Items
{
    public class PetWhistle : Item
    {
        public override int LabelNumber => 1126239;  // whistle

        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime NextLinkedTime { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public string Account { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public BaseCreature PetLinked { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool IsUsable => NextLinkedTime < DateTime.UtcNow;

        [Constructable]
        public PetWhistle()
            : base(0xA4E7)
        {
            LootType = LootType.Blessed;
        }

        public PetWhistle(Serial serial)
            : base(serial)
        {
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!string.IsNullOrEmpty(Account))
            {
                Account acct = from.Account as Account;

                if (acct == null || acct.Username != Account)
                {
                    from.SendLocalizedMessage(1151920); // This item is Account Bound, you are not permitted to take this action.
                    return;
                }
            }

            if (IsChildOf(from.Backpack))
            {
                if (PetLinked != null)
                {
                    from.CloseGump(typeof(PetWhistleGump));
                    from.SendGump(new PetWhistleGump(this));
                }
                else
                {
                    from.SendLocalizedMessage(1159390); // Target a bonded pet to link this whistle.
                    from.Target = new InternalTarget(this);
                }
            }
            else
            {
                from.SendLocalizedMessage(1062334); // This item must be in your backpack to be used.
            }
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            if (PetLinked != null)
                list.Add(1159360, PetLinked.Name); // Pet Whistle for ~1_name~

            if (!string.IsNullOrEmpty(Account))
                list.Add(1155526); // Account Bound
        }

        public override void GetContextMenuEntries(Mobile from, List<ContextMenuEntry> list)
        {
            base.GetContextMenuEntries(from, list);

            list.Add(new LinkBondedPetEntry(from, this));
        }

        private class LinkBondedPetEntry : ContextMenuEntry
        {
            private readonly PetWhistle _Item;
            private readonly Mobile m_From;

            public LinkBondedPetEntry(Mobile from, PetWhistle item)
                : base(1159393, 6) // Link Bonded Pet
            {
                m_From = from;
                _Item = item;

                Enabled = _Item.IsUsable;
            }

            public override void OnClick()
            {
                if (m_From == null || _Item.Deleted)
                    return;

                if (_Item.IsChildOf(m_From.Backpack))
                {
                    m_From.SendLocalizedMessage(1159390); // Target a bonded pet to link this whistle.
                    m_From.Target = new InternalTarget(_Item);
                }
                else
                {
                    m_From.SendLocalizedMessage(1062334); // This item must be in your backpack to be used.
                }
            }
        }

        private class InternalTarget : Target
        {
            private readonly PetWhistle _Item;

            public InternalTarget(PetWhistle item)
                : base(12, true, TargetFlags.None)
            {
                _Item = item;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (_Item.Deleted)
                    return;

                if (!_Item.IsChildOf(from.Backpack))
                {
                    from.SendLocalizedMessage(1060640); // The item must be in your backpack to use it.
                    return;
                }

                if (targeted is BaseCreature && ((BaseCreature)targeted).ControlMaster == from)
                {
                    if (((BaseCreature)targeted).IsBonded)
                    {
                        if (_Item.IsUsable)
                        {
                            _Item.PetLinked = (BaseCreature)targeted;
                            _Item.NextLinkedTime = DateTime.UtcNow + TimeSpan.FromDays(7);
                            _Item.InvalidateProperties();

                            from.CloseGump(typeof(PetWhistleGump));
                            from.SendGump(new PetWhistleGump(_Item));
                        }
                        else
                        {
                            if ((DateTime.UtcNow - _Item.NextLinkedTime).Seconds > 60)
                            {
                                from.SendLocalizedMessage(1159391); // You must wait ~1_minutes~ minutes before you can link another pet to this whistle.
                            }
                            else
                            {
                                from.SendLocalizedMessage(1159392); // You must wait ~1_seconds~ seconds before you can link another pet to this whistle.
                            }
                        }
                    }
                    else
                    {
                        from.SendLocalizedMessage(1159373); // This item can only be linked to a bonded pet.
                    }
                }
                else
                {
                    from.SendLocalizedMessage(1149667); // Invalid target.
                }
            }
        }

        public class PetWhistleGump : Gump
        {
            private readonly PetWhistle _Item;

            public PetWhistleGump(PetWhistle item)
                : base(100, 100)
            {
                _Item = item;

                AddPage(0);

                AddBackground(0, 0, 255, 260, 0x5B4);
                AddAlphaRegion(0, 0, 255, 260);
                AddItem(100, 39, 42215);
                AddHtmlLocalized(81, 73, 157, 22, 1159405, 0x76F2, false, false); // Attack
                AddButton(41, 73, 0xFA5, 0xFA6, 1000, GumpButtonType.Reply, 0);
                AddHtmlLocalized(81, 97, 157, 22, 1159407, 0x76F2, false, false); // Block
                AddButton(41, 97, 0xFA5, 0xFA6, 1002, GumpButtonType.Reply, 0);
                AddHtmlLocalized(81, 121, 157, 22, 1159375, 0x76F2, false, false); // Play Dead
                AddButton(41, 121, 0xFA5, 0xFA6, 1003, GumpButtonType.Reply, 0);
                AddHtmlLocalized(81, 145, 157, 22, 1159378, 0x76F2, false, false); // Settle Down
                AddButton(41, 145, 0xFA5, 0xFA6, 1004, GumpButtonType.Reply, 0);
                AddHtmlLocalized(81, 169, 157, 22, 1159377, 0x76F2, false, false); // Wait
                AddButton(41, 169, 0xFA5, 0xFA6, 1005, GumpButtonType.Reply, 0);
                AddHtmlLocalized(81, 193, 157, 22, 1159406, 0x76F2, false, false); // Eat
                AddButton(41, 193, 0xFA5, 0xFA6, 1006, GumpButtonType.Reply, 0);
                AddHtmlLocalized(81, 217, 157, 22, 1159388, 0x76F2, false, false); // What's that?
                AddButton(41, 217, 0xFA5, 0xFA6, 1008, GumpButtonType.Reply, 0);
                AddHtmlLocalized(40, 15, 200, 18, 1159360, item.PetLinked.Name, 0x6A05, false, false); // Pet Whistle for ~1_name~
            }

            public override void OnResponse(NetState sender, RelayInfo info)
            {
                if (_Item.Deleted || _Item.PetLinked == null)
                    return;

                Mobile m = sender.Mobile;

                if (_Item.PetLinked.ControlOrder != OrderType.Stay || _Item.PetLinked.ControlMaster != m)
                    m.SendLocalizedMessage(1159389); // You must command your pet to stay before using this item.

                Effects.PlaySound(m.Location, m.Map, 1665);

                switch (info.ButtonID)
                {
                    case 1000:
                        _Item.PetLinked.Animate(AnimationType.Attack, 0);
                        break;
                    case 1002:
                        _Item.PetLinked.Animate(AnimationType.Block, 0);
                        break;
                    case 1003:
                        _Item.PetLinked.Animate(AnimationType.Die, 0);
                        break;
                    case 1004:
                        _Item.PetLinked.Animate(AnimationType.Impact, 0);
                        break;
                    case 1005:
                        _Item.PetLinked.Animate(AnimationType.Fidget, 0);
                        break;
                    case 1006:
                        _Item.PetLinked.Animate(AnimationType.Eat, 0);
                        break;
                    case 1008:
                        _Item.PetLinked.Animate(AnimationType.Alert, 0);
                        break;
                }
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);

            writer.Write(Account);
            writer.Write(NextLinkedTime);
            writer.Write(PetLinked);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            Account = reader.ReadString();
            NextLinkedTime = reader.ReadDateTime();
            PetLinked = (BaseCreature)reader.ReadMobile();
        }
    }
}
