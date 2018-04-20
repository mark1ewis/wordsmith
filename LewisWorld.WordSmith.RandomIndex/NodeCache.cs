using System;
using System.IO;
using System.Text;

namespace LewisWorld.WordSmith.RandomIndex
{
    public class NodeCache : IDisposable, ILibrary
    {
        private FileStream file;
        private BinaryReader reader;
        private RandomNode root;
        private LRUCache<UInt32, RandomNode> cache = new LRUCache<uint, RandomNode>(1000);

        public INode Root { 
            get {
                return root;
            }
        }

        public NodeCache(string filename)
        {
            file = new FileStream(filename, FileMode.Open, FileAccess.Read);
            reader = new BinaryReader(file, Encoding.UTF32);
            root = Read(0);
        }

        public RandomNode Get(UInt32 offset)
        {
            if (offset == 0) return null;
            else
            {
                var node = cache.get(offset);
                if (node != null)
                {
                    return node;
                }
                else
                {
                    node = Read(offset);
                    cache.add(offset, node);
                    return node;
                }
            }
        }

        private RandomNode Read(UInt32 offset)
        {
            file.Seek(offset * Node.SIZE, 0);
            var letter = reader.ReadChar();
            var firstChild = reader.ReadUInt32();
            var nextSibling = reader.ReadUInt32();
            var terminal = reader.ReadBoolean();
            return new RandomNode(this, letter, firstChild, nextSibling, terminal);
        }

        public void Dispose()
        {
            if (reader != null) reader.Dispose();
            if (file != null) file.Dispose();
        }
    }
}
