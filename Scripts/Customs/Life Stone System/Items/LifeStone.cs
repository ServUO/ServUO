using System; 
using Server; 
using Server.Mobiles;
using Server.Items;
using Server.Gumps;
using Server.ContextMenus;
using System.Collections.Generic;
using Server.Network;
using Server.Targeting;

namespace Server.Items
{
    public class LifeStone : Item, IUsesRemaining
    {
        [Constructable]
        public LifeStone()
            : this(Utility.RandomMinMax(100, 100))
        {
        }

        [Constructable(AccessLevel.GameMaster)]
        public LifeStone(int uses)
            : base(LifeStoneSettings.StoneItemID)
        {
            Movable = true;
            Name = "A Life Stone";
            Hue = 96;
            Weight = 0.0;

            TeleportCorpse = LifeStoneSettings.TeleportCorpse;

            if (LifeStoneSettings.BlessNewStones)
                LootType = LootType.Blessed;

            if (LifeStoneSettings.AllowInDungeons)
                AllowInDungeons = true;

            Hue = LifeStoneSettings.StoneHue;
            UsesRemaining = uses;
            ShowUsesRemaining = true;
        }

        private Point3D m_LinkedAnkh;
        private Map m_LinkedMap;
        private  bool m_TeleportCorpse;
        private Mobile m_LastOwner;
        private LifeStone m_Stone;
        private bool m_TeleportPets;
        private bool m_AllowInDungeons;

        private int m_UsesRemaining;
        private bool m_ShowUsesRemaining;

        [CommandProperty(AccessLevel.Counselor, AccessLevel.GameMaster)]
        public Point3D LinkedAnkh
        {
            get { return m_LinkedAnkh; }
            set { m_LinkedAnkh = value; }
        }

        [CommandProperty(AccessLevel.Counselor, AccessLevel.GameMaster)]
        public Map LinkedMap
        {
            get { return m_LinkedMap; }
            set { m_LinkedMap = value; }
        }

        [CommandProperty(AccessLevel.Counselor, AccessLevel.Administrator)]
        public bool TeleportCorpse
        {
            get { return m_TeleportCorpse; }
            set { m_TeleportCorpse = value; }
        }

        [CommandProperty(AccessLevel.Counselor, AccessLevel.Administrator)]
        public Mobile LastOwner
        {
            get { return m_LastOwner; }
            set { m_LastOwner = value; }
        }

        [CommandProperty(AccessLevel.Counselor, AccessLevel.GameMaster)]
        public int UsesRemaining
        {
            get { return m_UsesRemaining; }
            set { m_UsesRemaining = value; InvalidateProperties(); }
        }

        [CommandProperty(AccessLevel.Counselor, AccessLevel.GameMaster)]
        public bool ShowUsesRemaining
        {
            get { return m_ShowUsesRemaining; }
            set { m_ShowUsesRemaining = value; InvalidateProperties(); }
        }

        [CommandProperty(AccessLevel.Counselor, AccessLevel.GameMaster)]
        public bool TeleportPets
        {
            get { return m_TeleportPets; }
            set { m_TeleportPets = value; }
        }

        [CommandProperty(AccessLevel.Counselor, AccessLevel.GameMaster)]
        public bool AllowInDungeons { get { return m_AllowInDungeons; } set { m_AllowInDungeons = value; } }

        public override void OnDoubleClick(Mobile from)
        {
            if (!IsChildOf(from.Backpack))
            {
                from.SendLocalizedMessage(1032001);
            }
            else
            {
                if (LinkedMap != null)
                {
                    LastOwner = from;
                    bool changed = false;

                    for (int i = 0; i < LifeStoneCore.LifeStoneList.Count; i++)
                    {
                        if (LifeStoneCore.LifeStoneList[i].LifeStoneOwner == from)
                        {
                            LifeStoneCore.LifeStoneList[i].Stone = this;
                            changed = true;
                        }
                    }

                    if (changed == false)
                    {
                        LifeStoneCore.LifeStoneList.Add(new LifeStoneInfo(from, this));
                    }

                    from.SendMessage("You have set this Life Stone as your active one.");
                }
                else
                {
                    LastOwner = from;
                    from.SendMessage("Please target an Ankh to link your Life Stone to.");
                    from.Target = new InternalTarget(this);
                }
            }
        }

        public static void GetContextMenuEntries(Mobile from, Item item, List<ContextMenuEntry> list)
        {
            list.Add(new SetAnkhEntry(from, item));
        }

        public override void GetContextMenuEntries(Mobile from, List<ContextMenuEntry> list)
        {
            GetContextMenuEntries(from, this, list);
        }

        public override void GetProperties( ObjectPropertyList list )
        {
            base.GetProperties( list );

            list.Add(String.Format("{0} Charges", m_UsesRemaining));
        }

        public LifeStone(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)1); // version 

            //Version 1
            writer.Write(m_TeleportPets);
            writer.Write(m_AllowInDungeons);
            // Version 0
            writer.Write(m_LinkedAnkh);
            writer.Write(m_LinkedMap);
            writer.Write(m_TeleportCorpse);
            writer.Write(m_UsesRemaining);
            writer.Write(m_ShowUsesRemaining);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch (version)
            {
                case 1:
                    {
                        m_TeleportPets = reader.ReadBool();
                        m_AllowInDungeons = reader.ReadBool();
                        goto case 0;
                    }
                case 0:
                    {
                        m_LinkedAnkh = reader.ReadPoint3D();
                        m_LinkedMap = reader.ReadMap();
                        m_TeleportCorpse = reader.ReadBool();
                        m_UsesRemaining = reader.ReadInt();
                        m_ShowUsesRemaining = reader.ReadBool();
                        break;
                    }
            }
        }

        private class InternalTarget : Target
        {
            private LifeStone m_Stone;

            public InternalTarget(LifeStone stone)
                : base(5, false, TargetFlags.None)
            {
                m_Stone = stone;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (targeted is Item)
                {
                    Item m_Ankh = targeted as Item;

                    // Ankh ItemID's
                    if (m_Ankh.ItemID >= 2 && m_Ankh.ItemID <= 5)
                    {
                        m_Stone.LinkedAnkh = from.Location;
                        m_Stone.LinkedMap = from.Map;
                        from.SendMessage("Your life stone has been attached to this Ankh.");
                        from.SendMessage("When you die, you now have an option to teleport to here.");

                        bool changed = false;

                        for (int i = 0; i < LifeStoneCore.LifeStoneList.Count; i++)
                        {
                            if (LifeStoneCore.LifeStoneList[i].LifeStoneOwner == from)
                            {
                                LifeStoneCore.LifeStoneList[i].Stone = m_Stone;
                                changed = true;
                            }
                        }

                        if (changed == false)
                        {
                            LifeStoneCore.LifeStoneList.Add(new LifeStoneInfo(from, m_Stone));
                        }
                    }
                    else
                        from.SendMessage("That isn't an Ankh!");
                }
                else
                    from.SendMessage("That isn't an Ankh!");
            }
        }

        private class SetAnkhEntry : ContextMenuEntry
        {
            private LifeStone m_Item;
            private Mobile m_Mobile;

            public SetAnkhEntry(Mobile from, Item item)
                : base(2055)
            {
                m_Item = (LifeStone)item;
                m_Mobile = from;
            }

            public override void OnClick()
            {
                m_Mobile.SendMessage("Please target a new Ankh to link your Life Stone to.");
                m_Mobile.Target = new InternalTarget(m_Item);
            }
        }
    }
}