using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserInterface
{
    class Container
    {

    }

    class ProjectContainer
    {
        public string Name;
        public int MaxMembers;
        public int MinMembers;

        public ProjectContainer(string name, int max, int min)
        {
            Name = name;
            MaxMembers = max;
            MinMembers = min;
        }
    }
}
