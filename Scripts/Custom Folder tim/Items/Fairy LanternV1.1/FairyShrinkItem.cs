using System;
using Server.Items;
using Server.Mobiles;

namespace Server.Items {
    public class FairyShrinkItem : Item {
        
        #region Constructors
        public FairyShrinkItem (Serial serial)
            : base(serial) {
        }

        public FairyShrinkItem (BaseCreature c) {
            m_toDeletePet = true;
            m_link = c;
            /*if ( !(c.Body==400 || c.Body==401) )
            {			
                Hue = c.Hue;
            }*/
            Name = c.Name + " [in lantern]";
            //ItemID=ShrinkTable.Lookup( c );
            ItemID = 0xa22;
            Weight = 2.0;
            Layer = Layer.OneHanded;
            Light = LightType.Circle300;
			LootType = LootType.Blessed;
        }
        #endregion

        #region Properties
        private Mobile m_BondOwner;
        private BaseCreature m_link;
        private bool m_toDeletePet;
        private bool m_WonderfullyHappyOnUnshrink;

        [CommandProperty(AccessLevel.GameMaster)]
        public Mobile BondOwner {
            get { return m_BondOwner; }
            set { m_BondOwner = value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public BaseCreature Link {
            get {
                return m_link;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool ToDeletePet {
            get {
                return m_toDeletePet;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool WonderfullyHappyOnUnshrink {
            get {
                return m_WonderfullyHappyOnUnshrink;
            }
            set {
                m_WonderfullyHappyOnUnshrink = value;
            }
        }
        #endregion

        /// <summary>
        /// Called when a mobile double cliks on the item.
        /// </summary>
        /// <param name="from">The mobile who double clicked.</param>
        public override void OnDoubleClick (Mobile from) {
            if (!Movable)
                return;

            if (from.InRange(this.GetWorldLocation(), 2) == false) {
                from.SendLocalizedMessage(500486);	//That is too far away.
                return;
            } else if (m_link == null || m_link.Deleted) {
                from.SendMessage("Unfortunelately, your pet is lost forever in faerie realms.");
                return;
                //Basically just an "In case something happened"  Thing so Server don't crash.
            } else if (!from.CheckAlive()) {
                from.SendLocalizedMessage(1060190);	//You cannot do that while dead!
            } else if (from.Followers + m_link.ControlSlots > from.FollowersMax) {
                from.SendMessage("You have too many followers to unshrink that creature.");
                return;
            } else {
                bool alreadyOwned = m_link.Owners.Contains(from);
                if (!alreadyOwned) { m_link.Owners.Add(from); }

                m_link.SetControlMaster(from);
                m_toDeletePet = false;

                m_link.Location = from.Location;

                m_link.Map = from.Map;

                m_link.ControlTarget = from;

                m_link.ControlOrder = OrderType.Follow;

                if (m_link.Summoned)
                    m_link.SummonMaster = from;

                if (m_WonderfullyHappyOnUnshrink)
                    m_link.Loyalty = 100;

                m_link.IsBonded = (m_link.IsBonded && from == BondOwner);

                this.Delete();
                Item NewLantern = new FairyLantern();

                if (from != null) {
                    from.SendMessage("You release the creature");
                    if (!from.AddToBackpack(NewLantern)) {
                        NewLantern.MoveToWorld(new Point3D(from.X, from.Y, from.Z), from.Map);
                        from.SendMessage("Your backpack is full so the fairy lantern falls to the ground");
                    }
                } else {
                    NewLantern.MoveToWorld(new Point3D(from.X, from.Y, from.Z), from.Map);  // place lantern at current location
                }
            }

        }

        /// <summary>
        /// Called when the item is deleted
        /// </summary>
        public override void OnDelete () {
            //If m_toDeletePet sets to true, deletes also the mobile
            if (m_toDeletePet && m_link != null) {
                    m_link.Delete();
            }
            base.OnDelete();
        }

        #region Serialization
        public override void Serialize (GenericWriter writer) {
            base.Serialize(writer);

            writer.Write((int)1); // version

            writer.Write(m_BondOwner);

            writer.Write(m_link);
            writer.Write(m_toDeletePet);

        }

        public override void Deserialize (GenericReader reader) {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch (version) {
                case 1: {
                    m_BondOwner = reader.ReadMobile();
                    goto case 0;
                }
                case 0: {
                    m_link = (BaseCreature)reader.ReadMobile();
                    m_toDeletePet = reader.ReadBool();
                    break;
                }

            }

            if (m_link != null)
                m_link.IsStabled = true;

        }
        #endregion
    }
}