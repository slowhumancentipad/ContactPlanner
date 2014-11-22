using System;

namespace ContactPlanner
{
    public class Contact
    {
        public String FirstName     { get; set; }

        public String SecondName    { get; set; }

        public String LastName      { get; set; }

        public String Telephone     { get; set; }

        public String Email         { get; set; }

        public Contact(
                string _firstname
            ,   string _secondname
            ,   string _lastname
            ,   string _telephone
            ,   string _email
            )
        {
            FirstName   = _firstname;
            SecondName  = _secondname;
            LastName    = _lastname;
            Telephone   = _telephone;
            Email       = _email;
        }

    }
}
