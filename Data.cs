using System;
using System.Collections.Generic;

namespace ContactPlanner
{
    static class Data
    {
        static public Dictionary< DateTime, List< Event > > Events { get; set; }

        static public List< Contact > Contacts { get; set; }

        static public DateTime CurrentDate { get; set; }

        static public DateTime LastDate { get; set; }

        static public int LastId { get; set; }
    }
}
