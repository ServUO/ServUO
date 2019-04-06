using System;
using Server.Mobiles;
using Server.ContextMenus;
using System.Collections.Generic;
using Server.Gumps;

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

    [FlipableAttribute(0x9E36, 0x9E37)]
    public class FermentationBarrel : Item
    {
        private static readonly int MinFruit = 20;
        private static readonly int MaxFruit = 80;

        private FruitType _FruitType;
        private int _Fruit;
        private int _BacterialResistance;

        private bool _Fermenting;
        private DateTime _FermentingEnds;

        private bool _Fermented;
        private bool _BadBatch;
        private int _BottlesRemaining;
        public string _Vintage;

        public Mobile _Maker;

        [CommandProperty(AccessLevel.GameMaster)]
        public FruitType FruitType { get { return _FruitType; } set { _FruitType = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int Fruit { get { return _Fruit; } set { _Fruit = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int BacterialResistance { get { return _BacterialResistance; } set { _BacterialResistance = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool Fermenting { get { return _Fermenting; } set { _Fermenting = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime FermentingEnds { get { return _FermentingEnds; } set { _FermentingEnds = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool Fermented { get { return _Fermented; } set { _Fermented = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool BadBatch { get { return _BadBatch; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int BottlesRemaining { get { return _BottlesRemaining; } set { _BottlesRemaining = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public string Vintage { get { return _Vintage; } set { _Vintage = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public Mobile Maker { get { return _Maker; } set { _Maker = value; } }

        public bool HasYeast { get { return _BacterialResistance > 0; } }

        public override int LabelNumber { get { return 1124526; } } // Fermentation Barrel

        public override double DefaultWeight
        {
            get
            {
                if (BottlesRemaining <= 0)
                    return 1.0;

                return (double)BottlesRemaining;
            }
        }

        [Constructable]
        public FermentationBarrel()
            : base(0x9E36)
        {
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (from.InRange(this.GetWorldLocation(), 2))
            {
                if (_Fermenting && _FermentingEnds < DateTime.UtcNow)
                {
                    _Fermented = true;

                    int skill = (int)((from.Skills[SkillName.Cooking].Value + from.Skills[SkillName.Alchemy].Value) / 2);

                    skill *= _BacterialResistance / 4;

                    _BadBatch = skill < Utility.Random(225);

                    if (!_BadBatch)
                    {
                        BottlesRemaining = _Fruit / 4;
                    }
                }
                
                if (_Fermented)
                {
                    if (_BadBatch)
                    {
                        from.PrivateOverheadMessage(Server.Network.MessageType.Regular, 1154, 1157258, from.NetState); // *You gently taste the fermentation...it's spoiled! You should probably empty the barrel*
                    }
                    else if (_BottlesRemaining > 0)
                    {
                        BottleOfWine bottle = new BottleOfWine(_FruitType, _Vintage, _Maker);

                        if (from.Backpack != null && from.Backpack.TryDropItem(from, bottle, false))
                        {
                            from.PlaySound(0x240);
                            from.SendMessage("You pour the wine into a bottle and place it in your backpack.");

                            BottlesRemaining--;
                        }
                        else
                        {
                            from.SendLocalizedMessage(500720); // You don't have enough room in your backpack!
                            bottle.Delete();
                        }
                    }
                }
            }
        }

        public override bool OnDragDrop(Mobile from, Item dropped)
        {
            if (_Fermented)
            {
                from.SendLocalizedMessage(1157245); // The fermentation barrel is not empty. Empty it first to add fruit and yeast.
            }
            else if (_Fermenting)
            {
                from.SendLocalizedMessage(1157244); // You may not add anything to the fermentation barrel after fermentation has begun.
            }
            else if (dropped is Yeast)
            {
                if (HasYeast)
                {
                    from.SendLocalizedMessage(1157256); // You have already added yeast to the barrel.
                }
                else
                {
                    BacterialResistance = ((Yeast)dropped).BacterialResistance;
                    dropped.Delete();
                }
            }
            else
            {
                FruitType type = GetFruitType(dropped.GetType());

                if (_FruitType != FruitType.None && _FruitType != type)
                {
                    from.SendLocalizedMessage(1157243); // You may only put one type of fruit in the fermentation barrel at one time. Empty the barrel first.
                }
                else if (type != FruitType.None)
                {
                    if (_FruitType == FruitType.None)
                        _FruitType = type;

                    if (_Fruit + dropped.Amount <= MaxFruit)
                    {
                        Fruit += dropped.Amount;
                        dropped.Delete();
                    }
                    else
                    {
                        dropped.Amount -= MaxFruit - _Fruit;
                        Fruit = MaxFruit;
                    }
                }
                else if (!(dropped is Yeast))
                {
                    from.SendLocalizedMessage(1157246); // You may only put fruit and yeast in the fermentation barrel.
                }
            }

            return false;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            if (_Vintage != null)
            {
                list.Add(1157254, _Vintage); // <BASEFONT COLOR=#0099cc>~1_VAL~<BASEFONT COLOR=#FFFFFF>
            }

            if (_FruitType != FruitType.None)
            {
                list.Add(1157248 + (int)_FruitType);
            }

            if (_Fermented)
            {
                list.Add(1157259, _BottlesRemaining.ToString()); // Bottles Remaining: ~1_VAL~
            }
        }

        public override void GetContextMenuEntries(Mobile from, List<ContextMenuEntry> entries)
        {
            base.GetContextMenuEntries(from, entries);

            entries.Add(new SimpleContextMenuEntry(from, 1157231, m => // Begin Fermentation
                {
                    if (_Fermenting)
                    {
                        m.SendLocalizedMessage(1157237); // The fermentation process has already begun.
                    }
                    else if (!HasYeast || _Fruit < MinFruit)
                    {
                        m.SendLocalizedMessage(1157255); // You do not have enough fruit or yeast to begin fermentation.
                    }
                    else
                    {
                        m.SendGump(new ConfirmCallbackGump(m as PlayerMobile, 1157234, 1157240, null, confirm: StartFermentation, close: CancelFermentation)); // Are you sure you wish to start the fermentation process? 
                    }
                }, 3, !_Fermented && (HasYeast || _Fruit > 0)));

            entries.Add(new SimpleContextMenuEntry(from, 1157232, m => // Empty Barrel
            {
                m.SendGump(new ConfirmCallbackGump(m as PlayerMobile, 1157234, 1157235, null, confirm: DoEmpty)); // Are you sure you wish to end the fermentation process? All progress and materials will be lost!
            }, 3, HasYeast || _Fruit > 0));

            entries.Add(new SimpleContextMenuEntry(from, 1157233, m => // Rename Vintage
            {
                if (_Fermenting)
                {
                    m.SendLocalizedMessage(1157238); // You may not rename the vintage once the fermentation process has begun.
                }
                else
                {
                    m.SendLocalizedMessage(1157242); // What do you wish to call this vintage?
                    m.BeginPrompt((mob, text) =>
                        {
                            if (text != null)
                            {
                                text = text.Trim();
                                text = Utility.FixHtml(text);

                                if (text.Length > 15 || !Server.Guilds.BaseGuildGump.CheckProfanity(text))
                                {
                                    mob.SendMessage("That label is unacceptable. Please try again.");
                                }
                                else
                                {
                                    Vintage = text;
                                }
                            }
                        }, null);
                }
            }, 3, !_Fermented));
        }

        private void StartFermentation(Mobile m, object state)
        {
            m.SendLocalizedMessage(1157239); // The fermentation process has begun.

            _Fermenting = true;

            _FermentingEnds = DateTime.UtcNow + TimeSpan.FromHours(24);
        }

        private void CancelFermentation(Mobile m, object state)
        {
            m.SendLocalizedMessage(1157241); // The fermentation process has not been started.
        }

        private void DoEmpty(Mobile m, object state)
        {
            m.SendLocalizedMessage(1157236); // You empty the contents of the barrel.

            Reset();
        }

        private void Reset()
        {
            _FruitType = FruitType.None;
            _Fruit = 0;
            _BacterialResistance = 0;

            _Fermenting = false;
            _FermentingEnds = DateTime.MinValue;

            _Fermented = false;
            _BottlesRemaining = 0;
            _Vintage = null;

            _Maker = null;

            InvalidateProperties();
        }

        private FruitType GetFruitType(Type t)
        {
            for(int i = 0; i < _FruitTypes.Length; i++)
            {
                foreach (var type in _FruitTypes[i])
                {
                    if (type == t)
                        return (FruitType)i + 1;
                }
            }

            return FruitType.None;
        }

        private Type[][] _FruitTypes =
        {
            new Type[] { typeof(GrapeBunch), typeof(Grapes) },
            new Type[] { typeof(Apple) },
            new Type[] { typeof(Peach) },
            new Type[] { typeof(Pear) },
            new Type[] { typeof(Plum) }
        };

        public FermentationBarrel(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version

            writer.Write((int)_FruitType);
            writer.Write(_Fruit);
            writer.Write(_BacterialResistance);

            writer.Write(_Fermenting);
            writer.Write(_FermentingEnds);

            writer.Write(_Fermented);
            writer.Write(_BadBatch);
            writer.Write(_BottlesRemaining);
            writer.Write(_Vintage);

            writer.Write(_Maker);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            _FruitType = (FruitType)reader.ReadInt();
            _Fruit = reader.ReadInt();
            _BacterialResistance = reader.ReadInt();

            _Fermenting = reader.ReadBool();
            _FermentingEnds = reader.ReadDateTime();

            _Fermented = reader.ReadBool();
            _BadBatch = reader.ReadBool();
            _BottlesRemaining = reader.ReadInt();
            _Vintage = reader.ReadString();

            _Maker = reader.ReadMobile();
        }
    }

    public class BottleOfWine : BeverageBottle
    {
        private FruitType _FruitType;
        private string _Vintage;
        private Mobile _Maker;

        [CommandProperty(AccessLevel.GameMaster)]
        public FruitType FruitType { get { return _FruitType; } set { _FruitType = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public string Vintage { get { return _Vintage; } set { _Vintage = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public Mobile Maker { get { return _Maker; } set { _Maker = value; InvalidateProperties(); } }

        [Constructable]
        public BottleOfWine()
            : this(FruitType.Grape, null, null)
        {
        }

        [Constructable]
        public BottleOfWine(FruitType type, string vintage, Mobile maker)
            : base(BeverageType.Wine)
        {
            Quantity = MaxQuantity;
            
            _FruitType = type;
            _Vintage = vintage;
            _Maker = maker;
        }

        public override void AddNameProperty(ObjectPropertyList list)
        {
            list.Add(1049519, String.Format("#{0}", (1157248 + (int)_FruitType).ToString())); // a bottle of ~1_DRINK_NAME~
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            if (_Vintage != null)
            {
                list.Add(1157254, _Vintage); // <BASEFONT COLOR=#0099cc>~1_VAL~<BASEFONT COLOR=#FFFFFF>
            }

            if (_Maker != null)
            {
                list.Add(1150679, _Maker.Name); // Distiller: ~1_NAME~
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
            writer.Write((int)0);

            writer.Write((int)_FruitType);
            writer.Write(_Vintage);
            writer.Write(_Maker);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            _FruitType = (FruitType)reader.ReadInt();
            _Vintage = reader.ReadString();
            _Maker = reader.ReadMobile();
        }
    }
}
