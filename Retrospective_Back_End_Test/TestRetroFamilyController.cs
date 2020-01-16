using System;
using System.Collections.Generic;
using System.Linq;
using Castle.Core.Internal;
using Microsoft.AspNetCore.SignalR;
using Moq;
using Retrospective_Back_End.Controllers;
using Retrospective_Back_End.Realtime;
using Retrospective_Core.Models;
using Retrospective_Core.Services;
using Xunit;

namespace Retrospective_Back_End_Test
{
    public class TestRetroFamilyController
    {
        readonly Mock<IRetroRespectiveRepository> _mockRetrospectiveRepo;
        private readonly Mock<IHubContext<NotifyHub, ITypedHubClient>> _hubContext;


        public TestRetroFamilyController()
        {
            this._mockRetrospectiveRepo = new Mock<IRetroRespectiveRepository>();
            this._hubContext = new Mock<IHubContext<NotifyHub, ITypedHubClient>>();
        }

        [Fact]
        public void CanAddPostRetroFamily()
        {
            //Arrange
            IRetroRespectiveRepository repo = _mockRetrospectiveRepo.Object;
            var controller = new RetroFamiliesController(repo, _hubContext.Object);

            IList<RetroFamily> retroFamilies = new List<RetroFamily>();

            void Action(RetroFamily retroFamily)
            {
                retroFamilies.Add(retroFamily);
            }

            _mockRetrospectiveRepo.Setup(m => m.SaveRetroFamily(It.IsAny<RetroFamily>()))
                .Callback((Action<RetroFamily>) Action);

            //Act
            controller.Post(new RetroFamily()
            {
                Id = 5,
                Content = "RetroFamily 1"
            });

            //Assert
            Assert.True(retroFamilies.Any());
            RetroFamily createdRetroCard =
                retroFamilies.FirstOrDefault(r => r.Content.Equals("RetroFamily 1") && r.Id == 5);
            Assert.NotNull(createdRetroCard);
        }

        [Fact]
        public void CanUpdateRetroFamily()
        {
            //Arrange
            IRetroRespectiveRepository repo = _mockRetrospectiveRepo.Object;
            var controller = new RetroFamiliesController(repo, _hubContext.Object);

            IList<RetroFamily> retroFamilies = new List<RetroFamily>();

            RetroFamily f = new RetroFamily
            {
                Content = "Okay",
                Id = 0,
                Position = 0,
            };

            retroFamilies.Add(f);

            void Action(RetroFamily retroFamily)
            {
                RetroFamily entry = retroFamilies.FirstOrDefault(x => x.Id == retroFamily.Id);

                entry.Content = retroFamily.Content;
            }

            _mockRetrospectiveRepo.Setup(m => m.SaveRetroFamily(It.IsAny<RetroFamily>()))
                .Callback((Action<RetroFamily>) Action);

            //Act
            f.Content = "Wow!";

            controller.Put(f);

            //Assert
            Assert.True(retroFamilies.Any());
            Assert.True(retroFamilies.Count == 1);
            Assert.True(retroFamilies.ElementAt(0).Content.Equals("Wow!"));
        }


        [Fact]
        public void CanDeleteRetroFamily()
        {
            //Arrange
            IRetroRespectiveRepository repo = _mockRetrospectiveRepo.Object;
            var controller = new RetroFamiliesController(repo, _hubContext.Object);

            IList<RetroFamily> retroFamilies = new List<RetroFamily>();

            RetroFamily f = new RetroFamily {
                Content = "Okay",
                Id = 0,
                Position = 0,
            };

            retroFamilies.Add(f);

            void Action(RetroFamily retroFamily)
            {
                RetroFamily y = retroFamilies.FirstOrDefault(x => x.Id == retroFamily.Id);

                retroFamilies.Remove(y);
            }

            _mockRetrospectiveRepo.Setup(m => m.RetroFamilies).Returns(retroFamilies.AsQueryable());

            _mockRetrospectiveRepo.Setup(m => m.RemoveRetroFamily(It.IsAny<RetroFamily>()))
                .Callback((Action<RetroFamily>)Action);

            //Act
            controller.Delete(0);

            //Assert
            Assert.False(retroFamilies.Any());
            Assert.True(retroFamilies.IsNullOrEmpty());
        }

        [Fact]
        public void CanGetRetroFamily()
        {
            //Arrange
            IRetroRespectiveRepository repo = _mockRetrospectiveRepo.Object;
            var controller = new RetroFamiliesController(repo, _hubContext.Object);

            IList<RetroFamily> retroFamilies = new List<RetroFamily>();

            RetroFamily f = new RetroFamily {
                Content = "Okay",
                Id = 0,
                Position = 0,
            };

            retroFamilies.Add(f);

            _mockRetrospectiveRepo.Setup(r => r.RetroFamilies).Returns(retroFamilies.AsQueryable());

            //Act
            RetroFamily family = controller.Get(0).Value;

            //Assert
            Assert.NotNull(family);
            Assert.Equal("Okay", family.Content);
            Assert.Equal(0, family.Position);
        }

        [Fact]
        public async void CanGetAllRetroFamilies()
        {
            //Arrange
            IRetroRespectiveRepository repo = _mockRetrospectiveRepo.Object;
            var controller = new RetroFamiliesController(repo, _hubContext.Object);

            IList<RetroFamily> retroFamilies = new List<RetroFamily>();

            {
                RetroFamily f = new RetroFamily
                {
                    Content = "Okay",
                    Id = 0,
                    Position = 0,
                };

                retroFamilies.Add(f);
            }
            {
                RetroFamily f = new RetroFamily {
                    Content = "Okay2",
                    Id = 1,
                    Position = 1,
                };

                retroFamilies.Add(f);
            }

            _mockRetrospectiveRepo.Setup(r => r.RetroFamilies).Returns(retroFamilies.AsQueryable());
            
            //Act
            var entries = await controller.Get();

            //Arrange
            var test = entries.Value;

            Assert.Equal(2, test.Count());
        }
    }
}