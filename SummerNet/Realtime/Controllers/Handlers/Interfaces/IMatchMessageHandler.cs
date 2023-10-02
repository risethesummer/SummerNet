﻿using SummerNet.Realtime.Data;
using SummerNet.Realtime.Networks;

namespace SummerNet.Realtime.Controllers.Handlers.Interfaces;

public interface IMatchMessageHandler<TMessageData, TMatchData, TPlayerIndex, TPlayer> 
    where TMessageData : unmanaged
    where TPlayer : PlayerData<TPlayerIndex>
    where TMatchData : MatchData<TPlayerIndex, TPlayer>
{
    void OnMessage(IMatchController<TMatchData, TPlayerIndex, TPlayer> matchController, int tick, 
        in NetworkMessage<TMessageData> message);
}