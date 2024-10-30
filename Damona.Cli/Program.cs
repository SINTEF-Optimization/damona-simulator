using Damona.Cli;
using Damona.Schemas.AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using Spectre.Console.Cli;
using Spectre.Console.Cli.Extensions.DependencyInjection;

var services = new ServiceCollection();
services.AddAutoMapper(typeof(MapProfile));
services.AddSingleton(new SpecialtyMap());

using var registrar = new DependencyInjectionRegistrar(services);
var app = new CommandApp<RunCommand>(registrar);
return app.Run(args);