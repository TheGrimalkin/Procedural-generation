namespace Procedural_Generation;
// a class to store and use coordinates in the world
public class Tile
{
    public int X;
    public int Y;
    public TilesType Type;

    //constructor
    public Tile(int x, int y, TilesType type = TilesType.nulltype) 
    {
        X = x;
        Y = y;
        Type = type;
    }
}

//Reminder of Tile type :                           //TODO : create a "tileType" list
// 0 = void
// 1 = wall
// 9 = not specified