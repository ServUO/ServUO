using System;
using Server.Engines.Craft;
using Server.Mobiles;
using Server.Targeting;

namespace Server.Items
{
    [TypeAlias("Server.Items.DragonBarding")]
    public class DragonBardingDeed : Item, ICraftable
    {
        private bool m_Exceptional;
        private Mobile m_Crafter;
        private CraftResource m_Resource;

        public override int LabelNumber
        {
            get
            {
                return this.m_Exceptional ? 1053181 : 1053012;
            }
        }// dragon barding deed

        [CommandProperty(AccessLevel.GameMaster)]
        public Mobile Crafter
        {
            get
            {
                return this.m_Crafter;
            }
            set
            {
                this.m_Crafter = value;
                this.InvalidateProperties();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool Exceptional
        {
            get
            {
                return this.m_Exceptional;
            }
            set
            {
                this.m_Exceptional = value;
                this.InvalidateProperties();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public CraftResource Resource
        {
            get
            {
                return this.m_Resource;
            }
            set
            {
                this.m_Resource = value;
                this.Hue = CraftResources.GetHue(value);
                this.InvalidateProperties();
            }
        }

        [Constructable]
        public DragonBardingDeed()
            : base(0x14F0)
        {
            this.Weight = 1.0;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            if (this.m_Exceptional && this.m_Crafter != null)
				list.Add(1050043, m_Crafter.TitleName); // crafted by ~1_NAME~
        }
		public override void OnSingleClick(Mobile from)
		{
			base.OnSingleClick(from);

			if (m_Crafter != null)
			{
				LabelTo(from, 1050043, m_Crafter.TitleName); // crafted by ~1_NAME~
			}
		}
        
        public override void OnDoubleClick(Mobile from)
        {
            if (this.IsChildOf(from.Backpack))
            {
                from.BeginTarget(6, false, TargetFlags.None, new TargetCallback(OnTarget));
                from.SendLocalizedMessage(1053024); // Select the swamp dragon you wish to place the barding on.
            }
            else
            {
                from.SendLocalizedMessage(1042001); // That must be in your pack for you to use it.
            }
        }

        public virtual void OnTarget(Mobile from, object obj)
        {
            if (this.Deleted)
                return;

            SwampDragon pet = obj as SwampDragon;

            if (pet == null || pet.HasBarding)
            {
                from.SendLocalizedMessage(1053025); // That is not an unarmored swamp dragon.
            }
            else if (!pet.Controlled || pet.ControlMaster != from)
            {
                from.SendLocalizedMessage(1053026); // You can only put barding on a tamed swamp dragon that you own.
            }
            else if (!this.IsChildOf(from.Backpack))
            {
                from.SendLocalizedMessage(1060640); // The item must be in your backpack to use it.
            }
            else
            {
                pet.BardingExceptional = this.Exceptional;
                pet.BardingCrafter = this.Crafter;
                pet.BardingResource = this.Resource;
                pet.HasBarding = true;
                pet.Hue = this.Hue;
                pet.BardingHP = pet.BardingMaxHP;

                this.Delete();

                from.SendLocalizedMessage(1053027); // You place the barding on your swamp dragon.  Use a bladed item on your dragon to remove the armor.
            }
        }

        public DragonBardingDeed(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)1); // version

            writer.Write((bool)this.m_Exceptional);
            writer.Write((Mobile)this.m_Crafter);
            writer.Write((int)this.m_Resource);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch ( version )
            {
                case 1:
                case 0:
                    {
                        this.m_Exceptional = reader.ReadBool();
                        this.m_Crafter = reader.ReadMobile();

                        if (version < 1)
                            reader.ReadInt();

                        this.m_Resource = (CraftResource)reader.ReadInt();
                        break;
                    }
            }
        }

        #region ICraftable Members

        public int OnCraft(int quality, bool makersMark, Mobile from, CraftSystem craftSystem, Type typeRes, ITool tool, CraftItem craftItem, int resHue)
        {
            this.Exceptional = (quality >= 2);

            if (makersMark)
                this.Crafter = from;

            Type resourceType = typeRes;

            if (resourceType == null)
                resourceType = craftItem.Resources.GetAt(0).ItemType;

            this.Resource = CraftResources.GetFromType(resourceType);
            return quality;
        }
        #endregion
    }
}