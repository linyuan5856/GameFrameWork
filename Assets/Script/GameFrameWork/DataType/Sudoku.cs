using UnityEngine;
using System.Collections.Generic;


namespace Pandora
{
    public class Sudoku
    {
        private Rect rect;
        //private Gridding<T> grids = null;
        private float gridWidth;
        private float gridHeight;
        private int colNum;
        private int rowNum;

        private Dictionary<int, List<int>> ranges = new Dictionary<int, List<int>>();
        public Sudoku(Rect rect, float gridWidth, float gridHeight)
        {
            this.rect = rect;
            this.gridWidth = gridWidth;
            this.gridHeight = gridHeight;

            this.colNum = Mathf.CeilToInt(this.rect.width / gridWidth);
            this.rowNum = Mathf.CeilToInt(this.rect.height / gridHeight);

            //Debug.Log (this.rect);
            //Debug.Log("44%10="+(44%10));

            //Debug.LogWarning("colNum:" + this.colNum + ", rowNum:" + this.rowNum);
            /*
            string str = "";
            for (int row=0; row<this.rowNum; row++) {
                for (int col=0; col<this.colNum; col++) {
                    int index = this.coord_to_index (col, row);
                    str += (index < 10 ? "00" : (index<100 ? "0" :"")) + index + ",";
                }
                str += "\n";
            }
            Debug.Log (str);	
            this.GetAroundGrids (44);
            Debug.Log ("44%13="+ (44%13));	
            */
            int num = this.colNum * this.rowNum;
            for (int i = 0; i < num; i++)
            {
                this.ranges.Add(i, this.get_around_grids(i));
            }
        }

        public int ColNum { get { return this.colNum; } }

        public int RowNum { get { return this.rowNum; } }

        //public int X { get { return this.rect.x; } }

        //public int Y { get { return this.rect.y; } }

        public int Width { get { return (int)this.rect.width; } }

        public int Height { get { return (int)this.rect.height; } }

        public int GridWidth { get { return (int)this.gridWidth; } }

        public int GridHeight { get { return (int)this.gridHeight; } }

        public Rect Area { get { return this.rect; } }

        //
        public int GetIndexByPos(float pos_x, float pos_z)
        {
            int posX = (int)pos_x;
            int posYZ = (int)pos_z;

            posX = (posX - (int)this.rect.x);
            posYZ = ((int)this.rect.y - posYZ);
            //Debug.Log("X:"+posX+", Z:"+posYZ);
            //float xx = posX % this.gridWidth;
            //float yy = posYZ % this.gridHeight;

            int row = (int)(posYZ / this.gridHeight);
            int col = (int)(posX / this.gridWidth);

            int index = this.coord_to_index(col, row);
            //Debug.Log ("当前所在区域:: " + index + " col:" + col + ", row:" + row);

            //Logger.LogTemp(
            //    string.Format(
            //        "Index: x:{0},z:{1},gridWidth:{2},gridHeight:{3},row:{4},col:{5},rowNum:{6},colNum:{7},index:{8}", pos_x,
            //        pos_z, gridWidth, gridHeight, row, col, rowNum, colNum, index));
            return index;
        }

        public List<int> GetNewGrids(int oldIndex, int newIndex)
        {
            List<int> grids = new List<int>();
            if (oldIndex != newIndex && oldIndex >= 0 && newIndex >= 0)
            {
                List<int> oldGrids = this.GetAroundGrids(oldIndex);
                List<int> newGrids = this.GetAroundGrids(newIndex);
                if (oldGrids == null || newGrids == null)
                    return null;
                for (int i = 0; i < newGrids.Count; i++)
                {
                    if (!oldGrids.Contains(newGrids[i]))
                        grids.Add(newGrids[i]);
                }
            }

            return grids;
        }

        public List<int> get_around_grids(int colStart, int rowStart, int range)
        {
            List<int> ret = new List<int>();
            /*
            for (var row = 0; row < this.rowNum; row++)
                for (var col = 0; col < this.colNum; col++) {
                    if (row <= this.rowNum - 1 && row >= 0
                        && col <= this.colNum - 1 && col >= 0
                        && row <= rowStart + range && row >= rowStart - range
                        && col <= colStart + range && col >= colStart - range) {
                        int idx = this.coord_to_index (col, row);

                        ret.Add (idx);

                    }
                }*/
            //Debug.Log("GetAroundGrids:: col:"+colStart+", row:"+rowStart);
            // this line
            int index = this.coord_to_index(colStart, rowStart);
            if (index >= 0)
                ret.Add(index);

            index = this.coord_to_index(colStart - 1, rowStart);
            if (index >= 0)
                ret.Add(index);
            index = this.coord_to_index(colStart, rowStart);
            if (index >= 0)
                ret.Add(index);
            index = this.coord_to_index(colStart + 1, rowStart);
            if (index >= 0)
                ret.Add(index);

            // down
            index = this.coord_to_index(colStart - 1, rowStart + 1);
            if (index >= 0)
                ret.Add(index);
            index = this.coord_to_index(colStart, rowStart + 1);
            if (index >= 0)
                ret.Add(index);
            index = this.coord_to_index(colStart + 1, rowStart + 1);
            if (index >= 0)
                ret.Add(index);

            //up
            index = this.coord_to_index(colStart - 1, rowStart - 1);
            if (index >= 0)
                ret.Add(index);
            index = this.coord_to_index(colStart, rowStart - 1);
            if (index >= 0)
                ret.Add(index);
            index = this.coord_to_index(colStart + 1, rowStart - 1);
            if (index >= 0)
                ret.Add(index);

            return ret;
        }

        public List<int> get_around_grids(int index)
        {
            OffsetCoord coord = this.index_to_coord(index);
            //Debug.Log("index:"+index+"-------------------col:"+coord.col+",  row:"+coord.row);
            List<int> ret = this.get_around_grids(coord.col, coord.row, 1);

            return ret;
        }

        public List<int> GetAroundGrids(int index)
        {
            //Debug.LogWarning("get grid, index:"+index+ " ranges-count:"+ranges.Count);
            if (this.ranges.Count > index && index >= 0)
            {
                return this.ranges[index];
            }
            
                GameLogger.LogError("Index Out Of Range:" + index);
                return null;
        }

        public void printZhouWei(int index, List<int> zhouwe)
        {
            string str = "index:" + index + " 9G:" + zhouwe.Count + " ||";
            for (int i = 0; i < zhouwe.Count; i++)
            {
                str += zhouwe[i] + ", ";
            }
            Debug.Log(str);
        }

        public int coord_to_index(int col, int row)
        {
            return row * colNum + col;
        }

        public OffsetCoord index_to_coord(int idx)
        {
            int row = (int)(idx / this.colNum);
            int col = idx % this.colNum;
            return OffsetCoord.Gen(row, col);
        }

    }
}