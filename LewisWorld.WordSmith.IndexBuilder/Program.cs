using System;
using System.Threading.Tasks;

namespace LewisWorld.WordSmith.IndexBuilder
{
    class Program
    {
        static async Task Main(string[] args)
        {
            try
            {
                var builder = new LibraryBuilder();
                await builder.Build("/usr/share/dict/words", "/Users/mark/words.index");
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}
