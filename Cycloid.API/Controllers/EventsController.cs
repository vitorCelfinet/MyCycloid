﻿using Cycloid.Common.ParameterBinding;
using Cycloid.Managers;
using Cycloid.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using Cycloid.Handlers;

namespace Cycloid.API.Controllers
{
    /// <summary>
    /// The events controller
    /// </summary>
    [LogActionFilter]
    [RoutePrefix("v1/events")]
    public class EventsController : ApiController
    {
        private readonly IEventsManager _eventsManager;

        /// <summary>
        /// The events controller constructor
        /// </summary>
        /// <param name="eventsManager">The events manager</param>
        /// <param name="deviceManager"></param>
        public EventsController(IEventsManager eventsManager)
        {
            _eventsManager = eventsManager;
        }

        /// <summary>
        /// Gets the events by channel id
        /// </summary>
        /// <param name="sessionId">The session id</param>
        /// <param name="channelId">The channel id</param>
        /// <returns>The events</returns>
        [HttpGet]
        [ResponseType(typeof(List<Event>))]
        [Route("{channelId}")]
        public HttpResponseMessage GetByChannel([FromHeader("session-id")]string sessionId, [FromUri]string channelId)
        {
            if (string.IsNullOrEmpty(sessionId) || string.IsNullOrEmpty(channelId))
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest,"Invalid Input");

            var events = _eventsManager.GetEventsAsync(sessionId, channelId);
            
            if (!events.Result.Any())
                return Request.CreateResponse(HttpStatusCode.NotFound);
            return Request.CreateResponse(HttpStatusCode.OK, events.Result);
        }

        /// <summary>
        /// Gets the events playing at the moment
        /// </summary>
        /// <param name="sessionId">The session id</param>
        /// <returns>The events</returns>
        [HttpGet]
        [ResponseType(typeof(List<Event>))]
        [Route("now")]
        public HttpResponseMessage GetPlaying([FromHeader("session-id")]string sessionId)
        {
            if (string.IsNullOrEmpty(sessionId))
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Invalid Input");

            var events = _eventsManager.GetPlayingEventsAsync(sessionId);
            if (!events.Result.Any())
                return Request.CreateResponse(HttpStatusCode.NotFound);
            return Request.CreateResponse(HttpStatusCode.OK, events.Result);
        }
    }
}
