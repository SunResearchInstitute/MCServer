using System;

namespace MCServer
{
    public class Identifier
    {
        public String Namespace = "minecraft";
        public String Name = "";
        public Identifier(String value)
        {
            if (value.Contains(":"))
            {
                String[] split = value.Split(":");
                Namespace = split[0];
                Name = split[1];
            }
            else
            {
                Name = value;
            }
        }

        public override string ToString()
        {
            return Namespace + ":" + Name;
        }
    }
}