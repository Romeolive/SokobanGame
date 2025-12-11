

namespace SokobanMG.Enteties;

public class BoxOnGoal : Entity
{
    public BoxOnGoal()
    {
        Symbol = '*';
        IsWalkable = false;
        IsPushable = true;
    }
}