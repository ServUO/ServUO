using System;
using System.Collections;
using System.Collections.Generic;
using Server;
using Server.Gumps;
using Server.Items;
using Server.Network;
using Server.Targeting;
using Server.ContextMenus;

namespace Server.Mobiles
{
	public class Veterinarian : BaseVendor
	{
		private List<SBInfo> m_SBInfos = new List<SBInfo>();
		protected override List<SBInfo> SBInfos { get { return m_SBInfos; } }

		[Constructable]
		public Veterinarian()
			: base("the vet")
		{
			SetSkill(SkillName.AnimalLore, 85.0, 100.0);
			SetSkill(SkillName.Veterinary, 90.0, 100.0);
		}

		public override void InitSBInfo()
		{
			m_SBInfos.Add(new SBVeterinarian());
		}

		private static Dictionary<Mobile, Timer> m_ExpireTable = new Dictionary<Mobile, Timer>();

		public static BaseCreature[] GetDeadPets(Mobile from)
		{
			List<BaseCreature> pets = new List<BaseCreature>();
            IPooledEnumerable eable = from.GetMobilesInRange(12);

			foreach (Mobile m in eable)
			{
				BaseCreature bc = m as BaseCreature;

				if (bc != null && bc.IsDeadBondedPet && bc.ControlMaster == from && from.InLOS(bc))
					pets.Add(bc);
			}
            eable.Free();

            if (from.Backpack != null)
            {
                BrokenAutomatonHead head = from.Backpack.FindItemByType(typeof(BrokenAutomatonHead)) as BrokenAutomatonHead;

                if (head != null && head.Automaton != null && !head.Automaton.Deleted)
                    pets.Add(head.Automaton);
            }

			return pets.ToArray();
		}

		public static int GetResurrectionFee(BaseCreature bc)
		{
            if (bc is KotlAutomaton)
                return 0;

			int fee = (int)(100 + Math.Pow(1.1041, bc.MinTameSkill));

			if (fee > 30000)
				fee = 30000;
			else if (fee < 100)
				fee = 100;

			return fee;
		}

		public override void OnMovement(Mobile m, Point3D oldLocation)
		{
			if (this.InRange(m, 3) && !this.InRange(oldLocation, 3) && this.InLOS(m))
			{
				BaseCreature[] pets = GetDeadPets(m);

				if (pets.Length > 0)
				{
					m.Frozen = true;

					m_ExpireTable[m] = Timer.DelayCall(TimeSpan.FromMinutes(1.0), new TimerStateCallback<Mobile>(ResetExpire), m);

					m.CloseGump(typeof(VetResurrectGump));
					m.SendGump(new VetResurrectGump(this, pets));
				}
			}
		}

		public static void ResetExpire(Mobile m)
		{
			m.Frozen = false;
			m.CloseGump(typeof(VetResurrectGump));

			if (m_ExpireTable.ContainsKey(m))
			{
				Timer t = m_ExpireTable[m];

				if (t != null)
					t.Stop();

				m_ExpireTable.Remove(m);
			}
		}

		public Veterinarian(Serial serial)
			: base(serial)
		{
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.Write((int)0); // version
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			/*int version = */
			reader.ReadInt();
		}
	}

	public class VetResurrectGump : Gump
	{
		//public override int TypeID { get { return 0xF3E96; } }
		

		private Veterinarian m_Vet;
		private BaseCreature[] m_Pets;

		public VetResurrectGump(Veterinarian vet, BaseCreature[] pets)
			: base(150, 50)
		{
			m_Vet = vet;
			m_Pets = pets;

			AddPage(0);

			Closable = false;

			AddImage(0, 0, 0xE10);
			AddImageTiled(0, 14, 15, 380, 0xE13);
			AddImageTiled(380, 14, 14, 380, 0xE15);
			AddImage(0, 381, 0xE16);
			AddImageTiled(15, 381, 370, 16, 0xE17);
			AddImageTiled(15, 0, 370, 16, 0xE11);
			AddImage(380, 0, 0xE12);
			AddImage(380, 381, 0xE18);
			AddImageTiled(15, 15, 365, 370, 0xA40);

			AddHtmlLocalized(30, 20, 355, 35, 1113193, 0xFFFFFF, false, false); // Ah, thine pet seems to be in dire condition! I can help thee, but must charge a small fee...
			AddHtmlLocalized(30, 72, 345, 40, 1113284, 0x1DB2D, false, false); // Please select the pet you wish to resurrect:
			AddHtmlLocalized(20, 280, 345, 40, 1113286, 0x1DB2D, false, false); // <CENTER>Your pet will suffer 0.2 points of skill-loss if resurrected in this manner.</CENTER>
			AddImageTiled(95, 62, 200, 1, 0x23C5);
			AddImageTiled(15, 325, 365, 1, 0x2393);

			AddButton(110, 343, 0xF7, 0xF8, 1, GumpButtonType.Reply, 0);
			AddButton(230, 343, 0xF2, 0xF1, -1, GumpButtonType.Reply, 0);

			AddImageTiled(15, 14, 365, 1, 0x2393);
			AddImageTiled(380, 14, 1, 370, 0x2391);
			AddImageTiled(15, 385, 365, 1, 0x2393);
			AddImageTiled(15, 14, 1, 370, 0x2391);
			AddImageTiled(0, 0, 395, 1, 0x23C5);
			AddImageTiled(394, 0, 1, 397, 0x23C3);
			AddImageTiled(0, 396, 395, 1, 0x23C5);
			AddImageTiled(0, 0, 1, 397, 0x23C3);

			for (int i = 0, yOffset = 0; i < m_Pets.Length; i++, yOffset += 35)
			{
				BaseCreature pet = m_Pets[i];

				AddRadio(30, 102 + yOffset, 0x25FF, 0x2602, (i == 0), i);
				AddLabel(70, 107 + yOffset, 0x47E, String.Format("{0}  {1}", pet.Name, Veterinarian.GetResurrectionFee(pet).ToString()));
			}
		}

		public override void OnResponse(NetState sender, RelayInfo info)
		{
			Mobile from = sender.Mobile;

			from.Frozen = false;

			switch (info.ButtonID)
			{
				case -1:
					{
						// You decide against paying the Veterinarian, and the ghost of your pet looks at you sadly...
						from.SendLocalizedMessage(1113197);

						break;
					}
				case 1:
					{
						for (int i = 0; i < m_Pets.Length; i++)
						{
							BaseCreature pet = m_Pets[i];

							if (info.IsSwitched(i))
							{
								int fee = Veterinarian.GetResurrectionFee(pet);

								if (!pet.IsDeadBondedPet)
									from.SendLocalizedMessage(501041); // Target is not dead.
								else if (!from.CanSee(pet) || !from.InLOS(pet))
									from.SendLocalizedMessage(503376); // Target cannot be seen.
								else if (!from.InRange(pet, 12))
									from.SendLocalizedMessage(500643); // Target is too far away.
								else if (pet.ControlMaster != from)
									from.SendLocalizedMessage(1113200); // You must be the owner of that pet to have it resurrected.
								else if (pet.Corpse != null && !pet.Corpse.Deleted)
									from.SendLocalizedMessage(1113279); // That creature's spirit lacks cohesion. Try again in a few minutes.
								else if (Banker.Withdraw(from, fee))
								{
									pet.PlaySound(0x214);
									pet.ResurrectPet();

									for (int j = 0; j < pet.Skills.Length; ++j) // Decrease all skills on pet.
										pet.Skills[j].Base -= 0.2;

                                    if (pet.Map == Map.Internal)
                                        pet.MoveToWorld(from.Location, from.Map);

									from.SendLocalizedMessage(1060398, fee.ToString()); // ~1_AMOUNT~ gold has been withdrawn from your bank box.
									from.SendLocalizedMessage(1060022, Banker.GetBalance(from).ToString(), 0x16); // You have ~1_AMOUNT~ gold in cash remaining in your bank box.
								}
								else
									from.SendLocalizedMessage(1060020); // Unfortunately, you do not have enough cash in your bank to cover the cost of the healing.

								break;
							}
						}

						break;
					}
			}
		}
	}
}
