using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cycloid.Repositories;

namespace Cycloid.Managers
{
    public class DeviceManager : IDeviceManager
    {
        private readonly IDevicesRepository _devicesRepository;

        public DeviceManager(IDevicesRepository devicesRepository)
        {
            _devicesRepository = devicesRepository;
        }
        public string GetDeviceId(string sessionId)
        {
            return _devicesRepository.GetDevice(sessionId)?.Id;
        }
    }
}
