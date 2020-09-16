using Server.ContextMenus;
using Server.Gumps;
using Server.Multis;
using Server.Network;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Server.Items
{
    public class SecretChestArray
    {
        public Mobile Mobile { get; set; }
        public bool Permission { get; set; }
        public int TrialsNumber { get; set; }
        public DateTime Expire { get; set; }
    }

    [Flipable(0x9707, 0x9706)]
    public class SecretChest : LockableContainer
    {
        public List<SecretChestArray> list = new List<SecretChestArray>();

        public override int LabelNumber => 1151583;  // Secret Chest

        public override int DefaultGumpID => 0x58E;

        public int[] SecretKey { get; set; } = { 0, 0, 0, 0, 0 };

        [CommandProperty(AccessLevel.GameMaster)]
        public Mobile LockingPerson { get; set; }

        [Constructable]
        public SecretChest()
            : this(0x9707)
        {
        }

        [Constructable]
        public SecretChest(int id)
            : base(id)
        {
            Weight = 5;
        }

        public bool CheckPermission(Mobile from)
        {
            SecretChestArray p = list.FirstOrDefault(x => x.Mobile == from);

            return LockingPerson.Account == from.Account || p != null && p.Permission;
        }

        public override void Open(Mobile from)
        {
            if (Locked && from.AccessLevel < AccessLevel.GameMaster && LockingPerson.Account != from.Account)
            {
                SecretChestArray l = list.FirstOrDefault(x => x.Mobile == from);

                if (l == null)
                {
                    l = new SecretChestArray { Mobile = from, TrialsNumber = 3 };
                    list.Add(l);
                }

                if (l.Permission)
                {
                    from.SendLocalizedMessage(1151600); // You remember the key number of this chest, so you can open this now.
                    DisplayTo(from);
                    return;
                }

                if (l.TrialsNumber == 0 && l.Expire < DateTime.UtcNow)
                {
                    l.TrialsNumber = 3;
                }

                if (l.TrialsNumber > 0)
                {
                    from.SendLocalizedMessage(501747); // It appears to be locked.
                    from.SendLocalizedMessage(1151527); // Enter the key number to open.
                    from.SendLocalizedMessage(1152346, string.Format("{0}", l.TrialsNumber)); // Number of tries left: ~1_times~
                    from.SendGump(new SecretChestGump(this, false));
                }
                else
                {
                    from.SendLocalizedMessage(1151599); // You cannot access the chest now, having exhausted your number of attempts.
                }
            }
            else
            {
                DisplayTo(from);
            }
        }

        public override void GetContextMenuEntries(Mobile from, List<ContextMenuEntry> list)
        {
            base.GetContextMenuEntries(from, list);

            if (LockingPerson == null || LockingPerson != null && LockingPerson.Account == from.Account)
            {
                list.Add(new SetEditKeyNumber(from, this));
                list.Add(new ResetKeyNumber(from, this));
            }
        }

        private class SetEditKeyNumber : ContextMenuEntry
        {
            private readonly SecretChest Chest;
            private readonly Mobile Mobile;

            public SetEditKeyNumber(Mobile m, SecretChest c)
                : base(1151608, -1) // Set/Edit Key Number
            {
                Mobile = m;
                Chest = c;

                if (!c.IsChildOf(m.Backpack))
                    Flags |= CMEFlags.Disabled;
            }

            public override void OnClick()
            {
                if (Mobile != null && Chest != null)
                {
                    if (Chest.Locked)
                        Mobile.SendLocalizedMessage(1151588); // Edit your key number. If you don't want to change current key number, click CANCEL button.
                    else
                        Mobile.SendLocalizedMessage(1151525); // Set your key number. If you want to leave this unlocked, click CANCEL button.

                    Mobile.SendGump(new SecretChestGump(Chest, true));
                }
            }
        }

        private class ResetKeyNumber : ContextMenuEntry
        {
            private readonly SecretChest Chest;
            private readonly Mobile Mobile;

            public ResetKeyNumber(Mobile m, SecretChest c)
                : base(1151609, -1) // Reset Key Number
            {
                Mobile = m;
                Chest = c;

                if (!c.IsChildOf(m.Backpack) || !Chest.Locked)
                    Flags |= CMEFlags.Disabled;
            }

            public override void OnClick()
            {
                if (Mobile != null && Chest != null)
                {
                    Chest.Locked = false;
                    Chest.LockingPerson = null;
                    Chest.list.Clear();
                    Mobile.SendLocalizedMessage(1151598); // You have reset the number key of this chest. This is unlocked now.
                }
            }
        }

        public override bool DisplaysContent => false;

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            if (Locked)
                list.Add(1151610); // Locked

            list.Add(1072241, "{0}\t{1}\t{2}\t{3}", TotalItems, MaxItems, TotalWeight, MaxWeight);
            // Contents: ~1_COUNT~/~2_MAXCOUNT~ items, ~3_WEIGHT~/~4_MAXWEIGHT~ stones
        }

        public override bool TryDropItem(Mobile from, Item dropped, bool sendFullMessage)
        {
            if (Locked && !CheckPermission(from))
            {
                from.SendLocalizedMessage(1151591); // You cannot place items into a locked chest!
                return false;
            }

            if (!CheckHold(from, dropped, true, true))
            {
                return false;
            }

            BaseHouse house = BaseHouse.FindHouseAt(this);

            if (house != null && IsLockedDown)
            {
                if (!house.CheckAccessibility(this, from))
                {
                    PrivateOverheadMessage(MessageType.Regular, 0x21, 1061637, from.NetState); // You are not allowed to access this!
                    from.SendLocalizedMessage(501727); // You cannot lock that down!
                    return false;
                }
            }

            DropItem(dropped);

            return true;
        }

        public override bool IsAccessibleTo(Mobile m)
        {
            return true;
        }

        public SecretChest(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);

            writer.Write(LockingPerson);

            writer.Write(list.Count);

            for (int i = 0; i < list.Count; ++i)
            {
                writer.Write(list[i].Mobile);
                writer.Write(list[i].Permission);
                writer.Write(list[i].TrialsNumber);
                writer.Write(list[i].Expire);
            }

            writer.Write(SecretKey.Length);

            for (int i = 0; i < SecretKey.Length; ++i)
            {
                writer.Write(SecretKey[i]);
            }
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            LockingPerson = reader.ReadMobile();

            int count = reader.ReadInt();

            for (int i = 0; i < count; ++i)
            {
                Mobile m = reader.ReadMobile();

                if (m != null)
                {
                    list.Add(new SecretChestArray { Mobile = m, Permission = reader.ReadBool(), TrialsNumber = reader.ReadInt(), Expire = reader.ReadDateTime() });
                }
            }

            count = reader.ReadInt();

            for (int i = 0; i < count; ++i)
            {
                SecretKey[i] = reader.ReadInt();
            }
        }
    }

    public class SecretChestGump : Gump
    {
        private readonly SecretChest Chest;
        private readonly int[] TempSecretKey;
        private readonly bool SetEdit;

        public SecretChestGump(SecretChest chest, bool setedit)
            : this(chest, null, setedit)
        {
        }

        public SecretChestGump(SecretChest chest, int[] sk, bool setedit)
            : base(0, 0)
        {
            Chest = chest;

            SetEdit = setedit;

            if (setedit)
            {
                TempSecretKey = chest.SecretKey;
            }
            else
            {
                if (sk == null)
                {
                    TempSecretKey = new int[] { 0, 0, 0, 0, 0 };
                }
                else
                {
                    TempSecretKey = sk;
                }
            }

            AddPage(0);

            AddImage(50, 50, 0x58D);

            AddButton(133, 270, 0x81A, 0x81B, SetEdit ? 2 : 1, GumpButtonType.Reply, 0); // OKAY
            AddButton(320, 270, 0x819, 0x818, 0, GumpButtonType.Reply, 0); // CANCEL

            for (int i = 0; i < TempSecretKey.Length; ++i)
            {
                AddButton(192 + (24 * i), 190, 0x58C, 0x58C, 10 + (i * 2), GumpButtonType.Reply, 0);
                AddImage(190 + (24 * i), 200, 0x58F + TempSecretKey[i]);
                AddButton(192 + (24 * i), 230, 0x599, 0x599, 11 + (i * 2), GumpButtonType.Reply, 0);
            }
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            Mobile from = sender.Mobile;

            if (Chest == null)
                return;

            switch (info.ButtonID)
            {
                case 0:
                    {
                        from.SendLocalizedMessage(1042021); // Cancelled.
                        break;
                    }
                case 1:
                    {
                        SecretChestArray l = Chest.list.FirstOrDefault(x => x.Mobile == from);

                        if (l == null)
                            return;

                        if (Chest.SecretKey.SequenceEqual(TempSecretKey))
                        {
                            from.SendLocalizedMessage(1151589); // You succeed at entering the correct key number; you may open the chest now.
                            l.Permission = true;
                            Chest.DisplayTo(from);
                        }
                        else
                        {
                            l.TrialsNumber--;

                            if (l.TrialsNumber > 0)
                            {
                                from.SendLocalizedMessage(1151590); // The number which you have entered is wrong. You still can't open this chest...                                
                                from.SendLocalizedMessage(1152346, string.Format("{0}", l.TrialsNumber)); // Number of tries left: ~1_times~
                                Timer.DelayCall(TimeSpan.FromSeconds(0.2), () => from.SendGump(new SecretChestGump(Chest, TempSecretKey, SetEdit)));
                            }
                            else
                            {
                                l.Expire = DateTime.UtcNow + TimeSpan.FromDays(1);
                                from.SendLocalizedMessage(1151611); // You have exhausted your unlock attempts for this chest; you must wait 24 hours to try again.
                            }
                        }
                        break;
                    }
                case 2:
                    {
                        for (int i = 0; i < Chest.SecretKey.Length; ++i)
                        {
                            Chest.SecretKey[i] = TempSecretKey[i];
                        }

                        Chest.Locked = true;
                        Chest.LockingPerson = from;
                        Chest.list.Clear();
                        from.SendLocalizedMessage(1151528); // The key numbers have been set; this chest is now locked.

                        break;
                    }
                default:
                    {
                        int index = info.ButtonID - 10;

                        if (info.ButtonID % 2 == 0)
                        {
                            index /= 2;

                            if (TempSecretKey[index] == 9)
                            {
                                TempSecretKey[index] = 0;
                            }
                            else
                            {
                                TempSecretKey[index]++;
                            }
                        }
                        else
                        {
                            index = (index - 1) / 2;

                            if (TempSecretKey[index] == 0)
                            {
                                TempSecretKey[index] = 9;
                            }
                            else
                            {
                                TempSecretKey[index]--;
                            }
                        }

                        Timer.DelayCall(TimeSpan.FromSeconds(0.2), () => from.SendGump(new SecretChestGump(Chest, TempSecretKey, SetEdit)));

                        break;
                    }
            }
        }
    }
}
