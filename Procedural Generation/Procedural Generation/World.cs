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
    
    private int GetIndexY(int y)
    {
        int indexY = -y + _yOffset;
        if (indexY < 0 || indexY >= Grid.GetLength(1))
        {
            throw new IndexOutOfRangeException($"Y ({y}) out of bounds [{-_yOffset}, {_yOffset}]");
        }
        return indexY;
    }
    
    //Function that returns the coordinate from an index. Used mostly in other functions
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
        return new Tile(x, y, Grid[GetIndexX(x), GetIndexY(y)]);
    }

    public void WriteTile(int x, int y, TilesType type) 
    {
        Grid[GetIndexX(x), GetIndexY(y)] = type;
    }

    //Func that returns a list of every tiles at X range from a base point
    public List<Tile> TilesAtRange(int range, Tile baseTile) 
    {
        List<Tile> returnList = new List<Tile>();
        for (int x = baseTile.X - range; x <= baseTile.X + range; x++)
        {
            if (x < -_xOffset || x > _xOffset)
            {
                continue;
            }
            for (int y = baseTile.Y - range; y <= baseTile.Y + range; y++)
            {
                if (y < -_yOffset || y > _yOffset)
                {
                    continue;
                }
                
                if(baseTile.X == x && baseTile.Y==y) {continue;} //Prevents counting itself
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
            if (tile.X == baseTile.X && tile.Y == baseTile.Y) {continue;}     //prevents counting itself
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
            if (tile.X == baseTile.X && tile.Y == baseTile.Y) {continue;}     //prevents counting itself
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
            foreach (Tile tile in TilesAtRange(range, new Tile(baseWall.EndPos[0], baseWall.EndPos[1], TilesType.wall)))
            {
                if (tile.Type == TilesType.wall )      
                {
                    if (tile.Y != baseWall.EndPos[1]) // true if isn't on the same line 
                    {
                        return true;
                    }
                    if ( tile.X > baseWall.EndPos[0] || tile.X < baseWall.EndNeg[0]) //true if tile is not in the wall interval
                    {
                        return true;
                    }
                }
            }
        }
        else
        {
            foreach (Tile tile in TilesAtRange(range, new Tile(baseWall.EndNeg[0], baseWall.EndNeg[1], TilesType.wall)))
            {
                if (tile.Type == TilesType.wall )      
                {
                    if (tile.Y != baseWall.EndNeg[1]) 
                    {
                        return true;
                    }
                    if ( tile.X > baseWall.EndPos[0] || tile.X < baseWall.EndNeg[0]) 
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
            foreach (Tile tile in TilesAtRange(range, new Tile(baseWall.EndPos[0], baseWall.EndPos[1], TilesType.wall)))
            {
                if (tile.Type == TilesType.wall )      
                {
                    if (tile.X != baseWall.EndPos[0]) 
                    {
                        return true;
                    }
                    if ( tile.Y > baseWall.EndPos[1] || tile.Y < baseWall.EndNeg[1]) 
                    {
                        return true;
                    }
                }
            }
        }
        else
        {
            foreach (Tile tile in TilesAtRange(range, new Tile(baseWall.EndNeg[0], baseWall.EndNeg[1], TilesType.wall)))
            {
                if (tile.Type == TilesType.wall )      
                {
                    if (tile.X != baseWall.EndNeg[0]) 
                    {
                        return true;
                    }
                    if ( tile.Y > baseWall.EndPos[1] || tile.Y < baseWall.EndNeg[1]) 
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
                    if (!IsThereTile(new Tile(GetCoordX(x),GetCoordY(y),TilesType.wall),1))
                    {
                        Grid[x, y] = TilesType.wall;
                        _wallList.Add(new Wall(new int[]{GetCoordX(x),GetCoordY(y)},new int[]{GetCoordX(x),GetCoordY(y)}, (Orientation)_random.Next(1,3)));
                    }
                }
            }
        }
    }
    
    //Func to start the wall's extension 
    public void WallSeedGrowth()    //TODO don't work
    {
        
        //debugging
        //_wallList.Add(new Wall(new []{0,2},new []{0,2}, Orientation.horizontal));
        //WriteTile(0,2, TilesType.wall);
        
        //List<int> indexesToRemove = new List<int>();
        int currentIndex;
        bool updatedPos;
        bool updatedNeg;
        for (int i = 0; i < 3; i++)
        {
            //if (_wallList.Count>0)
            //{
                currentIndex = 0;
                foreach (var wall in _wallList)
                {
                    updatedPos = false;
                    updatedNeg = false;
                    if (wall.Direction == Orientation.horizontal)
                    {
                        if (wall.EndPos[0] + 1 <= _xOffset && !WallSearchX(wall,1,true))     //this doesn't work     
                        {
                            WriteTile(wall.EndPos[0]+1, wall.EndPos[1], TilesType.wall);
                            wall.EndPos[0]++;
                            updatedPos = true;
                        }
                        if ( wall.EndNeg[0] -1 >= -_xOffset && !WallSearchX(wall,1,false))
                        {
                            WriteTile(wall.EndNeg[0]-1, wall.EndNeg[1], TilesType.wall);
                            wall.EndNeg[0]--;
                            updatedNeg = true;
                        }
                    }
                    if (wall.Direction == Orientation.vertical)
                    {
                        if (wall.EndPos[1] + 1 <= _yOffset && !WallSearchY(wall,1,true))
                        {
                            WriteTile(wall.EndPos[0], wall.EndPos[1]+1, TilesType.wall);
                            wall.EndPos[1]++;
                            updatedPos = true;
                        }
                        if ( wall.EndNeg[1] -1 >= -_yOffset && !WallSearchY(wall,1,false))
                        {
                            WriteTile(wall.EndNeg[0], wall.EndNeg[1]-1, TilesType.wall);
                            wall.EndNeg[1]--;
                            updatedNeg = true;
                        }
                    }

                    if (updatedNeg==false && updatedPos==false)
                    {
                        //indexesToRemove.Add(currentIndex);
                    }
                    currentIndex++;
                }
                PrintWorld();
                Console.WriteLine();
            //}
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
                    Console.Write(" \u2B1A"); 
                }
            }
            Console.WriteLine();
        }
    }
}