using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Perell.Artemis
{
    public class Line
    {
        int mLength;

        public Line(int length)
        {
            mLength = length;
            Reset();
        }


        public Cell[] cell;

        public void Reset()
        {
            cell = new Cell[mLength];
        }

        public void SetCell(int cellToSet, Cell newValue)
        {
            cell[cellToSet - 1] = newValue;
        }
    }
}
