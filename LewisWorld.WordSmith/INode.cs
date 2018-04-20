using System;
namespace LewisWorld.WordSmith
{
    public interface INode
    {
        char Letter {
            get;
        }
        INode FirstChild {
            get;
        }
        INode NextSibling {
            get;
        }
        bool Terminal {
            get;
        }
        INode Get(char letter);
    }
}
