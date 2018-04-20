using System;
using System.Threading.Tasks;
using LewisWorld.WordSmith.MemMapIndex;

namespace LewisWorld.WordSmith.IndexBuilder
{
    class Program
    {
        static async Task Main(string[] args)
        {
            try
            {
                var builder = new LibraryBuilder();
                await builder.Build("/usr/share/dict/words", "/Users/mark/words.index2");
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}
