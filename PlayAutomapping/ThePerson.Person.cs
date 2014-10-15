using System.Collections.Generic;

namespace PlayAutomapping
{
    public static partial class ThePerson
    {
        public class Person
        {
            public  int     PersonId    { get; set; }
            public  string  PersonName  { get; set; }

            
            public IList<Hobby> Hobbies { get; set; }
            

            void AddHobby(Hobby hobby)
            {
                hobby.Person = this;
                this.Hobbies.Add(hobby);                
            }

        }

        public class Hobby
        {
            public      Person  Person              { get; set; }

            public      int     HobbyId             { get; set; }
            
            public      string  HobbyDescription    { get; set; }
        }
    }

   
}
