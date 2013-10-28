/**************************************
*Script Name: Staff Runebook          *
*Author: Joeku                        *
*For use with RunUO 2.0 RC2           *
*Client Tested with: 6.0.9.2          *
*Version: 1.10                        *
*Initial Release: 11/25/07            *
*Revision Date: 02/04/09              *
**************************************/

using System;
using System.Collections.Generic;
using Server;

namespace Joeku.SR
{
    public class SR_RuneAccount
    {
        public string Username;
        public List<SR_Rune> Runes;
        public int RunebookCount, RuneCount;
        public int PageIndex = -1;
        public SR_RuneAccount(string username)
            : this(username, new List<SR_Rune>())
        {
        }

        public SR_RuneAccount(string username, List<SR_Rune> runes)
        {
            this.Username = username;
            this.Runes = runes;
            this.FindCounts();

            SR_Main.AddInfo(this);
        }

        public int Count
        {
            get
            {
                return this.Runes.Count;
            }
        }
        public SR_Rune ChildRune
        {
            get
            {
                SR_Rune rune = null;

                if (this.PageIndex > -1)
                    rune = this.Runes[this.PageIndex];

                if (rune != null)
                {
                    while (rune.PageIndex > -1)
                        rune = rune.Runes[rune.PageIndex];
                }

                return rune;
            }
        }
        // Legacy... binary serialization only used in v1.00, deserialization preserved to migrate data.
        public static void Deserialize(GenericReader reader, int version)
        {
            List<SR_Rune> runes = new List<SR_Rune>();

            string username = reader.ReadString();
            Console.Write("  Account: {0}... ", username);
            int count = reader.ReadInt();
            for (int i = 0; i < count; i++)
                runes.Add(SR_Rune.Deserialize(reader, version));
            new SR_RuneAccount(username, runes);
            Console.WriteLine("done.");
        }

        public void ResetPageIndex()
        {
            this.Runes[this.PageIndex].ResetPageIndex();
            this.PageIndex = -1;
        }

        public void Clear()
        {
            this.Runes.Clear();
            this.RunebookCount = 0;
            this.RuneCount = 0;
            this.PageIndex = -1;
        }

        public void AddRune(SR_Rune rune)
        {
            for (int i = 0; i < this.Count; i++)
                if (this.Runes[i] == rune)
                    this.Runes.RemoveAt(i);

            if (rune.IsRunebook)
            {
                this.Runes.Insert(this.RunebookCount, rune);
                this.RunebookCount++;
            }
            else
            {
                this.Runes.Add(rune);
                this.RuneCount++;
            }
        }

        public void RemoveRune(int index)
        {
            this.RemoveRune(index, false);
        }

        public void RemoveRune(int index, bool pageIndex)
        {
            if (this.Runes[index].IsRunebook)
                this.RunebookCount--;
            else
                this.RuneCount--;

            if (pageIndex && this.PageIndex == index)
                this.PageIndex = -1;

            this.Runes.RemoveAt(index);
        }

        public void FindCounts()
        {
            int runebookCount = 0, runeCount = 0;
            for (int i = 0; i < this.Runes.Count; i++)
                if (this.Runes[i].IsRunebook)
                    runebookCount++;
                else
                    runeCount++;

            this.RunebookCount = runebookCount;
            this.RuneCount = runeCount;
        }
    }
}