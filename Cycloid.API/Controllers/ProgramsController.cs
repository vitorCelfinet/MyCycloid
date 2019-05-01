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
    /// The program controller
    /// </summary>
    [LogActionFilter]
    [RoutePrefix("v1/programs")]
    public class ProgramsController : ApiController
    {
        private readonly IProgramsManager _programsManager;

        /// <summary>
        /// The Programs Controller constructor
        /// </summary>
        /// <param name="programsManager"></param>
        public ProgramsController(IProgramsManager programsManager)
        {
            _programsManager = programsManager;
        }

        /// <summary>
        /// Gets the program
        /// </summary>
        /// <param name="id">The program id</param>
        /// <returns>The program</returns>
        [HttpGet]
        [ResponseType(typeof(Program))]
        [Route("{id}")]
        public HttpResponseMessage Get([FromUri]string id)
        {
            if (string.IsNullOrEmpty(id))
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Invalid Input");

            Program program = _programsManager.GetById(id);
            if (program == null)
                return Request.CreateResponse(HttpStatusCode.NotFound);
            return Request.CreateResponse(HttpStatusCode.OK, program);

        }

        /// <summary>
        /// Gets the programs by channel id
        /// </summary>
        /// <param name="channelId">The channel id</param>
        /// <param name="skip">The number of elements to skip</param>
        /// <param name="take">The number of elements to take</param>
        /// <returns>The programs list</returns>
        [HttpGet]
        [ResponseType(typeof(List<Program>))]
        [Route("")]
        public HttpResponseMessage GetByChannel([FromUri]string channelId, [FromUri]int skip = 0, [FromUri]int take = 10)
        {
            if (string.IsNullOrEmpty(channelId) || string.IsNullOrEmpty(channelId))
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Invalid Input");

            IList<Program> programs = _programsManager.GetByChannelId(channelId, skip, take);

            if (!programs.Any())
                return Request.CreateResponse(HttpStatusCode.NotFound);
            return Request.CreateResponse(HttpStatusCode.OK, programs);
        }
    }
}
