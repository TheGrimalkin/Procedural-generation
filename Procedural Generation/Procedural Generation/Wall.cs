namespace Procedural_Generation;

    public class Wall 
    {
        public Orientation Direction;
        public Coordinates EndPos;
        public Coordinates EndNeg;

        public Wall(Coordinates endPos, Coordinates endNeg, Orientation direction)
        { 
            Direction = direction;
            EndPos = endPos;
            EndNeg = endNeg;

        }
    }
    
    //direction 3 means not assigned, 1 horizontal and 2 vertical