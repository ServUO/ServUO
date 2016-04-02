using System;
using Server.Engines.VeteranRewards;
using Server.Gumps;
using Server.Mobiles;
using Server.Targeting;

namespace Server.Items
{
    public class WeaponEngravingTool : Item, IUsesRemaining, IRewardItem
    { 
        private int m_UsesRemaining;
        private bool m_IsRewardItem;
        [Constructable]
        public WeaponEngravingTool()
            : this(10)
        {
        }

        [Constructable]
        public WeaponEngravingTool(int uses)
            : base(0x32F8)
        {
            this.LootType = LootType.Blessed;
            this.Weight = 1.0;
			
            this.m_UsesRemaining = uses;
        }

        public WeaponEngravingTool(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1076158;
            }
        }// Weapon Engraving Tool
        [CommandProperty(AccessLevel.GameMaster)]
        public int UsesRemaining
        {
            get
            {
                return this.m_UsesRemaining;
            }
            set
            {
                this.m_UsesRemaining = value;
                this.InvalidateProperties();
            }
        }
        public virtual bool ShowUsesRemaining
        { 
            get
            {
                return true;
            }
            set
            {
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
        public static WeaponEngravingTool Find(Mobile from)
        {
            if (from.Backpack != null)
                return from.Backpack.FindItemByType(typeof(WeaponEngravingTool)) as WeaponEngravingTool;
				
            return null;
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (this.m_IsRewardItem && !RewardSystem.CheckIsUsableBy(from, this, null))
                return;
			
            if (this.m_UsesRemaining > 0)
            {
                from.SendLocalizedMessage(1072357); // Select an object to engrave.
                from.Target = new TargetWeapon(this);
            }
            else 
            {
                if (from.Skills.Tinkering.Value == 0)
                {
                    from.SendLocalizedMessage(1076179); // Since you have no tinkering skill, you will need to find an NPC tinkerer to repair this for you.					
                }
                else if (from.Skills.Tinkering.Value < 75.0)
                { 
                    from.SendLocalizedMessage(1076178); // Your tinkering skill is too low to fix this yourself.  An NPC tinkerer can help you repair this for a fee.
                }
                else
                { 
                    Item diamond = from.Backpack.FindItemByType(typeof(BlueDiamond));	
					
                    if (diamond != null)
                        from.SendGump(new ConfirmGump(this, null));	
                    else 
                        from.SendLocalizedMessage(1076166); // You do not have a blue diamond needed to recharge the engraving tool.	
                }
				
                from.SendLocalizedMessage(1076163); // There are no charges left on this engraving tool.
            }
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
			
            if (this.m_IsRewardItem)
                list.Add(1076224); // 8th Year Veteran Reward
			
            if (this.ShowUsesRemaining)
                list.Add(1060584, this.m_UsesRemaining.ToString()); // uses remaining: ~1_val~			
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
			
            writer.Write((int)0); // version
			
            writer.Write((int)this.m_UsesRemaining);
            writer.Write((bool)this.m_IsRewardItem);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
			
            int version = reader.ReadInt();
			
            this.m_UsesRemaining = reader.ReadInt();
            this.m_IsRewardItem = reader.ReadBool();
        }

        public virtual void Recharge(Mobile from, Mobile guildmaster)
        { 
            if (from.Backpack != null)
            {
                Item diamond = from.Backpack.FindItemByType(typeof(BlueDiamond));			
									
                if (guildmaster != null)
                {
                    if (this.m_UsesRemaining <= 0)
                    { 
                        if (diamond != null && Banker.Withdraw(from, 100000))
                        { 
                            diamond.Consume();
                            this.UsesRemaining = 10;			
                            guildmaster.Say(1076165); // Your weapon engraver should be good as new!
                        }
                        else
                            guildmaster.Say(1076167); // You need a 100,000 gold and a blue diamond to recharge the weapon engraver.
                    }
                    else
                        guildmaster.Say(1076164); // I can only help with this if you are carrying an engraving tool that needs repair.
                }
                else
                { 
                    if (from.Skills.Tinkering.Value == 0)
                    {
                        from.SendLocalizedMessage(1076179); // Since you have no tinkering skill, you will need to find an NPC tinkerer to repair this for you.					
                    }
                    else if (from.Skills.Tinkering.Value < 75.0)
                    { 
                        from.SendLocalizedMessage(1076178); // Your tinkering skill is too low to fix this yourself.  An NPC tinkerer can help you repair this for a fee.
                    }
                    else if (diamond != null)
                    {
                        diamond.Consume();
						
                        if (Utility.RandomDouble() < from.Skills.Tinkering.Value / 100)
                        { 
                            this.UsesRemaining = 10;			
                            from.SendLocalizedMessage(1076165); // Your weapon engraver should be good as new! ?????
                        }
                        else
                            from.SendLocalizedMessage(1076175); // You cracked the diamond attempting to fix the weapon engraver.
                    }
                    else
                        from.SendLocalizedMessage(1076166); // You do not have a blue diamond needed to recharge the engraving tool.	
                }
            }
        }

        public class ConfirmGump : Gump
        { 
            private readonly WeaponEngravingTool m_Engraver;
            private readonly Mobile m_Guildmaster;
            public ConfirmGump(WeaponEngravingTool engraver, Mobile guildmaster)
                : base(200, 200)
            { 
                this.m_Engraver = engraver;
                this.m_Guildmaster = guildmaster;
			
                this.Closable = false;
                this.Disposable = true;
                this.Dragable = true;
                this.Resizable = false;
			
                this.AddPage(0);

                this.AddBackground(0, 0, 291, 133, 0x13BE);
                this.AddImageTiled(5, 5, 280, 100, 0xA40);
				
                if (guildmaster != null)
                {
                    this.AddHtmlLocalized(9, 9, 272, 100, 1076169, 0x7FFF, false, false); // It will cost you 100,000 gold and a blue diamond to recharge your weapon engraver with 10 charges.
                    this.AddHtmlLocalized(195, 109, 120, 20, 1076172, 0x7FFF, false, false); // Recharge it
                }
                else
                {
                    this.AddHtmlLocalized(9, 9, 272, 100, 1076176, 0x7FFF, false, false); // You will need a blue diamond to repair the tip of the engraver.  A successful repair will give the engraver 10 charges.
                    this.AddHtmlLocalized(195, 109, 120, 20, 1076177, 0x7FFF, false, false); // Replace the tip.
                }
				
                this.AddButton(160, 107, 0xFB7, 0xFB8, (int)Buttons.Confirm, GumpButtonType.Reply, 0);				
                this.AddButton(5, 107, 0xFB1, 0xFB2, (int)Buttons.Cancel, GumpButtonType.Reply, 0);
                this.AddHtmlLocalized(40, 109, 100, 20, 1060051, 0x7FFF, false, false); // CANCEL
            }

            private enum Buttons
            {
                Cancel,
                Confirm
            }
            public override void OnResponse(Server.Network.NetState state, RelayInfo info)
            { 
                if (this.m_Engraver == null || this.m_Engraver.Deleted)
                    return;
					
                if (info.ButtonID == (int)Buttons.Confirm)
                    this.m_Engraver.Recharge(state.Mobile, this.m_Guildmaster);
            }
        }

        private class TargetWeapon : Target
        {
            private readonly WeaponEngravingTool m_Tool;
            public TargetWeapon(WeaponEngravingTool tool)
                : base(-1, true, TargetFlags.None)
            {
                this.m_Tool = tool;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (this.m_Tool == null || this.m_Tool.Deleted)
                    return;
					
                if (targeted is BaseWeapon)
                {
                    BaseWeapon item = (BaseWeapon)targeted;
					
                    from.CloseGump(typeof(InternalGump));
                    from.SendGump(new InternalGump(this.m_Tool, item));
                }
                else
                    from.SendLocalizedMessage(1072309); // The selected item cannot be engraved by this engraving tool.
            }
        }

        private class InternalGump : Gump
        {
            private readonly WeaponEngravingTool m_Tool;
            private readonly BaseWeapon m_Target;
            public InternalGump(WeaponEngravingTool tool, BaseWeapon target)
                : base(0, 0)
            {
                this.m_Tool = tool;
                this.m_Target = target;
			
                this.Closable = true;
                this.Disposable = true;
                this.Dragable = true;
                this.Resizable = false;
				
                this.AddBackground(50, 50, 400, 300, 0xA28);

                this.AddPage(0);

                this.AddHtmlLocalized(50, 70, 400, 20, 1072359, 0x0, false, false); // <CENTER>Engraving Tool</CENTER>
                this.AddHtmlLocalized(75, 95, 350, 145, 1076229, 0x0, true, true); // Please enter the text to add to the selected object. Leave the text area blank to remove any existing text.  Removing text does not use a charge.
                this.AddButton(125, 300, 0x81A, 0x81B, (int)Buttons.Okay, GumpButtonType.Reply, 0);
                this.AddButton(320, 300, 0x819, 0x818, (int)Buttons.Cancel, GumpButtonType.Reply, 0);
                this.AddImageTiled(75, 245, 350, 40, 0xDB0);
                this.AddImageTiled(76, 245, 350, 2, 0x23C5);
                this.AddImageTiled(75, 245, 2, 40, 0x23C3);
                this.AddImageTiled(75, 285, 350, 2, 0x23C5);
                this.AddImageTiled(425, 245, 2, 42, 0x23C3);
				
                this.AddTextEntry(75, 245, 350, 40, 0x0, (int)Buttons.Text, "");
            }

            private enum Buttons
            {
                Cancel,
                Okay,
                Text
            }
            public override void OnResponse(Server.Network.NetState state, RelayInfo info)
            { 
                if (this.m_Tool == null || this.m_Tool.Deleted || this.m_Target == null || this.m_Target.Deleted)
                    return;
			
                if (info.ButtonID == (int)Buttons.Okay)
                {
                    TextRelay relay = info.GetTextEntry((int)Buttons.Text);
					
                    if (relay != null)
                    {
                        if (String.IsNullOrEmpty(relay.Text))
                        {
                            this.m_Target.EngravedText = null;
                            state.Mobile.SendLocalizedMessage(1072362); // You remove the engraving from the object.
                        }
                        else
                        {
                            if (relay.Text.Length > 64)
                                this.m_Target.EngravedText = Utility.FixHtml(relay.Text.Substring(0, 64));
                            else
                                this.m_Target.EngravedText = Utility.FixHtml(relay.Text);
						
                            state.Mobile.SendLocalizedMessage(1072361); // You engraved the object.	
                            this.m_Target.InvalidateProperties();						
                            this.m_Tool.UsesRemaining -= 1;
                            this.m_Tool.InvalidateProperties();
                        }
                    }
                }
                else
                    state.Mobile.SendLocalizedMessage(1072363); // The object was not engraved.
            }
        }
    }
}