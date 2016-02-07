using System;
using Server.Engines.Quests.Haven;
using Server.Engines.VeteranRewards;
using Server.Gumps;
using Server.Network;
using Server.Targeting;

namespace Server.Items
{
    public class CannonAddonComponent : AddonComponent
    {
        public CannonAddonComponent(int itemID)
            : base(itemID)
        { 
            this.LootType = LootType.Blessed;
        }

        public CannonAddonComponent(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1076157;
            }
        }// Decorative Cannon
        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
			
            if (this.Addon is CannonAddon)
            {
                if (((CannonAddon)this.Addon).IsRewardItem)
                    list.Add(1076223); // 7th Year Veteran Reward
					
                list.Add(1076207, ((CannonAddon)this.Addon).Charges.ToString()); // Remaining Charges: ~1_val~
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

    public class CannonAddon : BaseAddon
    { 
        private static readonly int[] m_Effects = new int[]
        {
            0x36B0, 0x3728, 0x3709, 0x36FE
        };
        private CannonDirection m_CannonDirection;
        private int m_Charges;
        private bool m_IsRewardItem;
        [Constructable]
        public CannonAddon(CannonDirection direction)
        {
            this.m_CannonDirection = direction;

            switch ( direction )
            {
                case CannonDirection.North:
                    {
                        this.AddComponent(new CannonAddonComponent(0xE8D), 0, 0, 0);
                        this.AddComponent(new CannonAddonComponent(0xE8C), 0, 1, 0);
                        this.AddComponent(new CannonAddonComponent(0xE8B), 0, 2, 0);

                        break;
                    }
                case CannonDirection.East:
                    {
                        this.AddComponent(new CannonAddonComponent(0xE96), 0, 0, 0);
                        this.AddComponent(new CannonAddonComponent(0xE95), -1, 0, 0);
                        this.AddComponent(new CannonAddonComponent(0xE94), -2, 0, 0);

                        break;
                    }
                case CannonDirection.South:
                    {
                        this.AddComponent(new CannonAddonComponent(0xE91), 0, 0, 0);
                        this.AddComponent(new CannonAddonComponent(0xE92), 0, -1, 0);
                        this.AddComponent(new CannonAddonComponent(0xE93), 0, -2, 0);

                        break;
                    }
                default:
                    {
                        this.AddComponent(new CannonAddonComponent(0xE8E), 0, 0, 0);
                        this.AddComponent(new CannonAddonComponent(0xE8F), 1, 0, 0);
                        this.AddComponent(new CannonAddonComponent(0xE90), 2, 0, 0);

                        break;
                    }
            }
        }

        public CannonAddon(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddonDeed Deed
        {
            get
            {
                CannonDeed deed = new CannonDeed();
                deed.Charges = this.m_Charges;
                deed.IsRewardItem = this.m_IsRewardItem;

                return deed; 
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public CannonDirection CannonDirection
        { 
            get
            {
                return this.m_CannonDirection;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public int Charges 
        { 
            get
            {
                return this.m_Charges;
            }
            set
            { 
                this.m_Charges = value; 
				
                foreach (AddonComponent c in this.Components)
                    c.InvalidateProperties(); 
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public bool IsRewardItem
        {
            get
            {
                return this.m_IsRewardItem;
            }
            set
            { 
                this.m_IsRewardItem = value; 
				
                foreach (AddonComponent c in this.Components)
                    c.InvalidateProperties(); 
            }
        }
        public override void OnComponentUsed(AddonComponent c, Mobile from)
        {
            if (from.InRange(this.Location, 2))
            {
                if (this.m_Charges > 0)
                {
                    from.Target = new InternalTarget(this);
                }
                else
                {
                    if (from.Backpack != null)
                    {
                        PotionKeg keg = from.Backpack.FindItemByType(typeof(PotionKeg)) as PotionKeg;
						
                        if (this.Validate(keg) > 0)
                            from.SendGump(new InternalGump(this, keg));
                        else
                            from.SendLocalizedMessage(1076198); // You do not have a full keg of explosion potions needed to recharge the cannon.
                    }
                }
            }
            else
                from.SendLocalizedMessage(1076766); // That is too far away.
        }

        public int Validate(PotionKeg keg)
        {
            if (keg != null && !keg.Deleted && keg.Held == 100)
            {
                if (keg.Type == PotionEffect.ExplosionLesser)
                    return 5;
                else if (keg.Type == PotionEffect.Explosion)
                    return 10;
                else if (keg.Type == PotionEffect.ExplosionGreater)
                    return 15;
            }
			
            return 0;
        }

        public void Fill(Mobile from, PotionKeg keg)
        {
            this.Charges = this.Validate(keg);
			
            if (this.Charges > 0)
            {
                keg.Delete();
                from.SendLocalizedMessage(1076199); // Your cannon is recharged.
            }
            else
                from.SendLocalizedMessage(1076198); // You do not have a full keg of explosion potions needed to recharge the cannon.
        }

        public void DoFireEffect(IPoint3D target)
        {
            Map map = this.Map;
			
            if (target == null || map == null)
                return;
				
            Effects.PlaySound(target, map, Utility.RandomList(0x11B, 0x11C, 0x11D));
            Effects.SendLocationEffect(target, map, Utility.RandomList(m_Effects), 16, 1);
						
            for (int count = Utility.Random(3); count > 0; count--)
            { 
                IPoint3D location = new Point3D(target.X + Utility.RandomMinMax(-1, 1), target.Y + Utility.RandomMinMax(-1, 1), target.Z);
                int effect = Utility.RandomList(m_Effects);
                Effects.SendLocationEffect(location, map, effect, 16, 1);
            }
			
            this.Charges -= 1;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(0); // version

            writer.Write((int)this.m_CannonDirection);
            writer.Write((int)this.m_Charges);
            writer.Write((bool)this.m_IsRewardItem);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();

            this.m_CannonDirection = (CannonDirection)reader.ReadInt();
            this.m_Charges = reader.ReadInt();
            this.m_IsRewardItem = reader.ReadBool();
        }

        private class InternalTarget : Target
        {
            private readonly CannonAddon m_Cannon;
            public InternalTarget(CannonAddon cannon)
                : base(12, true, TargetFlags.None)
            {
                this.m_Cannon = cannon;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (this.m_Cannon == null || this.m_Cannon.Deleted)
                    return;
					
                IPoint3D p = targeted as IPoint3D;
			
                if (p == null)
                    return;
					
                if (from.InLOS(new Point3D(p)))
                {
                    if (!Utility.InRange(new Point3D(p), this.m_Cannon.Location, 2))
                    {
                        bool allow = false;
					
                        int x = p.X - this.m_Cannon.X;
                        int y = p.Y - this.m_Cannon.Y;
						
                        switch ( this.m_Cannon.CannonDirection )
                        {
                            case CannonDirection.North:
                                if (y < 0 && Math.Abs(x) <= -y / 3)
                                    allow = true;
								
                                break;
                            case CannonDirection.East:
                                if (x > 0 && Math.Abs(y) <= x / 3)
                                    allow = true;
								
                                break;
                            case CannonDirection.South:
                                if (y > 0 && Math.Abs(x) <= y / 3)
                                    allow = true;
									
                                break;
                            case CannonDirection.West:
                                if (x < 0 && Math.Abs(y) <= -x / 3)
                                    allow = true;
								
                                break;
                        }
						
                        if (allow && Utility.InRange(new Point3D(p), this.m_Cannon.Location, 14))
                            this.m_Cannon.DoFireEffect(p);
                        else
                            from.SendLocalizedMessage(1076203); // Target out of range.							
                    }
                    else
                        from.SendLocalizedMessage(1076215); // Cannon must be aimed farther away.
                }
                else
                    from.SendLocalizedMessage(1049630); // You cannot see that target!		
            }

            protected override void OnTargetOutOfRange(Mobile from, object targeted)
            {
                from.SendLocalizedMessage(1076203); // Target out of range.
            }
        }

        private class InternalGump : Gump
        {
            private readonly CannonAddon m_Cannon;
            private readonly PotionKeg m_Keg;
            public InternalGump(CannonAddon cannon, PotionKeg keg)
                : base(50, 50)
            {
                this.m_Cannon = cannon;
                this.m_Keg = keg;
				
                this.Closable = true;
                this.Disposable = true;
                this.Dragable = true;
                this.Resizable = false;

                this.AddPage(0);
				
                this.AddBackground(0, 0, 291, 133, 0x13BE);
                this.AddImageTiled(5, 5, 280, 100, 0xA40);
				
                this.AddHtmlLocalized(9, 9, 272, 100, 1076196, cannon.Validate(keg).ToString(), 0x7FFF, false, false); // You will need a full keg of explosion potions to recharge the cannon.  Your keg will provide ~1_CHARGES~ charges.
				
                this.AddButton(5, 107, 0xFB1, 0xFB2, (int)Buttons.Cancel, GumpButtonType.Reply, 0);
                this.AddHtmlLocalized(40, 109, 100, 20, 1060051, 0x7FFF, false, false); // CANCEL
				
                this.AddButton(160, 107, 0xFB7, 0xFB8, (int)Buttons.Recharge, GumpButtonType.Reply, 0);
                this.AddHtmlLocalized(195, 109, 120, 20, 1076197, 0x7FFF, false, false); // Recharge
            }

            private enum Buttons
            {
                Cancel,
                Recharge
            }
            public override void OnResponse(NetState state, RelayInfo info)
            { 
                if (this.m_Cannon == null || this.m_Cannon.Deleted)
                    return;
					
                if (info.ButtonID == (int)Buttons.Recharge)
                    this.m_Cannon.Fill(state.Mobile, this.m_Keg);
            }
        }
    }

    public class CannonDeed : BaseAddonDeed, IRewardItem, IRewardOption
    {
        private CannonDirection m_Direction;
        private int m_Charges;
        private bool m_IsRewardItem;
        [Constructable]
        public CannonDeed()
            : base()
        { 
            this.LootType = LootType.Blessed;
        }

        public CannonDeed(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1076195;
            }
        }// A deed for a cannon
        public override BaseAddon Addon
        { 
            get
            {
                CannonAddon addon = new CannonAddon(this.m_Direction);
                addon.Charges = this.m_Charges;
                addon.IsRewardItem = this.m_IsRewardItem;

                return addon; 
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public int Charges 
        { 
            get
            {
                return this.m_Charges;
            }
            set
            {
                this.m_Charges = value;
                this.InvalidateProperties();
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public bool IsRewardItem
        {
            get
            {
                return this.m_IsRewardItem;
            }
            set
            {
                this.m_IsRewardItem = value;
                this.InvalidateProperties();
            }
        }
        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);			
			
            if (this.m_IsRewardItem)
                list.Add(1076223); // 7th Year Veteran Reward
			
            list.Add(1076207, this.m_Charges.ToString()); // Remaining Charges: ~1_val~
        }

        public override void OnDoubleClick(Mobile from)
        { 
            if (this.m_IsRewardItem && !RewardSystem.CheckIsUsableBy(from, this, null))
                return;
		
            if (this.IsChildOf(from.Backpack))
            {
                from.CloseGump(typeof(RewardOptionGump));
                from.SendGump(new RewardOptionGump(this));
            }
            else
                from.SendLocalizedMessage(1042038); // You must have the object in your backpack to use it.          	
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(0); // version
			
            writer.Write((int)this.m_Charges);
            writer.Write((bool)this.m_IsRewardItem);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();
			
            this.m_Charges = reader.ReadInt();
            this.m_IsRewardItem = reader.ReadBool();
        }

        public void GetOptions(RewardOptionList list)
        {
            list.Add((int)CannonDirection.South, 1075386); // South
            list.Add((int)CannonDirection.East, 1075387); // East
            list.Add((int)CannonDirection.North, 1075389); // North
            list.Add((int)CannonDirection.West, 1075390); // West
        }

        public void OnOptionSelected(Mobile from, int option)
        {
            this.m_Direction = (CannonDirection)option;

            if (!this.Deleted)
                base.OnDoubleClick(from);
        }
    }
}