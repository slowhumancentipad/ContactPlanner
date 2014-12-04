using System;
using System.Collections.Generic;

namespace ContactPlanner
{
    public enum PriorityKind 
    { 
            Unspecified
        ,   Low
        ,   Middle
        ,   High 
    }   

    public class Event
    {
        private DateTime m_date;
        private PriorityKind m_priority;
        private int m_id;

        public String Time        
        { 
            get 
            { 
                return "  " + m_date.ToShortTimeString();
            }
            
            private set { } 
        }

        public String DateAndTime
        {
            get
            {
                return m_date.ToShortDateString() + ' ' + m_date.ToShortTimeString();
            }

            private set { }
        }

        public String Header            { get; set; }

        public String Description       { get; set; }

        public List< Contact > Contacts { get; set; }

        public Event(
                DateTime        _date
            ,   string          _header
            ,   string          _descripton
            ,   List< Contact > _contactList
            ,   PriorityKind    _priority
            ,   int             _id
            )
        {
            m_date      = _date;
            Header      = _header;
            Description = _descripton;
            Contacts    = _contactList;
            m_priority    = _priority;
            m_id        = _id;
        }

        public Event(DateTime _date)
        {
            m_date = _date;
            Contacts = new List<Contact>();
            m_priority = PriorityKind.Unspecified;
        }

        public int getId()
        {
            return m_id;
        }


        public DateTime getDate()
        {
            return m_date;
        }


        public void setDate(DateTime _date)
        {
            m_date = _date;
        }


        public PriorityKind getPriority()
        {
            return m_priority;
        }


        public void setPriority(PriorityKind _priority)
        {
            m_priority = _priority;
        }
    }
}
