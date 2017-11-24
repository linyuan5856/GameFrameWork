using UnityEngine;
using System.Collections;
using System;

public enum HexLayout
{
    Odd_r_Horizontal =0,
    Even_r_Horizontal =1,
    Odd_q_Vertical =2,
    Even_q_Vertical =3
}

public struct OffsetCoord
{
    public static OffsetCoord invalid = new OffsetCoord(-1, -1);

    public static OffsetCoord IndexToCoord (int index, int rowNum, int colNum)
    {
        if(index<0 || index>=rowNum*colNum)
            return OffsetCoord.invalid;
        int row = index / colNum;
        int col = index % colNum;
        return OffsetCoord.Gen (row, col);
    }
    
    public static int CoordToIndex (int row, int col, int rowNum, int colNum)
    {
        if (row < 0 || col < 0 || row >= rowNum || col >= colNum)
            return -1;
        return row * colNum + col;
    }

    public static OffsetCoord Gen(int row, int col)
    {
        return new OffsetCoord(row, col);
    }

    //--------------------
    public int row;
    public int col;
    //public HexLayout layout = HexLayout.Odd_q_Horizontal;
    private OffsetCoord(int row, int col)
    {
        this.row = row;
        this.col = col;
    }

    public bool IsInvalid()
    {
        return (this.col == -1 || this.row == -1);
    }

    public new string ToString()
    {
        return this.row + ":" + this.col;
    }

    public static int Distance(OffsetCoord a, OffsetCoord b)
    { 
        return Mathf.Abs(a.row - b.row) + Mathf.Abs(a.col - b.col);
    }

    public CubeCoord ToCube(HexLayout layout = HexLayout.Odd_r_Horizontal)
    {
        int x = -1;
        int y = -1;
        int z = -1;
        switch (layout)
        {
            case HexLayout.Odd_r_Horizontal:
                x = this.col - (this.row - (this.row & 1)) / 2;
                z = this.row;
                y = -x - z;
                break;
            case HexLayout.Even_r_Horizontal:
                x = this.col - (this.row + (this.row & 1)) / 2;
                z = row;
                y = -x - z;
                break;
            case HexLayout.Odd_q_Vertical:
                x = col;
                z = this.row - (this.col - (this.col & 1)) / 2;
                y = -x - z;
                break;
            case HexLayout.Even_q_Vertical:
                x = this.col;
                z = this.row - (this.col + (this.col & 1)) / 2;
                y = -x - z;
                break;
        }

        return CubeCoord.Gen(x, y, z);
    }

}

public struct AxialCoord
{
    public int q;
    public int r;
    public static AxialCoord invalid = new AxialCoord(-1, -1);

    public static AxialCoord Gen(int q, int r)
    {
        return new AxialCoord(q, r);
    }
    
    private AxialCoord(int q, int r)
    {
        this.q = q;
        this.r = r;
    }
    
    public bool IsInvalid()
    {
        return (this.r == -1 || this.q == -1);
    }
    
    public new string ToString()
    {
        return this.q + ":" + this.r;
    }

    public CubeCoord ToCube()
    {
        int x = this.q;
        int z = this.r;
        int y = -x - z;
        return CubeCoord.Gen(x, y, z);
    }
}

public struct CubeCoord
{
    public int x;
    public int y;
    public int z;
    public static CubeCoord invalid = new CubeCoord(-1, -1, -1);

    public static CubeCoord Gen(int x, int y, int z)
    {
        return new CubeCoord(x, y, z);
    }
    
    private CubeCoord(int x, int y, int z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }

    public static int Distance(CubeCoord a, CubeCoord b)
    {    
        return (Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y) + Mathf.Abs(a.z - b.z)) / 2;
    }
    
//    public static int Distance(CubeCoord a, CubeCoord b)
//    {
//        return Mathf.Max(Mathf.Abs(a.x - b.x), Mathf.Abs(a.y - b.y), Mathf.Abs(a.z - b.z));
//    }

//    public bool IsInvalid()
//    {
//        return (this.x == -1 || this.y == -1 || this.z==-1);
//    }
    
    public new string ToString()
    {
        return this.x + ":" + this.y + ":" + this.z;
    }

    public OffsetCoord ToOffset(HexLayout layout)
    {
        int row = -1;
        int col = -1;
        switch (layout)
        {
            case HexLayout.Odd_r_Horizontal:
                col = this.x + (this.z + (this.z & 1)) / 2;
                row = z;
                break;
            case HexLayout.Even_r_Horizontal:
                col = this.x + (this.z + (this.z & 1)) / 2;
                row = this.z;
                break;
            case HexLayout.Odd_q_Vertical:
                col = this.x;
                row = this.z + (this.x - (this.x & 1)) / 2;
                break;
            case HexLayout.Even_q_Vertical:
                col = this.x;
                row = this.z + (this.x + (this.x & 1)) / 2;
                break;
        }
        
        return OffsetCoord.Gen(row,col);
    }

    public AxialCoord ToAxial()
    {
        int q = this.x;
        int r = this.z;
        return AxialCoord.Gen(q,r);
    }
    
}