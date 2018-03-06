using System;
using Server.Items;
using Server.Mobiles;
using Server.Engines.Craft;
using Server.ContextMenus;
using System.Collections.Generic;

namespace Server.Items
{
    public class ChocolateNutcracker : CandyCane, ICraftable
    {
        public enum ChocolateType
        {
            Milk = 0x461,
            Dark = 0x465,
            White = 0x47E,
        }

        public override int LabelNumber
        {
            get
            {
                switch (this.Type)
                {
                    default:
                    case ChocolateType.Milk: return _Wrapped ? 1156388 : 1156391;
                    case ChocolateType.Dark: return _Wrapped ? 1156387 : 1156390;
                    case ChocolateType.White: return _Wrapped ? 1156389 : 1156392;
                }
            }
        }

        private ChocolateType _Type;
        private bool _Wrapped;

        [CommandProperty(AccessLevel.GameMaster)]
        public ChocolateType Type { get { return _Type; } set { _Type = value; Hue = !_Wrapped ? (int)_Type : 0; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool Wrapped { get { return _Wrapped; } set { _Wrapped = value; InvalidateID(); InvalidateProperties(); } }

        [Constructable]
        public ChocolateNutcracker()
            : this(ChocolateType.Dark)
        {
        }

        [Constructable]
        public ChocolateNutcracker(ChocolateType type) 
            : base(39952)
        {
            Wrapped = true;
            this.Type = type;
        }

        private void InvalidateID()
        {
            if (_Wrapped)
            {
                ItemID = 39952;
                Hue = 0;
            }
            else
            {
                ItemID = 39954;
                Hue = (int)_Type;
            }
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (IsChildOf(from.Backpack) && Wrapped)
                Unwrap(from);
            else
                base.OnDoubleClick(from);
        }

        private void Unwrap(Mobile from)
		{
			from.PrivateOverheadMessage(Server.Network.MessageType.Regular, 1154, 1156393, from.NetState); // *You carefully peel back the wrapper...*
            from.PlaySound(Utility.Random(0x21F, 4));

            Timer.DelayCall(TimeSpan.FromSeconds(1), () =>
            {
                if (0.10 > Utility.RandomDouble())
                {
                    from.PrivateOverheadMessage(Server.Network.MessageType.Regular, 1154, 1156439, from.NetState); // *You peel back to the wrapper to reveal a Card of Semidar!*
                    from.AddToBackpack(new CardOfSemidar());
                }

                Wrapped = false;
            });
		}

        public override void GetContextMenuEntries(Mobile from, List<ContextMenuEntry> list)
        {
            if(!_Wrapped)
                base.GetContextMenuEntries(from, list);
        }

        public override int OnCraft(int quality, bool makersMark, Mobile from, CraftSystem craftSystem, Type typeRes, ITool tool, CraftItem craftItem, int resHue)
        {
            if (craftItem != null && craftItem.Data is ChocolateType)
            {
                this.Type = (ChocolateType)craftItem.Data;
            }

            return quality;
        }

        public ChocolateNutcracker(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0);
            writer.Write((int)_Type);
            writer.Write(_Wrapped);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
            _Type = (ChocolateType)reader.ReadInt();
            _Wrapped = reader.ReadBool();
        }
    }
}