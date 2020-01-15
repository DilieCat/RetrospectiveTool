using System;
using System.Collections.Generic;
using Moq;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Retrospective_Back_End.Controllers;
using Xunit;
using Retrospective_Core.Services;
using Retrospective_Core.Models;
using Xunit.Abstractions;
using Assert = Xunit.Assert;
using Retrospective_Back_End.Utils;

namespace Retrospective_Back_End_Test
{
    public class TestRetrospectiveController
    {
        private readonly ITestOutputHelper _testOutputHelper;
        readonly Mock<IRetroRespectiveRepository> _mockRetrospectiveRepo;
        readonly IList<Retrospective> _retrospectives;
        readonly Mock<IDecoder> _decoderMock = new Mock<IDecoder>();

        public TestRetrospectiveController(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
            this._mockRetrospectiveRepo = new Mock<IRetroRespectiveRepository>();
            this._retrospectives = new List<Retrospective>()
            {
                new Retrospective {
                    Title = "Board 1",
                    Description = "Dit is board 1",
                    CreatedDate = DateTime.Now,
                    RetroUserId = 1,
                    RetroColumns = new List<RetroColumn>()
                    {
                        new RetroColumn
                        {
                            Title = "Kolom 1",
                            RetroCards = new List<RetroCard>()
                            {
                                new RetroCard
                                {
                                    Content = "Dit is kaart 1",
                                    Position = 1
                                }
                            }
                        }
                    }

                },
                new Retrospective
                {
                    Title = "Board 2",
                    Description = "Dit is board 2"
                }
            };
        }

        [Fact]
        public async void GetAllRetrospectives()
        {
            //Arrange
            _mockRetrospectiveRepo.Setup(m => m.getAll()).Returns(_retrospectives.AsQueryable());
            var controller = new RetrospectivesController(_mockRetrospectiveRepo.Object, _decoderMock.Object);

            //Act
            var result = await controller.GetRetrospectives();

            //Assert
            var test = result.Value.FirstOrDefault()?.Title;
            Assert.True(test != null && test.Equals("Board 1"));
            Assert.Equal(2, _retrospectives.Count());
        }

        [Fact]
        public void PostRetrospective_ShouldCreateThreeColumns()
        {
            //arrange
            var controller = new RetrospectivesController(_mockRetrospectiveRepo.Object, _decoderMock.Object);

            _decoderMock.Setup(x => x.DecodeToken(It.IsAny<string>())).Returns("1");

            var retrospective = new Retrospective
            {
                Title = "Custom title",
                Description = "Custom description"
            };

            //act
            var response = controller.PostRetrospective(retrospective);

            //assert
            CreatedAtActionResult result = response.Result as CreatedAtActionResult;

            Assert.True(result?.Value is Retrospective);

            if (result?.Value is Retrospective retroResult)
            {
	            Assert.Equal(retrospective.Title, retroResult.Title);
	            Assert.Equal(3, retroResult.RetroColumns.Count);

	            RetroColumn retroColumn = retroResult.RetroColumns.FirstOrDefault(r => r.Title == "To do");
	            Assert.Equal("To do", retroColumn.Title);

	            retroColumn = retroResult.RetroColumns.FirstOrDefault(r => r.Title == "Doing");
	            Assert.Equal("Doing", retroColumn.Title);

                retroColumn = retroResult.RetroColumns.FirstOrDefault(r => r.Title == "Done");
	            Assert.Equal("Done", retroColumn.Title);
            }
        }

        [Fact]
        public void DeleteRetrospective_ShouldCleanBoard()
        {
            //Arrange
            void Action(Retrospective _retrospective)
            {
                foreach (var retroCard in _retrospectives.FirstOrDefault().RetroColumns)
                {
                    retroCard.RetroCards.Clear();
                    retroCard.RetroFamilies.Clear();
                }
            }

            _mockRetrospectiveRepo.Setup(m => m.Retrospectives).Returns(_retrospectives.AsQueryable());
            _mockRetrospectiveRepo.Setup(r => r.CleanRetrospective(It.IsAny<Retrospective>())).Callback((Action<Retrospective>) Action);
            _decoderMock.Setup(x => x.DecodeToken(It.IsAny<string>())).Returns("1");
            
            var controller = new RetrospectivesController(_mockRetrospectiveRepo.Object, _decoderMock.Object);

           


            //Act
            controller.CleanRetrospective(_retrospectives.FirstOrDefault().Id);

            //Assert
            int retroCardsSize = 0;
            int retroFamiliesSize = 0;
            foreach (var retroCard in _retrospectives.FirstOrDefault().RetroColumns)
            {
                retroCardsSize += retroCard.RetroCards.Count();
                retroFamiliesSize += retroCard.RetroFamilies.Count();
            }

            Assert.Equal(0, retroCardsSize);
            Assert.Equal(0, retroFamiliesSize);
        }
    }
}
