using System;
using EFDDD.DataModel;
using EFDDD.DataModel.EF;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data.Entity.Migrations;

namespace DataModel.Test
{
    [TestClass]
    public class EF_HelpersTests
    {
        private static void SeedContext(IContext context)
        {
            context.HomeOwners.AddOrUpdate(x => x.Id,
                        new HomeOwner
                        {
                            Id = new Guid("9E35B69F-DE06-49C7-A797-1059BBB55D0A"),
                            Name = "Blevins Dale"
                        },
                        new HomeOwner
                        {
                            Id = new Guid("D596B755-133E-47DD-BD3D-E2EC54C6507F"),
                            Name = "Katharine Alvarado"
                        });

            context.Home.AddOrUpdate(x => x.Id,
                new Home
                {
                    Id = new Guid("A7FBD140-1CE1-4D6F-9123-29142FDA9E8B"),
                    Address = "901 Willow Street, Klagetoh, Kentucky, 4422",
                    DateBuilt = new DateTime(2000, 1, 1),
                    Neighborhood = new Neighborhood("Central Estates", true),
                    HomeOwnerId = new Guid("9E35B69F-DE06-49C7-A797-1059BBB55D0A")
                },
                new Home
                {
                    Id = new Guid("5DA841F7-1111-4F71-8004-CCCFDDB1BE84"),
                    Address = "285 Elliott Place, Bagtown, California, 7415",
                    DateBuilt = new DateTime(2000, 12, 15),
                    Neighborhood = new Neighborhood("Westwood", false),
                    HomeOwnerId = new Guid("D596B755-133E-47DD-BD3D-E2EC54C6507F")
                },
                new Home
                {
                    Id = new Guid("29D25509-78B6-423F-9B00-A6831341A278"),
                    Address = "754 Butler Street, Seymour, Wyoming, 5762",
                    DateBuilt = new DateTime(1982, 5, 1),
                    Neighborhood = new Neighborhood("754 Butler Street, Seymour, Wyoming, 5762", true),
                    HomeOwnerId = new Guid("9E35B69F-DE06-49C7-A797-1059BBB55D0A")
                });
        }

        [TestClass]
        public class GetTableNameMethod
        {
            [TestMethod]
            public void ShouldMatch()
            {
                // Arrange
                var connectionString = Helpers.GetRandomConnectionString();
                var dbContext = new Context(connectionString);

                try
                {
                    Helpers.SetInitializer(new ContextInitializer(SeedContext));
                    dbContext.Database.Initialize(true); //necessary when not performing query through the EF api

                    string expected, actual;

                    actual = dbContext.GetTableName<Home>();
                    expected = string.Format("{0}.{1}", Constants.DB_SCHEMA, "Homes");
                    Assert.AreEqual(expected, actual);


                    actual = dbContext.GetTableName<HomeOwner>();
                    expected = string.Format("{0}.{1}", Constants.DB_SCHEMA, "HomeOwners");
                    Assert.AreEqual(expected, actual);
                }
                catch { throw; }
                finally { dbContext.Database.Delete(); }
            }
        }

        [TestClass]
        public class DoesIdExistMethod
        {
            [TestMethod]
            public void ShouldEqualTrue()
            {
                // Arrange
                var connectionString = Helpers.GetRandomConnectionString();
                var dbContext = new Context(connectionString);

                try
                {
                    Helpers.SetInitializer(new ContextInitializer(SeedContext));
                    dbContext.Database.Initialize(true); //necessary when not performing query through the EF api

                    // Act
                    var doesIdExist = dbContext.DoesIdExist<Home>("Id", Guid.ParseExact("A7FBD140-1CE1-4D6F-9123-29142FDA9E8B", "D"));
                    var actual = doesIdExist == true;


                    // Assert
                    Assert.IsTrue(actual);
                }
                catch { throw; }
                finally { dbContext.Database.Delete(); }
            }

            [TestMethod]
            public void ShouldEqualFalse()
            {
                // Arrange
                var connectionString = Helpers.GetRandomConnectionString();
                var dbContext = new Context(connectionString);

                try
                {
                    Helpers.SetInitializer(new ContextInitializer(SeedContext));
                    dbContext.Database.Initialize(true); //necessary when not performing query through the EF api

                    // Act
                    var doesIdExist = dbContext.DoesIdExist<Home>("Id", Guid.NewGuid());
                    var actual = doesIdExist == false;


                    // Assert
                    Assert.IsTrue(actual);
                }
                catch { throw; }
                finally { dbContext.Database.Delete(); }
            }
        }
    }
}
