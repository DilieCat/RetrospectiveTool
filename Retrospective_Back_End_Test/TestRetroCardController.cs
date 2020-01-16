using Moq;
using Retrospective_Back_End.Controllers;
using Retrospective_Core.Models;
using Retrospective_Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.SignalR;
using Retrospective_Back_End.Realtime;
using Xunit;

namespace Retrospective_Back_End_Test
{
    public class TestRetroCardController
    {
	    readonly Mock<IRetroRespectiveRepository> _mockRetrospectiveRepo;
	    private readonly Mock<IHubContext<NotifyHub, ITypedHubClient>> _hubContext;


        public TestRetroCardController() {
            this._mockRetrospectiveRepo = new Mock<IRetroRespectiveRepository>();
            this._hubContext = new Mock<IHubContext<NotifyHub, ITypedHubClient>>();
        }

        [Fact]
        public void AdditionOfARetroCard()
        {
            //Arrange
            IRetroRespectiveRepository repo = _mockRetrospectiveRepo.Object;
            var controller = new RetroCardsController(repo, _hubContext.Object);

            IList<RetroColumn> retroColumns = new List<RetroColumn>();
            retroColumns.Add(new RetroColumn{Id = 0});

            this._mockRetrospectiveRepo.Setup(r => r.RetroColumns).Returns(retroColumns.AsQueryable);

            IList<RetroCard> retroCards = new List<RetroCard>();

            void Action(RetroCard retroCard)
            {
	            retroCards.Add(retroCard);
            }

            _mockRetrospectiveRepo.Setup(m => m.SaveRetroCard(It.IsAny<RetroCard>())).Callback((Action<RetroCard>) Action);

            //Act
            controller.PostRetroCard(new RetroCard
            {
                Id = 5,
                Content = "RetroCard 1"
            });

            //Assert
            Assert.True(retroCards.Any());
            RetroCard createdRetroCard = retroCards.FirstOrDefault(r => r.Content.Equals("RetroCard 1"));
            Assert.NotNull(createdRetroCard);
        }
    }
}
