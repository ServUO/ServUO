using System;
using Server.Mobiles;

namespace Server.Items
{
    public class ImprisonedDog : BaseImprisonedMobile
    {
        [Constructable]
        public ImprisonedDog()
            : base(0x1F1C)
        {
            this.Weight = 1.0;
            this.Hue = 0x485;
        }

        public ImprisonedDog(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1075091;
            }
        }// An Imprisoned Dog
        public override BaseCreature Summon
        {
            get
            {
                return new TravestyDog();
            }
        }
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
			
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
			
            int version = reader.ReadInt();
        }
    }
}

namespace Server.Mobiles
{ 
    public class TravestyDog : Dog
    { 
        private string m_Name;
        private DateTime m_NextAttempt;
        [Constructable]
        public TravestyDog()
            : base()
        {
            this.Hue = 2301;
		
            this.m_Name = null;
            this.m_NextAttempt = DateTime.UtcNow;
        }

        public TravestyDog(Serial serial)
            : base(serial)
        {
        }

        public override bool DeleteOnRelease
        {
            get
            {
                return true;
            }
        }
        public bool Morphed
        {
            get
            {
                return this.m_Name != null;
            }
        }
        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
			
            list.Add(1049646); // (summoned)
        }

        public void DeleteItems()
        { 
            for (int i = this.Items.Count - 1; i >= 0; i --)
                if (this.Items[i] is ClonedItem)
                    this.Items[i].Delete();
        }

        public void BeginMorph(Mobile to)
        { 
            if (to == null || !this.Alive || this.Morphed)
                return;
				
            this.m_Name = this.Name;
		
            this.Body = to.Body; 
            this.Hue = to.Hue; 
            this.Name = to.Name;
            this.Female = to.Female;
            this.Title = to.Title;
            this.HairItemID = to.HairItemID;
            this.HairHue = to.HairHue;
            this.FacialHairItemID = to.FacialHairItemID;
            this.FacialHairHue = to.FacialHairHue;
			  				
            for (int i = to.Items.Count - 1; i >= 0; i --)
            {
                Item item = to.Items[i];
			
                if (item.Layer != Layer.Backpack && item.Layer != Layer.Mount)
                    this.AddItem(new ClonedItem(item));
            }
			
            this.PlaySound(0x511);
            this.FixedParticles(0x376A, 1, 14, 5045, EffectLayer.Waist);
			
            Timer.DelayCall(TimeSpan.FromSeconds(60), new TimerCallback(EndMorph));
        }

        public void EndMorph()
        { 
            this.DeleteItems();
			
            this.Body = 0xD9;
            this.Hue = 2301;
            this.Name = this.m_Name;
            this.Female = false;
            this.Title = null;
            this.HairItemID = 0;
            this.HairHue = 0;
            this.FacialHairItemID = 0;
            this.FacialHairHue = 0;
			
            this.m_Name = null;
			
            this.PlaySound(0x511);
            this.FixedParticles(0x376A, 1, 14, 5045, EffectLayer.Waist);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
			
            writer.Write((int)0); // version
			
            writer.Write((string)this.m_Name);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
			
            int version = reader.ReadInt();
			
            this.m_Name = reader.ReadString();
            this.m_NextAttempt = DateTime.UtcNow;
			
            if (this.Morphed)
                this.EndMorph();				
        }

        protected override bool OnMove(Direction d)
        {
            if (!this.Morphed && this.m_NextAttempt <= DateTime.UtcNow)
            {
                foreach (Mobile m in this.GetMobilesInRange(6))
                {
                    if (!m.Hidden && m.Alive && Utility.RandomDouble() < 0.25)
                    {
                        this.BeginMorph(m);
                        break;
                    }
                }
				
                this.m_NextAttempt = DateTime.UtcNow + TimeSpan.FromSeconds(90);
            }
		
            return base.OnMove(d);
        }

        private class ClonedItem : Item
        { 
            public ClonedItem(Item item)
                : base(item.ItemID)
            {
                this.Name = item.Name;
                this.Weight = item.Weight;
                this.Hue = item.Hue;
                this.Layer = item.Layer;
            }

            public ClonedItem(Serial serial)
                : base(serial)
            {
            }

            public override DeathMoveResult OnParentDeath(Mobile parent)
            {
                this.Delete();
				
                return DeathMoveResult.RemainEquiped;
            }

            public override void Serialize(GenericWriter writer)
            {
                base.Serialize(writer);
				
                writer.Write((int)0); // version
            }

            public override void Deserialize(GenericReader reader)
            {
                base.Deserialize(reader);
				
                int version = reader.ReadInt();
            }
        }
    }
}