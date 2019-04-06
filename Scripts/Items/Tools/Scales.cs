using System;
using Server.Targeting;
using Server.Engines.Craft;

namespace Server.Items
{
    public class Scales : Item, IResource
    {
        private CraftResource _Resource;
        private Mobile _Crafter;
        private ItemQuality _Quality;

        [CommandProperty(AccessLevel.GameMaster)]
        public CraftResource Resource { get { return _Resource; } set { _Resource = value; _Resource = value; Hue = CraftResources.GetHue(_Resource); InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public Mobile Crafter { get { return _Crafter; } set { _Crafter = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public ItemQuality Quality { get { return _Quality; } set { _Quality = value; InvalidateProperties(); } }

        public bool PlayerConstructed { get { return true; } }

        [Constructable]
        public Scales()
            : base(0x1852)
        {
            Weight = 4.0;
        }

        public Scales(Serial serial)
            : base(serial)
        {
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            if (_Crafter != null)
            {
                list.Add(1050043, _Crafter.TitleName); // crafted by ~1_NAME~
            }

            if (_Quality == ItemQuality.Exceptional)
            {
                list.Add(1060636); // Exceptional
            }
        }

        public override void AddNameProperty(ObjectPropertyList list)
        {
            if (_Resource > CraftResource.Iron)
            {
                list.Add(1053099, "#{0}\t{1}", CraftResources.GetLocalizationNumber(_Resource), String.Format("#{0}", LabelNumber.ToString())); // ~1_oretype~ ~2_armortype~
            }
            else
            {
                base.AddNameProperty(list);
            }
        }

        public virtual int OnCraft(int quality, bool makersMark, Mobile from, CraftSystem craftSystem, Type typeRes, ITool tool, CraftItem craftItem, int resHue)
        {
            Quality = (ItemQuality)quality;

            if (makersMark)
                Crafter = from;

            if (!craftItem.ForceNonExceptional)
            {
                if (typeRes == null)
                    typeRes = craftItem.Resources.GetAt(0).ItemType;

                Resource = CraftResources.GetFromType(typeRes);
            }

            return quality;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)1); // version

            writer.Write((int)_Resource);
            writer.Write(_Crafter);
            writer.Write((int)_Quality);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch (version)
            {
                case 1:
                    _Resource = (CraftResource)reader.ReadInt();
                    _Crafter = reader.ReadMobile();
                    _Quality = (ItemQuality)reader.ReadInt();
                    break;
                case 0:
                    break;
            }
        }

        public override void OnDoubleClick(Mobile from)
        {
            from.SendLocalizedMessage(502431); // What would you like to weigh?
            from.Target = new InternalTarget(this);
        }

        private class InternalTarget : Target
        {
            private readonly Scales m_Item;
            public InternalTarget(Scales item)
                : base(1, false, TargetFlags.None)
            {
                m_Item = item;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                string message;

                if (targeted == m_Item)
                {
                    message = "It cannot weight itself.";
                }
                else if (targeted is Item)
                {
                    Item item = (Item)targeted;
                    object root = item.RootParent;

                    if ((root != null && root != from) || item.Parent == from)
                    {
                        message = "You decide that item's current location is too awkward to get an accurate result.";
                    }
                    else if (item.Movable)
                    {
                        if (item.Amount > 1)
                            message = "You place one item on the scale. ";
                        else
                            message = "You place that item on the scale. ";

                        double weight = item.Weight;

                        if (weight <= 0.0)
                            message += "It is lighter than a feather.";
                        else
                            message += String.Format("It weighs {0} stones.", weight);
                    }
                    else
                    {
                        message = "You cannot weigh that object.";
                    }
                }
                else
                {
                    message = "You cannot weigh that object.";
                }

                from.SendMessage(message);
            }
        }
    }
}