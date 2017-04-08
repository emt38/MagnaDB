using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagnaDB
{
    /// <summary>
    /// You can create the tables in a SQL Server to try this OUT :)
    /// </summary>
    class Foo
    {
        public int IdFoo { get; set; }

        [ForeignRelation("<{0}.IdFoo> = {1}.IdFoo")]
        public IEnumerable<Bar> Bars { get; set; }
    }

    class Bar
    {
        public int IdBar { get; set; }
        public int IdFoo { get; set; }

        [ForeignRelation("<{0}.IdFoo> = {1}.IdFoo")]
        public Foo MyFoo { get; set; }
    }
}
