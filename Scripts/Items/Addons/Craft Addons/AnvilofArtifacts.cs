using System;
using Server.Gumps;
using Server.Multis;
using Server.Network;
using Server.Engines.Craft;
using Server.Mobiles;

using System.Collections.Generic;
using Server.ContextMenus;
using System.Linq;

namespace Server.Items
{
    public class AnvilofArtifactsComponent : LocalizedAddonComponent, IFlipable
    {
        public override bool ForceShowProperties => true;

        [CommandProperty(AccessLevel.GameMaster)]
        public bool TurnedOn
        {
            get { return IsActive; }
            set
            {
                if ((!value && IsActive) || (value && !IsActive))
                {
                    ToggleOnOff();
                }
            }
        }

        public virtual bool IsActive => ItemID == 0xA103 || ItemID == 0xA109;

        public AnvilofArtifactsComponent(int inactiveid)
            : base(inactiveid, 1125242) // anvil of artifacts
        {
        }

        public AnvilofArtifactsComponent(Serial serial)
            : base(serial)
        {
        }

        public void OnFlip(Mobile m)
        {
            switch (ItemID)
            {
                case 0xA102:
                    ItemID = 0xA108;
                    break;
                case 0xA103:
                    ItemID = 0xA109;
                    break;
                case 0xA108:
                    ItemID = 0xA102;
                    break;
                case 0xA109:
                    ItemID = 0xA103;
                    break;
            }
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            if (Addon == null)
                return;

            list.Add(1060584, ((AnvilofArtifactsAddon)Addon).UsesRemaining.ToString()); // uses remaining: ~1_val~
        }

        public override void GetContextMenuEntries(Mobile from, List<ContextMenuEntry> list)
        {
            base.GetContextMenuEntries(from, list);

            if (Addon == null)
                return;

            BaseHouse house = BaseHouse.FindHouseAt(this);

            if (house != null && house.IsOwner(from))
            {
                list.Add(new SimpleContextMenuEntry(from, 1155742, m => // Toggle: On/Off
                {
                    ToggleOnOff();

                }, 8));
            }
        }

        private void ToggleOnOff()
        {
            switch (ItemID)
            {
                case 0xA102:
                    ItemID = 0xA109;
                    break;
                case 0xA103:
                    ItemID = 0xA108;
                    break;
                case 0xA108:
                    ItemID = 0xA103;
                    break;
                case 0xA109:
                    ItemID = 0xA102;
                    break;
            }

            Effects.PlaySound(GetWorldLocation(), Map, 42);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(1); // Version

            writer.Write(TurnedOn);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            if (version == 0)
            {
                reader.ReadInt();
                reader.ReadInt();
            }

            TurnedOn = reader.ReadBool();
        }
    }

    public class AnvilofArtifactsAddon : BaseAddon
    {
        public override BaseAddonDeed Deed => new AnvilofArtifactsDeed(UsesRemaining);

        private int m_UsesRemaining;

        [CommandProperty(AccessLevel.GameMaster)]
        public int UsesRemaining
        {
            get { return m_UsesRemaining; }
            set { m_UsesRemaining = value; UpdateProperties(); }
        }

        [Constructable]
        public AnvilofArtifactsAddon(bool east, int uses)
            : base()
        {
            UsesRemaining = uses;

            if (east)
            {
                AddComponent(new AnvilofArtifactsComponent(0xA102), 0, 0, 0);
            }
            else
            {
                AddComponent(new AnvilofArtifactsComponent(0xA108), 0, 0, 0);
            }
        }

        public AnvilofArtifactsAddon(Serial serial)
            : base(serial)
        {
        }

        public override void OnComponentUsed(AddonComponent c, Mobile from)
        {
            BaseHouse house = BaseHouse.FindHouseAt(from);

            if (house != null && (house.IsOwner(from) || (house.Addons.ContainsKey(this) && house.Addons[this] == from)))
            {
                if (from is PlayerMobile pm && UsesRemaining > 0)
                {
                    from.CloseGump(typeof(AnvilofArtifactsGump));
                    BaseGump.SendGump(new AnvilofArtifactsGump(pm, this));
                }
            }
            else
            {
                from.SendLocalizedMessage(502092); // You must be in your house to do this.
            }
        }       

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);

            writer.Write(m_UsesRemaining);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();            

            m_UsesRemaining = reader.ReadInt();
        }
    }

    public class AnvilofArtifactsDeed : BaseAddonDeed, IRewardOption
    {
        public override int LabelNumber => 1125242;  // anvil of artifacts
        public override BaseAddon Addon => new AnvilofArtifactsAddon(m_Direction, UsesRemaining);

        private int m_UsesRemaining;

        [CommandProperty(AccessLevel.GameMaster)]
        public int UsesRemaining
        {
            get { return m_UsesRemaining; }
            set { m_UsesRemaining = value; InvalidateProperties(); }
        }

        private bool m_Direction;

        [Constructable]
        public AnvilofArtifactsDeed()
            : this(18)
        {
        }

        [Constructable]
        public AnvilofArtifactsDeed(int uses)
            : base()
        {
            LootType = LootType.Blessed;
            UsesRemaining = uses;
        }

        public AnvilofArtifactsDeed(Serial serial)
           : base(serial)
        {
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            list.Add(1060584, UsesRemaining.ToString()); // uses remaining: ~1_val~
        }

        public void GetOptions(RewardOptionList list)
        {
            list.Add(0, 1159443); // Anvil of Artifacts (South) 
            list.Add(1, 1159444); // Anvil of Artifacts (East)
        }

        public void OnOptionSelected(Mobile from, int choice)
        {
            m_Direction = choice == 1;

            if (!Deleted)
                base.OnDoubleClick(from);
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (IsChildOf(from.Backpack))
            {
                from.CloseGump(typeof(RewardOptionGump));
                from.SendGump(new RewardOptionGump(this, 1076170)); // Choose Direction
            }
            else
            {
                from.SendLocalizedMessage(1062334); // This item must be in your backpack to be used.
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);

            writer.Write(m_UsesRemaining);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            m_UsesRemaining = reader.ReadInt();
        }
    }

    public class AnvilofArtifactsGump : BaseGump
    {
        private AnvilofArtifactsAddon Addon { get; set; }
        private AnvilOfArtifactsEntry Entry { get; set; }

        public AnvilofArtifactsGump(PlayerMobile pm, AnvilofArtifactsAddon addon)
            : base(pm, 50, 50)
        {
            if (addon == null)
                return;

            Addon = addon;
            Entry = CraftContext.GetAnvilEntry(User);
        }

        public override void AddGumpLayout()
        {
            AddPage(0);

            if (Entry.Ready)
            {
                AddBackground(0, 0, 410, 390, 0x13BE);
                AddImageTiled(5, 5, 400, 25, 0xA40);
                AddHtmlLocalized(5, 8, 400, 25, 1113302, "#1159437", 0x7FFF, false, false); // <CENTER>~1_VAL~</CENTER>
                AddImageTiled(5, 35, 400, 52, 0xA40);
                AddHtmlLocalized(10, 40, 390, 52, 1159446, 0x7FFF, false, false); // *CAUTION* You are under the following EXCEPTIONAL ITEM buff. Do you want to remove it?
                AddImageTiled(5, 92, 123, 146, 0xA40);
                AddImageTiled(5, 238, 123, 146, 0xA40);
                AddImageTiled(128, 92, 143, 146, 0xA40);
                AddImageTiled(128, 238, 143, 146, 0xA40);
                AddImageTiled(276, 92, 128, 292, 0xA40);

                AddButton(281, 97, 0xFB7, 0xFB8, 1, GumpButtonType.Reply, 0);
                AddHtmlLocalized(316, 99, 88, 146, 1149968, 0x7FFF, false, false); // REMOVE

                AddHtmlLocalized(60, 97, 125, 18, 1114513, "#1159451", 0x6B55, false, false); // <DIV ALIGN=CENTER>~1_TOKEN~</DIV>

                for (int i = 0; i < _ResistOrder.Length; i++)
                {
                    AddHtmlLocalized(60, 115 + (18 * i), 125, 18, 1114514, string.Format("#{0}", 1061158 + (int)_ResistOrder[i]), 0x6B45, false, false); // <DIV ALIGN=RIGHT>~1_TOKEN~</DIV>
                    AddLabel(185, 115 + (18 * i), 0x9C2, string.Format("{0}", Entry.Exceptional[_ResistOrder[i]]));
                }

                AddHtmlLocalized(60, 223, 125, 18, 1114513, "#1159450", 0x6B55, false, false); // <DIV ALIGN=CENTER>~1_TOKEN~</DIV>

                for (int i = 0; i < _ResistOrder.Length; i++)
                {
                    AddHtmlLocalized(60, 241 + (18 * i), 125, 18, 1114514, string.Format("#{0}", 1061158 + (int)_ResistOrder[i]), 0x6B45, false, false); // <DIV ALIGN=RIGHT>~1_TOKEN~</DIV>
                    AddLabel(185, 241 + (18 * i), 0x9C2, string.Format("{0}", Entry.Runic[_ResistOrder[i]]));
                }
            }
            else
            {
                AddBackground(0, 0, 410, 470, 0x13BE);
                AddImageTiled(5, 5, 400, 25, 0xA40);
                AddHtmlLocalized(5, 8, 400, 25, 1113302, "#1159437", 0x7FFF, false, false); // <CENTER>~1_VAL~</CENTER>
                AddImageTiled(5, 35, 400, 125, 0xA40);
                AddHtmlLocalized(10, 40, 390, 125, 1159447, 0x7FFF, false, false); // The next time that you try to craft an exceptional piece of armor the bonus resists will be the values input below. The required materials to craft will be increased while this buff is active. One charge on this item will be consumed per use and only one instance of this buff can be active at any time.
                AddImageTiled(5, 165, 123, 150, 0xA40);
                AddImageTiled(5, 315, 123, 150, 0xA40);
                AddImageTiled(128, 165, 143, 150, 0xA40);
                AddImageTiled(128, 315, 143, 150, 0xA40);
                AddImageTiled(276, 165, 128, 300, 0xA40);

                AddButton(281, 170, 0xFB7, 0xFB8, 1, GumpButtonType.Reply, 0);
                AddHtmlLocalized(316, 172, 88, 150, 1149935, 0x7FFF, false, false); // OKAY

                AddButton(281, 320, 0xFB1, 0xFB3, 0, GumpButtonType.Reply, 0);
                AddHtmlLocalized(316, 322, 88, 150, 1060051, 0x7FFF, false, false); // CANCEL

                int armslorebonus = Siege.SiegeShard ? (int)Math.Floor(User.Skills[SkillName.ArmsLore].Base / 12.5) : (int)Math.Floor(User.Skills[SkillName.ArmsLore].Base / 20.0);

                AddHtmlLocalized(60, 170, 125, 18, 1114513, "#1159451", 0x6B55, false, false); // <DIV ALIGN=CENTER>~1_TOKEN~</DIV>
                AddHtmlLocalized(60, 188, 125, 18, 1152355, string.Format("{0}", 15 + armslorebonus), 0x6B55, false, false); // <CENTER>Total Must Equal ~1_VAL~</CENTER>
                
                for (int i = 0; i < _ResistOrder.Length; i++)
                {
                    AddHtmlLocalized(60, 206 + (18 * i), 125, 18, 1114514, string.Format("#{0}", 1061158 + (int)_ResistOrder[i]), 0x6B45, false, false); // <DIV ALIGN=RIGHT>~1_TOKEN~</DIV>
                    AddBackground(185, 206 + (18 * i), 30, 18, 0x2486);
                    AddTextEntry(187, 208 + (18 * i), 26, 14, 0x0, i, string.Format("{0}", Entry.Exceptional[_ResistOrder[i]]), 2);
                }

                AddHtmlLocalized(60, 314, 125, 18, 1114513, "#1159450", 0x6B55, false, false); // <DIV ALIGN=CENTER>~1_TOKEN~</DIV>
                AddHtmlLocalized(60, 332, 125, 18, 1152355, string.Format("{0}", 6 + armslorebonus), 0x6B55, false, false); // <CENTER>Total Must Equal ~1_VAL~</CENTER>
                
                for (int i = 0; i < _ResistOrder.Length; i++)
                {
                    AddHtmlLocalized(60, 350 + (18 * i), 125, 18, 1114514, string.Format("#{0}", 1061158 + (int)_ResistOrder[i]), 0x6B45, false, false); // <DIV ALIGN=RIGHT>~1_TOKEN~</DIV>
                    AddBackground(185, 350 + (18 * i), 30, 18, 0x2486);
                    AddTextEntry(187, 352 + (18 * i), 26, 14, 0x0, i + 5, string.Format("{0}", Entry.Runic[_ResistOrder[i]]), 2);
                }
            }
        }

        private static readonly ResistanceType[] _ResistOrder = new ResistanceType[] { ResistanceType.Physical, ResistanceType.Fire, ResistanceType.Cold, ResistanceType.Poison, ResistanceType.Energy };

        public override void OnResponse(RelayInfo info)
        {
            if (Addon == null || Addon.Deleted)
                return;

            int armslorebonus = (int)Math.Floor(User.Skills[SkillName.ArmsLore].Base / 20);
            int maxexp = 15 + armslorebonus;
            int maxrunic = 6 + armslorebonus;

            if (!Entry.Ready)
            {
                int index = 0;

                for (int i = 0; i < _ResistOrder.Length; i++)
                {
                    var textEntry = info.GetTextEntry(index);

                    if (textEntry != null)
                    {
                        var value = Utility.ToInt32(textEntry.Text);

                        if (value >= 0)
                        {
                            if (value > maxexp)
                            {
                                value = maxexp;
                            }

                            Entry.Exceptional[_ResistOrder[i]] = value;
                        }
                    }

                    index++;
                }

                for (int i = 0; i < _ResistOrder.Length; i++)
                {
                    var textEntry = info.GetTextEntry(index);

                    if (textEntry != null)
                    {
                        var value = Utility.ToInt32(textEntry.Text);

                        if (value >= 0)
                        {
                            if (value > maxexp)
                            {
                                value = maxexp;
                            }

                            Entry.Runic[_ResistOrder[i]] = value;
                        }
                    }

                    index++;
                }
            }

            if (info.ButtonID == 1)
            {
                if (Entry.Ready)
                {
                    if (Entry.Anvil == null || Entry.Anvil.Deleted)
                    {
                        User.SendLocalizedMessage(1159438); // A charge could not be refunded to the last Anvil of Artifacts tool you used, you must use this charge.
                    }
                    else if (!User.InRange(Entry.Anvil.GetWorldLocation(), 5))
                    {
                        User.SendLocalizedMessage(1159439); // A charge could not be refunded to the last Anvil of Artifacts tool you used because the item is not in range.
                    }
                    else
                    {
                        Entry.Anvil.UsesRemaining++;
                        Entry.Anvil = null;

                        User.SendLocalizedMessage(1159440); // A charged has been refunded to your Anvil of Artifacts tool.
                        User.SendLocalizedMessage(1159448); // The magical aura that surrounded you dissipates and you feel that your exceptional item resist chances have returned to normal.
                    }
                }
                else if (Addon.UsesRemaining > 0 && Entry.Exceptional.Sum(x => x.Value) == maxexp && Entry.Runic.Sum(x => x.Value) == maxrunic)
                {
                    Entry.Anvil = Addon;
                    Entry.Anvil.UsesRemaining--;

                    User.SendLocalizedMessage(1159449); // A magical aura surrounds you and you feel that your next attempt to craft an exceptional piece of armor will have favorable bonus resists.
                    Effects.SendPacket(User.Location, User.Map, new ParticleEffect(EffectType.FixedFrom, User.Serial, Server.Serial.Zero, 0x37C4, User.Location, User.Location, 1, 35, false, false, 1161, 0, 2, 0, 1, User.Serial, 209, 0));
                    User.PlaySound(580);
                }
                else
                {
                    Timer.DelayCall(TimeSpan.FromSeconds(0.2), () => Refresh());
                }
            }
        }
    }
}

