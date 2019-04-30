using Cycloid.Common.ParameterBinding;
using Cycloid.Managers;
using Cycloid.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;

namespace Cycloid.API.Controllers
{
    /// <summary>
    /// The channels controller
    /// </summary>
    [RoutePrefix("v1/channels")]
    public class ChannelsController : ApiController 
    {
        private readonly IChannelsManager _channelsManager;

        /// <summary>
        /// The channels controller constructor
        /// </summary>
        /// <param name="channelsManager">The channels manager</param>
        public ChannelsController(IChannelsManager channelsManager)
        {
            _channelsManager = channelsManager;
        }

        /// <summary>
        /// Gets all channels
        /// </summary>
        /// <returns>The channels</returns>
        [HttpGet]
        [ResponseType(typeof(List<Channel>))]
        [Route("")]
        public HttpResponseMessage Get()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the subscribed channels
        /// </summary>
        /// <param name="sessionId">The session id</param>
        /// <returns>The subscribed channel ids</returns>
        [HttpGet]
        [ResponseType(typeof(List<Channel>))]
        [Route("subscribed")]
        public HttpResponseMessage GetSubscribedChannels([FromHeader("session-id")]string sessionId)
        {
            throw new NotImplementedException();
        }
    }
}
