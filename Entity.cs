namespace MCServer
{
    public class Entity
    {
        public readonly int EntityId = Server.EntityIdCounter++;
        public double X = 0, Y = 0, Z = 0;
        public float Yaw = 0, Pitch = 0;
    }
}