using System;
using Server.Engines.Craft;
using Server.Network;

namespace Server.Items
{
    public abstract class LockableContainer : TrapableContainer, ILockable, ILockpickable, ICraftable, IShipwreckedItem
    {
        private bool m_Locked;
        private int m_LockLevel, m_MaxLockLevel, m_RequiredSkill;
        private uint m_KeyValue;
        private Mobile m_Picker;
		private Mobile m_Crafter;
        private bool m_TrapOnLockpick;

		[CommandProperty(AccessLevel.GameMaster)]
		public Mobile Crafter
		{
			get { return m_Crafter; }
			set
			{
				m_Crafter = value;
				InvalidateProperties();
			}
		}

		[CommandProperty(AccessLevel.GameMaster)]
        public Mobile Picker
        {
            get
            {
                return this.m_Picker;
            }
            set
            {
                this.m_Picker = value;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int MaxLockLevel
        {
            get
            {
                return this.m_MaxLockLevel;
            }
            set
            {
                this.m_MaxLockLevel = value;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int LockLevel
        {
            get
            {
                return this.m_LockLevel;
            }
            set
            {
                this.m_LockLevel = value;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int RequiredSkill
        {
            get
            {
                return this.m_RequiredSkill;
            }
            set
            {
                this.m_RequiredSkill = value;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public virtual bool Locked
        {
            get
            {
                return this.m_Locked;
            }
            set
            {
                this.m_Locked = value;

                if (this.m_Locked)
                    this.m_Picker = null;

                this.InvalidateProperties();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public uint KeyValue
        {
            get
            {
                return this.m_KeyValue;
            }
            set
            {
                this.m_KeyValue = value;
            }
        }

        public override bool TrapOnOpen
        {
            get
            {
                return !this.m_TrapOnLockpick;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool TrapOnLockpick
        {
            get
            {
                return this.m_TrapOnLockpick;
            }
            set
            {
                this.m_TrapOnLockpick = value;
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)7); // version

			writer.Write(this.m_Crafter);

            writer.Write(this.m_IsShipwreckedItem);

            writer.Write((bool)this.m_TrapOnLockpick);

            writer.Write((int)this.m_RequiredSkill);

            writer.Write((int)this.m_MaxLockLevel);

            writer.Write(this.m_KeyValue);
            writer.Write((int)this.m_LockLevel);
            writer.Write((bool)this.m_Locked);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch ( version )
            {
				case 7:
					{
						this.m_Crafter = reader.ReadMobile();

						goto case 6;
					}
                case 6:
                    {
                        this.m_IsShipwreckedItem = reader.ReadBool();

                        goto case 5;
                    }
                case 5:
                    {
                        this.m_TrapOnLockpick = reader.ReadBool();

                        goto case 4;
                    }
                case 4:
                    {
                        this.m_RequiredSkill = reader.ReadInt();

                        goto case 3;
                    }
                case 3:
                    {
                        this.m_MaxLockLevel = reader.ReadInt();

                        goto case 2;
                    }
                case 2:
                    {
                        this.m_KeyValue = reader.ReadUInt();

                        goto case 1;
                    }
                case 1:
                    {
                        this.m_LockLevel = reader.ReadInt();

                        goto case 0;
                    }
                case 0:
                    {
                        if (version < 3)
                            this.m_MaxLockLevel = 100;

                        if (version < 4)
                        {
                            if ((this.m_MaxLockLevel - this.m_LockLevel) == 40)
                            {
                                this.m_RequiredSkill = this.m_LockLevel + 6;
                                this.m_LockLevel = this.m_RequiredSkill - 10;
                                this.m_MaxLockLevel = this.m_RequiredSkill + 39;
                            }
                            else
                            {
                                this.m_RequiredSkill = this.m_LockLevel;
                            }
                        }

                        this.m_Locked = reader.ReadBool();

                        break;
                    }
            }
        }

        public LockableContainer(int itemID)
            : base(itemID)
        {
            this.m_MaxLockLevel = 100;
        }

        public LockableContainer(Serial serial)
            : base(serial)
        {
        }

        public override bool CheckContentDisplay(Mobile from)
        {
            return !this.m_Locked && base.CheckContentDisplay(from);
        }

        public override bool TryDropItem(Mobile from, Item dropped, bool sendFullMessage)
        {
            if (from.AccessLevel < AccessLevel.GameMaster && this.m_Locked)
            {
                from.SendLocalizedMessage(501747); // It appears to be locked.
                return false;
            }

            return base.TryDropItem(from, dropped, sendFullMessage);
        }

        public override bool OnDragDropInto(Mobile from, Item item, Point3D p)
        {
            if (from.AccessLevel < AccessLevel.GameMaster && this.m_Locked)
            {
                from.SendLocalizedMessage(501747); // It appears to be locked.
                return false;
            }

            return base.OnDragDropInto(from, item, p);
        }

        public override bool CheckLift(Mobile from, Item item, ref LRReason reject)
        {
            if (!base.CheckLift(from, item, ref reject))
                return false;

            if (item != this && from.AccessLevel < AccessLevel.GameMaster && this.m_Locked)
                return false;

            return true;
        }

        public override bool CheckItemUse(Mobile from, Item item)
        {
            if (!base.CheckItemUse(from, item))
                return false;

            if (item != this && from.AccessLevel < AccessLevel.GameMaster && this.m_Locked)
            {
                from.LocalOverheadMessage(MessageType.Regular, 0x3B2, 1019045); // I can't reach that.
                return false;
            }

            return true;
        }

        public override bool DisplaysContent
        {
            get
            {
                return !this.m_Locked;
            }
        }

        public virtual bool CheckLocked(Mobile from)
        {
            bool inaccessible = false;

            if (this.m_Locked)
            {
                int number;

                if (from.AccessLevel >= AccessLevel.GameMaster)
                {
                    number = 502502; // That is locked, but you open it with your godly powers.
                }
                else
                {
                    number = 501747; // It appears to be locked.
                    inaccessible = true;
                }

                from.Send(new MessageLocalized(this.Serial, this.ItemID, MessageType.Regular, 0x3B2, 3, number, "", ""));
            }

            return inaccessible;
        }

        public override void OnTelekinesis(Mobile from)
        {
            if (this.CheckLocked(from))
            {
                Effects.SendLocationParticles(EffectItem.Create(this.Location, this.Map, EffectItem.DefaultDuration), 0x376A, 9, 32, 5022);
                Effects.PlaySound(this.Location, this.Map, 0x1F5);
                return;
            }

            base.OnTelekinesis(from);
        }

        public override void OnDoubleClickSecureTrade(Mobile from)
        {
            if (this.CheckLocked(from))
                return;

            base.OnDoubleClickSecureTrade(from);
        }

        public override void Open(Mobile from)
        {
            if (this.CheckLocked(from))
                return;

            base.Open(from);
        }

        public override void OnSnoop(Mobile from)
        {
            if (this.CheckLocked(from))
                return;

            base.OnSnoop(from);
        }

        public virtual void LockPick(Mobile from)
        {
            this.Locked = false;
            this.Picker = from;

            if (this.TrapOnLockpick && this.ExecuteTrap(from))
            {
                this.TrapOnLockpick = false;
            }
        }

        public override void AddNameProperties(ObjectPropertyList list)
        {
            base.AddNameProperties(list);

			if (m_Crafter != null)
			{
				list.Add(1050043, m_Crafter.Name); // crafted by ~1_NAME~
			}

			if (this.m_IsShipwreckedItem)
                list.Add(1041645); // recovered from a shipwreck
        }

        public override void OnSingleClick(Mobile from)
        {
            base.OnSingleClick(from);

			if (m_Crafter != null)
			{
				this.LabelTo(from, 1050043, m_Crafter.Name); // crafted by ~1_NAME~
			}

			if (this.m_IsShipwreckedItem)
                this.LabelTo(from, 1041645);	//recovered from a shipwreck
        }

        #region ICraftable Members

        public int OnCraft(int quality, bool makersMark, Mobile from, CraftSystem craftSystem, Type typeRes, BaseTool tool, CraftItem craftItem, int resHue)
        {
			if(makersMark)
			{
				Crafter = from;
			}
            if (from.CheckSkill(SkillName.Tinkering, -5.0, 15.0))
            {
                from.SendLocalizedMessage(500636); // Your tinker skill was sufficient to make the item lockable.

                Key key = new Key(KeyType.Copper, Key.RandomValue());

                this.KeyValue = key.KeyValue;
                this.DropItem(key);

                double tinkering = from.Skills[SkillName.Tinkering].Value;
                int level = (int)(tinkering * 0.8);

                this.RequiredSkill = level - 4;
                this.LockLevel = level - 14;
                this.MaxLockLevel = level + 35;

                if (this.LockLevel == 0)
                    this.LockLevel = -1;
                else if (this.LockLevel > 95)
                    this.LockLevel = 95;

                if (this.RequiredSkill > 95)
                    this.RequiredSkill = 95;

                if (this.MaxLockLevel > 95)
                    this.MaxLockLevel = 95;
            }
            else
            {
                from.SendLocalizedMessage(500637); // Your tinker skill was insufficient to make the item lockable.
            }

            return 1;
        }

        #endregion

        #region IShipwreckedItem Members

        private bool m_IsShipwreckedItem;

        [CommandProperty(AccessLevel.GameMaster)]
        public bool IsShipwreckedItem
        {
            get
            {
                return this.m_IsShipwreckedItem;
            }
            set
            {
                this.m_IsShipwreckedItem = value;
            }
        }
        #endregion
    }
}