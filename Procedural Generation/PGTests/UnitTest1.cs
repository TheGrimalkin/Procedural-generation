namespace PGTests;
using Procedural_Generation;

public class Tests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void TestGetTile()
    {
        World world = new World(11, 11, 5);
        Assert.AreEqual(new Tile(0,0, 0), world.GetTile(0,0));
    }

    [Test]
    public void TestWorld()
    {
        World world = new World(11, 9, 5);
        Assert.AreEqual(11, world.SizeX);
        Assert.AreEqual(9, world.SizeY);
    }
    
    [Test]
    public void TestEmptyWorld()
    {
        World world = new World(11, 9, 5);
        world.Grid[0, 0] = TilesType.wall;
        world.EmptyWorld();
    }
    
}