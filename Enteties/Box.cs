

namespace SokobanMG.Enteties;

public class Box : Entity
{
    public Box()
    {
        Symbol = '$';
        IsWalkable = false;
        IsPushable = true;
    }
}