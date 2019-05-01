using System;
using System.Collections.Generic;
using System.Linq;
using Cycloid.Models;
using RestSharp;

namespace Cycloid.Services
{
    public class ProgramsRestService : IProgramsService
    {
        private IList<Program> _programs;

        public IList<Program> Programs
        {
            get => _programs ?? new List<Program>();
            set => _programs = value;
        }
        
        public IList<Program> GetByChannelId(string channelId)
        {
            if (!Programs.Any())
                Programs = GetPrograms();

            return Programs.Where(c => c.ChannelId.Equals(channelId)).ToList();
        }

        public Program GetById(string id)
        {
            if (!Programs.Any())
                Programs = GetPrograms();

            return Programs.FirstOrDefault(c => c.Id.Equals(id));
        }

        private IList<Program> GetPrograms()
        {
            var client = new RestClient(new Uri("http://tomahawk.cycloid.pt/ott.programs/"));

            var request = new RestRequest("programs", Method.GET);
            var result = client.Execute<List<Program>>(request);

            if (result.ErrorException != null)
                Console.Write($"Erro: {result.ErrorMessage}");

            return result.Data;
        }
    }
}
