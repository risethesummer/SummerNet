namespace Realtime.Controllers.Transporters.Messages;

public static class NetworkMessageCommonInfo
{
    public const int HeaderArgumentSize = sizeof(ushort);
    public static class ServerMsgArgumentPosition
    {
        public const int HeaderValuesCount = 3;
        public const int HeaderSize = HeaderValuesCount * HeaderArgumentSize;
        public static readonly Range Opcode = Range.EndAt(HeaderArgumentSize); 
        public static readonly Range PayloadLength = new(Opcode.End, HeaderArgumentSize * 2); 
        public static readonly Range Payload = new(PayloadLength.End, HeaderArgumentSize * 3); 
    }
    
    public static class ClientMsgArgumentPosition
    {
        public const int HeaderValuesCount = 2;
        public const int HeaderSize = HeaderValuesCount * HeaderArgumentSize;
        // public static readonly Range OwnerIdx = Range.EndAt(HeaderArgumentSize);
        public static readonly Range Opcode = Range.EndAt(HeaderArgumentSize); 
        public static readonly Range PayloadLength = new(Opcode.End, HeaderArgumentSize * 2); 
    }
}