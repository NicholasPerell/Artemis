using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Line
{
    public Line()
    {
        Reset();
    }


    public Cell[] cell;

    public void Reset()
    {
        cell = new Cell[7];
    }

    public void SetCell(int cellToSet, Cell newValue)
    {
        cell[cellToSet-1] = newValue;
    }
}
