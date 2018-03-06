using System;
using System.Collections.Generic;
using Server.ContextMenus;
using Server.Mobiles;

namespace Server.Items
{
    [FlipableAttribute(0x2790, 0x27DB)]
    public class LeatherNinjaBelt : BaseWaist, IDyable, INinjaWeapon
    {
        private int m_UsesRemaining;
        private Poison m_Poison;
        private int m_PoisonCharges;
        [Constructable]
        public LeatherNinjaBelt()
            : base(0x2790)

        {
            Weight = 1.0;
            Layer = Layer.Waist;
        }

        public LeatherNinjaBelt(Serial serial)
            : base(serial)
        {
        }

        public override CraftResource DefaultResource
        {
            get
            {
                return CraftResource.RegularLeather;
            }
        }
        public virtual int WrongAmmoMessage
        {
            get
            {
                return 1063301;
            }
        }//You can only place shuriken in a ninja belt.
        public virtual int NoFreeHandMessage
        {
            get
            {
                return 1063299;
            }
        }//You must have a free hand to throw shuriken.
        public virtual int EmptyWeaponMessage
        {
            get
            {
                return 1063297;
            }
        }//You have no shuriken in your ninja belt!
        public virtual int RecentlyUsedMessage
        {
            get
            {
                return 1063298;
            }
        }//You cannot throw another shuriken yet.
        public virtual int FullWeaponMessage
        {
            get
            {
                return 1063302;
            }
        }//You cannot add any more shuriken.
        public virtual int WeaponMinRange
        {
            get
            {
                return 2;
            }
        }
        public virtual int WeaponMaxRange
        {
            get
            {
                return 10;
            }
        }
        public virtual int WeaponDamage
        {
            get
            {
                return Utility.RandomMinMax(3, 5);
            }
        }
        public virtual Type AmmoType
        {
            get
            {
                return typeof(Shuriken);
            }
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
        [CommandProperty(AccessLevel.GameMaster)]
        public Poison Poison
        {
            get
            {
                return m_Poison;
            }
            set
            {
                m_Poison = value;
                InvalidateProperties();
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public int PoisonCharges
        {
            get
            {
                return m_PoisonCharges;
            }
            set
            {
                m_PoisonCharges = value;
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
        public void AttackAnimation(Mobile from, Mobile to)
        {
            if (from.Body.IsHuman)
            {
                if (Core.SA)
                {
                    from.Animate(AnimationType.Attack, 4);
                }
                else
                {
                    from.Animate(from.Mounted ? 26 : 9, 7, 1, true, false, 0);
                }                
            }

            from.PlaySound(0x23A);
            from.MovingEffect(to, 0x27AC, 1, 0, false, false);
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            list.Add(1060584, m_UsesRemaining.ToString()); // uses remaining: ~1_val~

            if (m_Poison != null && m_PoisonCharges > 0)
                list.Add(1062412 + m_Poison.Level, m_PoisonCharges.ToString());
        }

        public override bool OnEquip(Mobile from)
        {
            if (base.OnEquip(from))
            {
                from.SendLocalizedMessage(1070785); // Double click this item each time you wish to throw a shuriken.
                return true;
            }
            return false;
        }

        public override void OnDoubleClick(Mobile from)
        {
            NinjaWeapon.AttemptShoot((PlayerMobile)from, this);
        }

        public override void GetContextMenuEntries(Mobile from, List<ContextMenuEntry> list)
        {
            base.GetContextMenuEntries(from, list);

            if (IsChildOf(from))
            {
                list.Add(new NinjaWeapon.LoadEntry(this, 6222));
                list.Add(new NinjaWeapon.UnloadEntry(this, 6223));
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0);

            writer.Write((int)m_UsesRemaining);

            Poison.Serialize(m_Poison, writer);
            writer.Write((int)m_PoisonCharges);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch ( version )
            {
                case 0:
                    {
                        m_UsesRemaining = reader.ReadInt();

                        m_Poison = Poison.Deserialize(reader);
                        m_PoisonCharges = reader.ReadInt();

                        break;
                    }
            }
        }
    }
}
