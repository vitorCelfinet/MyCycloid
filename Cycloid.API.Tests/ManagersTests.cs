using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cycloid.Managers;
using Cycloid.Models;
using Cycloid.Repositories;
using Cycloid.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Cycloid.API.Tests
{
    [TestClass]
    public class ManagersTests
    {
        private Mock<IChannelsService> _channelServiceMock;
        private Mock<IDevicesRepository> _deviceRepositoryMock;

        public ManagersTests()
        {
            _channelServiceMock = new Mock<IChannelsService>();
            _channelServiceMock.Setup(c => c.GetChannels())
                .Returns(new List<Channel> { new Channel() { Id = "1", Name = "channel1" }, new Channel() { Id = "2", Name = "channel2" } });
            _channelServiceMock.Setup(c => c.GetSubscribedChannelsByDeviceId(It.IsAny<string>()))
                .Returns(new[] { "rtp1", "rtp2", "rtp3" });

            _deviceRepositoryMock = new Mock<IDevicesRepository>();
            _deviceRepositoryMock.Setup(c => c.GetDevice(It.IsAny<string>()))
                .Returns(new Device() { Id = "device-001", SessionId = "session-001" });
        }

        #region ChannelsManager Tests
        [TestMethod]
        public void When_GetAllChannels_Should_Return_AllChannelds()
        {
            var manager = new ChannelsManager(_channelServiceMock.Object, _deviceRepositoryMock.Object);

            var result = manager.GetAllChannels();
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Count() == 2);
        }

        [TestMethod]
        public void When_GetSubscribedChannels_Should_Return_AllChanneldsSubscribed()
        {
            var manager = new ChannelsManager(_channelServiceMock.Object, _deviceRepositoryMock.Object);

            var result = manager.GetSubscribedChannelsBySessionId("mysession");
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Count() == 3);
        }

        [TestMethod]
        public void When_GetById_Should_Return_Channel()
        {
            var manager = new ChannelsManager(_channelServiceMock.Object, _deviceRepositoryMock.Object);

            var result = manager.GetById("1");
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Name == "channel1");
        }
        #endregion

        #region DeviceManager Tests
        [TestMethod]
        public void When_GetDeviceId_Should_Return_Id()
        {
            var manager = new DeviceManager(_deviceRepositoryMock.Object);

            var result = manager.GetDeviceId("session1");
            Assert.IsNotNull(result);
            Assert.IsTrue(result == "device-001");
        }
        #endregion

        #region EventsManager Tests
        [TestMethod]
        public void When_GetEvent_from_channel_Should_Return_Events_between_yesterday_and_tomorrow()
        {
            var sessionId = "session-001";
            var channelId = "rtp1";
            var channel = new Channel {Id = channelId, Name = channelId, Position = 1};
            var programManagerMock = new Mock<IProgramsManager>();
            programManagerMock.Setup(c => c.GetByChannelId(channelId))
                .Returns(GetProgramsList(channelId));

            var channelsManagerMock = new Mock<IChannelsManager>();
            channelsManagerMock.Setup(c => c.GetById(channelId))
                .Returns(channel);
            channelsManagerMock.Setup(c => c.GetSubscribedChannelsBySessionId(sessionId))
                .Returns(new List<Channel> {channel});

            var manager = new EventsManager(programManagerMock.Object, channelsManagerMock.Object);
            
            var resultTask = manager.GetEventsAsync(sessionId, channelId);
            resultTask.Wait();

            var result = resultTask.Result;

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Count == 2);
            Assert.IsTrue(result[0].IsSubscribed);
            Assert.IsTrue(result[1].IsSubscribed);
        }

        private IList<Program> GetProgramsList(string channelId)
        {
            return new List<Program>
            {
                new Program()
                {
                    Id = "program1",
                    ChannelId = channelId,
                    StartTime = DateTime.UtcNow.AddDays(-3),
                    EndTime = DateTime.UtcNow.AddDays(-2)
                },
                new Program()
                {
                    Id = "program2",
                    ChannelId = channelId,
                    StartTime = DateTime.UtcNow.AddDays(-2),
                    EndTime = DateTime.UtcNow.AddDays(-1)
                },
                new Program()
                {
                    Id = "program3",
                    ChannelId = "otherChannel",
                    StartTime = DateTime.UtcNow,
                    EndTime = DateTime.UtcNow.AddDays(1)
                },
                new Program()
                {
                    Id = "program4",
                    ChannelId = channelId,
                    StartTime = DateTime.UtcNow.AddDays(2.1),
                    EndTime = DateTime.UtcNow.AddDays(3)
                }
            };
        }



        [TestMethod]
        public void When_GetPlayingEvents_Should_Return_Previous_Current_Next_Events_from_each_channel()
        {
            var sessionId = "session-001";
            var channelId = "rtp1";

            var channel = new Channel { Id = channelId, Name = channelId, Position = 1 };
            var programManagerMock = new Mock<IProgramsManager>();
            programManagerMock.Setup(c=> c.GetAllGroupByChannel())
                .Returns(GetProgramsGrouped());

            programManagerMock.Setup(c => c.GetByChannelId(channelId))
                .Returns(GetProgramsList(channelId));

            var channelsManagerMock = new Mock<IChannelsManager>();
            channelsManagerMock.Setup(c => c.GetSubscribedChannelsBySessionId(sessionId))
                .Returns(new List<Channel> { channel });
            channelsManagerMock.Setup(c => c.GetById(channelId))
                .Returns(channel);
            channelsManagerMock.Setup(c => c.GetById("rtp2"))
                .Returns(new Channel() { Id = "rtp2", Name = "Rtp2" });
            channelsManagerMock.Setup(c => c.GetById(It.IsNotIn(new []{channelId, "rtp2"})))
                .Returns(new Channel(){Id = "idTest", Name = "channelTest"});

            var manager = new EventsManager(programManagerMock.Object, channelsManagerMock.Object);

            var resultTask = manager.GetPlayingEventsAsync(sessionId);
            resultTask.Wait();

            var result = resultTask.Result.OrderBy(c=> c.ChannelName).ThenBy(c => c.ProgramStartTime).ToList();

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Count == 7);
            Assert.IsTrue(result.Count(c=> c.ChannelName.Equals(channel.Name)) == 3);
            Assert.IsTrue(result.First(c=> c.ChannelName.Equals(channel.Name)).IsSubscribed);
            Assert.IsTrue(result.Count(c => c.ChannelName.Equals("channelTest")) == 1);
            Assert.IsFalse(result.First(c => c.ChannelName.Equals("channelTest")).IsSubscribed);
            Assert.IsTrue(result.Count(c => c.ChannelName.Equals("Rtp2")) == 3);
            Assert.IsFalse(result.First(c => c.ChannelName.Equals("Rtp2")).IsSubscribed);
        }

        private IEnumerable<IGrouping<string, Program>> GetProgramsGrouped()
        {
            return new List<Program>
            {
                new Program(){ChannelId = "rtp1", Description = "descr0", Title = "program0", StartTime = DateTime.UtcNow.AddHours(-2), EndTime = DateTime.UtcNow.AddHours(-1)},
                new Program(){ChannelId = "rtp1", Description = "descr1", Title = "program1", StartTime = DateTime.UtcNow.AddHours(-1), EndTime = DateTime.UtcNow.AddHours(-0.5)},
                new Program(){ChannelId = "rtp1", Description = "descr2", Title = "program2", StartTime = DateTime.UtcNow.AddHours(-0.5), EndTime = DateTime.UtcNow.AddHours(1)},
                new Program(){ChannelId = "rtp1", Description = "descr3", Title = "program3", StartTime = DateTime.UtcNow.AddHours(1), EndTime = DateTime.UtcNow.AddHours(2)},

                new Program(){ChannelId = "rtp2", Description = "descr4", Title = "program4", StartTime = DateTime.UtcNow.AddHours(-1), EndTime = DateTime.UtcNow},
                new Program(){ChannelId = "rtp2", Description = "descr5", Title = "program5", StartTime = DateTime.UtcNow, EndTime = DateTime.UtcNow.AddHours(1)},
                new Program(){ChannelId = "rtp2", Description = "descr6", Title = "program6", StartTime = DateTime.UtcNow.AddHours(1), EndTime = DateTime.UtcNow.AddHours(2)},
                
                new Program(){ChannelId = "Sic", Description = "descr7", Title = "program7", StartTime = DateTime.UtcNow.AddHours(-1), EndTime = DateTime.UtcNow.AddHours(1)},
                new Program(){ChannelId = "tvi", Description = "descr8", Title = "program8", StartTime = DateTime.UtcNow.AddHours(2), EndTime = DateTime.UtcNow.AddHours(3)}

            }.GroupBy(c => c.ChannelId);
        }

        #endregion

        [TestMethod]
        public void When_GetByChannel_Should_return_all_programs_from_Channel()
        {
            var channelId = "rtp1";
            var programServiceMock = new Mock<IProgramsService>();
            programServiceMock.Setup(c => c.GetAll()).Returns(GetProgramsList(channelId));

            var manager = new ProgramsManager(programServiceMock.Object);

            var result = manager.GetByChannelId(channelId);

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Count == 3);
            Assert.IsTrue(result.First().ChannelId.Equals(channelId));
        }

        [TestMethod]
        public void When_GetByChannel_with_SkipTake_Should_return_all_programs_from_Channel_filtered()
        {
            var channelId = "rtp1";
            var programServiceMock = new Mock<IProgramsService>();
            programServiceMock.Setup(c => c.GetAll()).Returns(GetProgramsList(channelId));

            var manager = new ProgramsManager(programServiceMock.Object);

            var result = manager.GetByChannelId(channelId, 0,1);

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Count == 1);
            Assert.IsTrue(result.First().ChannelId.Equals(channelId));
        }

        [TestMethod]
        public void When_GetById_Should_return_Program()
        {
            var channelId = "rtp1";
            var programServiceMock = new Mock<IProgramsService>();
            programServiceMock.Setup(c => c.GetAll()).Returns(GetProgramsList(channelId));

            var manager = new ProgramsManager(programServiceMock.Object);

            var result = manager.GetById("program3");

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Id.Equals("program3"));
        }

        [TestMethod]
        public void When_GetById_not_existing_Should_return_Null()
        {
            var channelId = "rtp1";
            var programServiceMock = new Mock<IProgramsService>();
            programServiceMock.Setup(c => c.GetAll()).Returns(GetProgramsList(channelId));

            var manager = new ProgramsManager(programServiceMock.Object);

            var result = manager.GetById("program10");

            Assert.IsNull(result);
        }

        [TestMethod]
        public void When_GetProgramsGrouped_Should_return_GroupPrograms()
        {
            var channelId = "rtp1";
            var programServiceMock = new Mock<IProgramsService>();
            programServiceMock.Setup(c => c.GetAll()).Returns(GetProgramsList(channelId));

            var manager = new ProgramsManager(programServiceMock.Object);

            var result = manager.GetAllGroupByChannel();

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Count()==2);
        }
    }
}
