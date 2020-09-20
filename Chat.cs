using System;
using System.IO;
using System.Text;
using System.Text.Json;

namespace MCServer
{
    public class Chat
    {
        public String Message;

        public Chat(String message)
        {
            Message = message;
        }

        public override string ToString()
        {
            MemoryStream stream = new MemoryStream();
            Utf8JsonWriter writer = new Utf8JsonWriter(stream);
            writer.WriteString("text", Message);
            writer.Flush();
            return Encoding.UTF8.GetString(stream.ToArray());
        }
    }
}