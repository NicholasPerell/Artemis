using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Artemis
{

    // all bb, 12/24/2021
    public class Cell
    {
        const string DEFAULT_CELL_VALUE = "NO_VALUE_PLACED_IN_THIS_CELL_YET";

        public string value = DEFAULT_CELL_VALUE;
        public int column = -1;
        public int row = -1;

        public Cell(string startingContents, int startingColumn, int startingRow)
        {
            if (startingColumn < 1 || startingRow < 1)
                Debug.LogError("INVALID COLUMN AND/OR ROW FOR CELL");

            value = startingContents;
            column = startingColumn;
            row = startingRow;
        }
        // so others can't use it, instead of the actual constructor
        private Cell() {; }
    }
}
