using System;
using System.Collections;
using Server.Network;
using Server.Spells;
using Server.Targeting;

namespace Server.Items
{
    public enum WandEffect
    {
        Clumsiness,
        Identification,
        Healing,
        Feeblemindedness,
        Weakness,
        MagicArrow,
        Harming,
        Fireball,
        GreaterHealing,
        Lightning,
        ManaDraining
    }

    public abstract class BaseWand : BaseBashing, ITokunoDyable
    {
        private WandEffect m_WandEffect;
        private int m_Charges;
        public BaseWand(WandEffect effect, int minCharges, int maxCharges)
            : base(Utility.RandomList(0xDF2, 0xDF3, 0xDF4, 0xDF5))
        {
            this.Weight = 1.0;
            this.Effect = effect;
            this.Charges = Utility.RandomMinMax(minCharges, maxCharges);
            this.Attributes.SpellChanneling = 1;
            this.Attributes.CastSpeed = -1;
            this.WeaponAttributes.MageWeapon = Utility.RandomMinMax(1, 10);
        }

        public BaseWand(Serial serial)
            : base(serial)
        {
        }

        public override WeaponAbility PrimaryAbility
        {
            get
            {
                return WeaponAbility.Dismount;
            }
        }
        public override WeaponAbility SecondaryAbility
        {
            get
            {
                return WeaponAbility.Disarm;
            }
        }
        public override int AosStrengthReq
        {
            get
            {
                return 5;
            }
        }
        public override int AosMinDamage
        {
            get
            {
                return 9;
            }
        }
        public override int AosMaxDamage
        {
            get
            {
                return 11;
            }
        }
        public override int AosSpeed
        {
            get
            {
                return 40;
            }
        }
        public override float MlSpeed
        {
            get
            {
                return 2.75f;
            }
        }
        public override int OldStrengthReq
        {
            get
            {
                return 0;
            }
        }
        public override int OldMinDamage
        {
            get
            {
                return 2;
            }
        }
        public override int OldMaxDamage
        {
            get
            {
                return 6;
            }
        }
        public override int OldSpeed
        {
            get
            {
                return 35;
            }
        }
        public override int InitMinHits
        {
            get
            {
                return 31;
            }
        }
        public override int InitMaxHits
        {
            get
            {
                return 110;
            }
        }
        public virtual TimeSpan GetUseDelay
        {
            get
            {
                return TimeSpan.FromSeconds(4.0);
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public WandEffect Effect
        {
            get
            {
                return this.m_WandEffect;
            }
            set
            {
                this.m_WandEffect = value;
                this.InvalidateProperties();
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
        public void ConsumeCharge(Mobile from)
        {
            --this.Charges;

            if (this.Charges == 0)
                from.SendLocalizedMessage(1019073); // This item is out of charges.

            this.ApplyDelayTo(from);
        }

        public virtual void ApplyDelayTo(Mobile from)
        {
            from.BeginAction(typeof(BaseWand));
            Timer.DelayCall(this.GetUseDelay, new TimerStateCallback(ReleaseWandLock_Callback), from);
        }

        public virtual void ReleaseWandLock_Callback(object state)
        {
            ((Mobile)state).EndAction(typeof(BaseWand));
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!from.CanBeginAction(typeof(BaseWand)))
            {
                from.SendLocalizedMessage(1070860); // You must wait a moment for the wand to recharge.
                return;
            }

            if (this.Parent == from)
            {
                if (this.Charges > 0)
                    this.OnWandUse(from);
                else
                    from.SendLocalizedMessage(1019073); // This item is out of charges.
            }
            else
            {
                from.SendLocalizedMessage(502641); // You must equip this item to use it.
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version

            writer.Write((int)this.m_WandEffect);
            writer.Write((int)this.m_Charges);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch ( version )
            {
                case 0:
                    {
                        this.m_WandEffect = (WandEffect)reader.ReadInt();
                        this.m_Charges = (int)reader.ReadInt();

                        break;
                    }
            }
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            switch ( this.m_WandEffect )
            {
                case WandEffect.Clumsiness:
                    list.Add(1017326, this.m_Charges.ToString());
                    break; // clumsiness charges: ~1_val~
                case WandEffect.Identification:
                    list.Add(1017350, this.m_Charges.ToString());
                    break; // identification charges: ~1_val~
                case WandEffect.Healing:
                    list.Add(1017329, this.m_Charges.ToString());
                    break; // healing charges: ~1_val~
                case WandEffect.Feeblemindedness:
                    list.Add(1017327, this.m_Charges.ToString());
                    break; // feeblemind charges: ~1_val~
                case WandEffect.Weakness:
                    list.Add(1017328, this.m_Charges.ToString());
                    break; // weakness charges: ~1_val~
                case WandEffect.MagicArrow:
                    list.Add(1060492, this.m_Charges.ToString());
                    break; // magic arrow charges: ~1_val~
                case WandEffect.Harming:
                    list.Add(1017334, this.m_Charges.ToString());
                    break; // harm charges: ~1_val~
                case WandEffect.Fireball:
                    list.Add(1060487, this.m_Charges.ToString());
                    break; // fireball charges: ~1_val~
                case WandEffect.GreaterHealing:
                    list.Add(1017330, this.m_Charges.ToString());
                    break; // greater healing charges: ~1_val~
                case WandEffect.Lightning:
                    list.Add(1060491, this.m_Charges.ToString());
                    break; // lightning charges: ~1_val~
                case WandEffect.ManaDraining:
                    list.Add(1017339, this.m_Charges.ToString());
                    break; // mana drain charges: ~1_val~
            }
        }

        public override void OnSingleClick(Mobile from)
        {
            ArrayList attrs = new ArrayList();

            if (this.DisplayLootType)
            {
                if (this.LootType == LootType.Blessed)
                    attrs.Add(new EquipInfoAttribute(1038021)); // blessed
                else if (this.LootType == LootType.Cursed)
                    attrs.Add(new EquipInfoAttribute(1049643)); // cursed
            }

            if (!this.Identified)
            {
                attrs.Add(new EquipInfoAttribute(1038000)); // Unidentified
            }
            else
            {
                int num = 0;

                switch ( this.m_WandEffect )
                {
                    case WandEffect.Clumsiness:
                        num = 3002011;
                        break;
                    case WandEffect.Identification:
                        num = 1044063;
                        break;
                    case WandEffect.Healing:
                        num = 3002014;
                        break;
                    case WandEffect.Feeblemindedness:
                        num = 3002013;
                        break;
                    case WandEffect.Weakness:
                        num = 3002018;
                        break;
                    case WandEffect.MagicArrow:
                        num = 3002015;
                        break;
                    case WandEffect.Harming:
                        num = 3002022;
                        break;
                    case WandEffect.Fireball:
                        num = 3002028;
                        break;
                    case WandEffect.GreaterHealing:
                        num = 3002039;
                        break;
                    case WandEffect.Lightning:
                        num = 3002040;
                        break;
                    case WandEffect.ManaDraining:
                        num = 3002041;
                        break;
                }

                if (num > 0)
                    attrs.Add(new EquipInfoAttribute(num, this.m_Charges));
            }

            int number;

            if (this.Name == null)
            {
                number = 1017085;
            }
            else
            {
                this.LabelTo(from, this.Name);
                number = 1041000;
            }

            if (attrs.Count == 0 && this.Crafter == null && this.Name != null)
                return;

            EquipmentInfo eqInfo = new EquipmentInfo(number, this.Crafter, false, (EquipInfoAttribute[])attrs.ToArray(typeof(EquipInfoAttribute)));

            from.Send(new DisplayEquipmentInfo(this, eqInfo));
        }

        public void Cast(Spell spell)
        {
            bool m = this.Movable;

            this.Movable = false;
            spell.Cast();
            this.Movable = m;
        }

        public virtual void OnWandUse(Mobile from)
        {
            from.Target = new WandTarget(this);
        }

        public virtual void DoWandTarget(Mobile from, object o)
        {
            if (this.Deleted || this.Charges <= 0 || this.Parent != from || o is StaticTarget || o is LandTarget)
                return;

            if (this.OnWandTarget(from, o))
                this.ConsumeCharge(from);
        }

        public virtual bool OnWandTarget(Mobile from, object o)
        {
            return true;
        }
    }
}