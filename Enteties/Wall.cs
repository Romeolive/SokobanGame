namespace SokobanMG.Enteties;

public class Wall : Entity
{
    public Wall()
    {
        Symbol = '#';
        IsWalkable = false;
    }
}