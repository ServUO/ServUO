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
    public class SR_Rune
    {
        public string Name;
        public bool IsRunebook = false;
        public List<SR_Rune> Runes;
        public int RunebookCount, RuneCount;
        public int PageIndex = -1;
        public SR_Rune ParentRune;
        public Map TargetMap = Map.Felucca;
        public Point3D TargetLoc = new Point3D(0, 0, 0);
        public SR_Rune(string name, Map map, Point3D loc)
            : this(name, false)
        {
            this.TargetMap = map;
            this.TargetLoc = loc;
        }

        public SR_Rune(string name, bool isRunebook)
            : this(name, isRunebook, new List<SR_Rune>())
        {
        }

        public SR_Rune(string name, bool isRunebook, List<SR_Rune> runes)
        {
            this.Name = name;
            this.IsRunebook = isRunebook;
            this.Runes = runes;
            this.FindCounts();
        }

        public int Count
        {
            get
            {
                return this.Runes.Count;
            }
        }
        public int Tier
        {
            get
            {
                if (this.ParentRune != null)
                    return this.ParentRune.Tier + 1;

                return 0;
            }
        }
        // Legacy... binary serialization only used in v1.00, deserialization preserved to migrate data.
        public static SR_Rune Deserialize(GenericReader reader, int version)
        {
            SR_Rune rune = null;

            string name = reader.ReadString();
            bool isRunebook = reader.ReadBool();

            Map targetMap = reader.ReadMap();
            Point3D targetLoc = reader.ReadPoint3D();

            if (isRunebook)
                rune = new SR_Rune(name, isRunebook);
            else
                rune = new SR_Rune(name, targetMap, targetLoc);

            int count = reader.ReadInt();
            for (int i = 0; i < count; i++)
                rune.AddRune(SR_Rune.Deserialize(reader, version));

            return rune;
        }

        public void ResetPageIndex()
        {
            if (!this.IsRunebook || this.PageIndex == -1)
                return;

            if (this.Runes[this.PageIndex] != null)
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

            rune.ParentRune = this;
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