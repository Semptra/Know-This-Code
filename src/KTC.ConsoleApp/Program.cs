namespace KTL.ConsoleApp
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using KTL.Core;

    class Program
    {
        static async Task Main(string[] args)
        {
            var codeProvider = new CodeProvider();
            int score = 0;

            while(true)
            {
                try
                {
                    Console.Clear();
                    Console.WriteLine("----------------------------------------------------------");
                    Console.WriteLine($"Score: {score}");
                    Console.WriteLine("----------------------------------------------------------");

                    var languages = codeProvider.RandomLanguages.Take(4).ToList();
                    var language = languages.GetRandomElement();
                    var code = await codeProvider.GetRandomCodeSample(language);

                    Console.WriteLine(code);
                    Console.WriteLine("----------------------------------------------------------");
                    for(int i = 0; i < 4; i++)
                    {
                        Console.WriteLine($"{i + 1}. {languages[i]}");
                    }
                    Console.Write("What is that language? ");
                    var ans = int.Parse(Console.ReadLine());

                    if (languages[ans - 1] == language)
                    {
                        Console.Write("YASS!");
                        score++;
                    }
                    else
                    {
                        Console.WriteLine($"NAAh, it's {language}. Game over!");
                        return;
                    }

                    Console.ReadLine();
                }
                catch
                {
                    Console.WriteLine("Sorry, some error happened. Press Enter to restart.");
                }
            }
        }
    }
}
