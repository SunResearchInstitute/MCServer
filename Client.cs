using System;
using System.Net.Sockets;
using System.Threading.Tasks;
using MCServer.Network;

namespace MCServer
{
    public enum ClientState
    {
        Handshaking,
        Status,
        Login,
        Play,
        Closed
    }

    public enum ChatMode
    {
        Enabled,
        CommandsOnly,
        Hidden
    }

    public enum PlayerHand
    {
        Left,
        Right
    }

    public class ClientSettings
    {
        public String Locale;
        public byte ViewDistance;
        public ChatMode ChatMode;
        public bool ChatColors;
        public byte DisplayedSkinParts;
        public PlayerHand MainHand;

        public bool CapeDisplayed
        {
            get => (DisplayedSkinParts & 1) != 0;
            set
            {
                if (value)
                    DisplayedSkinParts |= 1;
                else
                    DisplayedSkinParts &= 0b1111_1110;
            }
        }
        
        public bool JacketDisplayed
        {
            get => (DisplayedSkinParts & (1 << 1)) != 0;
            set
            {
                if (value)
                    DisplayedSkinParts |= (1 << 1);
                else
                    DisplayedSkinParts &= 0b1111_1101;
            }
        }
        
        public bool LeftSleeveDisplayed
        {
            get => (DisplayedSkinParts & (1 << 2)) != 0;
            set
            {
                if (value)
                    DisplayedSkinParts |= (1 << 2);
                else
                    DisplayedSkinParts &= 0b1111_1011;
            }
        }
        
        public bool RightSleeveDisplayed
        {
            get => (DisplayedSkinParts & (1 << 3)) != 0;
            set
            {
                if (value)
                    DisplayedSkinParts |= (1 << 3);
                else
                    DisplayedSkinParts &= 0b1111_0111;
            }
        }
        
        public bool LeftPantLegDisplayed
        {
            get => (DisplayedSkinParts & (1 << 4)) != 0;
            set
            {
                if (value)
                    DisplayedSkinParts |= (1 << 4);
                else
                    DisplayedSkinParts &= 0b1110_1111;
            }
        }
        
        public bool RightPantLegDisplayed
        {
            get => (DisplayedSkinParts & (1 << 5)) != 0;
            set
            {
                if (value)
                    DisplayedSkinParts |= (1 << 5);
                else
                    DisplayedSkinParts &= 0b1101_1111;
            }
        }
        
        public bool HatDisplayed
        {
            get => (DisplayedSkinParts & (1 << 6)) != 0;
            set
            {
                if (value)
                    DisplayedSkinParts |= (1 << 6);
                else
                    DisplayedSkinParts &= 0b1011_1111;
            }
        }
    }
    
    public class Client
    {
        public bool Compressed = false;
        public readonly NetworkClient NetworkClient;
        public ClientSettings Settings = new ClientSettings();
        public ClientState State = ClientState.Handshaking;
        public Player Player = null;

        public Client(NetworkStream stream)
        {
            NetworkClient = new NetworkClient(stream);
        }

        public async Task Handle()
        {
            Packet readPacket = new Packet(this);
            while (State != ClientState.Closed)
            {
                await readPacket.ReadFromClient();
                if (readPacket.PacketId == -1 || readPacket.Data == null)
                {
                    Console.WriteLine("Client Closed");
                    State = ClientState.Closed;
                    return;
                }

                Func<Client, byte[], Task> action;
                Console.WriteLine("[" + State + "] Packet " + readPacket.PacketId);
                switch (State)
                {
                    case ClientState.Handshaking:
                    {
                        if (!HandshakingHandler.Actions.TryGetValue(readPacket.PacketId, out action))
                        {
                            Console.WriteLine("Missing Packet " + readPacket.PacketId + " for handshaking!");
                            State = ClientState.Closed;
                            return;
                        }

                        break;
                    }
                    case ClientState.Status:
                    {
                        if (!StatusHandler.Actions.TryGetValue(readPacket.PacketId, out action))
                        {
                            Console.WriteLine("Missing Packet " + readPacket.PacketId + " for Status!");
                            State = ClientState.Closed;
                            return;
                        }

                        break;
                    }
                    case ClientState.Login:
                    {
                        if (!LoginHandler.Actions.TryGetValue(readPacket.PacketId, out action))
                        {
                            Console.WriteLine("Missing Packet " + readPacket.PacketId + " for Login!");
                            State = ClientState.Closed;
                            return;
                        }

                        break;
                    }
                    case ClientState.Play:
                    {
                        if (!PlayHandler.Actions.TryGetValue(readPacket.PacketId, out action))
                        {
                            Console.WriteLine("Missing Packet " + readPacket.PacketId + " for Play!");
                            State = ClientState.Closed;
                            return;
                        }

                        break;
                    }
                    default:
                    {
                        Console.WriteLine("Missing State " + State);
                        State = ClientState.Closed;
                        return;
                    }
                }
                await action.Invoke(this, readPacket.Data);
            }
            Console.WriteLine("Client Closed");
        }
    }
}