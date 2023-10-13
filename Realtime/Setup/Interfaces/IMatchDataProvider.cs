using Realtime.Data;

namespace Realtime.Setup.Interfaces;

/// <summary>
/// Provide initial data of a match <br/>
/// If we don't recognise any implementation, we'll provide a basic one from configurations for you 
/// </summary>
/// <typeparam name="TMatchData">Your custom data inherits from <see cref="MatchData{TPlayerIndex,TAuthData,TPlayer}"/></typeparam>
/// <typeparam name="TPlayerIndex">TId in your <see cref="PlayerData{TId, TAuthData}"/></typeparam>
/// <typeparam name="TPlayer">Your custom data inherits from <see cref="PlayerData{TId, TAuthData}"/></typeparam>
public interface IMatchDataProvider<TMatchData, TPlayerIndex, TPlayer>
{
    ValueTask<TMatchData?> ProvideMatchData();
}