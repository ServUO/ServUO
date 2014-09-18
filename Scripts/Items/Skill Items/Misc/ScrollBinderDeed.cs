using System;
using Server.Network;
using Server.Prompts;
using Server.Items;
using Server.Targeting;

namespace Server.Items
{
    public class ScrollBinderTarget : Target // Create our targeting class (which we derive from the base target class)
    {
        private ScrollBinderDeed m_Deed;

        public ScrollBinderTarget(ScrollBinderDeed deed)
            : base(1, false, TargetFlags.None)
        {
            m_Deed = deed;
        }

        protected override void OnTarget(Mobile from, object target) // Override the protected OnTarget() for our feature
        {
            if (this.m_Deed.Deleted || this.m_Deed.RootParent != from)
                return;

            if (target is Item)
            {
                Item item = (Item)target;

                if (item.RootParent != from)
                    from.SendMessage("That must be in your pack");


                //Shortcut the targets ... each checks to make sure the values are in scope, 
                //then passes back to Scrollbinder that called it Via a refference

                else if (target is PowerScroll)
                {
                    if (((PowerScroll)target).Value <= 120)
                        m_Deed.Calculate((PowerScroll)target, from);
                    else
                        from.SendMessage("A higher value does not exist");
                    return;
                }

                else if (target is ScrollofTranscendence)
                {
                    m_Deed.Calculate((ScrollofTranscendence)target, from);
                    return;
                }

                else if (target is StatCapScroll)
                {
                    if (((StatCapScroll)target).Value <= 250)
                        m_Deed.Calculate((StatCapScroll)target, from);
                    else
                        from.SendMessage("A higher value does not exist");
                    return;
                }
                else
                {
                    from.SendMessage("That is not an acceptable type"); // returns fail message to the player

                }
            }
        }
    }

    public class ScrollBinderDeed : Item // Create the item class which is derived from the base item class
    {
        private SkillName m_skillname;   // name of the skill to which this is associated

        private double m_skillvalue = 0.0;// Value of given skill/ stat

        private double m_maxneeded = 0.0;// total scrolls needed to create a new special scroll

        private double m_count = 0.0;// current count of scrolls added/ points of Trans added

        private Item check;// item refference - decides if the new target is accepable for combination

        [CommandProperty(AccessLevel.GameMaster)]
        public SkillName SName
        {
            get { return m_skillname; }

            set { m_skillname = value; InvalidateProperties(); }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public double SkillValue
        {
            get { return m_skillvalue; }

            set { m_skillvalue = value; InvalidateProperties(); }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public double Counter
        {
            get { return m_count; }

            set { m_count = value; InvalidateProperties(); }
        }

        [Constructable]
        public ScrollBinderDeed()
            : base(0x14F0)
        {

            this.Name = "a Scroll Binder";
            Weight = 1.0;
            Hue = 334;
            LootType = LootType.Cursed;
            ItemID = (0x014F0);
            InvalidateProperties();
        }

        public ScrollBinderDeed(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)1); // version

            writer.Write((int)m_skillname);
            writer.Write((double)m_skillvalue);
            writer.Write((double)m_maxneeded);
            writer.Write((double)m_count);

            // below - write the switch controller for the refference type
            if (check != null)
            {
                if (check is PowerScroll)
                    writer.Write((int)1);
                if (check is StatCapScroll)
                    writer.Write((int)2);
                if (check is ScrollofTranscendence)
                    writer.Write((int)3);
            }
            else
                writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
            switch (version)
            {
                case 1:
                    {
                        m_skillname = (SkillName)reader.ReadInt();
                        m_skillvalue = reader.ReadDouble();
                        m_maxneeded = reader.ReadDouble();
                        m_count = reader.ReadDouble();

                        //below - didn't waste time looking for how to serialize an invisible item and recall it
                        // so used an embedded switch system to create a new comparable refference
                        int i = reader.ReadInt();
                        if (i != 0)
                        {
                            switch (i)
                            {
                                case 1:
                                    {
                                        check = new PowerScroll(m_skillname, m_skillvalue);
                                        break;
                                    }
                                case 2:
                                    {
                                        check = new StatCapScroll(Convert.ToInt32(m_skillvalue));
                                        break;
                                    }
                                case 3:
                                    {
                                        check = new ScrollofTranscendence(m_skillname, m_skillvalue);
                                        break;
                                    }
                            }

                        }
                    }
                    goto case 0;
                case 0:
                    { break; }

            }
            InvalidateProperties();
        }

        public void Calculate(SpecialScroll scroll, Mobile from)// does the calculations for Scroll binder
        {
            if (check == null)// triggers if the scroll binder has not been acceptably used before
            {
                check = scroll;
                m_skillname = scroll.Skill;
                m_skillvalue = scroll.Value;
                if (scroll is PowerScroll || scroll is StatCapScroll) // PS and Stat scroll both use base SkillNames
                {
                    m_count += 1;
                    Create(m_count, from);
                }
                else
                    Create(m_skillvalue, from); // Scroll of Trans can use the value for the addition

                scroll.Delete();
                return;
            }
            // long if statement to ensure that both the target and the current refferences are the same type
            if ((check is PowerScroll && scroll is PowerScroll)
                || (check is StatCapScroll && scroll is StatCapScroll)
                || (check is ScrollofTranscendence && scroll is ScrollofTranscendence))
            {
                if (scroll.Skill == m_skillname) //same SkillName
                {
                    if (scroll is PowerScroll && (m_skillvalue == scroll.Value)) // Same Value so we aren't adding in 105s to 110s
                    {
                        m_skillname = scroll.Skill;
                        m_count += 1;

                        Create(m_count, from);
                        scroll.Delete();
                        return;
                    }
                    else if (scroll is StatCapScroll && (m_skillvalue == scroll.Value))// Same value so we aren't adding +20's and +5's
                    {
                        m_skillname = scroll.Skill;
                        m_count += 1;

                        Create(m_count, from);
                        scroll.Delete();
                        return;
                    }
                    else if (scroll is ScrollofTranscendence)// no value check herer since its cumlative
                    {
                        m_count += scroll.Value;

                        Create(m_count, from);
                        scroll.Delete();
                        return;

                    }
                    else
                    {
                        from.SendMessage("That is not the same skill amount");// different value message
                        return;
                    }
                }
                else
                {
                    from.SendMessage(" That does not have the same skill ");//different skillname message
                    return;
                }
            }
            else
            {
                from.SendMessage(" That is not the same type ");// different target and refference message 
                return;
            }
        }
        public override bool DisplayWeight { get { return false; } }

        private void Create(double amount, Mobile from)
        {
            if (check == null)// this should never be null at this point
                return;
            if (m_maxneeded != 0)// used as a check to verify value has been initialized
            {
                if (amount >= m_maxneeded)// check to see if Item needs to be created
                {
                    if (check is PowerScroll)
                        from.AddToBackpack(new PowerScroll(SName, (Convert.ToInt32(m_skillvalue) + 5)));
                    if (check is StatCapScroll)
                        from.AddToBackpack(new StatCapScroll(Convert.ToInt32(m_skillvalue) + 5));
                    if (check is ScrollofTranscendence)
                        from.AddToBackpack(new ScrollofTranscendence(m_skillname, m_maxneeded));

                    this.Delete(); // remove ScrollBinder
                    return;
                }
                InvalidateProperties(); // here to update properties from the Calculate Method
            }

            else  //Initializes the setup for storing the Scroll Binder Data and... sets the amount of each scroll needed
            {
                if (check is PowerScroll || check is StatCapScroll)
                {
                    int valid = Convert.ToInt32(m_skillvalue);
                    switch (valid)
                    {
                        //PowerScrolls
                        case 105:
                            {
                                m_maxneeded = 8;
                                break;
                            }
                        case 110:
                            {
                                m_maxneeded = 12;
                                break;
                            }
                        case 115:
                            {
                                m_maxneeded = 10;
                                break;
                            }
                        //StatCapScrolls (formula --> 225 + (stat adjument) = value)

                        case 230://5
                            {
                                m_maxneeded = 6;
                                break;
                            }
                        case 235://10
                            {
                                m_maxneeded = 8;
                                break;
                            }
                        case 240://15
                            {
                                m_maxneeded = 8;
                                break;
                            }
                        case 245://20
                            {
                                m_maxneeded = 5;
                                break;
                            }
                    }
                }
                if (check is ScrollofTranscendence)
                {
                    m_count += amount;
                    m_maxneeded = 10; // set maximum needed is  10 points of Trans scrolls... its fixed and doesn't change
                }
                InvalidateProperties();
                return;
            }
        }
        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            //list.Add(" (Double-Click to target scrolls) ");
            if (m_count == 0)
            { // Directions for use appears on item
                list.Add("Used to combine PS, Stat scrolls, or Scrolls of Transcendence \n ( Double-Click to target Scrolls )");
            }


            else
            { //displays according to Type stored so Players can see
                if (check is PowerScroll)
                    list.Add("{0} {1} :  {2} / {3}", m_skillvalue, m_skillname, m_count, m_maxneeded);

                if (check is StatCapScroll)
                    list.Add("+{0} Stats : {1} / {2}", (m_skillvalue - 225), m_count, m_maxneeded);

                if (check is ScrollofTranscendence)
                    list.Add("{0} Transcendence : {1} / {2}", m_skillname, m_count, m_maxneeded);
            }
        }

        public override void OnDoubleClick(Mobile from) // Override double click of the deed to call our target
        {
            if (!IsChildOf(from.Backpack)) // Make sure its in their pack
            {
                from.SendLocalizedMessage(1042001); // That must be in your pack for you to use it.
            }
            else
            {
                from.SendMessage("Which scroll would you like to add?");
                from.Target = new ScrollBinderTarget(this); // Call our target
            }
        }
    }
}