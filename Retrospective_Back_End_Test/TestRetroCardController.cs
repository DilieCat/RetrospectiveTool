﻿using Moq;
using Retrospective_Back_End.Controllers;
using Retrospective_Core.Models;
using Retrospective_Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace Retrospective_Back_End_Test
{
    public class TestRetroCardController
    {
        Mock<IRetroRespectiveRepository> mockRetrospectiveRepo;
        IList<RetroCard> retrocards;
        public TestRetroCardController() {
            this.mockRetrospectiveRepo = new Mock<IRetroRespectiveRepository>();
            this.retrocards = new List<RetroCard>() {
                 new RetroCard 
                 {
                     Content = "Test RetroCard",
                     Position = 2
                 },
                 new RetroCard
                 {
                     Content = "Test RetroCard 2",
                     Position = 5
                 },
                 new RetroCard
                 {
                     Content = "Last test RetroCard",
                     Position = 6
                 }

            };
        }

        [Fact]
        public void AdditionOfARetroCard()
        {
            //Arrange
            IRetroRespectiveRepository repo = mockRetrospectiveRepo.Object;
            var controller = new RetroCardsController(repo);

            IList<RetroCard> retroCards = new List<RetroCard>();

            Action<RetroCard> action = (RetroCard) =>
            {
                retroCards.Add(RetroCard);
            };

            mockRetrospectiveRepo.Setup(m => m.SaveRetroCard(It.IsAny<RetroCard>())).Callback(action);

            //Act
            controller.PostRetroCard(new RetroCard
            {
                Id = 5,
                Content = "RetroCard 1"
            });

            //Assert
            Assert.True(retroCards.Count() > 0);
            RetroCard createdRetroCard = retroCards.FirstOrDefault(r => r.Content.Equals("RetroCard 1"));
            Assert.NotNull(createdRetroCard);
        }
    }
}