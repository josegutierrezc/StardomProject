using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Effort;
using AppsManager.DL;
using System.Data.Entity;
using System.Linq;
using System.Collections.Generic;

namespace Solution.Tests.AppsManager
{
    [TestClass]
    public class UsersManagerTests
    {
        Mock<AppsManagerModel> mockContext;
        IQueryable<User> usersRepo;

        public UsersManagerTests()
        {
            usersRepo = new List<User>() {
                new User() { Id = 1, IsActive = true, Username = "sysadmin", FirstName = "a", LastName = "a" },
                new User() { Id = 2, IsActive = true, Username = "bb", FirstName = "b", LastName = "b" }
            }.AsQueryable();
        }

        [TestMethod]
        public void Add_Returns_False_If_User_With_Same_Username_Exists_Already()
        {
            //Arrange
            User user = new User() {
                Id = 0,
                IsActive = true,
                Username = "sysadmin",
                FirstName = "a",
                LastName = "a"
            };
            var mockSet = new Mock<DbSet<User>>();
            mockSet.As<IQueryable<User>>().Setup(m => m.Provider).Returns(usersRepo.Provider);
            mockSet.As<IQueryable<User>>().Setup(m => m.Expression).Returns(usersRepo.Expression);
            mockSet.As<IQueryable<User>>().Setup(m => m.ElementType).Returns(usersRepo.ElementType);
            mockSet.As<IQueryable<User>>().Setup(m => m.GetEnumerator()).Returns(usersRepo.GetEnumerator());

            var mockContext = new Mock<AppsManagerModel>();
            mockContext.Setup(m => m.Users).Returns(mockSet.Object);

            //Act
            UsersManager userMan = new UsersManager(mockContext.Object);

            //Assert
            Assert.IsFalse(userMan.Add(user));
        }

        [TestMethod]
        public void Add_Returns_True_If_User_Was_Added_Because_No_User_With_Its_Username_Exists()
        {
            //Arrange
            User user = new User()
            {
                Id = 0,
                IsActive = true,
                Username = "c",
                FirstName = "a",
                LastName = "a"
            };
            var mockSet = new Mock<DbSet<User>>();
            mockSet.As<IQueryable<User>>().Setup(m => m.Provider).Returns(usersRepo.Provider);
            mockSet.As<IQueryable<User>>().Setup(m => m.Expression).Returns(usersRepo.Expression);
            mockSet.As<IQueryable<User>>().Setup(m => m.ElementType).Returns(usersRepo.ElementType);
            mockSet.As<IQueryable<User>>().Setup(m => m.GetEnumerator()).Returns(usersRepo.GetEnumerator());

            var mockContext = new Mock<AppsManagerModel>();
            mockContext.Setup(m => m.Users).Returns(mockSet.Object);

            //Act
            UsersManager userMan = new UsersManager(mockContext.Object);

            //Assert
            Assert.IsTrue(userMan.Add(user));
        }

        [TestMethod]
        public void Update_Returns_False_If_User_Was_Not_Found()
        {
            //Arrange
            User user = new User()
            {
                Id = 0,
                IsActive = true,
                Username = "c",
                FirstName = "a",
                LastName = "a"
            };
            var mockSet = new Mock<DbSet<User>>();
            mockSet.As<IQueryable<User>>().Setup(m => m.Provider).Returns(usersRepo.Provider);
            mockSet.As<IQueryable<User>>().Setup(m => m.Expression).Returns(usersRepo.Expression);
            mockSet.As<IQueryable<User>>().Setup(m => m.ElementType).Returns(usersRepo.ElementType);
            mockSet.As<IQueryable<User>>().Setup(m => m.GetEnumerator()).Returns(usersRepo.GetEnumerator());

            var mockContext = new Mock<AppsManagerModel>();
            mockContext.Setup(m => m.Users).Returns(mockSet.Object);

            //Act
            UsersManager userMan = new UsersManager(mockContext.Object);

            //Assert
            Assert.IsFalse(userMan.Update(user));
        }

        [TestMethod]
        public void Update_Returns_False_If_User_FirstName_Is_Empty()
        {
            //Arrange
            User user = new User()
            {
                Id = 0,
                IsActive = true,
                Username = "c",
                FirstName = string.Empty,
                LastName = "a"
            };
            var mockSet = new Mock<DbSet<User>>();
            mockSet.As<IQueryable<User>>().Setup(m => m.Provider).Returns(usersRepo.Provider);
            mockSet.As<IQueryable<User>>().Setup(m => m.Expression).Returns(usersRepo.Expression);
            mockSet.As<IQueryable<User>>().Setup(m => m.ElementType).Returns(usersRepo.ElementType);
            mockSet.As<IQueryable<User>>().Setup(m => m.GetEnumerator()).Returns(usersRepo.GetEnumerator());

            var mockContext = new Mock<AppsManagerModel>();
            mockContext.Setup(m => m.Users).Returns(mockSet.Object);

            //Act
            UsersManager userMan = new UsersManager(mockContext.Object);

            //Assert
            Assert.IsFalse(userMan.Update(user));
        }

        [TestMethod]
        public void Delete_Returns_False_If_Username_Does_Not_Exist()
        {
            //Arrange
            string username = string.Empty;
            var mockSet = new Mock<DbSet<User>>();
            mockSet.As<IQueryable<User>>().Setup(m => m.Provider).Returns(usersRepo.Provider);
            mockSet.As<IQueryable<User>>().Setup(m => m.Expression).Returns(usersRepo.Expression);
            mockSet.As<IQueryable<User>>().Setup(m => m.ElementType).Returns(usersRepo.ElementType);
            mockSet.As<IQueryable<User>>().Setup(m => m.GetEnumerator()).Returns(usersRepo.GetEnumerator());

            var mockContext = new Mock<AppsManagerModel>();
            mockContext.Setup(m => m.Users).Returns(mockSet.Object);

            //Act
            UsersManager userMan = new UsersManager(mockContext.Object);

            //Assert
            Assert.IsFalse(userMan.Delete(username));
        }
    }
}
