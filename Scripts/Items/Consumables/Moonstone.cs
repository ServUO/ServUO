using System;
using Server.Mobiles;
using Server.Network;

namespace Server.Items
{
    public enum MoonstoneType
    {
        Felucca,
        Trammel
    }

    public class Moonstone : Item
    {
        private MoonstoneType m_Type;
        [Constructable]
        public Moonstone(MoonstoneType type)
            : base(0xF8B)
        {
            this.Weight = 1.0;
            this.m_Type = type;
        }

        public Moonstone(Serial serial)
            : base(serial)
        {
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public MoonstoneType Type
        {
            get
            {
                return this.m_Type;
            }
            set
            {
                this.m_Type = value;
                this.InvalidateProperties();
            }
        }
        public override int LabelNumber
        {
            get
            {
                return 1041490 + (int)this.m_Type;
            }
        }
        public override void OnSingleClick(Mobile from)
        {
            if (this.IsChildOf(from.Backpack))
            {
                this.Hue = Utility.RandomBirdHue();
                this.ProcessDelta();
                from.SendLocalizedMessage(1005398); // The stone's substance shifts as you examine it.
            }

            base.OnSingleClick(from);
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!this.IsChildOf(from.Backpack))
            {
                from.SendLocalizedMessage(1042001); // That must be in your pack for you to use it.
            }
            else if (from.Mounted)
            {
                from.SendLocalizedMessage(1005399); // You can not bury a stone while you sit on a mount.
            }
            else if (!from.Body.IsHuman)
            {
                from.SendLocalizedMessage(1005400); // You can not bury a stone in this form.
            }
            else if (Factions.Sigil.ExistsOn(from))
            {
                from.SendLocalizedMessage(1061632); // You can't do that while carrying the sigil.
            }
            else if (from.Map == this.GetTargetMap() || (from.Map != Map.Trammel && from.Map != Map.Felucca))
            {
                from.SendLocalizedMessage(1005401); // You cannot bury the stone here.
            }
            else if (from is PlayerMobile && ((PlayerMobile)from).Young)
            {
                from.SendLocalizedMessage(1049543); // You decide against traveling to Felucca while you are still young.
            }
            else if (from.Murderer)
            {
                from.SendLocalizedMessage(1005402); // The magic of the stone cannot be evoked by someone with blood on their hands.
            }
            else if (from.Criminal)
            {
                from.SendLocalizedMessage(1005403); // The magic of the stone cannot be evoked by the lawless.
            }
            else if (!Region.Find(from.Location, from.Map).IsDefault || !Region.Find(from.Location, this.GetTargetMap()).IsDefault)
            {
                from.SendLocalizedMessage(1005401); // You cannot bury the stone here.
            }
            else if (!this.GetTargetMap().CanFit(from.Location, 16))
            {
                from.SendLocalizedMessage(1005408); // Something is blocking the facet gate exit.
            }
            else
            {
                this.Movable = false;
                this.MoveToWorld(from.Location, from.Map);

                from.Animate(32, 5, 1, true, false, 0);

                new SettleTimer(this, from.Location, from.Map, this.GetTargetMap(), from).Start();
            }
        }

        public Map GetTargetMap()
        {
            return (this.m_Type == MoonstoneType.Felucca) ? Map.Felucca : Map.Trammel;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version

            writer.Write((int)this.m_Type);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch ( version )
            {
                case 0:
                    {
                        this.m_Type = (MoonstoneType)reader.ReadInt();

                        break;
                    }
            }
        }

        private class SettleTimer : Timer
        {
            private readonly Item m_Stone;
            private readonly Point3D m_Location;
            private readonly Map m_Map;
            private readonly Map m_TargetMap;
            private readonly Mobile m_Caster;
            private int m_Count;
            public SettleTimer(Item stone, Point3D loc, Map map, Map targetMap, Mobile caster)
                : base(TimeSpan.FromSeconds(2.5), TimeSpan.FromSeconds(1.0))
            {
                this.m_Stone = stone;

                this.m_Location = loc;
                this.m_Map = map;
                this.m_TargetMap = targetMap;

                this.m_Caster = caster;
            }

            protected override void OnTick()
            {
                ++this.m_Count;

                if (this.m_Count == 1)
                {
                    this.m_Stone.PublicOverheadMessage(MessageType.Regular, 0x3B2, 1005414); // The stone settles into the ground.
                }
                else if (this.m_Count >= 10)
                {
                    this.m_Stone.Location = new Point3D(this.m_Stone.X, this.m_Stone.Y, this.m_Stone.Z - 1);

                    if (this.m_Count == 16)
                    {
                        if (!Region.Find(this.m_Location, this.m_Map).IsDefault || !Region.Find(this.m_Location, this.m_TargetMap).IsDefault)
                        {
                            this.m_Stone.Movable = true;
                            this.m_Caster.AddToBackpack(this.m_Stone);
                            this.Stop();
                            return;
                        }
                        else if (!this.m_TargetMap.CanFit(this.m_Location, 16))
                        {
                            this.m_Stone.Movable = true;
                            this.m_Caster.AddToBackpack(this.m_Stone);
                            this.Stop();
                            return;
                        }

                        int hue = this.m_Stone.Hue;

                        if (hue == 0)
                            hue = Utility.RandomBirdHue();

                        new MoonstoneGate(this.m_Location, this.m_TargetMap, this.m_Map, this.m_Caster, hue);
                        new MoonstoneGate(this.m_Location, this.m_Map, this.m_TargetMap, this.m_Caster, hue);

                        this.m_Stone.Delete();
                        this.Stop();
                    }
                }
            }
        }
    }
}