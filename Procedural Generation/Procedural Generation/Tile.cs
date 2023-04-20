namespace Procedural_Generation;
// a class to store and use coordinates in the world
public class Tile
{
    public Coordinates coordinates;
    public TilesType Type;

    //constructor
    public Tile(Coordinates coords, TilesType type = TilesType.nulltype)
    {
        coordinates = coords;
        Type = type;
    }
}

//Reminder of Tile type :                           //TODO : create a "tileType" list
// 0 = void
// 1 = wall
// 9 = not specified