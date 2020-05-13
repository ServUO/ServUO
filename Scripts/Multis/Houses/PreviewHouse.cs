using Server.Items;
using System;
using System.Collections.Generic;

namespace Server.Multis
{
    public class PreviewHouse : BaseMulti
    {
        private List<Item> m_Components;
        private Timer m_Timer;

        public PreviewHouse(int multiID)
            : base(multiID)
        {
            m_Components = new List<Item>();

            MultiComponentList mcl = Components;

            for (int i = 1; i < mcl.List.Length; ++i)
            {
                MultiTileEntry entry = mcl.List[i];

                if (entry.m_Flags == 0)
                {
                    Item item = new Static((int)entry.m_ItemID);

                    item.MoveToWorld(new Point3D(X + entry.m_OffsetX, Y + entry.m_OffsetY, Z + entry.m_OffsetZ), Map);

                    m_Components.Add(item);
                }
            }

            if (multiID >= 0x13ec && multiID <= 0x147d)
            {
                AddSignAndPost(mcl);
                AddExteriorStairs(mcl);
            }

            m_Timer = new DecayTimer(this);
            m_Timer.Start();
        }

        public void AddSignAndPost(MultiComponentList mcl)
        {
            int xoffset = mcl.Min.X;
            int y = mcl.Height - 1 - mcl.Center.Y;

            Item signpost = new Static(9);
            signpost.MoveToWorld(new Point3D(X + xoffset, Y + y, Z + 7), Map);
            m_Components.Add(signpost);


            xoffset = Components.Min.X;
            y = Components.Height - Components.Center.Y;

            Item signhanger = new Static(0xB98);
            signhanger.MoveToWorld(new Point3D(X + xoffset, Y + y, Z + 7), Map);
            m_Components.Add(signhanger);

            Item housesign = new Static(0xBD2);
            housesign.MoveToWorld(new Point3D(X + xoffset, Y + y, Z + 7), Map);
            m_Components.Add(housesign);
        }

        public void AddExteriorStairs(MultiComponentList mcl)
        {
            // this won't work correctly without declaring a new mcl so it can then be resized
            MultiComponentList mclNew = new MultiComponentList(MultiData.GetComponents(ItemID));

            mclNew.Resize(mclNew.Width, mclNew.Height + 1);

            int xCenter = mcl.Center.X;
            int yCenter = mcl.Center.Y;
            int y = mcl.Height;

            for (int x = 1; x < mclNew.Width; ++x)
            {
                Item stair = new Static(0x751);
                stair.MoveToWorld(new Point3D(x - xCenter, y - yCenter, 0), Map);
                m_Components.Add(stair);
            }
        }


        public PreviewHouse(Serial serial)
            : base(serial)
        {
        }

        public override void OnLocationChange(Point3D oldLocation)
        {
            base.OnLocationChange(oldLocation);

            if (m_Components == null)
                return;

            int xOffset = X - oldLocation.X;
            int yOffset = Y - oldLocation.Y;
            int zOffset = Z - oldLocation.Z;

            for (int i = 0; i < m_Components.Count; ++i)
            {
                Item item = m_Components[i];

                item.MoveToWorld(new Point3D(item.X + xOffset, item.Y + yOffset, item.Z + zOffset), Map);
            }
        }

        public override void OnMapChange()
        {
            base.OnMapChange();

            if (m_Components == null)
                return;

            for (int i = 0; i < m_Components.Count; ++i)
            {
                Item item = m_Components[i];

                item.Map = Map;
            }
        }

        public override void OnDelete()
        {
            base.OnDelete();

            if (m_Components == null)
                return;

            for (int i = 0; i < m_Components.Count; ++i)
            {
                Item item = m_Components[i];

                item.Delete();
            }
        }

        public override void OnAfterDelete()
        {
            if (m_Timer != null)
                m_Timer.Stop();

            m_Timer = null;

            base.OnAfterDelete();
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version

            writer.Write(m_Components);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch (version)
            {
                case 0:
                    {
                        m_Components = reader.ReadStrongItemList();

                        break;
                    }
            }

            Timer.DelayCall(TimeSpan.Zero, Delete);
        }

        private class DecayTimer : Timer
        {
            private readonly Item m_Item;
            public DecayTimer(Item item)
                : base(TimeSpan.FromSeconds(20.0))
            {
                m_Item = item;
                Priority = TimerPriority.OneSecond;
            }

            protected override void OnTick()
            {
                m_Item.Delete();
            }
        }
    }
}
