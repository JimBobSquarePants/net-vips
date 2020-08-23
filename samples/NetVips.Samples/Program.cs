namespace NetVips
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Text;

    class Program
    {
        private static List<ISample> _samples = Assembly.GetExecutingAssembly().GetTypes()
            .Where(x => x.GetInterfaces().Contains(typeof(ISample)) && x.GetConstructor(Type.EmptyTypes) != null)
            .Select(x => Activator.CreateInstance(x) as ISample)
            .OrderBy(s => s?.Category)
            .ToList();

        static void Main(string[] args)
        {
            try
            {
                NetVips.Init();
            }
            catch (Exception)
            {
                Console.WriteLine("Error: Unable to init libvips. Please check your PATH environment variable.");
                Console.ReadLine();
                return;
            }

            Console.WriteLine($"libvips {NetVips.Version(0)}.{NetVips.Version(1)}.{NetVips.Version(2)}");

            Console.WriteLine(
                $"Type a number (1-{_samples.Count}) to execute a sample of your choice. Press <Enter> or type 'Q' to quit.");

            DisplayMenu();

            string input;
            do
            {
                string[] sampleArgs = { };
                if (args.Length > 0)
                {
                    var sampleId = _samples.Select((value, index) => new { Index = index + 1, value.Name })
                        .FirstOrDefault(s => s.Name.Equals(args[0]))?.Index;
                    input = sampleId != null ? $"{sampleId}" : "0";
                    sampleArgs = args.Skip(1).ToArray();
                }
                else
                {
                    input = Console.ReadLine();
                }

                if (int.TryParse(input, out var userChoice) && TryGetSample(userChoice, out var sample))
                {
                    Console.WriteLine($"Executing sample: {sample.Name}");
                    var result = sample.Execute(sampleArgs);
                    Console.WriteLine("Sample successfully executed!");
                    if (result != null)
                    {
                        Console.WriteLine($"Result: {result}");
                    }
                }
                else
                {
                    Console.WriteLine("Sample doesn't exists, try again");
                }

                // Clear any arguments
                args = new string[] { };
            } while (!string.IsNullOrEmpty(input) && !string.Equals(input, "Q", StringComparison.OrdinalIgnoreCase));
        }

        public static void DisplayMenu()
        {
            Console.WriteLine();
            Console.WriteLine("Menu:");

            string currCategory = null;
            var menu = _samples.Select((value, index) => new { Index = index + 1, value.Name, value.Category })
                .Aggregate(new StringBuilder(), (builder, pair) =>
                {
                    if (currCategory != pair.Category)
                    {
                        if (pair.Index > 1)
                        {
                            builder.AppendLine();
                        }

                        builder.AppendLine($" - {pair.Category}");

                        currCategory = pair.Category;
                    }

                    builder.AppendLine($"    {pair.Index}: {pair.Name}");
                    return builder;
                });

            Console.WriteLine(menu);
        }

        public static bool TryGetSample(int id, out ISample sample)
        {
            sample = _samples
                .Select((value, index) => new { Index = index + 1, Sample = value })
                .FirstOrDefault(pair => pair.Index == id)?.Sample;

            return sample != null;
        }
    }
}