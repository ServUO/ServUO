using System;
using Server.Mobiles;
using Server.Network;

namespace Server.Gumps
{
    public class PetResurrectGump : Gump
    {
        private readonly BaseCreature m_Pet;
        private readonly double m_HitsScalar;
        public PetResurrectGump(Mobile from, BaseCreature pet)
            : this(from, pet, 0.0)
        {
        }

        public PetResurrectGump(Mobile from, BaseCreature pet, double hitsScalar)
            : base(50, 50)
        {
            from.CloseGump(typeof(PetResurrectGump));

            this.m_Pet = pet;
            this.m_HitsScalar = hitsScalar;

            this.AddPage(0);

            this.AddBackground(10, 10, 265, 140, 0x242C);

            this.AddItem(205, 40, 0x4);
            this.AddItem(227, 40, 0x5);

            this.AddItem(180, 78, 0xCAE);
            this.AddItem(195, 90, 0xCAD);
            this.AddItem(218, 95, 0xCB0);

            this.AddHtmlLocalized(30, 30, 150, 75, 1049665, false, false); // <div align=center>Wilt thou sanctify the resurrection of:</div>
            this.AddHtml(30, 70, 150, 25, String.Format("<div align=CENTER>{0}</div>", pet.Name), true, false);

            this.AddButton(40, 105, 0x81A, 0x81B, 0x1, GumpButtonType.Reply, 0); // Okay
            this.AddButton(110, 105, 0x819, 0x818, 0x2, GumpButtonType.Reply, 0); // Cancel
        }

        public override void OnResponse(NetState state, RelayInfo info)
        {
            if (this.m_Pet.Deleted || !this.m_Pet.IsBonded || !this.m_Pet.IsDeadPet)
                return;

            Mobile from = state.Mobile;

            if (info.ButtonID == 1)
            {
                if (this.m_Pet.Map == null || !this.m_Pet.Map.CanFit(this.m_Pet.Location, 16, false, false))
                {
                    from.SendLocalizedMessage(503256); // You fail to resurrect the creature.
                    return;
                }
                else if (this.m_Pet.Region != null && this.m_Pet.Region.IsPartOf("Khaldun"))	//TODO: Confirm for pets, as per Bandage's script.
                {
                    from.SendLocalizedMessage(1010395); // The veil of death in this area is too strong and resists thy efforts to restore life.
                    return;
                }

                this.m_Pet.PlaySound(0x214);
                this.m_Pet.FixedEffect(0x376A, 10, 16);
                this.m_Pet.ResurrectPet();

                double decreaseAmount;

                if (from == this.m_Pet.ControlMaster)
                    decreaseAmount = 0.1;
                else
                    decreaseAmount = 0.2;

                for (int i = 0; i < this.m_Pet.Skills.Length; ++i)	//Decrease all skills on pet.
                    this.m_Pet.Skills[i].Base -= decreaseAmount;

                if (!this.m_Pet.IsDeadPet && this.m_HitsScalar > 0)
                    this.m_Pet.Hits = (int)(this.m_Pet.HitsMax * this.m_HitsScalar);
            }
        }
    }
}