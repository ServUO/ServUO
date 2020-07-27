using Server.Multis;
using Server.Network;
using System.Collections.Generic;

namespace Server.Items
{
    public class BasketOfHerbs : Item
    {
        public override int LabelNumber => 1075493; // Basket of Herbs

        private static readonly Dictionary<Mobile, BasketOfHerbs> _Table = new Dictionary<Mobile, BasketOfHerbs>();
        private SkillMod m_SkillMod;

        [Constructable]
        public BasketOfHerbs()
            : base(0x194F)
        {
            LootType = LootType.Blessed;
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (IsLockedDown || IsSecure)
            {
                if (!from.InRange(GetWorldLocation(), 1))
                {
                    from.LocalOverheadMessage(MessageType.Regular, 0x3B2, 1019045); // I can't reach that.
                }
                else
                {
                    BaseHouse house = BaseHouse.FindHouseAt(this);

                    if (house != null && house.IsOwner(from) && !_Table.ContainsKey(from))
                        AddBonus(from);
                }
            }
            else
            {
                from.SendLocalizedMessage(502692); // This must be in a house and be locked down to work.
            }
        }

        public override void OnRemoved(object parent)
        {
            if (m_SkillMod != null)
            {
                if (_Table.ContainsKey(m_SkillMod.Owner))
                {
                    _Table.Remove(m_SkillMod.Owner);
                }

                m_SkillMod.Remove();
            }

            m_SkillMod = null;

            base.OnRemoved(parent);
        }

        public static void CheckBonus(Mobile m)
        {
            if (m != null && _Table.ContainsKey(m) && _Table[m] != null)
            {
                _Table[m].RemoveBonus();
            }
        }

        public void AddBonus(Mobile m)
        {
            m_SkillMod = new DefaultSkillMod(SkillName.Cooking, true, 10)
            {
                ObeyCap = true
            };

            m.AddSkillMod(m_SkillMod);

            _Table[m] = this;

            m.SendLocalizedMessage(1075540); // The scent of fresh herbs begins to fill your home...
        }

        public void RemoveBonus()
        {
            m_SkillMod.Owner.SendLocalizedMessage(1075541); // The scent of herbs gradually fades away...

            if (_Table.ContainsKey(m_SkillMod.Owner))
            {
                _Table.Remove(m_SkillMod.Owner);
            }

            m_SkillMod.Remove();
            m_SkillMod = null;            
        }

        public BasketOfHerbs(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();
        }
    }
}
