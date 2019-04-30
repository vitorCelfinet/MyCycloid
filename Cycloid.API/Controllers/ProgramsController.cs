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
    /// The program controller
    /// </summary>
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
            throw new NotImplementedException();
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
        [Route("{channelId}")]
        public HttpResponseMessage GetByChannel([FromUri]string channelId, [FromUri]int skip = 0, [FromUri]int take = 10)
        {
            throw new NotImplementedException();
        }
    }
}
