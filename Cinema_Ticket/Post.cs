using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinema_Ticket
{
    public class Post
    {
        public Post(string name, string accesslevel)
        {
            postName = name;
            postAccessLevel = accesslevel;
        }
        public string postName { get; set; }
        public string postAccessLevel { get; set; }

        public override string ToString()
        {
            return this.postName + "   " + this.postAccessLevel;
        }
    }
}
