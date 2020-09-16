using System;

namespace Server.Items
{
    [Flipable(0x9DB1, 0x9DB2)]
    public class BrokenAutomatonHead : Item
    {
        public static int RepairAmount = 75;

        private KotlAutomaton _Automaton;

        [CommandProperty(AccessLevel.GameMaster)]
        public KotlAutomaton Automaton { get { return _Automaton; } set { _Automaton = value; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public virtual Type RepairResource
        {
            get
            {
                if (_Automaton == null)
                    return typeof(IronIngot);

                return _Automaton.RepairResource;
            }
        }

        public override bool Nontransferable => true;

        public BrokenAutomatonHead(KotlAutomaton automaton)
            : base(0x9DB1)
        {
            _Automaton = automaton;

            Weight = 1.0;
            LootType = LootType.Blessed;
        }

        public override void AddNameProperty(ObjectPropertyList list)
        {
            if (_Automaton != null)
            {
                if (_Automaton is BlackrockAutomaton)
                    list.Add(1157046, string.Format("{0}\t#{1}", _Automaton.Name, "1157178")); // ~1_NAME~'s Broken ~2_TYPE~ Automaton Head
                else
                    list.Add(1157046, string.Format("{0}\t#{1}", _Automaton.Name, CraftResources.GetLocalizationNumber(_Automaton.Resource).ToString())); // ~1_NAME~'s Broken ~2_TYPE~ Automaton Head
            }
            else
            {
                base.AddNameProperty(list);
            }
        }

        public virtual bool TryRepair(Mobile from)
        {
            if (from.Backpack != null && IsChildOf(from.Backpack))
            {
                Type res = RepairResource;

                if (_Automaton == null || _Automaton.ControlMaster != from)
                {
                    from.SendLocalizedMessage(1157051); // You must be the owner of that pet to have it repair.
                }
                else if (from.Backpack.GetAmount(res) < RepairAmount)
                {
                    if (res == typeof(CrystallineBlackrock))
                        from.SendLocalizedMessage(1157179, string.Format("\t{0}", _Automaton.Name));
                    else
                        from.SendLocalizedMessage(1157050, string.Format("#{0}\t{1}", CraftResources.GetLocalizationNumber(_Automaton.Resource).ToString(), _Automaton.Name)); // You need 75 ~1_MATERIAL~ ingots to repair the ~2_CREATURE~.
                }
                else
                {
                    from.Backpack.ConsumeTotal(res, RepairAmount);

                    return RetrieveAutomaton(from);
                }
            }

            return false;
        }

        public virtual bool RetrieveAutomaton(Mobile from)
        {
            KotlAutomaton automaton = _Automaton;

            Delete();

            if (automaton != null && !automaton.Deleted)
            {
                automaton.ResurrectPet();
                automaton.Hits = automaton.HitsMax;

                automaton.MoveToWorld(from.Location, from.Map);
                from.PlaySound(0x23B);
            }
            else
            {
                from.SendMessage("An error has occured, your automaton has been deleted!");
                return false;
            }

            return true;
        }


        public override bool OnDroppedInto(Mobile from, Container target, Point3D p)
        {
            if (target is BankBox)
                return true;

            return base.OnDroppedInto(from, target, p);
        }

        public override bool DropToWorld(Mobile from, Point3D p)
        {
            from.SendLocalizedMessage(1076254); // That item cannot be dropped.
            return false;
        }

        public override bool AllowSecureTrade(Mobile from, Mobile to, Mobile newOwner, bool accepted)
        {
            from.SendLocalizedMessage(1076256); // That item cannot be traded.
            return false;
        }

        public BrokenAutomatonHead(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version

            writer.Write(_Automaton);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            _Automaton = reader.ReadMobile() as KotlAutomaton;
        }
    }
}
