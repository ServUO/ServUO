using Server.SkillHandlers;

namespace Server.Items
{
    public class CylinderTrapTrainingKit : PuzzleChest, IRemoveTrapTrainingKit
    {
        public override int LabelNumber => 1159015;  // Cylinder Trap Training Kit
        public int Title => 1159017;  // A Cylinder Trap

        [Constructable]
        public CylinderTrapTrainingKit()
            : base(41875)
        {
            Solution = new PuzzleChestSolution();
            Movable = true;
        }

        public override void DisplayTo(Mobile to)
        {
        }

        public override bool CheckLocked(Mobile from)
        {
            return false;
        }

        public void OnRemoveTrap(Mobile from)
        {
            PuzzleChestSolution solution = GetLastGuess(from);

            if (solution != null)
                solution = new PuzzleChestSolution(solution);
            else
                solution = new PuzzleChestSolution(PuzzleChestCylinder.None, PuzzleChestCylinder.None, PuzzleChestCylinder.None, PuzzleChestCylinder.None, PuzzleChestCylinder.None);

            from.CloseGump(typeof(PuzzleGump));
            from.CloseGump(typeof(StatusGump));
            from.SendGump(new PuzzleGump(from, this, solution, 0));
        }

        public override void DoDamage(Mobile to)
        {
        }

        public override void OnDoubleClick(Mobile m)
        {
            if (m.InRange(GetWorldLocation(), 1))
            {
                m.SendLocalizedMessage(1159008); // That appears to be trapped, using the remove trap skill would yield better results...
            }
        }

        public override void LockPick(Mobile from)
        {
            from.SendLocalizedMessage(1159009); // You successfully disarm the trap!

            from.CheckTargetSkill(SkillName.RemoveTrap, this, 0, 100);

            Solution = new PuzzleChestSolution();
        }

        public CylinderTrapTrainingKit(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();
        }
    }
}
