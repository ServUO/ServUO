using Server.ContextMenus;
using Server.Gumps;
using Server.Mobiles;
using Server.Network;
using System;
using System.Collections.Generic;

namespace Server.Items
{
    public enum FruitType
    {
        None,
        Grape,
        Apple,
        Peach,
        Pear,
        Plum
    }

    [Flipable(0x9E36, 0x9E37)]
    public class FermentationBarrel : BaseContainer
    {
        public override int LabelNumber => 1124526;  // Fermentation Barrel

        public override int DefaultGumpID => 0x3E;

        public readonly int MinFruit = 4;

        [CommandProperty(AccessLevel.GameMaster)]
        public FruitType FruitType { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool Fermenting { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime FermentingEnds { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool Fermented { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public int BottlesRemaining { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public string Vintage { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public string Year { get; set; }

        public override bool DisplaysContent => false;

        [Constructable]
        public FermentationBarrel()
            : base(0x9E36)
        {
            MaxItems = 400;
        }

        public override void Open(Mobile from)
        {
            if (!Fermenting)
            {
                DisplayTo(from);
            }
            else if (FermentingEnds > DateTime.UtcNow)
            {
                from.SendLocalizedMessage(1157230); // You may not open the barrel once fermentation has begun!                
            }
            else if (Fermented)
            {
                if (BottlesRemaining == -1)
                {
                    from.PrivateOverheadMessage(MessageType.Regular, 0x22, 1157258, from.NetState); // *You gently taste the fermentation...it's spoiled! You should probably empty the barrel*
                }
                else if (BottlesRemaining > 0)
                {
                    var bottle = new BottleOfWine(FruitType, Vintage, Year);

                    if (from.Backpack != null && from.Backpack.TryDropItem(from, bottle, false))
                    {
                        from.PlaySound(0x240);

                        BottlesRemaining--;

                        InvalidateProperties();
                    }
                    else
                    {
                        from.SendLocalizedMessage(500720); // You don't have enough room in your backpack!
                        bottle.Delete();
                    }
                }
                else
                {
                    from.SendLocalizedMessage(1042975); // It's empty.
                }
            }
        }

        private Timer m_Timer;

        public void StartTimer()
        {
            if (m_Timer != null && m_Timer.Running)
                return;

            m_Timer = Timer.DelayCall(TimeSpan.FromMinutes(10), TimeSpan.FromMinutes(10), CheckFermente);
            m_Timer.Start();
        }

        public void StopTimer()
        {
            if (m_Timer != null)
                m_Timer.Stop();

            m_Timer = null;
        }

        public void CheckFermente()
        {
            if (!Fermented && FermentingEnds < DateTime.UtcNow)
            {
                Fermented = true;

                InvalidateProperties();

                StopTimer();
            }
        }

        public override void OnDelete()
        {
            StopTimer();

            base.OnDelete();
        }

        public override bool OnDragDrop(Mobile from, Item dropped)
        {
            if (!CheckDropItem(from, dropped))
            {
                return false;
            }

            return base.OnDragDrop(from, dropped);
        }

        public override bool OnDragDropInto(Mobile from, Item item, Point3D p)
        {
            if (!CheckDropItem(from, item))
            {
                return false;
            }

            return base.OnDragDropInto(from, item, p);
        }

        public bool CheckDropItem(Mobile from, Item dropped)
        {
            if (Fermented)
            {
                from.SendLocalizedMessage(1157245); // The fermentation barrel is not empty. Empty it first to add fruit and yeast.
            }
            else if (Fermenting)
            {
                from.SendLocalizedMessage(1157244); // You may not add anything to the fermentation barrel after fermentation has begun.
            }
            else if (dropped is Yeast)
            {
                if (HasYeast())
                {
                    from.SendLocalizedMessage(1157256); // You have already added yeast to the barrel.
                }
                else
                {
                    dropped.Movable = false;
                    return true;
                }
            }
            else
            {
                var type = GetFruitType(dropped.GetType());

                if (type == FruitType.None)
                {
                    from.SendLocalizedMessage(1157246); // You may only put fruit and yeast in the fermentation barrel.
                }
                else if (FruitType != FruitType.None && FruitType != type)
                {
                    from.SendLocalizedMessage(1157243); // You may only put one type of fruit in the fermentation barrel at one time. Empty the barrel first.
                }
                else if (dropped.Amount >= MaxItems)
                {
                    from.SendLocalizedMessage(1157285); // The barrel cannot hold anymore fruit.
                }
                else
                {
                    FruitType = type;
                    dropped.Movable = false;
                    return true;
                }
            }

            return false;
        }

        private bool HasFruitAndYeast()
        {
            return HasFruit() && HasYeast();
        }

        private bool HasFruit()
        {
            return FruitAmount() > MinFruit;
        }

        private int FruitAmount()
        {
            Type fruittype = null;
            var items = Items;

            if (items.Count > 0)
            {
                for (var i = items.Count - 1; i >= 0; --i)
                {
                    if (i >= items.Count)
                    {
                        continue;
                    }

                    if (GetFruitType(items[i].GetType()) != FruitType.None)
                    {
                        fruittype = items[i].GetType();
                    }
                }
            }

            return fruittype != null ? GetAmount(fruittype, true) : 0;
        }

        private bool HasYeast()
        {
            return GetYeast() != null;
        }

        private Yeast GetYeast()
        {
            return FindItemByType<Yeast>();
        }

        private FruitType GetFruitType(Type t)
        {
            for (int i = 0; i < _FruitTypes.Length; i++)
            {
                foreach (Type type in _FruitTypes[i])
                {
                    if (type == t)
                        return (FruitType)i + 1;
                }
            }

            return FruitType.None;
        }

        private readonly Type[][] _FruitTypes =
        {
            new Type[] { typeof(GrapeBunch), typeof(Grapes) },
            new Type[] { typeof(Apple) },
            new Type[] { typeof(Peach) },
            new Type[] { typeof(Pear) },
            new Type[] { typeof(Plum) }
        };

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            if (!string.IsNullOrEmpty(Vintage))
            {
                list.Add(1157254, Vintage); // <BASEFONT COLOR=#0099cc>~1_VAL~<BASEFONT COLOR=#FFFFFF>
            }

            if (Fermenting && FruitType != FruitType.None)
            {
                list.Add(1157248 + (int)FruitType);
            }

            if (!string.IsNullOrEmpty(Year))
            {
                list.Add(1114057, Year); // ~1_val~
            }

            if (Fermented && BottlesRemaining != 0)
            {
                list.Add(1157259, BottlesRemaining.ToString()); // Bottles Remaining: ~1_VAL~
            }
        }

        public override void GetContextMenuEntries(Mobile from, List<ContextMenuEntry> list)
        {
            base.GetContextMenuEntries(from, list);

            if (from.Alive && InRange(from, 2))
            {
                if ((IsLockedDown || IsSecure) && !IsAccessibleTo(from))
                    return;

                list.Add(new BeginFermentation(from, this));
                list.Add(new EmptyBarrel(from, this));
                list.Add(new Rename(from, this));
                list.Add(new FermentationHelp(from));
            }
        }

        private class BeginFermentation : ContextMenuEntry
        {
            private readonly Mobile m_Mobile;
            private readonly FermentationBarrel _Barrel;

            public BeginFermentation(Mobile m, FermentationBarrel barrel)
                : base(1157231, 2) // Begin Fermentation
            {
                m_Mobile = m;
                _Barrel = barrel;

                if (_Barrel.Fermenting)
                {
                    Flags |= CMEFlags.Disabled;
                }
            }

            public override void OnClick()
            {
                if (_Barrel.Fermenting)
                {
                    m_Mobile.SendLocalizedMessage(1157237); // The fermentation process has already begun.
                }
                else if (!_Barrel.HasFruitAndYeast())
                {
                    m_Mobile.SendLocalizedMessage(1157255); // You do not have enough fruit or yeast to begin fermentation.
                }
                else
                {
                    m_Mobile.SendGump(new ConfirmFermentationGump(m_Mobile as PlayerMobile, 1157234, 1157240, confirm: _Barrel.StartFermentation)); // Are you sure you wish to start the fermentation process? 
                }
            }
        }

        private void StartFermentation(Mobile m, object state)
        {
            if (m == null)
                return;

            if (!Fermented && HasFruitAndYeast())
            {
                m.SendLocalizedMessage(1157239); // The fermentation process has begun.                

                Year = DateTime.UtcNow.Year.ToString();
                FermentingEnds = DateTime.UtcNow + TimeSpan.FromHours(24);
                Fermenting = true;

                int successchance = (int)((m.Skills[SkillName.Cooking].Value + m.Skills[SkillName.Alchemy].Value) / 2) * GetYeast().BacterialResistance / 4;

                if (successchance < Utility.Random(225))
                {
                    BottlesRemaining = FruitAmount() / 4;
                }
                else
                {
                    BottlesRemaining = -1;
                }

                var items = Items;

                if (items.Count > 0)
                {
                    for (var i = items.Count - 1; i >= 0; --i)
                    {
                        if (i >= items.Count)
                        {
                            continue;
                        }

                        items[i].Delete();
                    }
                }

                InvalidateProperties();

                StartTimer();
            }
            else
            {
                m.SendLocalizedMessage(1157241); // The fermentation process has not been started.
            }
        }

        private class EmptyBarrel : ContextMenuEntry
        {
            private readonly Mobile m_Mobile;
            private readonly FermentationBarrel _Barrel;

            public EmptyBarrel(Mobile m, FermentationBarrel barrel)
                : base(1157232, 2) // Empty Barrel
            {
                m_Mobile = m;
                _Barrel = barrel;
            }

            public override void OnClick()
            {
                if (_Barrel.Fermenting)
                {
                    m_Mobile.SendGump(new ConfirmFermentationGump(m_Mobile as PlayerMobile, 1157234, 1157235, confirm: _Barrel.DoEmpty)); // Are you sure you wish to end the fermentation process? All progress and materials will be lost!
                }
                else
                {
                    _Barrel.DoEmpty(m_Mobile, null);
                }
            }
        }

        private void DoEmpty(Mobile m, object state)
        {
            FruitType = FruitType.None;

            if (Fermenting)
            {
                m.SendLocalizedMessage(1157236); // You empty the contents of the barrel.                

                BottlesRemaining = 0;
                Fermenting = false;
                Fermented = false;
                FermentingEnds = DateTime.MinValue;

                StopTimer();
            }
            else
            {
                var items = Items;

                if (items.Count > 0)
                {
                    for (var i = items.Count - 1; i >= 0; --i)
                    {
                        if (i >= items.Count)
                        {
                            continue;
                        }

                        items[i].Movable = true;

                        m.AddToBackpack(items[i]);
                    }
                }
            }

            InvalidateProperties();
        }

        private class Rename : ContextMenuEntry
        {
            private readonly Mobile m_Mobile;
            private readonly FermentationBarrel _Barrel;

            public Rename(Mobile m, FermentationBarrel barrel)
                : base(1157233, 2) // Rename Vintage
            {
                m_Mobile = m;
                _Barrel = barrel;

                if (_Barrel.Fermenting)
                {
                    Flags |= CMEFlags.Disabled;
                }
            }

            public override void OnClick()
            {
                if (_Barrel.Fermenting)
                {
                    m_Mobile.SendLocalizedMessage(1157238); // You may not rename the vintage once the fermentation process has begun.
                }
                else if (!m_Mobile.HasGump(typeof(RenameGump)))
                {
                    m_Mobile.SendGump(new RenameGump(_Barrel));
                }
            }
        }

        private class RenameGump : Gump
        {
            private readonly FermentationBarrel _Barrel;

            public RenameGump(FermentationBarrel barrel)
                : base(0, 0)
            {
                _Barrel = barrel;

                AddBackground(50, 50, 400, 200, 0xA28);

                AddPage(0);

                AddHtmlLocalized(125, 70, 400, 20, 1157242, 0x0, false, false); // What do you wish to call this vintage?
                AddButton(125, 200, 0x81A, 0x81B, 1, GumpButtonType.Reply, 0);
                AddButton(320, 200, 0x819, 0x818, 0, GumpButtonType.Reply, 0);
                AddImageTiled(75, 125, 350, 40, 0xDB0);
                AddImageTiled(76, 125, 350, 2, 0x23C5);
                AddImageTiled(75, 125, 2, 40, 0x23C3);
                AddImageTiled(75, 165, 350, 2, 0x23C5);
                AddImageTiled(425, 125, 2, 42, 0x23C3);
                AddTextEntry(78, 126, 343, 37, 0x47D, 15, "", 20);
            }

            public override void OnResponse(NetState sender, RelayInfo info)
            {
                if (_Barrel == null || _Barrel.Deleted)
                    return;

                if (info.ButtonID == 1)
                {
                    TextRelay text = info.GetTextEntry(15);

                    if (text != null && !string.IsNullOrEmpty(text.Text))
                    {
                        _Barrel.Vintage = text.Text;
                        _Barrel.Year = DateTime.UtcNow.Year.ToString();
                        _Barrel.InvalidateProperties();
                    }
                }
            }
        }

        public class ConfirmFermentationGump : ConfirmCallbackGump
        {
            public ConfirmFermentationGump(
            PlayerMobile user,
            TextDefinition title,
            TextDefinition body,
            Action<Mobile, object> confirm = null)
            : base(user, title, body, null, null, confirm, null, 340, 340, 1006044, 1060051)
            {
            }

            public override void AddGumpLayout()
            {
                int height = Body.Number == 1157235 ? 20 : 0;

                AddPage(0);

                AddBackground(0, 0, 291, 99 + height, 0x13BE);
                AddImageTiled(5, 6, 280, 20, 0xA40);
                AddHtmlLocalized(9, 8, 280, 20, Title.Number, 0x7FFF, false, false);
                AddImageTiled(5, 31, 280, 40 + height, 0xA40);

                AddHtmlLocalized(9, 35, 272, 40 + height, Body.Number, 0x7FFF, false, false);

                AddButton(215, 73 + height, 0xFB7, 0xFB8, 1, GumpButtonType.Reply, 0);
                AddHtmlLocalized(250, 75 + height, 65, 20, ConfirmLocalization, 0x7FFF, false, false);

                AddButton(5, 73 + height, 0xFB1, 0xFB2, 0, GumpButtonType.Reply, 0);
                AddHtmlLocalized(40, 75 + height, 100, 20, CloseLocalization, 0x7FFF, false, false);
            }

            public override void OnResponse(RelayInfo info)
            {
                if (info.ButtonID == 1)
                {
                    ConfirmCallback?.Invoke(User, State);
                }
            }
        }

        private class FermentationHelp : ContextMenuEntry
        {
            private readonly Mobile m_Mobile;

            public FermentationHelp(Mobile mobile)
                : base(1061037, 2) // Help
            {
                m_Mobile = mobile;
            }

            public override void OnClick()
            {
                if (!m_Mobile.HasGump(typeof(FermentationHelpGump)))
                {
                    m_Mobile.SendGump(new FermentationHelpGump());
                }
            }
        }

        private class FermentationHelpGump : Gump
        {
            public FermentationHelpGump()
                : base(200, 200)
            {
                AddPage(0);

                AddBackground(0, 0, 291, 279, 0x13BE);
                AddImageTiled(5, 6, 280, 20, 0xA40);
                AddHtmlLocalized(9, 8, 280, 20, 1124526, 0x7FFF, false, false); // Fermentation Barrel
                AddImageTiled(5, 31, 280, 220, 0xA40);
                AddHtmlLocalized(9, 35, 272, 220, 1157313, 0x7FFF, false, false); // Fermentation<br>Add fruit and yeast to the barrel and begin the fermentation process.<br>Fermentation takes 24 hours and will produce 1 bottle for every 4 fruit.  Success is determined by the bacterial resistance of the yeast and your Alchemy & Cooking Skills.<br>The following fruit can be fermented: Grapes, Apples, Peaches, Pears, and Plums.
            }
        }

        public FermentationBarrel(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // version

            writer.Write((int)FruitType);
            writer.Write(Fermenting);
            writer.Write(FermentingEnds);
            writer.Write(Fermented);
            writer.Write(BottlesRemaining);
            writer.Write(Vintage);
            writer.Write(Year);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();

            FruitType = (FruitType)reader.ReadInt();
            Fermenting = reader.ReadBool();
            FermentingEnds = reader.ReadDateTime();
            Fermented = reader.ReadBool();
            BottlesRemaining = reader.ReadInt();
            Vintage = reader.ReadString();
            Year = reader.ReadString();

            if (Fermenting && !Fermented && FermentingEnds > DateTime.UtcNow)
            {
                StartTimer();
            }
        }
    }

    public class BottleOfWine : BeverageBottle
    {
        public override int LabelNumber => 1042963;  // a bottle of Wine

        [CommandProperty(AccessLevel.GameMaster)]
        public FruitType FruitType { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public string Vintage { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public string Year { get; set; }

        public override int ComputeItemID()
        {
            return 0x9c7;
        }

        [Constructable]
        public BottleOfWine()
            : this(FruitType.Grape, null, null)
        {
        }

        [Constructable]
        public BottleOfWine(FruitType type, string vintage, string year)
            : base(BeverageType.Wine)
        {
            Quantity = MaxQuantity;

            FruitType = type;
            Vintage = vintage;
            Year = year;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            if (!string.IsNullOrEmpty(Vintage))
            {
                list.Add(1157254, Vintage); // <BASEFONT COLOR=#0099cc>~1_VAL~<BASEFONT COLOR=#FFFFFF>
            }

            if (FruitType != FruitType.None)
            {
                list.Add(1157248 + (int)FruitType);
            }

            if (!string.IsNullOrEmpty(Year))
            {
                list.Add(1114057, Year); // ~1_val~
            }

            list.Add(GetQuantityDescription());
        }

        public BottleOfWine(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(1);

            writer.Write((int)FruitType);
            writer.Write(Vintage);
            writer.Write(Year);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            FruitType = (FruitType)reader.ReadInt();
            Vintage = reader.ReadString();

            if (version > 0)
            {
                Year = reader.ReadString();
            }
            else
            {
                reader.ReadMobile();
            }

        }
    }
}
