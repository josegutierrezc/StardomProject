using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Effort;
using AppsManager.DL;
using System.Data.Entity;
using System.Linq;
using System.Collections.Generic;

namespace AppsManager.DL
{
    [TestClass]
    public class AgenciesManagerTests
    {
        Mock<AppsManagerModel> mockContext;
        IQueryable<Agency> agenciesRepo;

        public AgenciesManagerTests() {
            agenciesRepo = new List<Agency>() {
                new Agency() { Id = 1, Number = "0000" },
                new Agency() { Id = 1, Number = "1111" },
            }.AsQueryable();
        }

        [TestMethod]
        public void GetByNumber_Returns_Null_If_Agency_Does_Not_Exist() {
            //Arrange
            string agencyNumber = string.Empty;
            var mockSet = new Mock<DbSet<Agency>>();
            mockSet.As<IQueryable<Agency>>().Setup(m => m.Provider).Returns(agenciesRepo.Provider);
            mockSet.As<IQueryable<Agency>>().Setup(m => m.Expression).Returns(agenciesRepo.Expression);
            mockSet.As<IQueryable<Agency>>().Setup(m => m.ElementType).Returns(agenciesRepo.ElementType);
            mockSet.As<IQueryable<Agency>>().Setup(m => m.GetEnumerator()).Returns(agenciesRepo.GetEnumerator());

            var mockContext = new Mock<AppsManagerModel>();
            mockContext.Setup(m => m.Agencies).Returns(mockSet.Object);

            //Act
            AgenciesManager agenMan = new AgenciesManager(mockContext.Object);

            //Assert
            Assert.IsNull(agenMan.GetByNumber(agencyNumber));
        }

        [TestMethod]
        public void Add_Returns_False_If_An_Agency_With_Same_Number_Exists_Already() {
            //Arrange
            Agency newAgency = new Agency() { Id = 3, Number = "0000" };
            var mockSet = new Mock<DbSet<Agency>>();
            mockSet.As<IQueryable<Agency>>().Setup(m => m.Provider).Returns(agenciesRepo.Provider);
            mockSet.As<IQueryable<Agency>>().Setup(m => m.Expression).Returns(agenciesRepo.Expression);
            mockSet.As<IQueryable<Agency>>().Setup(m => m.ElementType).Returns(agenciesRepo.ElementType);
            mockSet.As<IQueryable<Agency>>().Setup(m => m.GetEnumerator()).Returns(agenciesRepo.GetEnumerator());

            var mockContext = new Mock<AppsManagerModel>();
            mockContext.Setup(m => m.Agencies).Returns(mockSet.Object);

            //Act
            AgenciesManager agenMan = new AgenciesManager(mockContext.Object);

            //Assert
            Assert.IsFalse(agenMan.Add(newAgency));
        }

        [TestMethod]
        public void Delete_Returns_False_If_User_Wants_To_Remove_Agency_With_Number_0000()
        {
            //Arrange
            string agencyNumber = "0000";
            var mockSet = new Mock<DbSet<Agency>>();
            mockSet.As<IQueryable<Agency>>().Setup(m => m.Provider).Returns(agenciesRepo.Provider);
            mockSet.As<IQueryable<Agency>>().Setup(m => m.Expression).Returns(agenciesRepo.Expression);
            mockSet.As<IQueryable<Agency>>().Setup(m => m.ElementType).Returns(agenciesRepo.ElementType);
            mockSet.As<IQueryable<Agency>>().Setup(m => m.GetEnumerator()).Returns(agenciesRepo.GetEnumerator());

            var mockContext = new Mock<AppsManagerModel>();
            mockContext.Setup(m => m.Agencies).Returns(mockSet.Object);

            //Act
            AgenciesManager agenMan = new AgenciesManager(mockContext.Object);

            //Assert
            Assert.IsFalse(agenMan.Delete(agencyNumber));
        }

        [TestMethod]
        public void Delete_Returns_False_If_User_Wants_To_Remove_Agency_That_Does_Not_Exists()
        {
            //Arrange
            string agencyNumber = "XXXX";
            var mockSet = new Mock<DbSet<Agency>>();
            mockSet.As<IQueryable<Agency>>().Setup(m => m.Provider).Returns(agenciesRepo.Provider);
            mockSet.As<IQueryable<Agency>>().Setup(m => m.Expression).Returns(agenciesRepo.Expression);
            mockSet.As<IQueryable<Agency>>().Setup(m => m.ElementType).Returns(agenciesRepo.ElementType);
            mockSet.As<IQueryable<Agency>>().Setup(m => m.GetEnumerator()).Returns(agenciesRepo.GetEnumerator());

            var mockContext = new Mock<AppsManagerModel>();
            mockContext.Setup(m => m.Agencies).Returns(mockSet.Object);

            //Act
            AgenciesManager agenMan = new AgenciesManager(mockContext.Object);

            //Assert
            Assert.IsFalse(agenMan.Delete(agencyNumber));
        }

        [TestMethod]
        public void Update_Returns_False_If_User_Wants_To_Modify_Agency_With_Number_0000()
        {
            //Arrange
            string agencyNumber = "0000";
            var mockSet = new Mock<DbSet<Agency>>();
            mockSet.As<IQueryable<Agency>>().Setup(m => m.Provider).Returns(agenciesRepo.Provider);
            mockSet.As<IQueryable<Agency>>().Setup(m => m.Expression).Returns(agenciesRepo.Expression);
            mockSet.As<IQueryable<Agency>>().Setup(m => m.ElementType).Returns(agenciesRepo.ElementType);
            mockSet.As<IQueryable<Agency>>().Setup(m => m.GetEnumerator()).Returns(agenciesRepo.GetEnumerator());

            var mockContext = new Mock<AppsManagerModel>();
            mockContext.Setup(m => m.Agencies).Returns(mockSet.Object);

            //Act
            AgenciesManager agenMan = new AgenciesManager(mockContext.Object);

            //Assert
            Assert.IsFalse(agenMan.Delete(agencyNumber));
        }
    }
    
}
