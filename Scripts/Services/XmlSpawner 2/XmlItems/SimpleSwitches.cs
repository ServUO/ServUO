using System;
using System.Collections;
using Server.Mobiles;

/*
** SimpleLever, SimpleSwitch, and CombinationLock
** Version 1.03
** updated 5/06/04
** ArteGordon
**
*/
namespace Server.Items
{
    public interface ILinkable
    {
        Item Link { set; get; }
        void Activate(Mobile from, int state, ArrayList links);
    }

    public class SimpleLever : Item, ILinkable
    {
        private int m_LeverState = 0;
        private leverType m_LeverType = leverType.Two_State;
        private int m_LeverSound = 936;
        private Item m_TargetItem0 = null;
        private string m_TargetProperty0 = null;
        private Item m_TargetItem1 = null;
        private string m_TargetProperty1 = null;
        private Item m_TargetItem2 = null;
        private string m_TargetProperty2 = null;
        private Item m_LinkedItem = null;
        private bool already_being_activated = false;
        private bool m_Disabled = false;
        [Constructable]
        public SimpleLever()
            : base(0x108C)
        {
            this.Name = "A lever";
            this.Movable = false;
        }

        public SimpleLever(Serial serial)
            : base(serial)
        {
        }

        public enum leverType
        {
            Two_State,
            Three_State
        }
        [CommandProperty(AccessLevel.Spawner)]
        public bool Disabled
        {
            set
            {
                this.m_Disabled = value;
            }
            get
            {
                return this.m_Disabled;
            }
        }
        [CommandProperty(AccessLevel.Spawner)]
        public Item Link
        {
            set
            {
                this.m_LinkedItem = value;
            }
            get
            {
                return this.m_LinkedItem;
            }
        }
        [CommandProperty(AccessLevel.Spawner)]
        public int LeverState
        {
            get
            {
                return this.m_LeverState;
            }
            set
            {
                // prevent infinite recursion 
                if (!this.already_being_activated)
                {
                    this.already_being_activated = true;
                    this.Activate(null, value, null);
                    this.already_being_activated = false;
                }

                this.InvalidateProperties();
            }
        }
        [CommandProperty(AccessLevel.Spawner)]
        public int LeverSound
        {
            get
            {
                return this.m_LeverSound;
            }
            set
            {
                this.m_LeverSound = value;
                this.InvalidateProperties();
            }
        }
        [CommandProperty(AccessLevel.Spawner)]
        public leverType LeverType
        {
            get
            {
                return this.m_LeverType;
            }
            set
            {
                this.m_LeverType = value;
                this.LeverState = 0;
                this.InvalidateProperties();
            }
        }
        [CommandProperty(AccessLevel.Spawner)]
        new public virtual Direction Direction
        {
            get
            {
                return base.Direction;
            }
            set
            { 
                base.Direction = value; 
                this.SetLeverStatic(); 
                this.InvalidateProperties(); 
            }
        }
        [CommandProperty(AccessLevel.Spawner)]
        public Item Target0Item
        {
            get
            {
                return this.m_TargetItem0;
            }
            set
            {
                this.m_TargetItem0 = value;
                this.InvalidateProperties();
            }
        }
        [CommandProperty(AccessLevel.Spawner)]
        public string Target0Property
        {
            get
            {
                return this.m_TargetProperty0;
            }
            set
            {
                this.m_TargetProperty0 = value;
                this.InvalidateProperties();
            }
        }
        [CommandProperty(AccessLevel.Spawner)]
        public string Target0ItemName
        { 
            get 
            { 
                if (this.m_TargetItem0 != null && !this.m_TargetItem0.Deleted) 
                    return this.m_TargetItem0.Name; 
                else 
                    return null; 
            }
        }
        [CommandProperty(AccessLevel.Spawner)]
        public Item Target1Item
        {
            get
            {
                return this.m_TargetItem1;
            }
            set
            {
                this.m_TargetItem1 = value;
                this.InvalidateProperties();
            }
        }
        [CommandProperty(AccessLevel.Spawner)]
        public string Target1Property
        {
            get
            {
                return this.m_TargetProperty1;
            }
            set
            {
                this.m_TargetProperty1 = value;
                this.InvalidateProperties();
            }
        }
        [CommandProperty(AccessLevel.Spawner)]
        public string Target1ItemName
        { 
            get 
            { 
                if (this.m_TargetItem1 != null && !this.m_TargetItem1.Deleted) 
                    return 
                    this.m_TargetItem1.Name; 
                else 
                    return null; 
            }
        }
        [CommandProperty(AccessLevel.Spawner)]
        public Item Target2Item
        {
            get
            {
                return this.m_TargetItem2;
            }
            set
            {
                this.m_TargetItem2 = value;
                this.InvalidateProperties();
            }
        }
        [CommandProperty(AccessLevel.Spawner)]
        public string Target2Property
        {
            get
            {
                return this.m_TargetProperty2;
            }
            set
            {
                this.m_TargetProperty2 = value;
                this.InvalidateProperties();
            }
        }
        [CommandProperty(AccessLevel.Spawner)]
        public string Target2ItemName
        { 
            get 
            { 
                if (this.m_TargetItem2 != null && !this.m_TargetItem2.Deleted) 
                    return this.m_TargetItem2.Name; 
                else 
                    return null; 
            }
        }
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)2); // version
            // version 2
            writer.Write(this.m_Disabled);
            // version 1
            writer.Write(this.m_LinkedItem);
            // version 0
            writer.Write(this.m_LeverState);
            writer.Write(this.m_LeverSound);
            int ltype = (int)this.m_LeverType;
            writer.Write(ltype);
            writer.Write(this.m_TargetItem0);
            writer.Write(this.m_TargetProperty0);
            writer.Write(this.m_TargetItem1);
            writer.Write(this.m_TargetProperty1);
            writer.Write(this.m_TargetItem2);
            writer.Write(this.m_TargetProperty2);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
            switch (version)
            {
                case 2:
                    {
                        this.m_Disabled = reader.ReadBool();
                        goto case 1;
                    }
                case 1:
                    {
                        this.m_LinkedItem = reader.ReadItem();
                        goto case 0;
                    }
                case 0:
                    {
                        this.m_LeverState = reader.ReadInt();
                        this.m_LeverSound = reader.ReadInt();
                        int ltype = reader.ReadInt();
                        switch (ltype)
                        {
                            case (int)leverType.Two_State:
                                this.m_LeverType = leverType.Two_State;
                                break;
                            case (int)leverType.Three_State:
                                this.m_LeverType = leverType.Three_State;
                                break;
                        }
                        this.m_TargetItem0 = reader.ReadItem();
                        this.m_TargetProperty0 = reader.ReadString();
                        this.m_TargetItem1 = reader.ReadItem();
                        this.m_TargetProperty1 = reader.ReadString();
                        this.m_TargetItem2 = reader.ReadItem();
                        this.m_TargetProperty2 = reader.ReadString();
                    }
                    break;
            }
        }

        public void SetLeverStatic()
        {
            switch (this.Direction)
            {
                case Direction.North:
                case Direction.South:
                case Direction.Right:
                case Direction.Up:
                    if (this.m_LeverType == leverType.Two_State)
                        this.ItemID = 0x108c + this.m_LeverState * 2;
                    else
                        this.ItemID = 0x108c + this.m_LeverState;
                    break;
                case Direction.East:
                case Direction.West:
                case Direction.Left:
                case Direction.Down:
                    if (this.m_LeverType == leverType.Two_State)
                        this.ItemID = 0x1093 + this.m_LeverState * 2;
                    else
                        this.ItemID = 0x1093 + this.m_LeverState;
                    break;
                default:
                    break;
            }
        }

        public void Activate(Mobile from, int state, ArrayList links)
        {
            if (this.Disabled)
                return;

            string status_str = null;

            // assign the lever state
            this.m_LeverState = state;

            if (this.m_LeverState < 0)
                this.m_LeverState = 0;
            if (this.m_LeverState > 1 && this.m_LeverType == leverType.Two_State)
                this.m_LeverState = 1;
            if (this.m_LeverState > 2)
                this.m_LeverState = 2;

            // update the graphic
            this.SetLeverStatic();

            // play the switching sound if possible
            //if (from != null)
            //{
            //	from.PlaySound(m_LeverSound);
            //}
            try
            {
                Effects.PlaySound(this.Location, this.Map, this.m_LeverSound);
            }
            catch
            {
            }

            // if a target object has been specified then apply the property modification
            if (this.m_LeverState == 0 && this.m_TargetItem0 != null && !this.m_TargetItem0.Deleted && this.m_TargetProperty0 != null && this.m_TargetProperty0.Length > 0)
            {
                BaseXmlSpawner.ApplyObjectStringProperties(null, this.m_TargetProperty0, this.m_TargetItem0, from, this, out status_str);
            }
            if (this.m_LeverState == 1 && this.m_TargetItem1 != null && !this.m_TargetItem1.Deleted && this.m_TargetProperty1 != null && this.m_TargetProperty1.Length > 0)
            {
                BaseXmlSpawner.ApplyObjectStringProperties(null, this.m_TargetProperty1, this.m_TargetItem1, from, this, out status_str);
            }
            if (this.m_LeverState == 2 && this.m_TargetItem2 != null && !this.m_TargetItem2.Deleted && this.m_TargetProperty2 != null && this.m_TargetProperty2.Length > 0)
            {
                BaseXmlSpawner.ApplyObjectStringProperties(null, this.m_TargetProperty2, this.m_TargetItem2, from, this, out status_str);
            }

            // if the switch is linked, then activate the link as well
            if (this.Link != null && this.Link is ILinkable)
            {
                if (links == null)
                {
                    links = new ArrayList();
                }
                // activate other linked objects if they have not already been activated
                if (!links.Contains(this))
                {
                    links.Add(this);

                    ((ILinkable)this.Link).Activate(from, state, links);
                }
            }

            // report any problems to staff
            if (status_str != null && from != null && from.IsStaff())
            {
                from.SendMessage("{0}", status_str);
            }
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (from == null || this.Disabled)
                return;

            if (!from.InRange(this.GetWorldLocation(), 2) || !from.InLOS(this))
            {
                from.SendLocalizedMessage(500446); // That is too far away.
                return;
            }

            // change the switch state
            this.m_LeverState = this.m_LeverState + 1;

            if (this.m_LeverState > 1 && this.m_LeverType == leverType.Two_State)
                this.m_LeverState = 0;
            if (this.m_LeverState > 2)
                this.m_LeverState = 0;

            // carry out the switch actions
            this.Activate(from, this.m_LeverState, null);
        }
    }

    public class SimpleSwitch : Item, ILinkable
    {
        private int m_SwitchState = 0;
        private int m_SwitchSound = 939;
        private Item m_TargetItem0 = null;
        private string m_TargetProperty0 = null;
        private Item m_TargetItem1 = null;
        private string m_TargetProperty1 = null;
        private Item m_LinkedItem = null;
        private bool already_being_activated = false;
        private bool m_Disabled = false;
        [Constructable]
        public SimpleSwitch()
            : base(0x108F)
        {
            this.Name = "A switch";
            this.Movable = false;
        }

        public SimpleSwitch(Serial serial)
            : base(serial)
        {
        }

        [CommandProperty(AccessLevel.Spawner)]
        public bool Disabled
        {
            set
            {
                this.m_Disabled = value;
            }
            get
            {
                return this.m_Disabled;
            }
        }
        [CommandProperty(AccessLevel.Spawner)]
        public Item Link
        {
            set
            {
                this.m_LinkedItem = value;
            }
            get
            {
                return this.m_LinkedItem;
            }
        }
        [CommandProperty(AccessLevel.Spawner)]
        public int SwitchState
        {
            set
            {
                // prevent infinite recursion 
                if (!this.already_being_activated)
                {
                    this.already_being_activated = true;
                    this.Activate(null, value, null);
                    this.already_being_activated = false;
                }

                this.InvalidateProperties();
            }
            get
            {
                return this.m_SwitchState;
            }
        }
        [CommandProperty(AccessLevel.Spawner)]
        public int SwitchSound
        {
            get
            {
                return this.m_SwitchSound;
            }
            set
            {
                this.m_SwitchSound = value;
                this.InvalidateProperties();
            }
        }
        [CommandProperty(AccessLevel.Spawner)]
        new public virtual Direction Direction
        {
            get
            {
                return base.Direction;
            }
            set
            {
                base.Direction = value;
                this.SetSwitchStatic();
                this.InvalidateProperties();
            }
        }
        [CommandProperty(AccessLevel.Spawner)]
        public Item Target0Item
        {
            get
            {
                return this.m_TargetItem0;
            }
            set
            {
                this.m_TargetItem0 = value;
                this.InvalidateProperties();
            }
        }
        [CommandProperty(AccessLevel.Spawner)]
        public string Target0Property
        {
            get
            {
                return this.m_TargetProperty0;
            }
            set
            {
                this.m_TargetProperty0 = value;
                this.InvalidateProperties();
            }
        }
        [CommandProperty(AccessLevel.Spawner)]
        public string Target0ItemName
        {
            get
            {
                if (this.m_TargetItem0 != null && !this.m_TargetItem0.Deleted)
                    return this.m_TargetItem0.Name;
                else
                    return null;
            }
        }
        [CommandProperty(AccessLevel.Spawner)]
        public Item Target1Item
        {
            get
            {
                return this.m_TargetItem1;
            }
            set
            {
                this.m_TargetItem1 = value;
                this.InvalidateProperties();
            }
        }
        [CommandProperty(AccessLevel.Spawner)]
        public string Target1Property
        {
            get
            {
                return this.m_TargetProperty1;
            }
            set
            {
                this.m_TargetProperty1 = value;
                this.InvalidateProperties();
            }
        }
        [CommandProperty(AccessLevel.Spawner)]
        public string Target1ItemName
        {
            get
            {
                if (this.m_TargetItem1 != null && !this.m_TargetItem1.Deleted)
                    return this.m_TargetItem1.Name;
                else
                    return null;
            }
        }
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)2); // version
            // version 2
            writer.Write(this.m_Disabled);
            // version 1
            writer.Write(this.m_LinkedItem);
            // version 0
            writer.Write(this.m_SwitchState);
            writer.Write(this.m_SwitchSound);
            writer.Write(this.m_TargetItem0);
            writer.Write(this.m_TargetProperty0);
            writer.Write(this.m_TargetItem1);
            writer.Write(this.m_TargetProperty1);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
            switch (version)
            {
                case 2:
                    {
                        this.m_Disabled = reader.ReadBool();
                        goto case 1;
                    }
                case 1:
                    {
                        this.m_LinkedItem = reader.ReadItem();
                        goto case 0;
                    }
                case 0:
                    {
                        this.m_SwitchState = reader.ReadInt();
                        this.m_SwitchSound = reader.ReadInt();
                        this.m_TargetItem0 = reader.ReadItem();
                        this.m_TargetProperty0 = reader.ReadString();
                        this.m_TargetItem1 = reader.ReadItem();
                        this.m_TargetProperty1 = reader.ReadString();
                    }
                    break;
            }
        }

        public void SetSwitchStatic()
        {
            switch (this.Direction)
            {
                case Direction.North:
                case Direction.South:
                case Direction.Right:
                case Direction.Up:
                    this.ItemID = 0x108f + this.m_SwitchState;
                    break;
                case Direction.East:
                case Direction.West:
                case Direction.Left:
                case Direction.Down:
                    this.ItemID = 0x1091 + this.m_SwitchState;
                    break;
                default:
                    this.ItemID = 0x108f + this.m_SwitchState;
                    break;
            }
        }

        public void Activate(Mobile from, int state, ArrayList links)
        {
            if (this.Disabled)
                return;

            string status_str = null;

            // assign the switch state
            this.m_SwitchState = state;

            if (this.m_SwitchState < 0)
                this.m_SwitchState = 0;
            if (this.m_SwitchState > 1)
                this.m_SwitchState = 1;

            // update the graphic
            this.SetSwitchStatic();

            //if (from != null)
            //{
            //	from.PlaySound(m_SwitchSound);
            //}
            try
            {
                Effects.PlaySound(this.Location, this.Map, this.m_SwitchSound);
            }
            catch
            {
            }

            // if a target object has been specified then apply the property modification
            if (this.m_SwitchState == 0 && this.m_TargetItem0 != null && !this.m_TargetItem0.Deleted && this.m_TargetProperty0 != null && this.m_TargetProperty0.Length > 0)
            {
                BaseXmlSpawner.ApplyObjectStringProperties(null, this.m_TargetProperty0, this.m_TargetItem0, from, this, out status_str);
            }

            if (this.m_SwitchState == 1 && this.m_TargetItem1 != null && !this.m_TargetItem1.Deleted && this.m_TargetProperty1 != null && this.m_TargetProperty1.Length > 0)
            {
                BaseXmlSpawner.ApplyObjectStringProperties(null, this.m_TargetProperty1, this.m_TargetItem1, from, this, out status_str);
            }

            // if the switch is linked, then activate the link as well
            if (this.Link != null && this.Link is ILinkable)
            {
                if (links == null)
                {
                    links = new ArrayList();
                }
                // activate other linked objects if they have not already been activated
                if (!links.Contains(this))
                {
                    links.Add(this);

                    ((ILinkable)this.Link).Activate(from, state, links);
                }
            }

            // report any problems to staff
            if (status_str != null && from != null && from.IsStaff())
            {
                from.SendMessage("{0}", status_str);
            }
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (from == null || this.Disabled)
                return;

            if (!from.InRange(this.GetWorldLocation(), 2) || !from.InLOS(this))
            {
                from.SendLocalizedMessage(500446); // That is too far away.
                return;
            }

            // change the switch state
            this.m_SwitchState = this.m_SwitchState + 1;

            if (this.m_SwitchState > 1)
                this.m_SwitchState = 0;

            // activate the switch
            this.Activate(from, this.m_SwitchState, null);
        }
    }

    public class CombinationLock : Item
    {
        private int m_Combination = 0;
        private Item m_Digit0Object = null;
        private string m_Digit0Property = null;
        private Item m_Digit1Object = null;
        private string m_Digit1Property = null;
        private Item m_Digit2Object = null;
        private string m_Digit2Property = null;
        private Item m_Digit3Object = null;
        private string m_Digit3Property = null;
        private Item m_Digit4Object = null;
        private string m_Digit4Property = null;
        private Item m_Digit5Object = null;
        private string m_Digit5Property = null;
        private Item m_Digit6Object = null;
        private string m_Digit6Property = null;
        private Item m_Digit7Object = null;
        private string m_Digit7Property = null;
        private Item m_TargetItem = null;
        private string m_TargetProperty = null;
        private int m_CombinationSound = 940;
        [Constructable]
        public CombinationLock()
            : base(0x1BBF)
        {
            this.Name = "A combination lock";
            this.Movable = false;
        }

        public CombinationLock(Serial serial)
            : base(serial)
        {
        }

        [CommandProperty(AccessLevel.Spawner)]
        public int Combination
        {
            get
            {
                return this.m_Combination;
            }
            set
            {
                this.m_Combination = value;
                if (this.m_Combination < 0)
                    this.m_Combination = 0;
                if (this.m_Combination > 99999999)
                    this.m_Combination = 99999999;
                this.InvalidateProperties();
            }
        }
        [CommandProperty(AccessLevel.Spawner)]
        public Item Digit0Object
        {
            get
            {
                return this.m_Digit0Object;
            }
            set
            {
                this.m_Digit0Object = value;
                this.InvalidateProperties();
            }
        }
        [CommandProperty(AccessLevel.Spawner)]
        public string Digit0Property
        {
            get
            {
                return this.m_Digit0Property;
            }
            set
            {
                this.m_Digit0Property = value;
                this.InvalidateProperties();
            }
        }
        [CommandProperty(AccessLevel.Spawner)]
        public int Digit0
        {
            get
            {
                return (this.CheckDigit(this.m_Digit0Object, this.m_Digit0Property));
            }
        }
        [CommandProperty(AccessLevel.Spawner)]
        public Item Digit1Object
        {
            get
            {
                return this.m_Digit1Object;
            }
            set
            {
                this.m_Digit1Object = value;
                this.InvalidateProperties();
            }
        }
        [CommandProperty(AccessLevel.Spawner)]
        public string Digit1Property
        {
            get
            {
                return this.m_Digit1Property;
            }
            set
            {
                this.m_Digit1Property = value;
                this.InvalidateProperties();
            }
        }
        [CommandProperty(AccessLevel.Spawner)]
        public int Digit1
        {
            get
            {
                return (this.CheckDigit(this.m_Digit1Object, this.m_Digit1Property));
            }
        }
        [CommandProperty(AccessLevel.Spawner)]
        public Item Digit2Object
        {
            get
            {
                return this.m_Digit2Object;
            }
            set
            {
                this.m_Digit2Object = value;
                this.InvalidateProperties();
            }
        }
        [CommandProperty(AccessLevel.Spawner)]
        public string Digit2Property
        {
            get
            {
                return this.m_Digit2Property;
            }
            set
            {
                this.m_Digit2Property = value;
                this.InvalidateProperties();
            }
        }
        [CommandProperty(AccessLevel.Spawner)]
        public int Digit2
        {
            get
            {
                return (this.CheckDigit(this.m_Digit2Object, this.m_Digit2Property));
            }
        }
        [CommandProperty(AccessLevel.Spawner)]
        public Item Digit3Object
        {
            get
            {
                return this.m_Digit3Object;
            }
            set
            {
                this.m_Digit3Object = value;
                this.InvalidateProperties();
            }
        }
        [CommandProperty(AccessLevel.Spawner)]
        public string Digit3Property
        {
            get
            {
                return this.m_Digit3Property;
            }
            set
            {
                this.m_Digit3Property = value;
                this.InvalidateProperties();
            }
        }
        [CommandProperty(AccessLevel.Spawner)]
        public int Digit3
        {
            get
            {
                return (this.CheckDigit(this.m_Digit3Object, this.m_Digit3Property));
            }
        }
        [CommandProperty(AccessLevel.Spawner)]
        public Item Digit4Object
        {
            get
            {
                return this.m_Digit4Object;
            }
            set
            {
                this.m_Digit4Object = value;
                this.InvalidateProperties();
            }
        }
        [CommandProperty(AccessLevel.Spawner)]
        public string Digit4Property
        {
            get
            {
                return this.m_Digit4Property;
            }
            set
            {
                this.m_Digit4Property = value;
                this.InvalidateProperties();
            }
        }
        [CommandProperty(AccessLevel.Spawner)]
        public int Digit4
        {
            get
            {
                return (this.CheckDigit(this.m_Digit4Object, this.m_Digit4Property));
            }
        }
        [CommandProperty(AccessLevel.Spawner)]
        public Item Digit5Object
        {
            get
            {
                return this.m_Digit5Object;
            }
            set
            {
                this.m_Digit5Object = value;
                this.InvalidateProperties();
            }
        }
        [CommandProperty(AccessLevel.Spawner)]
        public string Digit5Property
        {
            get
            {
                return this.m_Digit5Property;
            }
            set
            {
                this.m_Digit5Property = value;
                this.InvalidateProperties();
            }
        }
        [CommandProperty(AccessLevel.Spawner)]
        public int Digit5
        {
            get
            {
                return (this.CheckDigit(this.m_Digit5Object, this.m_Digit5Property));
            }
        }
        [CommandProperty(AccessLevel.Spawner)]
        public Item Digit6Object
        {
            get
            {
                return this.m_Digit6Object;
            }
            set
            {
                this.m_Digit6Object = value;
                this.InvalidateProperties();
            }
        }
        [CommandProperty(AccessLevel.Spawner)]
        public string Digit6Property
        {
            get
            {
                return this.m_Digit6Property;
            }
            set
            {
                this.m_Digit6Property = value;
                this.InvalidateProperties();
            }
        }
        [CommandProperty(AccessLevel.Spawner)]
        public int Digit6
        {
            get
            {
                return (this.CheckDigit(this.m_Digit6Object, this.m_Digit6Property));
            }
        }
        [CommandProperty(AccessLevel.Spawner)]
        public Item Digit7Object
        {
            get
            {
                return this.m_Digit7Object;
            }
            set
            {
                this.m_Digit7Object = value;
                this.InvalidateProperties();
            }
        }
        [CommandProperty(AccessLevel.Spawner)]
        public string Digit7Property
        {
            get
            {
                return this.m_Digit7Property;
            }
            set
            {
                this.m_Digit7Property = value;
                this.InvalidateProperties();
            }
        }
        [CommandProperty(AccessLevel.Spawner)]
        public int Digit7
        {
            get
            {
                return (this.CheckDigit(this.m_Digit7Object, this.m_Digit7Property));
            }
        }
        [CommandProperty(AccessLevel.Spawner)]
        public Item TargetItem
        {
            get
            {
                return this.m_TargetItem;
            }
            set
            {
                this.m_TargetItem = value;
                this.InvalidateProperties();
            }
        }
        [CommandProperty(AccessLevel.Spawner)]
        public string TargetProperty
        {
            get
            {
                return this.m_TargetProperty;
            }
            set
            {
                this.m_TargetProperty = value;
                this.InvalidateProperties();
            }
        }
        [CommandProperty(AccessLevel.Spawner)]
        public string TargetItemName
        {
            get
            {
                if (this.m_TargetItem != null && !this.m_TargetItem.Deleted)
                    return this.m_TargetItem.Name;
                else
                    return null;
            }
        }
        [CommandProperty(AccessLevel.Spawner)]
        public int CombinationSound
        {
            get
            {
                return this.m_CombinationSound;
            }
            set
            {
                this.m_CombinationSound = value;
                this.InvalidateProperties();
            }
        }
        [CommandProperty(AccessLevel.Spawner)]
        public bool Matched
        {
            get
            {
                return (this.m_Combination == this.CurrentValue);
            }
        }
        [CommandProperty(AccessLevel.Spawner)]

        public int CurrentValue
        {
            get
            {
                int value = this.Digit0 + this.Digit1 * 10 + this.Digit2 * 100 + this.Digit3 * 1000 + this.Digit4 * 10000 + this.Digit5 * 100000 + this.Digit6 * 1000000 + this.Digit7 * 10000000;
                return value;
            }
        }
        public int SetDigit(int value)
        {
            if (value < 0)
                return 0;
            if (value > 9)
                return 9;
            return value;
        }

        public int CheckDigit(object o, string property)
        {
            if (o == null)
                return 0;
            if (property == null || property.Length <= 0)
                return (0);
            Type ptype;
            int ival = -1;
            string testvalue;
            // check to see whether this is a direct value request, or a test
            string[] argtest = BaseXmlSpawner.ParseString(property, 2, "<>!=");
            if (argtest.Length > 1)
            {
                // ok, its a test, so test it
                string status_str;
                if (BaseXmlSpawner.CheckPropertyString(null, o, property, null, out status_str))
                {
                    return 1; // true
                }
                else
                    return 0; // false
            }
            // otherwise get the value of the property requested
            string result = BaseXmlSpawner.GetPropertyValue(null, o, property, out ptype);

            string[] arglist = BaseXmlSpawner.ParseString(result, 2, "=");
            if (arglist.Length < 2)
                return -1;
            string[] arglist2 = BaseXmlSpawner.ParseString(arglist[1], 2, " ");
            if (arglist2.Length > 0)
            {
                testvalue = arglist2[0].Trim();
            }
            else
            {
                return -1;
            }

            if (BaseXmlSpawner.IsNumeric(ptype))
            {
                try
                {
                    ival = Convert.ToInt32(testvalue, 10);
                }
                catch
                {
                }
            }
            return ival;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version

            writer.Write(this.m_Combination);
            writer.Write(this.m_CombinationSound);
            writer.Write(this.m_Digit0Object);
            writer.Write(this.m_Digit0Property);
            writer.Write(this.m_Digit1Object);
            writer.Write(this.m_Digit1Property);
            writer.Write(this.m_Digit2Object);
            writer.Write(this.m_Digit2Property);
            writer.Write(this.m_Digit3Object);
            writer.Write(this.m_Digit3Property);
            writer.Write(this.m_Digit4Object);
            writer.Write(this.m_Digit4Property);
            writer.Write(this.m_Digit5Object);
            writer.Write(this.m_Digit5Property);
            writer.Write(this.m_Digit6Object);
            writer.Write(this.m_Digit6Property);
            writer.Write(this.m_Digit7Object);
            writer.Write(this.m_Digit7Property);
            writer.Write(this.m_TargetItem);
            writer.Write(this.m_TargetProperty);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
            switch (version)
            {
                case 0:
                    {
                        this.m_Combination = reader.ReadInt();
                        this.m_CombinationSound = reader.ReadInt();
                        this.m_Digit0Object = reader.ReadItem();
                        this.m_Digit0Property = reader.ReadString();
                        this.m_Digit1Object = reader.ReadItem();
                        this.m_Digit1Property = reader.ReadString();
                        this.m_Digit2Object = reader.ReadItem();
                        this.m_Digit2Property = reader.ReadString();
                        this.m_Digit3Object = reader.ReadItem();
                        this.m_Digit3Property = reader.ReadString();
                        this.m_Digit4Object = reader.ReadItem();
                        this.m_Digit4Property = reader.ReadString();
                        this.m_Digit5Object = reader.ReadItem();
                        this.m_Digit5Property = reader.ReadString();
                        this.m_Digit6Object = reader.ReadItem();
                        this.m_Digit6Property = reader.ReadString();
                        this.m_Digit7Object = reader.ReadItem();
                        this.m_Digit7Property = reader.ReadString();
                        this.m_TargetItem = reader.ReadItem();
                        this.m_TargetProperty = reader.ReadString();
                    }
                    break;
            }
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (from == null)
                return;

            if (!from.InRange(this.GetWorldLocation(), 2) || !from.InLOS(this))
            {
                from.SendLocalizedMessage(500446); // That is too far away.
                return;
            }
            string status_str;
            // test the combination and apply the property to the target item
            if (this.Matched)
            {
                //from.PlaySound(m_CombinationSound);
                try
                {
                    Effects.PlaySound(this.Location, this.Map, this.m_CombinationSound);
                }
                catch
                {
                }

                BaseXmlSpawner.ApplyObjectStringProperties(null, this.m_TargetProperty, this.m_TargetItem, from, this, out status_str);
            }
        }
    }
}