using Charon.Cli;
using Charon.Dojo;

await Console.Out.WriteLineAsync(@"
 _____ _                             _____  _     _____ 
/  __ \ |                           /  __ \| |   |_   _|
| /  \/ |__   __ _ _ __ ___  _ __   | /  \/| |     | |  
| |   | '_ \ / _` | '__/ _ \| '_ \  | |    | |     | |  
| \__/\ | | | (_| | | | (_) | | | | | \__/\| |_____| |_ 
 \____/_| |_|\__,_|_|  \___/|_| |_|  \____/\_____/\___/ 
");

ArgumentHandling.Sample();

if (args.Length == 0)
    DojoRunner.Execute();
