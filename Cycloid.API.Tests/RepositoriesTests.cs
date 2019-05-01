using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cycloid.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Cycloid.API.Tests
{
    [TestClass]
    public class RepositoriesTests
    {
        [TestMethod]
        public void When_GetDevice_by_sessionId_Should_Return_newDevice()
        {
            var repo = new DevicesRepository();

            var device = repo.GetDevice("session-001");

            Assert.IsNotNull(device);
            Assert.IsTrue(device.SessionId=="session-001");
            Assert.IsTrue(device.Id == "device-001");
        }
    }
}
