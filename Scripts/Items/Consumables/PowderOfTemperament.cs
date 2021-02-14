using Server.Network;
using Server.Targeting;

namespace Server.Items
{
    public class PowderOfTemperament : Item, IUsesRemaining
    {
        public static bool CanPOFJewelry = Config.Get("Loot.CanPOFJewelry", false);

        private int m_UsesRemaining;

        [Constructable]
        public PowderOfTemperament()
            : this(10)
        {
        }

        [Constructable]
        public PowderOfTemperament(int charges)
            : base(4102)
        {
            Weight = 1.0;
            Hue = 2419;
            UsesRemaining = charges;
        }

        public PowderOfTemperament(Serial serial)
            : base(serial)
        {
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int UsesRemaining
        {
            get
            {
                return m_UsesRemaining;
            }
            set
            {
                m_UsesRemaining = value;
                InvalidateProperties();
            }
        }
        public bool ShowUsesRemaining
        {
            get
            {
                return true;
            }
            set
            {
            }
        }
        public override int LabelNumber => 1049082;// powder of fortifying
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0);
            writer.Write(m_UsesRemaining);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch (version)
            {
                case 0:
                    {
                        m_UsesRemaining = reader.ReadInt();
                        break;
                    }
            }
        }

        public override void AddUsesRemainingProperties(ObjectPropertyList list)
        {
            list.Add(1060584, m_UsesRemaining.ToString()); // uses remaining: ~1_val~
        }

        public virtual void DisplayDurabilityTo(Mobile m)
        {
            LabelToAffix(m, 1017323, AffixType.Append, ": " + m_UsesRemaining.ToString()); // Durability
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (IsChildOf(from.Backpack))
                from.Target = new InternalTarget(this);
            else
                from.SendLocalizedMessage(1042001); // That must be in your pack for you to use it.
        }

        private class InternalTarget : Target
        {
            private readonly PowderOfTemperament m_Powder;
            public InternalTarget(PowderOfTemperament powder)
                : base(2, false, TargetFlags.None)
            {
                m_Powder = powder;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (m_Powder.Deleted || m_Powder.UsesRemaining <= 0)
                {
                    from.SendLocalizedMessage(1049086); // You have used up your powder of temperament.
                    return;
                }

                if (targeted is Item item)
                {
                    bool noGo = false;
                    int antique = 0;

                    if (!Engines.Craft.Repair.AllowsRepair(item, null) || (item is BaseJewel && !CanPOFJewelry))
                    {
                        from.SendLocalizedMessage(1049083); // You cannot use the powder on that item.
                        return;
                    }

                    #region SA
                    if (item is BaseWeapon weapon)
                    {
                        if (weapon.Attributes.Brittle > 0 || weapon.NegativeAttributes.Brittle > 0)
                            noGo = true;
                        antique = weapon.NegativeAttributes.Antique;
                    }
                    else if (item is BaseArmor armor)
                    {
                        if (armor.Attributes.Brittle > 0 || armor.NegativeAttributes.Brittle > 0)
                            noGo = true;
                        antique = armor.NegativeAttributes.Antique;
                    }
                    else if (item is BaseClothing clothing)
                    {
                        if (clothing.Attributes.Brittle > 0 || clothing.NegativeAttributes.Brittle > 0)
                            noGo = true;
                        antique = clothing.NegativeAttributes.Antique;
                    }
                    else if (item is BaseJewel jewel)
                    {
                        if (jewel.Attributes.Brittle > 0 || jewel.NegativeAttributes.Brittle > 0)
                            noGo = true;
                        antique = jewel.NegativeAttributes.Antique;
                    }
                    else if (item is BaseTalisman talisman && talisman.Attributes.Brittle > 0)
                    {
                        noGo = true;
                    }
                    if (noGo)
                    {
                        from.SendLocalizedMessage(1149799); //That cannot be used on brittle items.
                        return;
                    }
                    #endregion

                    if (item is IDurability wearable)
                    {
                        if (!wearable.CanFortify)
                        {
                            from.SendLocalizedMessage(1049083); // You cannot use the powder on that item.
                            return;
                        }

                        if ((item.IsChildOf(from.Backpack) || item.Parent == from) && m_Powder.IsChildOf(from.Backpack))
                        {
                            int origMaxHP = wearable.MaxHitPoints;
                            int origCurHP = wearable.HitPoints;

                            if (origMaxHP > 0)
                            {
                                int initMaxHP = antique <= 0 ? 255 : antique == 1 ? 250 : antique == 2 ? 200 : 150;

                                wearable.UnscaleDurability();

                                if (wearable.MaxHitPoints < initMaxHP)
                                {
                                    if (antique > 0)
                                    {
                                        wearable.MaxHitPoints = initMaxHP;
                                        wearable.HitPoints += initMaxHP;
                                    }
                                    else
                                    {
                                        int bonus = initMaxHP - wearable.MaxHitPoints;

                                        if (bonus > 10)
                                            bonus = 10;

                                        wearable.MaxHitPoints += bonus;
                                        wearable.HitPoints += bonus;
                                    }

                                    wearable.ScaleDurability();

                                    if (wearable.MaxHitPoints > 255)
                                        wearable.MaxHitPoints = 255;
                                    if (wearable.HitPoints > 255)
                                        wearable.HitPoints = 255;

                                    if (wearable.MaxHitPoints > origMaxHP)
                                    {
                                        from.SendLocalizedMessage(1049084); // You successfully use the powder on the item.
                                        from.PlaySound(0x247);

                                        --m_Powder.UsesRemaining;

                                        if (m_Powder.UsesRemaining <= 0)
                                        {
                                            from.SendLocalizedMessage(1049086); // You have used up your powder of fortifying.
                                            m_Powder.Delete();
                                        }

                                        if (antique > 0)
                                        {
                                            if (wearable is BaseWeapon bw) bw.NegativeAttributes.Antique++;
                                            if (wearable is BaseArmor ba) ba.NegativeAttributes.Antique++;
                                            if (wearable is BaseJewel bj) bj.NegativeAttributes.Antique++;
                                            if (wearable is BaseClothing bc) bc.NegativeAttributes.Antique++;
                                        }
                                    }
                                    else
                                    {
                                        wearable.MaxHitPoints = origMaxHP;
                                        wearable.HitPoints = origCurHP;
                                        from.SendLocalizedMessage(1049085); // The item cannot be improved any further.
                                    }
                                }
                                else
                                {
                                    from.SendLocalizedMessage(1049085); // The item cannot be improved any further.
                                    wearable.ScaleDurability();
                                }
                            }
                            else
                            {
                                from.SendLocalizedMessage(1049083); // You cannot use the powder on that item.
                            }
                        }
                        else
                        {
                            from.SendLocalizedMessage(1042001); // That must be in your pack for you to use it.
                        }
                    }
                }
                else
                {
                    from.SendLocalizedMessage(1049083); // You cannot use the powder on that item.
                }
            }
        }
    }
}
