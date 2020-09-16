using Server.Engines.Craft;
using System;

namespace Server.Items
{
    public class BarrelLid : Item, IResource, IQuality
    {
        private CraftResource _Resource;
        private ItemQuality _Quality;

        [CommandProperty(AccessLevel.GameMaster)]
        public CraftResource Resource { get { return _Resource; } set { _Resource = value; Hue = CraftResources.GetHue(_Resource); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public ItemQuality Quality { get { return _Quality; } set { _Quality = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool PlayerConstructed => _Resource != CraftResource.None;

        [Constructable]
        public BarrelLid()
            : base(0x1DB8)
        {
            Weight = 2;
        }

        public override void AddCraftedProperties(ObjectPropertyList list)
        {
            if (_Quality == ItemQuality.Exceptional)
            {
                list.Add(1060636); // Exceptional
            }
        }

        public int OnCraft(int quality, bool makersMark, Mobile from, CraftSystem craftSystem, Type typeRes, ITool tool, CraftItem craftItem, int resHue)
        {
            Quality = (ItemQuality)quality;

            if (!craftItem.ForceNonExceptional)
            {
                if (typeRes == null)
                    typeRes = craftItem.Resources.GetAt(0).ItemType;

                Resource = CraftResources.GetFromType(typeRes);
            }

            return quality;
        }

        public BarrelLid(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(1);

            writer.Write((int)_Resource);
            writer.Write((int)_Quality);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch (version)
            {
                case 1:
                    {
                        _Resource = (CraftResource)reader.ReadInt();
                        _Quality = (ItemQuality)reader.ReadInt();
                        break;
                    }
                case 0: break;
            }
        }
    }

    [Flipable(0x1EB1, 0x1EB2, 0x1EB3, 0x1EB4)]
    public class BarrelStaves : Item, IResource, IQuality
    {
        private CraftResource _Resource;
        private ItemQuality _Quality;

        [CommandProperty(AccessLevel.GameMaster)]
        public CraftResource Resource { get { return _Resource; } set { _Resource = value; Hue = CraftResources.GetHue(_Resource); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public ItemQuality Quality { get { return _Quality; } set { _Quality = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool PlayerConstructed => _Resource != CraftResource.None;

        [Constructable]
        public BarrelStaves()
            : base(0x1EB1)
        {
            Weight = 1;
        }

        public override void AddCraftedProperties(ObjectPropertyList list)
        {
            if (_Quality == ItemQuality.Exceptional)
            {
                list.Add(1060636); // Exceptional
            }
        }

        public int OnCraft(int quality, bool makersMark, Mobile from, CraftSystem craftSystem, Type typeRes, ITool tool, CraftItem craftItem, int resHue)
        {
            Quality = (ItemQuality)quality;

            if (!craftItem.ForceNonExceptional)
            {
                if (typeRes == null)
                    typeRes = craftItem.Resources.GetAt(0).ItemType;

                Resource = CraftResources.GetFromType(typeRes);
            }

            return quality;
        }

        public BarrelStaves(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(1);

            writer.Write((int)_Resource);
            writer.Write((int)_Quality);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch (version)
            {
                case 1:
                    {
                        _Resource = (CraftResource)reader.ReadInt();
                        _Quality = (ItemQuality)reader.ReadInt();
                        break;
                    }
                case 0: break;
            }
        }
    }

    public class BarrelHoops : Item, IResource, IQuality
    {
        private CraftResource _Resource;
        private ItemQuality _Quality;

        [CommandProperty(AccessLevel.GameMaster)]
        public CraftResource Resource { get { return _Resource; } set { _Resource = value; Hue = CraftResources.GetHue(_Resource); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public ItemQuality Quality { get { return _Quality; } set { _Quality = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool PlayerConstructed => _Resource != CraftResource.None;

        [Constructable]
        public BarrelHoops()
            : base(0x1DB7)
        {
            Weight = 5;
        }

        public override void AddCraftedProperties(ObjectPropertyList list)
        {
            if (_Quality == ItemQuality.Exceptional)
            {
                list.Add(1060636); // Exceptional
            }
        }

        public int OnCraft(int quality, bool makersMark, Mobile from, CraftSystem craftSystem, Type typeRes, ITool tool, CraftItem craftItem, int resHue)
        {
            Quality = (ItemQuality)quality;

            if (!craftItem.ForceNonExceptional)
            {
                if (typeRes == null)
                    typeRes = craftItem.Resources.GetAt(0).ItemType;

                Resource = CraftResources.GetFromType(typeRes);
            }

            return quality;
        }

        public BarrelHoops(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1011228;// Barrel hoops
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(2);

            writer.Write((int)_Resource);
            writer.Write((int)_Quality);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch (version)
            {
                case 2:
                    {
                        _Resource = (CraftResource)reader.ReadInt();
                        goto case 1;
                    }
                case 1:
                    {
                        _Quality = (ItemQuality)reader.ReadInt();
                        break;
                    }
                case 0: break;
            }
        }
    }

    public class BarrelTap : Item, IResource, IQuality
    {
        private CraftResource _Resource;
        private ItemQuality _Quality;

        [CommandProperty(AccessLevel.GameMaster)]
        public CraftResource Resource { get { return _Resource; } set { _Resource = value; Hue = CraftResources.GetHue(_Resource); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public ItemQuality Quality { get { return _Quality; } set { _Quality = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool PlayerConstructed => _Resource != CraftResource.None;

        [Constructable]
        public BarrelTap()
            : base(0x1004)
        {
            Weight = 1;
        }

        public override void AddCraftedProperties(ObjectPropertyList list)
        {
            if (_Quality == ItemQuality.Exceptional)
            {
                list.Add(1060636); // Exceptional
            }
        }

        public int OnCraft(int quality, bool makersMark, Mobile from, CraftSystem craftSystem, Type typeRes, ITool tool, CraftItem craftItem, int resHue)
        {
            Quality = (ItemQuality)quality;

            if (!craftItem.ForceNonExceptional)
            {
                if (typeRes == null)
                    typeRes = craftItem.Resources.GetAt(0).ItemType;

                Resource = CraftResources.GetFromType(typeRes);
            }

            return quality;
        }

        public BarrelTap(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(2);

            writer.Write((int)_Resource);
            writer.Write((int)_Quality);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch (version)
            {
                case 2:
                    {
                        _Resource = (CraftResource)reader.ReadInt();
                        goto case 1;
                    }
                case 1:
                    {
                        _Quality = (ItemQuality)reader.ReadInt();
                        break;
                    }
                case 0: break;
            }
        }
    }
}
