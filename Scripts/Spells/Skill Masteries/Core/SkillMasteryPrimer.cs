using System;
using Server;
using Server.Mobiles;
using Server.Spells.SkillMasteries;
using System.Collections.Generic;
using System.Linq;
using Server.ContextMenus;

namespace Server.Items
{
    public class SkillMasteryPrimer : Item
    {
        [CommandProperty(AccessLevel.GameMaster)]
        public SkillName Skill { get; private set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public int SpellID { get; private set; }

        [Constructable]
        public SkillMasteryPrimer(int spellID, SkillName skill) : base(7714)
        {
            SpellID = spellID;
            Skill = skill;
            LootType = LootType.Cursed;
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (IsChildOf(from.Backpack))
            {
                if (MasteryInfo.HasLearned(from, SpellID, Skill))
                    from.SendLocalizedMessage(1155884, String.Format("#{0}", MasteryInfo.GetLocalization(SpellID, Skill))); // You are already proficient in this level of ~1_MasterySkill~
                //else if (MasteryInfo.CanLearn(from, SpellID))
                //    from.SendLocalizedMessage(1115709); // Your skills are not high enough to invoke this mastery ability.
                else if(MasteryInfo.LearnMastery(from, SpellID, Skill))
                {
                    from.SendLocalizedMessage(1155885, String.Format("#{0}", MasteryInfo.GetLocalization(SpellID, Skill))); // You have increased your proficiency in ~1_SkillMastery~!

                    Effects.SendLocationParticles(EffectItem.Create(from.Location, from.Map, EffectItem.DefaultDuration), 0, 0, 0, 0, 0, 5060, 0);
                    Effects.PlaySound(from.Location, from.Map, 0x243);

                    Effects.SendMovingParticles(new Entity(Serial.Zero, new Point3D(from.X - 6, from.Y - 6, from.Z + 15), from.Map), from, 0x36D4, 7, 0, false, true, 0xAA8, 0, 9502, 1, 0, (EffectLayer)255, 0x100);
                    Effects.SendMovingParticles(new Entity(Serial.Zero, new Point3D(from.X - 4, from.Y - 6, from.Z + 15), from.Map), from, 0x36D4, 7, 0, false, true, 0xAA8, 0, 9502, 1, 0, (EffectLayer)255, 0x100);
                    Effects.SendMovingParticles(new Entity(Serial.Zero, new Point3D(from.X - 6, from.Y - 4, from.Z + 15), from.Map), from, 0x36D4, 7, 0, false, true, 0xAA8, 0, 9502, 1, 0, (EffectLayer)255, 0x100);

                    Effects.SendTargetParticles(from, 0x375A, 35, 90, 0x00, 0x00, 9502, (EffectLayer)255, 0x100);

                    Delete();
                }
            }
        }

        public override void AddNameProperty(ObjectPropertyList list)
        {
            list.Add(1155882, String.Format("#{0}", MasteryInfo.GetLocalization(SpellID, Skill))); // Primer on ~1_Skill~
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            list.Add(1155883, String.Format("{0}", GetVolume(MasteryInfo.GetVolume(SpellID, Skill)))); // Volume ~1_Level~
        }

        private string GetVolume(Volume volume)
        {
            if (volume == Volume.One)
                return "I";

            if (volume == Volume.Two)
                return "II";

            if (volume == Volume.Three)
                return "III";

            return "Error";
        }

        public static void CheckPrimerDrop(BaseCreature killed)
        {
            List<DamageStore> rights = killed.GetLootingRights();

            rights.ForEach(ds =>
            {
                if (ds.m_HasRight)
                {
                    Mobile m = ds.m_Mobile;

                    if (Utility.RandomDouble() < 0.10)
                    {
                        SkillMasteryPrimer primer = GetRandom();

                        if (primer != null)
                        {
                            if (m.Backpack == null || !m.Backpack.TryDropItem(m, primer, false))
                                m.BankBox.DropItem(primer);
                        }

                        m.SendLocalizedMessage(1156209); // You have received a mastery primer!
                    }
                }
            });
        }

        public static SkillMasteryPrimer GetRandom(Volume volume = Volume.Three, Volume exclude = Volume.One)
        {
            List<MasteryInfo> available = new List<MasteryInfo>();

            foreach (MasteryInfo info in MasteryInfo.Infos.Where(i => i.Volume <= volume && i.Volume != exclude && i.SpellID > 705))
                available.Add(info);

            if (available.Count == 0)
                return null;

            MasteryInfo random = available[Utility.Random(available.Count)];

            SkillMasteryPrimer primer = new SkillMasteryPrimer(random.SpellID, random.MasterySkill);

            available.Clear();
            available.TrimExcess();

            return primer;
        }

        public static SkillMasteryPrimer GetPrimer(SkillName name, Volume volume = Volume.One)
        {
            MasteryInfo info = MasteryInfo.Infos.FirstOrDefault(i => i.MasterySkill == name && i.Volume == volume);

            if (info == null)
                return null;

            return new SkillMasteryPrimer(info.SpellID, name);
        }

        public SkillMasteryPrimer(Serial serial)
            : base(serial)
        {
        }

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version

            writer.Write((int)Skill);
            writer.Write(SpellID);
		}

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            Skill = (SkillName)reader.ReadInt();
            SpellID = reader.ReadInt();
        }
    }
}