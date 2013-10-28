using System;
using System.Globalization;
using Server.Engines.Quests;
using Server.Mobiles;
using Server.Network;
using Haven = Server.Engines.Quests.Haven;
using Necro = Server.Engines.Quests.Necro;

namespace Server.Items
{
    public class BankCheck : Item
    {
        private int m_Worth;
        public BankCheck(Serial serial)
            : base(serial)
        {
        }

        [Constructable]
        public BankCheck(int worth)
            : base(0x14F0)
        {
            this.Weight = 1.0;
            this.Hue = 0x34;
            this.LootType = LootType.Blessed;

            this.m_Worth = worth;
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int Worth
        {
            get
            {
                return this.m_Worth;
            }
            set
            {
                this.m_Worth = value;
                this.InvalidateProperties();
            }
        }
        public override bool DisplayLootType
        {
            get
            {
                return Core.AOS;
            }
        }
        public override int LabelNumber
        {
            get
            {
                return 1041361;
            }
        }// A bank check
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version

            writer.Write((int)this.m_Worth);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            this.LootType = LootType.Blessed;

            int version = reader.ReadInt();

            switch ( version )
            {
                case 0:
                    {
                        this.m_Worth = reader.ReadInt();
                        break;
                    }
            }
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            string worth;

            if (Core.ML)
                worth = this.m_Worth.ToString("N0", CultureInfo.GetCultureInfo("en-US"));
            else
                worth = this.m_Worth.ToString();

            list.Add(1060738, worth); // value: ~1_val~
        }

        public override void OnSingleClick(Mobile from)
        {
            from.Send(new MessageLocalizedAffix(this.Serial, this.ItemID, MessageType.Label, 0x3B2, 3, 1041361, "", AffixType.Append, String.Concat(" ", this.m_Worth.ToString()), "")); // A bank check:
        }

        public override void OnDoubleClick(Mobile from)
        {
            BankBox box = from.FindBankNoCreate();

            if (box != null && this.IsChildOf(box))
            {
                this.Delete();

                int deposited = 0;

                int toAdd = this.m_Worth;

                Gold gold;

                while (toAdd > 60000)
                {
                    gold = new Gold(60000);

                    if (box.TryDropItem(from, gold, false))
                    {
                        toAdd -= 60000;
                        deposited += 60000;
                    }
                    else
                    {
                        gold.Delete();

                        from.AddToBackpack(new BankCheck(toAdd));
                        toAdd = 0;

                        break;
                    }
                }

                if (toAdd > 0)
                {
                    gold = new Gold(toAdd);

                    if (box.TryDropItem(from, gold, false))
                    {
                        deposited += toAdd;
                    }
                    else
                    {
                        gold.Delete();

                        from.AddToBackpack(new BankCheck(toAdd));
                    }
                }

                // Gold was deposited in your account:
                from.SendLocalizedMessage(1042672, true, " " + deposited.ToString());

                PlayerMobile pm = from as PlayerMobile;

                if (pm != null)
                {
                    QuestSystem qs = pm.Quest;

                    if (qs is Necro.DarkTidesQuest)
                    {
                        QuestObjective obj = qs.FindObjective(typeof(Necro.CashBankCheckObjective));

                        if (obj != null && !obj.Completed)
                            obj.Complete();
                    }

                    if (qs is Haven.UzeraanTurmoilQuest)
                    {
                        QuestObjective obj = qs.FindObjective(typeof(Haven.CashBankCheckObjective));

                        if (obj != null && !obj.Completed)
                            obj.Complete();
                    }
                }
            }
            else
            {
                from.SendLocalizedMessage(1047026); // That must be in your bank box to use it.
            }
        }
    }
}