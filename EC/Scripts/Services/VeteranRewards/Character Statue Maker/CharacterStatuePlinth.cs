using System;
using Server.Gumps;
using Server.Mobiles;
using Server.Multis;

namespace Server.Items
{
    public class CharacterStatuePlinth : Static, IAddon
    {
        private CharacterStatue m_Statue;
        public CharacterStatuePlinth(CharacterStatue statue)
            : base(0x32F2)
        {
            this.m_Statue = statue;

            this.InvalidateHue();
        }

        public CharacterStatuePlinth(Serial serial)
            : base(serial)
        {
        }

        public Item Deed
        {
            get
            {
                return new CharacterStatueDeed(this.m_Statue);
            }
        }
        public override int LabelNumber
        {
            get
            {
                return 1076201;
            }
        }// Character Statue
        public override void OnAfterDelete()
        {
            base.OnAfterDelete();

            if (this.m_Statue != null && !this.m_Statue.Deleted)
                this.m_Statue.Delete();
        }

        public override void OnMapChange()
        {
            if (this.m_Statue != null)
                this.m_Statue.Map = this.Map;
        }

        public override void OnLocationChange(Point3D oldLocation)
        {
            if (this.m_Statue != null)
                this.m_Statue.Location = new Point3D(this.X, this.Y, this.Z + 5);
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (this.m_Statue != null)
                from.SendGump(new CharacterPlinthGump(this.m_Statue));
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt((int)0); // version

            writer.Write((Mobile)this.m_Statue);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();

            this.m_Statue = reader.ReadMobile() as CharacterStatue;

            if (this.m_Statue == null || this.m_Statue.SculptedBy == null || this.Map == Map.Internal)
            {
                Timer.DelayCall(TimeSpan.Zero, new TimerCallback(Delete));
            }
        }

        public void InvalidateHue()
        {
            if (this.m_Statue != null)
                this.Hue = 0xB8F + (int)this.m_Statue.StatueType * 4 + (int)this.m_Statue.Material;
        }

        public virtual bool CouldFit(IPoint3D p, Map map)
        {
            Point3D point = new Point3D(p.X, p.Y, p.Z);

            if (map == null || !map.CanFit(point, 20))
                return false;

            BaseHouse house = BaseHouse.FindHouseAt(point, map, 20);

            if (house == null)
                return false;

            AddonFitResult result = CharacterStatueTarget.CheckDoors(point, 20, house);

            if (result == AddonFitResult.Valid)
                return true;

            return false;
        }

        private class CharacterPlinthGump : Gump
        {
            public CharacterPlinthGump(CharacterStatue statue)
                : base(60, 30)
            {
                this.Closable = true;
                this.Disposable = true;
                this.Dragable = true;
                this.Resizable = false;

                this.AddPage(0);
                this.AddImage(0, 0, 0x24F4);
                this.AddHtml(55, 50, 150, 20, statue.Name, false, false);
                this.AddHtml(55, 75, 150, 20, statue.SculptedOn.ToString(), false, false);
                this.AddHtmlLocalized(55, 100, 150, 20, this.GetTypeNumber(statue.StatueType), 0, false, false);
            }

            public int GetTypeNumber(StatueType type)
            {
                switch ( type )
                {
                    case StatueType.Marble:
                        return 1076181;
                    case StatueType.Jade:
                        return 1076180;
                    case StatueType.Bronze:
                        return 1076230;
                    default:
                        return 1076181;
                }
            }
        }
    }
}