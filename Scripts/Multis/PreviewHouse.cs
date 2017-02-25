using System;
using System.Collections.Generic;
using Server.Items;

namespace Server.Multis
{
    public class PreviewHouse : BaseMulti
    {
        private List<Item> m_Components;
        private Timer m_Timer;
        public PreviewHouse(int multiID)
            : base(multiID)
        {
            this.m_Components = new List<Item>();

            MultiComponentList mcl = this.Components;

            for (int i = 1; i < mcl.List.Length; ++i)
            {
                MultiTileEntry entry = mcl.List[i];

                if (entry.m_Flags == 0)
                {
                    Item item = new Static((int)entry.m_ItemID);

                    item.MoveToWorld(new Point3D(this.X + entry.m_OffsetX, this.Y + entry.m_OffsetY, this.Z + entry.m_OffsetZ), this.Map);

                    this.m_Components.Add(item);
                }
            }

            if (multiID >= 0x13ec && multiID <= 0x147b)
            {
                AddSignAndPost(mcl);
                AddExteriorStairs(mcl);
            }

            this.m_Timer = new DecayTimer(this);
            this.m_Timer.Start();
        }

        public void AddSignAndPost(MultiComponentList mcl)
        {
            int xoffset = mcl.Min.X;
            int y = mcl.Height - 1 - mcl.Center.Y;

            Item signpost = new Static((int)9);
            signpost.MoveToWorld(new Point3D(X + xoffset, Y + y, Z + 7), this.Map);
            this.m_Components.Add(signpost);


            xoffset = Components.Min.X;
            y = Components.Height - Components.Center.Y;

            Item signhanger = new Static((int)0xB98);
            signhanger.MoveToWorld(new Point3D(X + xoffset, Y + y, Z + 7), this.Map);
            this.m_Components.Add(signhanger);

            Item housesign = new Static((int)0xBD2);
            housesign.MoveToWorld(new Point3D(X + xoffset, Y + y, Z + 7), this.Map);
            this.m_Components.Add(housesign);
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
                Item stair = new Static((int)0x751);
                stair.MoveToWorld(new Point3D(x - xCenter, y - yCenter, 0), this.Map);
                this.m_Components.Add(stair);
            }
        }


        public PreviewHouse(Serial serial)
            : base(serial)
        {
        }

        public override void OnLocationChange(Point3D oldLocation)
        {
            base.OnLocationChange(oldLocation);

            if (this.m_Components == null)
                return;

            int xOffset = this.X - oldLocation.X;
            int yOffset = this.Y - oldLocation.Y;
            int zOffset = this.Z - oldLocation.Z;

            for (int i = 0; i < this.m_Components.Count; ++i)
            {
                Item item = this.m_Components[i];

                item.MoveToWorld(new Point3D(item.X + xOffset, item.Y + yOffset, item.Z + zOffset), this.Map);
            }
        }

        public override void OnMapChange()
        {
            base.OnMapChange();

            if (this.m_Components == null)
                return;

            for (int i = 0; i < this.m_Components.Count; ++i)
            {
                Item item = this.m_Components[i];

                item.Map = this.Map;
            }
        }

        public override void OnDelete()
        {
            base.OnDelete();

            if (this.m_Components == null)
                return;

            for (int i = 0; i < this.m_Components.Count; ++i)
            {
                Item item = this.m_Components[i];

                item.Delete();
            }
        }

        public override void OnAfterDelete()
        {
            if (this.m_Timer != null)
                this.m_Timer.Stop();

            this.m_Timer = null;

            base.OnAfterDelete();
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version

            writer.Write(this.m_Components);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch ( version )
            {
                case 0:
                    {
                        this.m_Components = reader.ReadStrongItemList();

                        break;
                    }
            }

            Timer.DelayCall(TimeSpan.Zero, new TimerCallback(this.Delete));
        }

        private class DecayTimer : Timer
        {
            private readonly Item m_Item;
            public DecayTimer(Item item)
                : base(TimeSpan.FromSeconds(20.0))
            {
                this.m_Item = item;
                this.Priority = TimerPriority.OneSecond;
            }

            protected override void OnTick()
            {
                this.m_Item.Delete();
            }
        }
    }
}