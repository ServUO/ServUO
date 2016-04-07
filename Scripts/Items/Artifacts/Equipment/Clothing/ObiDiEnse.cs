using System;

namespace Server.Items
{
    [FlipableAttribute(0x1515, 0x1530)] 
    public class ObiDiEnse : Obi
	{
		public override bool IsArtifact { get { return true; } }
        private SkillMod m_SkillMod0;
        [Constructable] 
        public ObiDiEnse()
            : base(0x27A0)
        {
            this.Hue = 0;
            this.Attributes.BonusInt = 5;
            this.Attributes.NightSight = 1;
            this.SkillBonuses.SetValues(0, SkillName.Focus, 5.0);
        }

        public ObiDiEnse(Serial serial)
            : base(serial)
        { 
            this.DefineMods();
			
            if (this.Parent != null && this.Parent is Mobile) 
                this.SetMods((Mobile)this.Parent);
        }

        public override int LabelNumber
        {
            get
            {
                return 1112406;
            }
        }// Obi Di Ense 
        public override int ArtifactRarity
        {
            get
            {
                return 11;
            }
        }
        public override int InitMinHits
        {
            get
            {
                return 150;
            }
        }
        public override int InitMaxHits
        {
            get
            {
                return 150;
            }
        }
        public override bool OnEquip(Mobile from) 
        { 
            this.SetMods(from);
            return true;  
        }

        public override bool Dye(Mobile from, DyeTub sender)
        {
            from.SendLocalizedMessage(1042083); // You can not dye that.
            return false;
        }

        public override void OnRemoved(object parent) 
        { 
            if (parent is Mobile) 
            { 
                if (this.m_SkillMod0 != null) 
                    this.m_SkillMod0.Remove(); 		
            }
        }

        public override void OnSingleClick(Mobile from) 
        { 
            this.LabelTo(from, this.Name); 
        }

        public override void Serialize(GenericWriter writer) 
        { 
            base.Serialize(writer); 
            writer.Write((int)0); 
        }

        public override void Deserialize(GenericReader reader) 
        { 
            base.Deserialize(reader); 
            int version = reader.ReadInt(); 
        }

        private void DefineMods()
        {
            this.m_SkillMod0 = new DefaultSkillMod(SkillName.Stealth, true, 20); 
        }

        private void SetMods(Mobile wearer)
        { 
            wearer.AddSkillMod(this.m_SkillMod0); 
        }
    }
}