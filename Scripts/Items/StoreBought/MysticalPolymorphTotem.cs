using Server.Targeting;
using System;

namespace Server.Items
{
    public class MysticalPolymorphTotem : Item
    {
        public override int LabelNumber => 1158780;  // Mystical Polymorph Totem

        private int m_Body = 0;
        private int m_Hue = -1;

        [CommandProperty(AccessLevel.GameMaster)]
        public int Duration { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public string CostumeCreatureName { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool Transformed { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public int CostumeBody
        {
            get { return m_Body; }
            set { m_Body = value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int CostumeHue
        {
            get { return m_Hue; }
            set { m_Hue = value; }
        }

        [Constructable]
        public MysticalPolymorphTotem()
            : base(0xA276)
        {
            LootType = LootType.Blessed;
        }

        public MysticalPolymorphTotem(Serial serial)
            : base(serial)
        {
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            if (CostumeCreatureName != null)
            {
                list.Add(1158707, string.Format("{0}", CostumeCreatureName)); // a ~1_name~ costume
            }
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!IsChildOf(from.Backpack))
            {
                from.SendLocalizedMessage(1060640); // The item must be in your backpack to use it.                
            }
            else
            {
                if (CostumeBody == 0)
                {
                    from.SendLocalizedMessage(1158781); // Target the costume that you wish to merge with this Mystical Polymorph Totem.  Please be aware that this selection cannot be undone.
                    from.Target = new InternalTarget(this);
                }
                else
                {
                    if (!Transformed)
                    {
                        EnMask(from);
                    }
                    else
                    {
                        DeMask(from);
                    }
                }
            }
        }

        public override bool DropToWorld(Mobile from, Point3D p)
        {
            bool drop = base.DropToWorld(from, p);

            if (Transformed)
            {
                DeMask(from);
            }

            return drop;
        }

        private Timer m_Timer;

        private void EnMask(Mobile from)
        {
            if (from.Mounted || from.Flying)
            {
                from.SendLocalizedMessage(1010097); // You cannot use this while mounted or flying. 
            }
            else if (from.IsBodyMod || from.HueMod > -1)
            {
                from.SendLocalizedMessage(1158010); // You cannot use that item in this form.
            }
            else
            {
                Duration = 28800;

                if (m_Timer == null || !m_Timer.Running)
                    m_Timer = Timer.DelayCall(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1), delegate { Slice(from); });

                BuffInfo.AddBuff(from, new BuffInfo(BuffIcon.MysticalPolymorphTotem, 1158780, 1158017, TimeSpan.FromSeconds(Duration), from, CostumeCreatureName));

                ItemID = 0xA20B;
                from.BodyMod = m_Body;
                from.HueMod = m_Hue;
                Transformed = true;
            }
        }

        public virtual void Slice(Mobile from)
        {
            if (Duration > 0)
                Duration--;
            else
            {
                DeMask(from);

                if (m_Timer != null)
                    m_Timer.Stop();

                m_Timer = null;
            }
        }

        private void DeMask(Mobile from)
        {
            ItemID = 0xA276;
            from.BodyMod = 0;
            from.HueMod = -1;
            Transformed = false;
            Effects.SendLocationParticles(EffectItem.Create(from.Location, from.Map, EffectItem.DefaultDuration), 0x3728, 8, 20, 5042);
            from.PlaySound(250);
            BuffInfo.RemoveBuff(from, BuffIcon.MysticalPolymorphTotem);
        }

        private class InternalTarget : Target
        {
            private readonly MysticalPolymorphTotem m_Totem;

            public InternalTarget(MysticalPolymorphTotem totem)
                : base(12, true, TargetFlags.None)
            {
                m_Totem = totem;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (m_Totem.Deleted)
                    return;

                if (!m_Totem.IsChildOf(from.Backpack))
                {
                    from.SendLocalizedMessage(1060640); // The item must be in your backpack to use it.
                    return;
                }

                if (targeted is BaseCostume)
                {
                    BaseCostume costume = targeted as BaseCostume;

                    m_Totem.CostumeCreatureName = costume.CreatureName;
                    m_Totem.CostumeBody = costume.CostumeBody;
                    m_Totem.CostumeHue = costume.Hue;

                    m_Totem.InvalidateProperties();

                    costume.Delete();
                }
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);

            writer.Write(CostumeCreatureName);
            writer.Write(CostumeBody);
            writer.Write(CostumeHue);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();

            CostumeCreatureName = reader.ReadString();
            CostumeBody = reader.ReadInt();
            CostumeHue = reader.ReadInt();

            ItemID = 0xA276;
        }
    }
}
