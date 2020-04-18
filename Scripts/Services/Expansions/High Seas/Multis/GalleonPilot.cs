using Server.ContextMenus;
using Server.Gumps;
using Server.Items;
using Server.Multis;
using Server.Targeting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Server.Mobiles
{
    public class GalleonPilot : BaseCreature
    {
        [CommandProperty(AccessLevel.GameMaster)]
        public BaseGalleon Galleon { get; private set; }

        public List<Item> OriginalItems { get; private set; }

        public GalleonPilot(BaseGalleon galleon)
            : base(AIType.AI_Vendor, FightMode.None, 2, 1, 0.5, 2)
        {
            Body = 0x190;
            CantWalk = true;
            Galleon = galleon;
            Blessed = true;

            InitBody();
            InitOutfit();

            OriginalItems = new List<Item>();

            foreach (Item item in Items)
                OriginalItems.Add(item);

            SetSkill(SkillName.Cartography, 100.0);
        }

        public virtual void InitBody()
        {
            InitStats(100, 100, 25);

            SpeechHue = Utility.RandomDyedHue();
            Hue = Utility.RandomSkinHue();

            if (Female = GetGender())
            {
                Body = 0x191;
                Name = NameList.RandomName("female");
            }
            else
            {
                Body = 0x190;
                Name = NameList.RandomName("male");
            }
        }

        public virtual bool GetGender()
        {
            return Utility.RandomBool();
        }

        public virtual int GetHairHue()
        {
            return Utility.RandomHairHue();
        }

        public virtual int GetShoeHue()
        {
            if (0.1 > Utility.RandomDouble())
                return 0;

            return Utility.RandomNeutralHue();
        }

        public virtual void InitOutfit()
        {
            if (Map == Map.Tokuno)
            {
                if (Utility.Random(2) == 0)
                    SetWearable(new Kasa(GetRandomHue()));
                if (Utility.Random(2) == 0)
                    SetWearable(new Obi(GetRandomHue()));
                if (Utility.RandomDouble() < 0.25)
                {
                    if (Female)
                        SetWearable(new FemaleKimono(GetRandomHue()));
                    else
                    {
                        if (Utility.Random(2) == 0)
                            SetWearable(new MaleKimono(GetRandomHue()));
                        else
                            SetWearable(new Kamishimo(GetRandomHue()));
                    }
                }
                else
                {
                    switch (Utility.Random(2))
                    {
                        case 0:
                            {
                                SetWearable(new FancyShirt(GetRandomHue()));
                                SetWearable(new JinBaori(GetRandomHue())); break;
                            }
                        case 1: SetWearable(new HakamaShita(GetRandomHue())); break;

                    }
                    switch (Utility.Random(2))
                    {
                        case 0: SetWearable(new Hakama(GetRandomHue())); break;
                        case 1: SetWearable(new TattsukeHakama(GetRandomHue())); break;

                    }
                }
                switch (Utility.Random(2))
                {
                    case 0: SetWearable(new Sandals(GetShoeHue())); break;
                    case 1: SetWearable(new Waraji(GetShoeHue())); break;

                }
            }
            else
            {
                switch (Utility.Random(3))
                {
                    case 0: SetWearable(new FancyShirt(GetRandomHue())); break;
                    case 1: SetWearable(new Doublet(GetRandomHue())); break;
                    case 2: SetWearable(new Shirt(GetRandomHue())); break;
                }



                switch (Utility.Random(4))
                {
                    case 0: SetWearable(new Shoes(GetShoeHue())); break;
                    case 1: SetWearable(new Boots(GetShoeHue())); break;
                    case 2: SetWearable(new Sandals(GetShoeHue())); break;
                    case 3: SetWearable(new ThighBoots(GetShoeHue())); break;
                }



                if (Female)
                {
                    switch (Utility.Random(6))
                    {
                        case 0: SetWearable(new ShortPants(GetRandomHue())); break;
                        case 1:
                        case 2: SetWearable(new Kilt(GetRandomHue())); break;
                        case 3:
                        case 4:
                        case 5: SetWearable(new Skirt(GetRandomHue())); break;
                    }
                }
                else
                {
                    switch (Utility.Random(2))
                    {
                        case 0: SetWearable(new LongPants(GetRandomHue())); break;
                        case 1: SetWearable(new ShortPants(GetRandomHue())); break;
                    }
                }
            }
            int hairHue = GetHairHue();

            Utility.AssignRandomHair(this, hairHue);
            Utility.AssignRandomFacialHair(this, hairHue);
        }

        public virtual int GetRandomHue()
        {
            switch (Utility.Random(5))
            {
                default:
                case 0: return Utility.RandomBlueHue();
                case 1: return Utility.RandomGreenHue();
                case 2: return Utility.RandomRedHue();
                case 3: return Utility.RandomYellowHue();
                case 4: return Utility.RandomNeutralHue();
            }
        }

        public void SetFacing(Direction dir)
        {
            Direction = dir;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            if (Galleon != null)
            {
                list.Add(Galleon.Status);
                list.Add(1116580 + (int)Galleon.DamageTaken); //State: Prisine
            }
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (Galleon == null || !Galleon.IsOwner(from))
                base.OnDoubleClick(from);
            else if (Galleon != null && Galleon.Contains(from))
                Galleon.BeginRename(from);
            else if (Galleon != null)
                Galleon.BeginDryDock(from);
        }

        public override bool OnDragDrop(Mobile from, Item dropped)
        {
            if (dropped is MapItem && Galleon != null && Galleon.CanCommand(from) && Galleon.Contains(from))
            {
                Galleon.AssociateMap((MapItem)dropped);
            }

            return false;
        }

        public override bool CheckNonlocalLift(Mobile from, Item item)
        {
            if (Galleon == null || from == null)
                return false;

            if (Galleon.GetSecurityLevel(from) >= SecurityLevel.Captain)
                return true;

            return base.CheckNonlocalLift(from, item);
        }

        public override bool AllowEquipFrom(Mobile from)
        {
            if (Galleon == null || from == null)
                return false;

            if (Galleon.GetSecurityLevel(from) >= SecurityLevel.Captain)
                return true;

            return base.AllowEquipFrom(from);
        }

        public override void GetContextMenuEntries(Mobile from, List<ContextMenuEntry> list)
        {
            base.GetContextMenuEntries(from, list);

            if (Galleon != null)
            {
                if (Galleon.Contains(from))
                {
                    SecurityLevel level = Galleon.GetSecurityLevel(from);

                    if (level >= SecurityLevel.Crewman)
                    {
                        list.Add(new EmergencyRepairEntry(this, from));
                        list.Add(new ShipRepairEntry(this, from));
                    }

                    if (level == SecurityLevel.Captain)
                    {
                        list.Add(new RenameShipEntry(Galleon, from));
                        list.Add(new MoveTillermanEntry(this, from));
                        list.Add(new SecuritySettingsEntry(this, from));
                        list.Add(new ResetSecuritySettings(this, from));
                    }
                }
                else
                {
                    list.Add(new DryDockEntry(Galleon, from));
                }
            }
        }

        private class EmergencyRepairEntry : ContextMenuEntry
        {
            private readonly GalleonPilot m_Pilot;
            private readonly Mobile m_From;

            public EmergencyRepairEntry(GalleonPilot pilot, Mobile from)
                : base(1116589, 5) // Emergency Repairs
            {
                m_Pilot = pilot;
                m_From = from;
            }

            public override void OnClick()
            {
                if (m_Pilot != null && m_Pilot.Galleon != null)
                {
                    BaseGalleon g = m_Pilot.Galleon;

                    if (!g.Scuttled)
                        m_From.SendLocalizedMessage(1116595); // Your ship is not in need of emergency repairs in order to sail.
                    else if (g.IsUnderEmergencyRepairs())
                    {
                        TimeSpan left = g.GetEndEmergencyRepairs();
                        m_From.SendLocalizedMessage(1116592, left != TimeSpan.Zero ? left.TotalMinutes.ToString() : "0"); // Your ship is underway with emergency repairs holding for an estimated ~1_TIME~ more minutes.
                    }
                    else if (!g.TryEmergencyRepair(m_From))
                        m_From.SendLocalizedMessage(1116591, string.Format("{0}\t{1}", BaseBoat.EmergencyRepairClothCost.ToString(), BaseBoat.EmergencyRepairWoodCost)); //You need a minimum of ~1_CLOTH~ yards of cloth and ~2_WOOD~ pieces of lumber to effect emergency repairs.
                }
            }
        }

        private class ShipRepairEntry : ContextMenuEntry
        {
            private readonly GalleonPilot m_Pilot;
            private readonly Mobile m_From;

            public ShipRepairEntry(GalleonPilot pilot, Mobile from)
                : base(1116590, 5) // Permanent Repairs
            {
                m_Pilot = pilot;
                m_From = from;
            }

            public override void OnClick()
            {
                if (m_Pilot != null && m_Pilot.Galleon != null)
                {
                    if (!BaseGalleon.IsNearLandOrDocks(m_Pilot.Galleon))
                        m_From.SendLocalizedMessage(1116594); // Your ship must be near shore or a sea market in order to effect permanent repairs.
                    else if (m_Pilot.Galleon.DamageTaken == DamageLevel.Pristine)
                        m_From.SendLocalizedMessage(1116596); // Your ship is in pristine condition and does not need repairs.
                    else
                        m_Pilot.Galleon.TryRepairs(m_From);
                }
            }
        }

        private class MoveTillermanEntry : ContextMenuEntry
        {
            private readonly GalleonPilot m_Pilot;
            private readonly Mobile m_From;

            public MoveTillermanEntry(GalleonPilot pilot, Mobile from)
                : base(1116729, 5) // Move Tillerman
            {
                m_Pilot = pilot;
                m_From = from;
            }

            public override void OnClick()
            {
                m_From.Target = new RelocateTarget(m_Pilot, m_Pilot.Galleon);
                m_From.SendLocalizedMessage(1116736); // Where do you wish to station the pilot?
            }
        }

        private class RelocateTarget : Target
        {
            private readonly GalleonPilot m_Pilot;
            private readonly BaseGalleon m_Galleon;

            public RelocateTarget(GalleonPilot pilot, BaseGalleon galleon)
                : base(12, false, TargetFlags.None)
            {
                m_Pilot = pilot;
                m_Galleon = galleon;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (targeted is IPoint3D && from.Map != null)
                {
                    IPoint3D pnt = (IPoint3D)targeted;

                    BaseBoat boat = BaseBoat.FindBoatAt(pnt, from.Map);

                    if (boat != null && boat == m_Galleon && IsSurface(pnt))
                    {
                        IPooledEnumerable eable = m_Pilot.Map.GetObjectsInRange(new Point3D(pnt), 0);

                        foreach (object o in eable)
                        {
                            if (o is Mobile || o is Item)
                            {
                                from.SendLocalizedMessage(1011577); // This is an invalid location. 
                                from.Target = new RelocateTarget(m_Pilot, m_Galleon);
                                eable.Free();
                                return;
                            }
                        }
                        eable.Free();

                        StaticTarget st = (StaticTarget)pnt;
                        int z = m_Galleon.ZSurface;

                        if (st != null)
                            z = st.Z;

                        m_Pilot.MoveToWorld(new Point3D(pnt.X, pnt.Y, z), from.Map);
                    }
                    else
                    {
                        from.SendLocalizedMessage(1011577); // This is an invalid location. 
                        from.Target = new RelocateTarget(m_Pilot, m_Galleon);
                    }
                }
            }

            public bool IsSurface(IPoint3D pnt)
            {
                if (pnt is StaticTarget)
                {
                    StaticTarget st = (StaticTarget)pnt;

                    if ((st.Flags & TileFlag.Surface) > 0)
                        return true;
                }
                return false;
            }
        }

        private class SecuritySettingsEntry : ContextMenuEntry
        {
            private readonly GalleonPilot m_Pilot;
            private readonly Mobile m_From;

            public SecuritySettingsEntry(GalleonPilot pilot, Mobile from)
                : base(1149786, 5) // Security Settings
            {
                m_Pilot = pilot;
                m_From = from;
            }

            public override void OnClick()
            {
                if (m_Pilot != null && m_Pilot.Galleon != null && !m_From.HasGump(typeof(ShipSecurityGump)))
                    m_From.SendGump(new ShipSecurityGump(m_From, m_Pilot.Galleon));
            }
        }

        private class ResetSecuritySettings : ContextMenuEntry
        {
            private readonly GalleonPilot m_Pilot;
            private readonly Mobile m_From;

            public ResetSecuritySettings(GalleonPilot pilot, Mobile from)
                : base(1060700, 5) // Reset Security
            {
                m_Pilot = pilot;
                m_From = from;
            }

            public override void OnClick()
            {
                if (m_Pilot != null && m_Pilot.Galleon != null)
                {
                    m_From.SendGump(new BasicConfirmGump<BaseGalleon>(new TextDefinition(1116618), (m, boat) => // Are you sure you wish to clear your ship's access list?
                    {
                        boat.SecurityEntry.Manifest.Keys.Where(x => !boat.IsOwner(x)).ToList().ForEach(y =>
                        {
                            boat.SecurityEntry.RemoveFromAccessList(y);
                        });

                    }, m_Pilot.Galleon));
                }
            }
        }

        public override void OnAfterDelete()
        {
            if (Galleon != null)
                Galleon.Delete();
        }

        public override void AddNameProperties(ObjectPropertyList list)
        {
            if (Galleon != null)
            {
                string nameStr;

                if (Galleon.ShipName == null || Galleon.ShipName.Length == 0)
                    nameStr = "an unnamed ship";
                else
                    nameStr = string.Format("the {0}", Galleon.ShipName);

                list.Add(string.Format("{0} the Pilot of {1}", Name, nameStr));
            }
            else
                base.AddNameProperties(list);
        }

        public GalleonPilot(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(1);

            writer.Write(OriginalItems.Count);
            foreach (Item item in OriginalItems)
                writer.Write(item);

            writer.Write(Galleon);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            OriginalItems = new List<Item>();

            switch (version)
            {
                case 1:
                    int count = reader.ReadInt();
                    for (int i = 0; i < count; i++)
                    {
                        Item item = reader.ReadItem();
                        if (item != null && !item.Deleted)
                            OriginalItems.Add(item);
                    }
                    goto case 0;
                case 0:
                    Galleon = (BaseGalleon)reader.ReadItem();
                    break;
            }

            if (Galleon == null)
                Delete();
        }
    }
}
