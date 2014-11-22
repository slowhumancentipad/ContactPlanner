using System;
using System.Collections.Generic;

namespace ContactPlanner
{
    public enum PriorityKind 
    { 
            Low
        ,   Middle
        ,   High 
    }   

    public class Event
    {
        private int m_id;

        public DateTime Date            { get; set; }

        public String Header            { get; set; }

        public String Description       { get; set; }

        public List< Contact > Contacts { get; set; }

        public PriorityKind Priority    { get; set; }

        public Event(
                DateTime        _date
            ,   string          _header
            ,   string          _descripton
            ,   List< Contact > _contactList
            ,   PriorityKind    _priority
            ,   int             _id
            )
        {
            Date        = _date;
            Header      = _header;
            Description = _descripton;
            Contacts    = _contactList;
            Priority    = _priority;
            m_id        = _id;
        }

        public int getId()
        {
            return m_id;
        }
    }
}
