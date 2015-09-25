#region References
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading;

using Server.Accounting;
using Server.ContextMenus;
using Server.Diagnostics;
using Server.Gumps;
using Server.HuePickers;
using Server.Items;
using Server.Menus;
using Server.Menus.ItemLists;
using Server.Menus.Questions;
using Server.Mobiles;
using Server.Prompts;
using Server.Targeting;
#endregion

namespace Server.Network
{	
    public sealed class SmoothMovement : Packet
    {
        public SmoothMovement(BaseSmoothMulti multi, DynamicComponentList objectsToMove)
            : base(0xF6)
        {
            var length = (short)(3 + 15 + (objectsToMove.Count * 10));
			EnsureCapacity(length);

			length = (short)(((length & 0xff) << 8) | ((length >> 8) & 0xff));
			
			//Hungarian notation used to be compatible with the core code
			m_Stream.Seek(1, SeekOrigin.Begin);
			m_Stream.Write(length);			
			
            m_Stream.Write((int)multi.Serial);
            m_Stream.Write((byte)multi.Speed);
            m_Stream.Write((byte)multi.Moving);
            m_Stream.Write((byte)multi.Facing);

            m_Stream.Write((short)multi.X);
            m_Stream.Write((short)multi.Y);
            m_Stream.Write((short)multi.Z);

            m_Stream.Write((short)0);
			
			int count = 0;
			
			//Safety check : avoiding client stack overflow
			if (multi != null && multi.ContainedObjects != null)
				multi.ContainedObjects.Remove(multi);			
			
            objectsToMove.ForEachObject(onBoard =>
            {
                m_Stream.Write((int)onBoard.Serial);
                m_Stream.Write((short)onBoard.X);
                m_Stream.Write((short)onBoard.Y);
                m_Stream.Write((short)onBoard.Z);
				++count;
            },
            onBoard =>
            {
                m_Stream.Write((int)onBoard.Serial);
                m_Stream.Write((short)onBoard.X);
                m_Stream.Write((short)onBoard.Y);
                m_Stream.Write((short)onBoard.Z);
				++count;
            });
			
			m_Stream.Seek(16, System.IO.SeekOrigin.Begin);
			m_Stream.Write((short) count);			
        }
    }

    public sealed class ContainerMultiList : Packet
    {
        public ContainerMultiList(Mobile beholder, BaseSmoothMulti smoothMulti)
            : base(0xF7)
        {				
			if (smoothMulti != null && smoothMulti.ContainedObjects != null)
			{		
				//Safety check : avoiding client stack overflow
				smoothMulti.ContainedObjects.Remove(smoothMulti);
		
				EnsureCapacity(3 + 2 + (smoothMulti.ContainedObjects.Count + 1) * 26);
			
				m_Stream.Write((short)0); // count placeholder
				
				int count = 0;
				
				if (beholder.CanSee(smoothMulti))
				{
					//Display the smoothMulti
					m_Stream.Write( (byte) 0xF3 );
					m_Stream.Write( (short) 0x1 );				

					m_Stream.Write( (byte) 0x02 );
					m_Stream.Write( (int) smoothMulti.Serial );
					m_Stream.Write( (ushort) ( smoothMulti.ItemID & 0x7FFF ) );
					m_Stream.Write( (byte) 0 );

					m_Stream.Write( (short) smoothMulti.Amount );
					m_Stream.Write( (short) smoothMulti.Amount );

					m_Stream.Write( (short) ( smoothMulti.X & 0x7FFF ) );
					m_Stream.Write( (short) ( smoothMulti.Y & 0x3FFF ) );
					m_Stream.Write( (sbyte) smoothMulti.Z );

					m_Stream.Write( (byte) smoothMulti.Light );
					m_Stream.Write( (short) smoothMulti.Hue );
					m_Stream.Write( (byte) smoothMulti.GetPacketFlags() );	

					m_Stream.Write( (short) 0x00 );	
					++count;
					
					//Display smoothMulti components
					smoothMulti.ContainedObjects.ForEachObject(
						item => 
						{	
							if (beholder.CanSee(item))
							{
								// Embedded WorldItemHS packets
								m_Stream.Write( (byte) 0xF3 );
								m_Stream.Write( (short) 0x1 );

								Item i = (Item)item;

								m_Stream.Write( (byte) 0x00 );
								m_Stream.Write( (int) i.Serial );
								m_Stream.Write( (ushort) ( i.ItemID & 0xFFFF ) );
								m_Stream.Write( (byte) 0 );

								m_Stream.Write( (short) i.Amount );
								m_Stream.Write( (short) i.Amount );

								m_Stream.Write( (short) ( i.X & 0x7FFF ) );
								m_Stream.Write( (short) ( i.Y & 0x3FFF ) );
								m_Stream.Write( (sbyte) i.Z );

								m_Stream.Write( (byte) i.Light );
								m_Stream.Write( (short) i.Hue );
								m_Stream.Write( (byte) i.GetPacketFlags() );
								
								m_Stream.Write( (short) 0x00 );
								++count;
							}
						}, 
						mob =>
						{
							if (beholder.CanSee(mob))
							{
								// Embedded WorldItemHS packets
								m_Stream.Write( (byte) 0xF3 );
								m_Stream.Write( (short) 0x1 );

								Mobile m = (Mobile)mob;

								m_Stream.Write( (byte) 0x01 );
								m_Stream.Write( (int) m.Serial );
								m_Stream.Write( (short) m.Body );
								m_Stream.Write( (byte) 0 );

								m_Stream.Write( (short) 1 );
								m_Stream.Write( (short) 1 );

								m_Stream.Write( (short) ( m.X & 0x7FFF ) );
								m_Stream.Write( (short) ( m.Y & 0x3FFF ) );
								m_Stream.Write( (sbyte) m.Z );

								m_Stream.Write( (byte) m.Direction );
								m_Stream.Write( (short) m.Hue );
								m_Stream.Write( (byte) m.GetPacketFlags() );

								m_Stream.Write( (short) 0x00 );
								++count;
							}
						});
						
					m_Stream.Seek( 3, System.IO.SeekOrigin.Begin );
					m_Stream.Write( (short) count );						
				}
			}
        }					
	}
}