using System;
namespace LewisWorld.WordSmith
{
    public interface ILibrary : IDisposable
    {
        INode Root {
            get;
        } 
    }
}
