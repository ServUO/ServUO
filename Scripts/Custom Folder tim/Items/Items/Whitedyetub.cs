using System; 
using Server;
using Server.Engines.VeteranRewards;

namespace Server.Items 
{
    public class WhiteDyeTub : DyeTub, IRewardItem 
   {
       private bool m_IsRewardItem;
       [CommandProperty(AccessLevel.GameMaster)]
       public bool IsRewardItem
       {
           get { return m_IsRewardItem; }
           set { m_IsRewardItem = value; InvalidateProperties(); }
       }
      [Constructable] 
      public WhiteDyeTub() 
      { 
         Hue = DyedHue = 0x481; 
         Redyable = false; 
      } 

      public WhiteDyeTub( Serial serial ) : base( serial ) 
      { 
      }
      public override void OnDoubleClick(Mobile from)
      {
          if (m_IsRewardItem && !RewardSystem.CheckIsUsableBy(from, this, null))
          {
              from.SendMessage("This does not belong to you!!");
              return;
          }
          base.OnDoubleClick(from);
      }
      public override void Serialize( GenericWriter writer ) 
      { 
         base.Serialize( writer ); 

         writer.Write( (int) 0 ); // version 
         writer.Write((bool)m_IsRewardItem);
      } 

      public override void Deserialize( GenericReader reader ) 
      { 
         base.Deserialize( reader ); 

         int version = reader.ReadInt();
         m_IsRewardItem = reader.ReadBool();
      } 
   } 
}