using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;


using System.Linq;


using DomainMapping;
using PlayAutomapping;

using NHibernate.Linq;

namespace UnitTest
{
    [TestClass]
    public class TheUnitTest
    {
        [TestMethod]
        public void Test_entity()
        {
            using (var sf = Mapper.BuildSessionFactory(useUnitTest: true))
            using (var session = sf.OpenSession())
            {
                // Arrange
                string expectedProduct = "Tesla";
                string expectedCategory = "Car";

                // Act
                var p = session.Get<TheProduction.Product>(1);

                // Assert
                Assert.AreEqual(expectedProduct, p.ProductName, "Name");
                Assert.AreEqual(expectedCategory, p.ProductCategory.ProductCategoryName, "Product Category");
            }
        }

        [TestMethod]
        public void Test_collection()
        {
            using (var sf = Mapper.BuildSessionFactory(useUnitTest: true))
            using (var session = sf.OpenSession())
            {
                // Arrange
                string expectedPerson = "Linus";
                int expectedHobbyCount = 2;

                // Act
                var p = session.Get<ThePerson.Person>(1);

                // Assert
                Assert.AreEqual(expectedPerson, p.PersonName);
                Assert.AreEqual(expectedHobbyCount, p.Hobbies.Count());

            }
        }


        [TestMethod]
        public void Test_collection_list_all()
        {
            using (var sf = Mapper.BuildSessionFactory(useUnitTest: true))
            using (var session = sf.OpenSession())
            {  
                var p = session.Get<ThePerson.Person>(1);
                var hl = p.Hobbies.ToList(); // check the SQL if properly mapped                
            }
        }


        [TestMethod]
        public void Test_collection_list_all_using_fetch()
        {
            using (var sf = Mapper.BuildSessionFactory(useUnitTest: true))
            using (var session = sf.OpenSession())
            {
                var p = session.Query<ThePerson.Person>().Fetch(x => x.Hobbies).Single(x => x.PersonId == 1); // check the SQL if properly mapped
            }
        }


        [TestMethod]
        public void Test_DDD_ValueObject_directly()
        {
            using (var sf = Mapper.BuildSessionFactory(useUnitTest: true))
            using (var session = sf.OpenSession())
            {
                var hobbies = session.Query<ThePerson.Hobby>().ToList();

            }
        }

        [TestMethod]
        public void Test_save()
        {
            string personName = "Zorro " + Guid.NewGuid();
            SavePerson(personName);
        }

        private static void SavePerson(string personName)
        {

            // Arrange
            using (var sf = Mapper.BuildSessionFactory(useUnitTest: true))
            using (var session = sf.OpenSession())
            using (var tx = session.BeginTransaction())
            {
                var person = new ThePerson.Person
                {
                    PersonName = personName,
                    Hobbies = new[] 
                    { 
                        new ThePerson.Hobby { HobbyDescription = "Riding Horse" },
                        new ThePerson.Hobby { HobbyDescription = "Saving Princess" },
                        new ThePerson.Hobby { HobbyDescription = "Giving to the poor" }
                    }
                };


                foreach (var hobby in person.Hobbies)
                    hobby.Person = person;


                session.Save(person);

                tx.Commit();
            }



            using (var sf = Mapper.BuildSessionFactory(useUnitTest: true))
            using (var session = sf.OpenSession())
            using (var tx = session.BeginTransaction())
            {
                // Arrange
                int expectedCount = 3;

                // Act
                var person = session.Query<ThePerson.Person>().Where(x => x.PersonName == personName).Single();

                // Assert
                Assert.AreEqual(expectedCount, person.Hobbies.Count());
            }
        }

        [TestMethod]
        public void Test_delete()
        {
    
            // Arrange
            string personName = "Zorro " + Guid.NewGuid();
            SavePerson(personName);

            using (var sf = Mapper.BuildSessionFactory(useUnitTest: true))
            using (var session = sf.OpenSession())
            using (var tx = session.BeginTransaction())
            {
                int personId = session.Query<ThePerson.Person>().Single(x => x.PersonName == personName).PersonId;

                var person = session.Load<ThePerson.Person>(personId);

                // Act
                session.Delete(person);
                tx.Commit();
            }
        }

        [TestMethod]
        public void Test_table_not_on_schema()
        {
            using (var sf = Mapper.BuildSessionFactory(useUnitTest: true))
            using (var session = sf.OpenSession())
            using (var tx = session.BeginTransaction())
            {
                // Arrange
                string expected = "Something";

                // Act
                var t = session.Get<TableNotOnSchema>(1);

                // Act
                Assert.AreEqual(expected, t.TheValue);
            }
        }

    }
}
