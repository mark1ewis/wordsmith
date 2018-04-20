using System;
using System.IO.MemoryMappedFiles;
using System.Runtime.InteropServices;

namespace LewisWorld.WordSmith.MemMapIndex
{
    public class MappedLibrary : ILibrary
    {
        int nodeSize = Marshal.SizeOf(typeof(Node));
        MemoryMappedFile file;
        MemoryMappedViewAccessor accessor;
        LibraryNode root;

        public MappedLibrary(String path)
        {
            file = MemoryMappedFile.CreateFromFile(path);
            accessor = file.CreateViewAccessor();
            Node rootSpec;
            accessor.Read(0, out rootSpec);
            root = new LibraryNode(this, rootSpec);
        }



        public INode Root => root;

        internal Node GetNode(UInt32 offset)
        {
            Node node;
            accessor.Read(nodeSize * offset, out node);
            return node;
        }

        public void Dispose()
        {
            if(accessor != null)
            {
                accessor.Dispose();
            }
            if (file != null)
            {
                file.Dispose();
            }
        }
    }
}
