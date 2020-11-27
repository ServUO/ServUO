using Server.Engines.Craft;
using Server.Network;
using System;
using System.Collections.Generic;

namespace Server.Items
{
    #region Reward Clothing
    public class ZooMemberBonnet : Bonnet
    {
        public override int LabelNumber => 1073221; // Britannia Royal Zoo Member

        [Constructable]
        public ZooMemberBonnet()
            : this(0)
        {
        }

        [Constructable]
        public ZooMemberBonnet(int hue)
            : base(hue)
        {
        }

        public ZooMemberBonnet(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class ZooMemberFloppyHat : FloppyHat
    {
        public override int LabelNumber => 1073221; // Britannia Royal Zoo Member

        [Constructable]
        public ZooMemberFloppyHat()
            : this(0)
        {
        }

        [Constructable]
        public ZooMemberFloppyHat(int hue)
            : base(hue)
        {
        }

        public ZooMemberFloppyHat(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class LibraryFriendFeatheredHat : FeatheredHat
    {
        public override int LabelNumber => 1073347; // Friends of the Library Feathered Hat

        [Constructable]
        public LibraryFriendFeatheredHat()
            : this(0)
        {
        }

        [Constructable]
        public LibraryFriendFeatheredHat(int hue)
            : base(hue)
        {
        }

        public LibraryFriendFeatheredHat(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class JesterHatOfChuckles : JesterHat
    {
        public override int LabelNumber => 1073256;// Jester Hat of Chuckles - Museum of Vesper Replica

        public override int BasePhysicalResistance => 12;
        public override int BaseFireResistance => 12;
        public override int BaseColdResistance => 12;
        public override int BasePoisonResistance => 12;
        public override int BaseEnergyResistance => 12;

        public override int InitMinHits => 100;
        public override int InitMaxHits => 100;

        [Constructable]
        public JesterHatOfChuckles()
            : this(0)
        {
        }

        [Constructable]
        public JesterHatOfChuckles(int hue)
            : base(hue)
        {
            Attributes.Luck = 150;
        }

        public JesterHatOfChuckles(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

    public class NystulsWizardsHat : WizardsHat
    {
        public override int LabelNumber => 1073255;// Nystul's Wizard's Hat - Museum of Vesper Replica

        public override int BasePhysicalResistance => 10;
        public override int BaseFireResistance => 10;
        public override int BaseColdResistance => 10;
        public override int BasePoisonResistance => 10;
        public override int BaseEnergyResistance => 25;

        public override int InitMinHits => 100;
        public override int InitMaxHits => 100;

        [Constructable]
        public NystulsWizardsHat()
            : this(0)
        {
        }

        [Constructable]
        public NystulsWizardsHat(int hue)
            : base(hue)
        {
            Attributes.LowerManaCost = 15;
        }

        public NystulsWizardsHat(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

    public class GypsyHeaddress : SkullCap
    {
        public override int LabelNumber => 1073254;// Gypsy Headdress - Museum of Vesper Replica

        public override int BasePhysicalResistance => 15;
        public override int BaseFireResistance => 20;
        public override int BaseColdResistance => 20;
        public override int BasePoisonResistance => 15;
        public override int BaseEnergyResistance => 15;

        public override int InitMinHits => 100;
        public override int InitMaxHits => 100;

        [Constructable]
        public GypsyHeaddress()
            : this(0)
        {
        }

        [Constructable]
        public GypsyHeaddress(int hue)
            : base(hue)
        {
        }

        public GypsyHeaddress(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
    #endregion

    public abstract class BaseHat : BaseClothing, IShipwreckedItem
    {
        private bool m_IsShipwreckedItem;

        [CommandProperty(AccessLevel.GameMaster)]
        public bool IsShipwreckedItem
        {
            get
            {
                return m_IsShipwreckedItem;
            }
            set
            {
                m_IsShipwreckedItem = value;
            }
        }

        public BaseHat(int itemID)
            : this(itemID, 0)
        {
        }

        public BaseHat(int itemID, int hue)
            : base(itemID, Layer.Helm, hue)
        {
        }

        public BaseHat(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(2); // version

            writer.Write(m_IsShipwreckedItem);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            switch (version)
            {
                case 2: goto case 1;
                case 1:
                    {
                        m_IsShipwreckedItem = reader.ReadBool();
                        break;
                    }
            }

            if (version == 1)
            {
                Weight = -1;
            }
        }

        public override void AddEquipInfoAttributes(Mobile from, List<EquipInfoAttribute> attrs)
        {
            base.AddEquipInfoAttributes(from, attrs);

            if (m_IsShipwreckedItem)
                attrs.Add(new EquipInfoAttribute(1041645));	// recovered from a shipwreck
        }

        public override void AddNameProperties(ObjectPropertyList list)
        {
            base.AddNameProperties(list);

            if (m_IsShipwreckedItem)
                list.Add(1041645); // recovered from a shipwreck
        }

        public override int OnCraft(int quality, bool makersMark, Mobile from, CraftSystem craftSystem, Type typeRes, ITool tool, CraftItem craftItem, int resHue)
        {
            Quality = (ItemQuality)quality;

            if (Quality == ItemQuality.Exceptional)
            {
                DistributeBonuses(from, tool is BaseRunicTool ? 6 : 15);
            }

            return base.OnCraft(quality, makersMark, from, craftSystem, typeRes, tool, craftItem, resHue);
        }
    }

    [Flipable(0x2798, 0x27E3)]
    public class Kasa : BaseHat
    {
        public override int BasePhysicalResistance => 0;
        public override int BaseFireResistance => 5;
        public override int BaseColdResistance => 9;
        public override int BasePoisonResistance => 5;
        public override int BaseEnergyResistance => 5;

        public override int InitMinHits => 20;
        public override int InitMaxHits => 30;

        [Constructable]
        public Kasa()
            : this(0)
        {
        }

        [Constructable]
        public Kasa(int hue)
            : base(0x2798, hue)
        {
        }

        public Kasa(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();
        }
    }

    [Flipable(0x278F, 0x27DA)]
    public class ClothNinjaHood : BaseHat
    {
        public override int BasePhysicalResistance => 3;
        public override int BaseFireResistance => 3;
        public override int BaseColdResistance => 6;
        public override int BasePoisonResistance => 9;
        public override int BaseEnergyResistance => 9;

        public override int InitMinHits => 20;
        public override int InitMaxHits => 30;

        [Constructable]
        public ClothNinjaHood()
            : this(0)
        {
        }

        [Constructable]
        public ClothNinjaHood(int hue)
            : base(0x278F, hue)
        {
        }

        public ClothNinjaHood(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();
        }
    }

    [Flipable(0x2306, 0x2305)]
    public class FlowerGarland : BaseHat
    {
        public override int BasePhysicalResistance => 3;
        public override int BaseFireResistance => 3;
        public override int BaseColdResistance => 6;
        public override int BasePoisonResistance => 9;
        public override int BaseEnergyResistance => 9;

        public override int InitMinHits => 20;
        public override int InitMaxHits => 30;

        [Constructable]
        public FlowerGarland()
            : this(0)
        {
        }

        [Constructable]
        public FlowerGarland(int hue)
            : base(0x2306, hue)
        {

        }

        public FlowerGarland(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();
        }
    }

    public class FloppyHat : BaseHat
    {
        public override int BasePhysicalResistance => 0;
        public override int BaseFireResistance => 5;
        public override int BaseColdResistance => 9;
        public override int BasePoisonResistance => 5;
        public override int BaseEnergyResistance => 5;

        public override int InitMinHits => 20;
        public override int InitMaxHits => 30;

        [Constructable]
        public FloppyHat()
            : this(0)
        {
        }

        [Constructable]
        public FloppyHat(int hue)
            : base(0x1713, hue)
        {

        }

        public FloppyHat(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();
        }
    }

    public class WideBrimHat : BaseHat
    {
        public override int BasePhysicalResistance => 0;
        public override int BaseFireResistance => 5;
        public override int BaseColdResistance => 9;
        public override int BasePoisonResistance => 5;
        public override int BaseEnergyResistance => 5;

        public override int InitMinHits => 20;
        public override int InitMaxHits => 30;

        [Constructable]
        public WideBrimHat()
            : this(0)
        {
        }

        [Constructable]
        public WideBrimHat(int hue)
            : base(0x1714, hue)
        {

        }

        public WideBrimHat(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();
        }
    }

    public class Cap : BaseHat
    {
        public override int BasePhysicalResistance => 0;
        public override int BaseFireResistance => 5;
        public override int BaseColdResistance => 9;
        public override int BasePoisonResistance => 5;
        public override int BaseEnergyResistance => 5;

        public override int InitMinHits => 20;
        public override int InitMaxHits => 30;

        [Constructable]
        public Cap()
            : this(0)
        {
        }

        [Constructable]
        public Cap(int hue)
            : base(0x1715, hue)
        {

        }

        public Cap(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();
        }
    }

    public class SkullCap : BaseHat
    {
        public override int BasePhysicalResistance => 0;
        public override int BaseFireResistance => 3;
        public override int BaseColdResistance => 5;
        public override int BasePoisonResistance => 8;
        public override int BaseEnergyResistance => 8;

        public override int InitMinHits => 14;
        public override int InitMaxHits => 28;

        [Constructable]
        public SkullCap()
            : this(0)
        {
        }

        [Constructable]
        public SkullCap(int hue)
            : base(0x1544, hue)
        {

        }

        public SkullCap(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();
        }
    }

    public class Bandana : BaseHat
    {
        public override int BasePhysicalResistance => 0;
        public override int BaseFireResistance => 3;
        public override int BaseColdResistance => 5;
        public override int BasePoisonResistance => 8;
        public override int BaseEnergyResistance => 8;

        public override int InitMinHits => 20;
        public override int InitMaxHits => 30;

        [Constructable]
        public Bandana()
            : this(0)
        {
        }

        [Constructable]
        public Bandana(int hue)
            : base(0x1540, hue)
        {

        }

        public Bandana(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();
        }
    }

    public class BearMask : BaseHat, IRepairable
    {
        public CraftSystem RepairSystem => DefTailoring.CraftSystem;

        public override int BasePhysicalResistance => 5;
        public override int BaseFireResistance => 3;
        public override int BaseColdResistance => 8;
        public override int BasePoisonResistance => 4;
        public override int BaseEnergyResistance => 4;

        public override int InitMinHits => 20;
        public override int InitMaxHits => 30;

        [Constructable]
        public BearMask()
            : this(0)
        {
        }

        [Constructable]
        public BearMask(int hue)
            : base(0x1545, hue)
        {
        }

        public override bool Dye(Mobile from, DyeTub sender)
        {
            from.SendLocalizedMessage(sender.FailMessage);
            return false;
        }

        public BearMask(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();
        }
    }

    public class DeerMask : BaseHat, IRepairable
    {
        public CraftSystem RepairSystem => DefTailoring.CraftSystem;

        public override int BasePhysicalResistance => 2;
        public override int BaseFireResistance => 6;
        public override int BaseColdResistance => 8;
        public override int BasePoisonResistance => 1;
        public override int BaseEnergyResistance => 7;

        public override int InitMinHits => 20;
        public override int InitMaxHits => 30;

        [Constructable]
        public DeerMask()
            : this(0)
        {
        }

        [Constructable]
        public DeerMask(int hue)
            : base(0x1547, hue)
        {
        }

        public override bool Dye(Mobile from, DyeTub sender)
        {
            from.SendLocalizedMessage(sender.FailMessage);
            return false;
        }

        public DeerMask(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();
        }
    }

    public class HornedTribalMask : BaseHat, IRepairable
    {
        public CraftSystem RepairSystem => DefTailoring.CraftSystem;

        public override int BasePhysicalResistance => 6;
        public override int BaseFireResistance => 9;
        public override int BaseColdResistance => 0;
        public override int BasePoisonResistance => 4;
        public override int BaseEnergyResistance => 5;

        public override int InitMinHits => 20;
        public override int InitMaxHits => 30;

        [Constructable]
        public HornedTribalMask()
            : this(0)
        {
        }

        [Constructable]
        public HornedTribalMask(int hue)
            : base(0x1549, hue)
        {
        }

        public override bool Dye(Mobile from, DyeTub sender)
        {
            from.SendLocalizedMessage(sender.FailMessage);
            return false;
        }

        public HornedTribalMask(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();
        }
    }

    public class TribalMask : BaseHat, IRepairable
    {
        public CraftSystem RepairSystem => DefTailoring.CraftSystem;

        public override int BasePhysicalResistance => 3;
        public override int BaseFireResistance => 0;
        public override int BaseColdResistance => 6;
        public override int BasePoisonResistance => 10;
        public override int BaseEnergyResistance => 5;

        public override int InitMinHits => 20;
        public override int InitMaxHits => 30;

        [Constructable]
        public TribalMask()
            : this(0)
        {
        }

        [Constructable]
        public TribalMask(int hue)
            : base(0x154B, hue)
        {

        }

        public override bool Dye(Mobile from, DyeTub sender)
        {
            from.SendLocalizedMessage(sender.FailMessage);
            return false;
        }

        public TribalMask(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();
        }
    }

    public class TallStrawHat : BaseHat
    {
        public override int BasePhysicalResistance => 0;
        public override int BaseFireResistance => 5;
        public override int BaseColdResistance => 9;
        public override int BasePoisonResistance => 5;
        public override int BaseEnergyResistance => 5;

        public override int InitMinHits => 20;
        public override int InitMaxHits => 30;

        [Constructable]
        public TallStrawHat()
            : this(0)
        {
        }

        [Constructable]
        public TallStrawHat(int hue)
            : base(0x1716, hue)
        {

        }

        public TallStrawHat(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();
        }
    }

    public class StrawHat : BaseHat
    {
        public override int BasePhysicalResistance => 0;
        public override int BaseFireResistance => 5;
        public override int BaseColdResistance => 9;
        public override int BasePoisonResistance => 5;
        public override int BaseEnergyResistance => 5;

        public override int InitMinHits => 20;
        public override int InitMaxHits => 30;

        [Constructable]
        public StrawHat()
            : this(0)
        {
        }

        [Constructable]
        public StrawHat(int hue)
            : base(0x1717, hue)
        {

        }

        public StrawHat(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();
        }
    }

    public class OrcishKinMask : BaseHat
    {
        public override int BasePhysicalResistance => 1;
        public override int BaseFireResistance => 1;
        public override int BaseColdResistance => 7;
        public override int BasePoisonResistance => 7;
        public override int BaseEnergyResistance => 8;

        public override int InitMinHits => 20;
        public override int InitMaxHits => 30;

        public override bool Dye(Mobile from, DyeTub sender)
        {
            from.SendLocalizedMessage(sender.FailMessage);
            return false;
        }

        public override string DefaultName => "a mask of orcish kin";

        [Constructable]
        public OrcishKinMask()
            : this(0x8A4)
        {
        }

        [Constructable]
        public OrcishKinMask(int hue)
            : base(0x141B, hue)
        {
        }

        public override bool CanEquip(Mobile m)
        {
            if (!base.CanEquip(m))
                return false;

            if (m.BodyMod == 183 || m.BodyMod == 184)
            {
                m.SendLocalizedMessage(1061629); // You can't do that while wearing savage kin paint.
                return false;
            }

            return true;
        }

        public override void OnAdded(object parent)
        {
            base.OnAdded(parent);

            if (parent is Mobile)
                Misc.Titles.AwardKarma((Mobile)parent, -20, true);
        }

        public OrcishKinMask(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();
        }
    }

    public class OrcMask : BaseHat, IRepairable
    {
        public CraftSystem RepairSystem => DefTailoring.CraftSystem;

        public override int BasePhysicalResistance => 1;
        public override int BaseFireResistance => 1;
        public override int BaseColdResistance => 7;
        public override int BasePoisonResistance => 7;
        public override int BaseEnergyResistance => 8;

        public override int InitMinHits => 20;
        public override int InitMaxHits => 30;

        public override bool Dye(Mobile from, DyeTub sender)
        {
            from.SendLocalizedMessage(sender.FailMessage);
            return false;
        }

        public override int LabelNumber => 1025147; // orc mask

        [Constructable]
        public OrcMask()
            : base(0x141B)
        {
        }

        public OrcMask(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();
        }
    }

    public class SavageMask : BaseHat
    {
        public override int BasePhysicalResistance => 3;
        public override int BaseFireResistance => 0;
        public override int BaseColdResistance => 6;
        public override int BasePoisonResistance => 10;
        public override int BaseEnergyResistance => 5;

        public override int InitMinHits => 20;
        public override int InitMaxHits => 30;

        public static int GetRandomHue()
        {
            int v = Utility.RandomBirdHue();

            if (v == 2101)
                v = 0;

            return v;
        }

        public override bool Dye(Mobile from, DyeTub sender)
        {
            from.SendLocalizedMessage(sender.FailMessage);
            return false;
        }

        [Constructable]
        public SavageMask()
            : this(GetRandomHue())
        {
        }

        [Constructable]
        public SavageMask(int hue)
            : base(0x154B, hue)
        {
        }

        public SavageMask(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();
        }
    }

    public class WizardsHat : BaseHat
    {
        public override int BasePhysicalResistance => 0;
        public override int BaseFireResistance => 5;
        public override int BaseColdResistance => 9;
        public override int BasePoisonResistance => 5;
        public override int BaseEnergyResistance => 5;

        public override int InitMinHits => 20;
        public override int InitMaxHits => 30;

        [Constructable]
        public WizardsHat()
            : this(0)
        {
        }

        [Constructable]
        public WizardsHat(int hue)
            : base(0x1718, hue)
        {
        }

        public WizardsHat(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();
        }
    }

    public class MagicWizardsHat : BaseHat
    {
        public override int BasePhysicalResistance => 0;
        public override int BaseFireResistance => 5;
        public override int BaseColdResistance => 9;
        public override int BasePoisonResistance => 5;
        public override int BaseEnergyResistance => 5;

        public override int InitMinHits => 20;
        public override int InitMaxHits => 30;

        public override int LabelNumber => 1041072;// a magical wizard's hat

        public override int BaseStrBonus => -5;
        public override int BaseDexBonus => -5;
        public override int BaseIntBonus => +5;

        [Constructable]
        public MagicWizardsHat()
            : this(0)
        {
        }

        [Constructable]
        public MagicWizardsHat(int hue)
            : base(0x1718, hue)
        {
        }

        public MagicWizardsHat(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();
        }
    }

    public class Bonnet : BaseHat
    {
        public override int BasePhysicalResistance => 0;
        public override int BaseFireResistance => 5;
        public override int BaseColdResistance => 9;
        public override int BasePoisonResistance => 5;
        public override int BaseEnergyResistance => 5;

        public override int InitMinHits => 20;
        public override int InitMaxHits => 30;

        [Constructable]
        public Bonnet()
            : this(0)
        {
        }

        [Constructable]
        public Bonnet(int hue)
            : base(0x1719, hue)
        {
        }

        public Bonnet(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();
        }
    }

    public class FeatheredHat : BaseHat
    {
        public override int BasePhysicalResistance => 0;
        public override int BaseFireResistance => 5;
        public override int BaseColdResistance => 9;
        public override int BasePoisonResistance => 5;
        public override int BaseEnergyResistance => 5;

        public override int InitMinHits => 20;
        public override int InitMaxHits => 30;

        [Constructable]
        public FeatheredHat()
            : this(0)
        {
        }

        [Constructable]
        public FeatheredHat(int hue)
            : base(0x171A, hue)
        {
        }

        public FeatheredHat(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();
        }
    }

    public class TricorneHat : BaseHat
    {
        public override int BasePhysicalResistance => 0;
        public override int BaseFireResistance => 5;
        public override int BaseColdResistance => 9;
        public override int BasePoisonResistance => 5;
        public override int BaseEnergyResistance => 5;

        public override int InitMinHits => 20;
        public override int InitMaxHits => 30;

        [Constructable]
        public TricorneHat()
            : this(0)
        {
        }

        [Constructable]
        public TricorneHat(int hue)
            : base(0x171B, hue)
        {
        }

        public TricorneHat(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();
        }
    }

    public class JesterHat : BaseHat
    {
        public override int BasePhysicalResistance => 0;
        public override int BaseFireResistance => 5;
        public override int BaseColdResistance => 9;
        public override int BasePoisonResistance => 5;
        public override int BaseEnergyResistance => 5;

        public override int InitMinHits => 20;
        public override int InitMaxHits => 30;

        [Constructable]
        public JesterHat()
            : this(0)
        {
        }

        [Constructable]
        public JesterHat(int hue)
            : base(0x171C, hue)
        {
        }

        public JesterHat(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();
        }
    }

    public class ChefsToque : BaseHat
    {
        public override int LabelNumber => 1109618;  // Chef's Toque

        public override int BasePhysicalResistance => 0;
        public override int BaseFireResistance => 5;
        public override int BaseColdResistance => 9;
        public override int BasePoisonResistance => 5;
        public override int BaseEnergyResistance => 5;

        public override int InitMinHits => 20;
        public override int InitMaxHits => 30;

        [Constructable]
        public ChefsToque()
            : this(0)
        {
        }

        [Constructable]
        public ChefsToque(int hue)
            : base(0x781A, hue)
        {
        }

        public ChefsToque(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();
        }
    }    
}
