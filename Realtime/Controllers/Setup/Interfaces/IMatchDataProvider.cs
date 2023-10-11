namespace Realtime.Controllers.Setup.Interfaces;

/// <summary>
/// How to get data from other sources
/// </summary>
/// <typeparam name="TMatchData"></typeparam>
/// <typeparam name="TPlayerIndex"></typeparam>
/// <typeparam name="TPlayer"></typeparam>
public interface IMatchDataProvider<TMatchData, TPlayerIndex, TPlayer>
{
    ValueTask<TMatchData> ProvideMatchData();
}