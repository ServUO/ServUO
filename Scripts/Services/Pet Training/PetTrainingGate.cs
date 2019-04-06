using Server;
using System;
using Server.Mobiles;
using Server.Gumps;

namespace Server.Items
{
    public class PetTrainingGate : Item
    {
        public override bool ForceShowProperties { get { return true; } }

        public override string DefaultName
        {
            get
            {
                return "Pet Training Gate - You Must Start Pet Training Before Bringing Your Pet Through The Gate!";
            }
        }

        [Constructable]
        public PetTrainingGate()
            : base(3948)
        {
            Hue = 1918;
            Movable = false;
        }

        public override bool OnMoveOver(Mobile m)
        {
            if (m is BaseCreature)
            {
                var bc = m as BaseCreature;
                var profile = PetTrainingHelper.GetTrainingProfile(bc);

                if (bc.Controlled && bc.ControlMaster != null && bc.ControlMaster.InRange(bc.Location, 25)
                    && profile != null && profile.HasBegunTraining && profile.TrainingProgress < profile.TrainingProgressMax)
                {
                    profile.TrainingProgress = profile.TrainingProgressMax;
                    bc.FixedEffect(0x375A, 10, 30);

                    if (bc.ControlMaster is PlayerMobile)
                    {
                        var gump = bc.ControlMaster.FindGump<NewAnimalLoreGump>();

                        if (gump != null)
                            gump.Refresh();
                        else
                            BaseGump.SendGump(new NewAnimalLoreGump((PlayerMobile)bc.ControlMaster, bc));
                    }
                }
            }

            return true;
        }

        public PetTrainingGate(Serial serial) 
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class PetBondRemoveGate : Item
    {
        public override bool ForceShowProperties { get { return true; } }

        public override string DefaultName
        {
            get
            {
                return "Pet Bond Timer Remover";
            }
        }

        [Constructable]
        public PetBondRemoveGate()
            : base(3948)
        {
            Hue = 1911;
            Movable = false;
        }

        public override bool OnMoveOver(Mobile m)
        {
            if (m is BaseCreature)
            {
                var bc = m as BaseCreature;
                var profile = PetTrainingHelper.GetTrainingProfile(bc);

                if (bc.Controlled && bc.ControlMaster != null && bc.ControlMaster.InRange(bc.Location, 25)
                    && !bc.IsBonded)
                {
                    bc.IsBonded = true;
                    bc.FixedEffect(0x375A, 10, 30);
                }
            }

            return true;
        }

        public PetBondRemoveGate(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class PowerScrollGiver : Item
    {
        public override bool ForceShowProperties { get { return true; } }

        public override string DefaultName
        {
            get
            {
                return "+20 Power Scrolls";
            }
        }

        [Constructable]
        public PowerScrollGiver()
            : base(0x1183)
        {
            Hue = 2214;
            Movable = false;
        }

        public override void OnDoubleClick(Mobile m)
        {
            if (m.InRange(Location, 3))
            {
                var bag = new Bag();
                foreach (var sk in PetTrainingHelper.MagicSkills)
                {
                    bag.DropItem(new PowerScroll(sk, 120));
                }

                foreach (var sk in PetTrainingHelper.CombatSkills)
                {
                    bag.DropItem(new PowerScroll(sk, 120));
                }

                m.AddToBackpack(bag);
            }
        }

        public PowerScrollGiver(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}