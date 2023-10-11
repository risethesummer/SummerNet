namespace Realtime.Controllers.Transporters.Messages;

public static class NetworkMessageCommonInfo
{
    public const int HeaderArgumentSize = sizeof(ushort);
    public static class ServerMsgArgumentPosition
    {
        public const int HeaderValuesCount = 3;
        public const int HeaderSize = HeaderValuesCount * HeaderArgumentSize;
        public const int MessageId = 0;
        public const int Opcode = MessageId + HeaderArgumentSize;
        public const int PayloadLength = Opcode + HeaderArgumentSize;
        public const int Payload = PayloadLength + HeaderArgumentSize;
    }
    
    public static class ClientMsgArgumentPosition
    {
        public const int HeaderValuesCount = 3;
        public const int HeaderSize = HeaderValuesCount * HeaderArgumentSize;
        // public static readonly Range OwnerIdx = Range.EndAt(HeaderArgumentSize);
        public static readonly Range MessageId = Range.EndAt(HeaderArgumentSize); 
        public static readonly Range Opcode = new(MessageId.End, HeaderArgumentSize * 2);
        public static readonly Range PayloadLength = new(Opcode.End, HeaderArgumentSize * 3); 
    }
}