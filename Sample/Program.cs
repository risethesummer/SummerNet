﻿// See https://aka.ms/new-console-template for more information

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Realtime.Data;

using IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.Add(ServiceDescriptor.Singleton<>());
        services.AddSingleton<MyClass2>();        
        services.AddSingleton<MyClass>();        
    })
    .Build();

var myClass = host.Services.GetService<MyClass>();
await myClass!.DoStuff();

await host.RunAsync();

// Generate all code depending all 2 classes
public class MyPlayerData : PlayerData<int>
{
}
public class MatchData : MatchData<int, MyPlayerData>
{
}