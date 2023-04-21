using System.Diagnostics;
namespace Procedural_Generation;

public enum TilesType
{
    empty = 0, wall = 1, center = 8, nulltype = 9
}

public enum Orientation
{
    horizontal = 1, vertical = 2
}

public class World
{
    private Random _random = new Random();

    //parameters
    public int SizeX;
    public int SizeY;
    public readonly int ChanceOfGeneratingWall;
    public TilesType[,] Grid;
    public int _xOffset; //The amount to convert index to coord (translation needed for the switch)
    public int _yOffset;

    public List<Wall> _wallList = new List<Wall>(); 
    public List<Wall> FinishedWallList = new List<Wall>();

    //Constructor
    public World(int sizeX, int sizeY, int chanceOfGeneratingWall)
    {
        SizeX = sizeX;
        SizeY = sizeY;
        ChanceOfGeneratingWall = chanceOfGeneratingWall;
        Grid = new TilesType[sizeX, sizeY];
        EmptyWorld();
        _xOffset = (Grid.GetLength(0) - 1) / 2;
        _yOffset = (Grid.GetLength(1) - 1) / 2;
    }

// Here are all the functions to read and get tiles :
    
    //Functions to get the index from a coordinate. Used in other function to make the conversion to indexes
    private int GetIndexX(int x)
    { 
        int indexX = x + _xOffset;
        if (indexX < 0 || indexX >= Grid.GetLength(0))
        {
            throw new IndexOutOfRangeException($"X ({x}) out of bounds [{-_xOffset}, {_xOffset}]");
        }
        return indexX;
    }
    
    public int GetIndexY(int y)
    {
        int indexY = -y + _yOffset;
        if (indexY < 0 || indexY >= Grid.GetLength(1))
        {
            throw new IndexOutOfRangeException($"Y ({y}) out of bounds [{-_yOffset}, {_yOffset}]");
        }
        return indexY;
    }
    
    //Function that returns the coordinates from an index. Used mostly in other functions

    private  int GetCoordX(int x)
    { 
        int coordX = x - _xOffset;
        return coordX;
    }
    
    private  int GetCoordY(int y)
    { 
        int coordY = -y + _yOffset;
        return coordY;
    }

    //Funcs to interact with a specific tile
    public Tile GetTile(int x, int y)
    {
        return new Tile(new Coordinates(x,y), Grid[GetIndexX(x), GetIndexY(y)]);
    }

    public void WriteTile(Coordinates coords, TilesType type) 
    {
        Grid[GetIndexX(coords.x), GetIndexY(coords.y)] = type;
    }

    //Func that returns a list of every tiles at X range from a base point
    public List<Tile> TilesAtRange(int range, Tile baseTile) 
    {
        List<Tile> returnList = new List<Tile>();
        for (int x = baseTile.coordinates.x - range; x <= baseTile.coordinates.x + range; x++)
        {
            if (x < -_xOffset || x > _xOffset)
            {
                continue;
            }
            for (int y = baseTile.coordinates.y - range; y <= baseTile.coordinates.y + range; y++)
            {
                if (y < -_yOffset || y > _yOffset)
                {
                    continue;
                }
                
                if(baseTile.coordinates.x == x && baseTile.coordinates.y==y) {continue;} //Prevents counting itself
                returnList.Add(GetTile(x,y));
            }
        }
        return returnList;
    }
    
    //Func to check if there is x type of tile at y range, similar to HowMuchTilesAtRange but more efficient and for only one tile      
    public bool IsThereTile(Tile baseTile, int range)
    {
        foreach (Tile tile in TilesAtRange(range, baseTile))
        {
            if (tile.coordinates.x == baseTile.coordinates.x && tile.coordinates.y == baseTile.coordinates.y) {continue;}     //prevents counting itself
            if (tile.Type == baseTile.Type)
            {
                return true;
            }
        }
        return false;
    }

    public int HowMuchTilesAtRange(Tile baseTile, int range)            
    {
        int amount = 0;
        foreach (Tile tile in TilesAtRange(range, baseTile))
        {
            if (tile.coordinates.x == baseTile.coordinates.x && tile.coordinates.y == baseTile.coordinates.y) {continue;}     //prevents counting itself
            if (tile.Type == baseTile.Type)
            {
                amount++;
            }
        }
        return amount;
    }
    
    public bool WallSearchX(Wall baseWall, int range, bool isPositive)      //Like HowMuchTilesAtRange but doesn't count those who are from the same wall     
    {                                                                   // IsPositive is to check if it is the EndPos or the EndNeg that we need to target as a first tile
        if (isPositive)
        {
            foreach (Tile tile in TilesAtRange(range, new Tile(new Coordinates(baseWall.EndPos.x + 1, baseWall.EndPos.y), TilesType.wall)))
            {
                if (tile.Type == TilesType.wall )      
                {
                    if (tile.coordinates.y != baseWall.EndPos.y) // true if isn't on the same line 
                    {
                        return true;
                    }
                    if ( tile.coordinates.x > baseWall.EndPos.x || tile.coordinates.x < baseWall.EndNeg.x) //true if tile is not in the wall interval
                    {
                        return true;
                    }
                }
            }
        }
        else
        {
            foreach (Tile tile in TilesAtRange(range, new Tile(new Coordinates(baseWall.EndNeg.x - 1, baseWall.EndNeg.y), TilesType.wall)))
            {
                if (tile.Type == TilesType.wall )      
                {
                    if (tile.coordinates.y != baseWall.EndNeg.y) 
                    {
                        return true;
                    }
                    if ( tile.coordinates.x > baseWall.EndPos.x || tile.coordinates.x < baseWall.EndNeg.x) 
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }
    
    public bool WallSearchY(Wall baseWall, int range, bool isPositive)           //for the vertical direction (change in the interval calculation)
    {                                                                   
        if (isPositive)
        {
            foreach (Tile tile in TilesAtRange(range, new Tile(new Coordinates(baseWall.EndPos.x, baseWall.EndPos.y + 1), TilesType.wall)))
            {
                if (tile.Type == TilesType.wall )      
                {
                    if (tile.coordinates.x != baseWall.EndPos.x) 
                    {
                        return true;
                    }
                    if ( tile.coordinates.y > baseWall.EndPos.y || tile.coordinates.y < baseWall.EndNeg.y) 
                    {
                        return true;
                    }
                }
            }
        }
        else
        {
            foreach (Tile tile in TilesAtRange(range, new Tile(new Coordinates(baseWall.EndNeg.x, baseWall.EndNeg.y - 1), TilesType.wall)))
            {
                if (tile.Type == TilesType.wall )      
                {
                    if (tile.coordinates.x != baseWall.EndNeg.x) 
                    {
                        return true;
                    }
                    if ( tile.coordinates.y > baseWall.EndPos.y || tile.coordinates.y < baseWall.EndNeg.y) 
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }
    
    
    
    
    
//Here are all the function that relate to generating the world :

    //func to fill the grid with 0
    public void EmptyWorld()
    {
        for (int x = 0; x < SizeX; x++)
        {
            for (int y = 0; y < SizeY; y++)
            {
                Grid[x, y] = 0;
            }
        }
    }

    //Place Walls at random
    public void GenerateWallSeeds() //TODO to test, might be why WallSeedGrowth don't work
    {
        for (int y = 0; y < SizeY; y++)             //Cycle in the world to create wall (index used here for simplicity)
        {
            for (int x = 0; x < SizeX; x++)
            {
                if (_random.Next(1,101) <= ChanceOfGeneratingWall)
                {
                    if (!IsThereTile(new Tile(new Coordinates(GetCoordX(x),GetCoordY(y)),TilesType.wall),1))
                    {
                        Grid[x, y] = TilesType.wall;
                        _wallList.Add(new Wall(new Coordinates(GetCoordX(x),GetCoordY(y)),new Coordinates(GetCoordX(x),GetCoordY(y)), (Orientation)_random.Next(1,3)));
                    }
                }
            }
        }
    }
    
    //Func to start the wall's extension 
    public void WallSeedGrowth()
    {
        List<Wall> wallsToRemove = new List<Wall>();
        bool updatedPos;
        bool updatedNeg;
        foreach (var wall in _wallList)
        {
            updatedPos = false;
            updatedNeg = false;
            if (wall.Direction == Orientation.horizontal)       
            {
                if (wall.EndPos.x + 1 <= _xOffset && !WallSearchX(wall,1,true))  //Checks if able to put a wall        
                {
                    WriteTile(new Coordinates(wall.EndPos.x+1, wall.EndPos.y), TilesType.wall);
                    wall.EndPos.x++;
                    updatedPos = true;
                }
                if ( wall.EndNeg.x -1 >= -_xOffset && !WallSearchX(wall,1,false))
                {
                    WriteTile( new Coordinates(wall.EndNeg.x-1, wall.EndNeg.y), TilesType.wall);
                    wall.EndNeg.x--;
                    updatedNeg = true;
                }
            }
            if (wall.Direction == Orientation.vertical)
            {
                if (wall.EndPos.y + 1 <= _yOffset && !WallSearchY(wall,1,true))
                {
                    WriteTile(new Coordinates(wall.EndPos.x, wall.EndPos.y+1), TilesType.wall);
                    wall.EndPos.y++;
                    updatedPos = true;
                }
                if ( wall.EndNeg.y -1 >= -_yOffset && !WallSearchY(wall,1,false))
                {
                    WriteTile(new Coordinates(wall.EndNeg.x, wall.EndNeg.y-1), TilesType.wall);
                    wall.EndNeg.y--;
                    updatedNeg = true;
                }
            }

            if (!updatedPos && !updatedNeg)     // Checks if wall has finished growing
            {
                FinishedWallList.Add(wall);
                wallsToRemove.Add(wall);
            }
        }

        foreach (var wall in wallsToRemove)     //  Removes all the walls that are finished
        {
            _wallList.Remove(wall);
        }
        
        if (_wallList.Count>0)
        {
            WallSeedGrowth();               // Calls the function again if all the walls aren't finished
        }
    }
    
    
    public void PrintWorld()
    {
        for (int y = 0; y < SizeX; y++)
        {
            for (int x = 0; x < SizeY; x++)
            {
                if (Grid[x,y]==TilesType.wall)
                {
                    Console.Write(" \u25A0");
                }
                else
                {
                    Console.Write("  " ); 
                }
            }
            Console.WriteLine();
        }
    }
}