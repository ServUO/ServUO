using System;
using Server.Gumps;
using Server.Network;

namespace Server.Items
{
    public abstract class RoastingPigAddon : BaseAddon
    {
        public DateTime lastused;

        public RoastingPigAddon(DateTime m_lastused)
            : base()
        {
            lastused = m_lastused;
        }

        public RoastingPigAddon(Serial serial)
            : base(serial)
        {
        }

        public override abstract BaseAddonDeed Deed { get; }

        public override void OnComponentUsed(AddonComponent c, Mobile from)
        {
            if ((from.InRange(c.Location, 3)) && (c.LabelNumber == 1123329))
            {
                DateTime t1 = DateTime.UtcNow;
                TimeSpan result = t1 - lastused;
                int days = result.Days;

                if (days > 0)
                {
                    lastused = t1;
                    double random = Utility.RandomDouble();
                    if (0.6 >= random)
                        from.AddToBackpack(new PorkPlatter());
                    else
                        from.AddToBackpack(new PorkSandwich());
                    from.LocalOverheadMessage(Network.MessageType.Regular, 0x35, 1154556); //1154556
                }
                else
                {
                    from.SendLocalizedMessage(1154555); //The pig is not quite ready for eating yet.
                }
            }
            else
                from.LocalOverheadMessage(MessageType.Regular, 0x3B2, 1019045); // I can't reach that.
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            
            writer.Write((int)0);
            
            writer.Write(lastused);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            
            int version = reader.ReadInt();
            
            lastused = reader.ReadDateTime();
        }
    }

    public class RoastingPigOnASpit : RoastingPigAddon
    {
        
        [Constructable]
        public RoastingPigOnASpit(bool east, DateTime lastused)
            : base(lastused)
        {
            if (east) // east
            {
                this.AddComponent(new LocalizedAddonComponent(0x9989, 1123329), 0, 0, 0); //Roasting Pig on a Spit
                this.AddComponent(new LocalizedAddonComponent(0x9988, 1123328), 1, -1, 0);
            }
            else // south
            {
                this.AddComponent(new LocalizedAddonComponent(0x9995, 1123329), 0, 0, 0);
                this.AddComponent(new LocalizedAddonComponent(0x9994, 1123328), -1, 1, 0);
            }
        }

        public RoastingPigOnASpit(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddonDeed Deed
        {
            get
            {
                return new RoastingPigDeed(lastused.ToString("MM/dd/yyyy HH:MM:ss"));
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            
            writer.Write((int)0);
            
            writer.Write(lastused);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            
            int version = reader.ReadInt();
            
            lastused = reader.ReadDateTime();
        }
    }
  
    public class RoastingPigDeed : BaseAddonDeed
    {
        private bool m_East;
        private DateTime m_lastused;

        [Constructable]
        public RoastingPigDeed(String lastused)
            : base()
        {
            this.LootType = LootType.Blessed;
            this.m_lastused = DateTime.Parse(lastused);
        }

        public RoastingPigDeed(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddon Addon
        {
            get
            {
                return new RoastingPigOnASpit(this.m_East, this.m_lastused);
            }
        }
        public override int LabelNumber
        {
            get
            {
                return 1154557;
            }
        }// Deed for a Roasting Pig on a Spit
        public override void OnDoubleClick(Mobile from)
        {
            if (this.IsChildOf(from.Backpack))
            {
                from.CloseGump(typeof(InternalGump));
                from.SendGump(new InternalGump(this));
            }
            else
                from.SendLocalizedMessage(1062334); // This item must be in your backpack to be used.
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0);

            writer.Write(m_lastused);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            m_lastused = reader.ReadDateTime();
        }

        private void SendTarget(Mobile m)
        {
            base.OnDoubleClick(m);
        }

        private class InternalGump : Gump
        {
            private readonly RoastingPigDeed m_Deed;
            public InternalGump(RoastingPigDeed deed)
                : base(60, 36)
            {
                this.m_Deed = deed;

                this.AddPage(0);

                this.AddBackground(0, 0, 200, 160, 0x13BE);
                this.AddAlphaRegion(10, 10, 180, 140);

                this.AddImageTiled(10, 10, 180, 20, 0xA40);
                this.AddImageTiled(10, 40, 180, 110, 0xA40);
                this.AddImageTiled(10, 130, 180, 20, 0xA40);
                
                this.AddButton(20, 120, 0xFB1, 0xFB2, 0, GumpButtonType.Reply, 0);
                this.AddHtmlLocalized(55, 120, 125, 20, 1060051, 0x7FFF, false, false); // CANCEL
                this.AddHtmlLocalized(14, 12, 200, 20, 1154194, 0x7FFF, false, false); // Choose a Facing:
                
                this.AddPage(1);

                this.AddButton(19, 49, 0x845, 0x846, 1, GumpButtonType.Reply, 0);
                this.AddHtmlLocalized(44, 47, 213, 20, 1075386, 0x7FFF, false, false); // South
                this.AddButton(19, 73, 0x845, 0x846, 2, GumpButtonType.Reply, 0);
                this.AddHtmlLocalized(44, 71, 213, 20, 1075387, 0x7FFF, false, false); // East
            }

            public override void OnResponse(NetState sender, RelayInfo info)
            {
                if (this.m_Deed == null || this.m_Deed.Deleted || info.ButtonID == 0)
                    return;

                this.m_Deed.m_East = (info.ButtonID != 1);
                this.m_Deed.SendTarget(sender.Mobile);
            }
        }
    }
}