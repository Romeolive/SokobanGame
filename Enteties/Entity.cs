namespace SokobanMG.Enteties;


public abstract class Entity
{ 
        public char Symbol { get; protected set; }
        public bool IsWalkable { get; protected set; }
        public bool IsPushable { get; protected set; }
}

