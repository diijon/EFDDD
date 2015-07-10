using System;
using System.Collections.Generic;
using System.Linq;
using EFDDD;
using EFDDD.DomainModel;
using DataModel = EFDDD.DataModel;
using EFDDD.DataModel.EF;
using EFDDD.DomainDataMapper;
using EFDDD.DomainDataRepository;
using EFDDD.DomainDataRepository.Contracts;
using EFDDD.DomainDataRepository.Filters;
using EFDDD.DomainDataRepository.Sorters;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DataModelHelpers = EFDDD.DataModel.EF.Helpers;

namespace DomainDataRepository.Test
{
    [TestClass]
    public class GenericDbRepositoryTest
    {
        private static MappingWorker _mapper;

        [TestInitialize]
        public void Initialize()
        {
            MappingConfiguration.Configure();
            _mapper = new MappingWorker(AutoMapper.Mapper.Engine);
        }

        [TestClass]
        public class ConstructorMethod : GenericDbRepositoryTest
        {
            [TestMethod]
            public void ShouldConstruct()
            {
                var connectionString = DataModelHelpers.GetRandomConnectionString();
                var dbContext = new Context(connectionString);

                try
                {
                    Helpers.SetInitializer(new ContextInitializer(context =>
                    {
                        using (var repository = new GenericDbRepository<Home, DataModel.Home>(_mapper, dbContext))
                        {
                            
                        }
                    }));
                    dbContext.Database.Initialize(true); //necessary when not performing query through the EF api
                }
                catch { throw; }
                finally { dbContext.Database.Delete(); }
            }
        }

        [TestClass]
        public class CountMethod : GenericDbRepositoryTest
        {
            [TestMethod]
            public void ShouldEqualInsertedCount()
            {
                var connectionString = DataModelHelpers.GetRandomConnectionString();
                var dbContext = new Context(connectionString);

                try
                {
                    Helpers.SetInitializer(new ContextInitializer(context =>
                    {
                        using (var repository = new GenericDbRepository<Home, DataModel.Home>(_mapper, dbContext))
                        {
                            repository.Merge(new Home
                            {
                                Id = Guid.NewGuid(),
                                Address = ""
                            });
                            repository.Merge(new Home
                            {
                                Id = Guid.NewGuid(),
                                Address = ""
                            });
                        }
                    }));

                    using (var repository = new GenericDbRepository<Home, DataModel.Home>(_mapper, dbContext))
                    {
                        var expected = 2;

                        int? actual = 0;
                        repository.All(ref actual, pageSize: 1, pageIndex: 0);
                        Assert.AreEqual(expected, actual);

                        actual = repository.Count();
                        Assert.AreEqual(expected, actual);
                    }
                }
                catch { throw; }
                finally { dbContext.Database.Delete(); }
            }
        }

        [TestClass]
        public class AllMethod : GenericDbRepositoryTest
        {
            public static void SeedContext(IContext context)
            {
                using (var repository = new GenericDbRepository<Home, DataModel.Home>(_mapper, context))
                {
                    repository.Merge(new Home
                    {
                        Id = new Guid("A7FBD140-1CE1-4D6F-9123-29142FDA9E8B"),
                        Address = "901 Willow Street, Klagetoh, Kentucky, 4422",
                        DateBuilt = new DateTime(2000, 1, 1),
                        Neighborhood = new Neighborhood("Central Estates", true),
                        HomeOwnerId = new Guid("9E35B69F-DE06-49C7-A797-1059BBB55D0A"),
                        Rooms = new List<Room>
                        {
                            new Room("Dining Room", 20.2f),
                            new Room("Living Room", 188f)
                        }
                    });

                    repository.Merge(new Home
                    {
                        Id = new Guid("5DA841F7-1111-4F71-8004-CCCFDDB1BE84"),
                        Address = "285 Elliott Place, Bagtown, California, 7415",
                        DateBuilt = new DateTime(2000, 12, 15),
                        Neighborhood = new Neighborhood("Westwood", false),
                        HomeOwnerId = new Guid("D596B755-133E-47DD-BD3D-E2EC54C6507F"),
                        Rooms = new List<Room>
                        {
                            new Room("Dining Room", 172.4f),
                            new Room("Living Room", 216.2f)
                        }
                    });

                    repository.Merge(new Home
                    {
                        Id = new Guid("29D25509-78B6-423F-9B00-A6831341A278"),
                        Address = "754 Butler Street, Seymour, Wyoming, 5762",
                        DateBuilt = new DateTime(1982, 5, 1),
                        Neighborhood = new Neighborhood("754 Butler Street, Seymour, Wyoming, 5762", true),
                        HomeOwnerId = new Guid("9E35B69F-DE06-49C7-A797-1059BBB55D0A"),
                        Rooms = new List<Room>
                        {
                            new Room("Living Room", 350.6f)
                        }
                    });
                }
            }

            [TestMethod]
            public void ShouldEqualInsertedCount()
            {
                //Arrange
                var connectionString = DataModelHelpers.GetRandomConnectionString();
                var dbContext = new Context(connectionString);

                try
                {
                    Helpers.SetInitializer(new ContextInitializer(SeedContext));
                    using (var repository = new GenericDbRepository<Home, DataModel.Home>(_mapper, dbContext))
                    {
                        // Act
                        var data = repository.All();

                        // Assert
                        Assert.IsTrue(data.Any());
                    }
                }
                catch { throw; }
                finally { dbContext.Database.Delete(); }
            }

            [TestMethod]
            public void ShouldRespectFilter()
            {
                //Arrange
                var connectionString = DataModelHelpers.GetRandomConnectionString();
                var dbContext = new Context(connectionString);

                try
                {
                    Helpers.SetInitializer(new ContextInitializer(SeedContext));
                    using (var repository = new GenericDbRepository<Home, DataModel.Home>(_mapper, dbContext))
                    {
                        // Act
                        var data = repository.All(new FilterHomes(roomNames: new[]{"Dining Room"})).ToList();

                        // Assert
                        Assert.IsTrue(data.Any());
                        Assert.AreEqual(2, data.Count());
                    }
                }
                catch { throw; }
                finally { dbContext.Database.Delete(); }
            }

            [TestMethod]
            public void ShouldRespectSorter()
            {
                //Arrange
                var connectionString = DataModelHelpers.GetRandomConnectionString();
                var dbContext = new Context(connectionString);

                try
                {
                    Helpers.SetInitializer(new ContextInitializer(SeedContext));
                    using (var repository = new GenericDbRepository<Home, DataModel.Home>(_mapper, dbContext))
                    {
                        // Act
                        var data = repository.All(sorters: new SortHomes(SortOrder.ASC, dateBuilt: true)).ToList();

                        // Assert
                        Assert.IsTrue(data.Any());
                        Assert.AreEqual(new DateTime(1982, 5, 1), data.First().DateBuilt);
                    }
                }
                catch { throw; }
                finally { dbContext.Database.Delete(); }
            }

            [TestMethod]
            public void ShouldRespectPaging()
            {
                //Arrange
                var connectionString = DataModelHelpers.GetRandomConnectionString();
                var dbContext = new Context(connectionString);

                try
                {
                    Helpers.SetInitializer(new ContextInitializer(SeedContext));
                    using (var repository = new GenericDbRepository<Home, DataModel.Home>(_mapper, dbContext))
                    {
                        // Act
                        var data = repository.All(pageIndex: 2, pageSize: 1, sorters: new SortHomes(SortOrder.ASC, dateBuilt: true)).ToList();

                        // Assert
                        Assert.IsTrue(data.Any());
                        Assert.AreEqual(new DateTime(2000, 12, 15), data.First().DateBuilt);
                    }
                }
                catch { throw; }
                finally { dbContext.Database.Delete(); }
            }
        }
    }
}
