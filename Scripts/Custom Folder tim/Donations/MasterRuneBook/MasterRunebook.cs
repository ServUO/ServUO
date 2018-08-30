using System;
using Server;
using Server.Misc;
using Server.Spells;
using Server.Regions;
using Server.Items;
using Server.Gumps;
using Server.Network;
using Server.Menus;
using Server.Menus.Questions;
using Server.Engines.VeteranRewards;

namespace Server.Items
{
    public class MasterRunebook : Item, IRewardItem
    {
        private InternalRunebook[] m_Books;
        public InternalRunebook[] Books { get { return m_Books; } set { m_Books = value; } }

        private bool m_IsRewardItem;
        [CommandProperty(AccessLevel.GameMaster)]
        public bool IsRewardItem
        {
            get { return m_IsRewardItem; }
            set { m_IsRewardItem = value; InvalidateProperties(); }
        }
        [Constructable]
        public MasterRunebook()
            : base(0x2252)
        {
            Hue = Utility.RandomBlueHue();
            Weight = 0;
            Movable = true;
            Name = "Master Runebook";
            LootType = LootType.Blessed;
            m_Books = new InternalRunebook[51];
            for (int x = 0; x < 51; x++) m_Books[x] = new InternalRunebook(16);
        }

        public MasterRunebook(Serial serial) : base(serial) { }

        public override void OnDoubleClick(Mobile m)
        {
            if (m_IsRewardItem && !RewardSystem.CheckIsUsableBy(m, this, null))
            {
                m.SendMessage("This does not belong to you!!");
                return;
            }
            m.SendGump(new MasterRunebookGump(m, this)); 
        }
public override void OnDelete()
        {
            for (int x = 0; x < 51; x++) m_Books[x].Delete();
            base.OnDelete();
        }
        public override bool OnDragDrop(Mobile from, Item dropped)
        {
        
            if (dropped is RecallScroll)
            {
                Item scrolls = dropped as Item;
                int amount = scrolls.Amount;

                if (amount <= 0) return false;  //amount should not be zero, but better safe than sorry.

                int count = 0;  //this keeps track of the count of scrolls added to each runebook
                int total = 0;  //this keeps track of the total added to all runebooks
                for (int x = 0; x < 51; x++)
                {
                    if (Books[x].Entries.Count < 16)  //check each runebook
                    {
                        if (Books[x].CurCharges < Books[x].MaxCharges)  //if there is space for more charges...
                        {
                            from.Send(new PlaySound(0x249, from.Location));

                            if (amount > (Books[x].MaxCharges - Books[x].CurCharges))  //if the amount of scrolls is > than space free
                            {
                                count = Books[x].MaxCharges - Books[x].CurCharges;
                                scrolls.Consume(count);
                                Books[x].CurCharges = Books[x].MaxCharges;
                                total += count;
                                amount -= count;
                            }
                            else  //otherwise we just add/delete whatever scrolls are left
                            {
                                Books[x].CurCharges += amount;
                                scrolls.Delete();
                                total += amount;
                                amount = 0;
                            }
                        }
                    }
                    if (amount <= 0)
                    {
                        from.SendMessage("{0} Recall Scrolls were added.", total.ToString());
                        return true;
                    }
                }
            }
            else
                if (dropped is RecallRune)
                {
                    for (int x = 0; x < 51; x++)
                    {
                        if (Books[x].Entries.Count < 16)
                        {
                            RecallRune rune = (RecallRune)dropped;

                            if (rune.Marked && rune.TargetMap != null)
                            {
                                Books[x].Entries.Add(new RunebookEntry(rune.Target, rune.TargetMap, rune.Description, rune.House));

                                dropped.Delete();

                                from.Send(new PlaySound(0x42, GetWorldLocation()));

                                string desc = rune.Description;

                                if (desc == null || (desc = desc.Trim()).Length == 0)
                                    desc = "(indescript)";

                                from.SendMessage(desc);

                                return true;
                            }
                            else
                            {
                                from.SendLocalizedMessage(502409); // This rune does not have a marked location.
                            }
                        }
                    }
                    from.SendLocalizedMessage(502401); // This runebook is full.
                }
                else
                    if (dropped is Runebook)
                    {
                        for (int x = 0; x < 51; x++)
                        {
                            if (Books[x].Entries.Count == 0)
                            {
                                Runebook rb = dropped as Runebook;
                                for (int y = 0; y < rb.Entries.Count; y++)
                                {
                                    RunebookEntry rune = rb.Entries[y] as RunebookEntry;
                                    Books[x].Entries.Add(new RunebookEntry(rune.Location, rune.Map, rune.Description, rune.House));
                                }
                                Books[x].Name = rb.Name;
                                dropped.Delete();
                                return true;
                            }
                        }
                        from.SendLocalizedMessage(502401); // This runebook is full.
                    }

            return false;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); //version
            writer.Write((bool)m_IsRewardItem);
            for (int x = 0; x < 51; x++) writer.Write((Item)m_Books[x]);
            //for (int x = 0; x < 51; x++) m_Books[x].Delete();

        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
            m_IsRewardItem = reader.ReadBool();
            m_Books = new InternalRunebook[51];
            for (int x = 0; x < 51; x++) m_Books[x] = (InternalRunebook)reader.ReadItem();
        }
    }
}
