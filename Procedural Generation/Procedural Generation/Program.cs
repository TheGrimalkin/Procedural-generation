using Procedural_Generation;

bool debug = false;

//Initializing the world
World world = new World(11, 11, 10);

//Place Walls at random
world.GenerateWallSeeds();
world.WallSeedGrowth();

world.WriteTile(0,0, TilesType.center);






//prints the grid
//world.PrintWorld();




//All the tests for the funcs
if (debug)
{
    //Tests the TilesAtRange func
    Console.Write("These are the tiles within 1 tiles of (0, 0) : ");
    foreach (var v in world.TilesAtRange(1, new Tile(0,0)))
    {
        Console.Write(v.Type + " ");
    }
    Console.WriteLine();

//Tests the IsThereTile func
    Console.Write("Is there a wall around (0, 0) ? : " + world.IsThereTile(new Tile(0,0, TilesType.wall), 1));
    Console.WriteLine();

//Tests the HowMuchTilesAtRange func
    Console.Write("How much walls there is around (0, 0) ? : " + world.HowMuchTilesAtRange(new Tile(1,1, TilesType.wall), 1));
    Console.WriteLine();


//tests the % of walls in the grid
    float chance = 0;
    foreach (int i in world.Grid)
    {
        if (i== 1)
        {
            chance++;
        }
    }
    int gridLength = world.Grid.Length;
    Console.WriteLine($"There was a {chance/gridLength*100}% chance of generating a wall");
}
