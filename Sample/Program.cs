// See https://aka.ms/new-console-template for more information

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Realtime.Data;

Console.Title = "Hello world";
var host = new WebHostBuilder()
    .UseKestrel()
    .Build();     
using var app = Host.CreateDefaultBuilder(args)
    
     .ConfigureServices(services =>
     {
         // services.Add(ServiceDescriptor.Singleton<>());
         // services.AddSingleton<MyClass2>();        
         // services.AddSingleton<MyClass>();        
     })
     .Build();
// var myClass = host.Services.GetService<MyClass>();
// await myClass!.DoStuff();
//
await host.RunAsync();

// Generate all code depending all 2 classes
