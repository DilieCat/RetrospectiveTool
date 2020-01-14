using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Retrospective_Back_End.Realtime;
using Retrospective_Core.Models;
using Retrospective_Core.Services;

namespace Retrospective_Back_End.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RetroFamiliesController : ControllerBase
    {
        private readonly IRetroRespectiveRepository _repo;
        private readonly IHubContext<NotifyHub, ITypedHubClient> _hubContext;

        public RetroFamiliesController(IRetroRespectiveRepository repo, IHubContext<NotifyHub, ITypedHubClient> Context)
        {
            this._repo = repo;
            this._hubContext = Context;
        }

        /// <summary>
        /// Get all RetroFamilies
        /// </summary>
        // GET: api/RetroFamilies
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RetroFamily>>> Get()
        {
            return await Task.FromResult(_repo.RetroFamilies.ToList());
        }

        /// <summary>
        /// Get single RetroFamily by id
        /// </summary>
        // GET: api/RetroFamily/5
        [HttpGet("{id}", Name = "Get")]
        public ActionResult<RetroFamily> Get(int id)
        {
            var family = _repo.RetroFamilies.Include(retroFamily => retroFamily.RetroCards)
                .FirstOrDefault(x => x.Id == id);

            if (family == null)
                return NotFound();

            return family;
        }

        /// <summary>
        /// Create a new RetroFamily
        /// </summary>
        // POST: api/RetroFamily
        [HttpPost]
        public ActionResult<RetroFamily> Post([FromBody] RetroFamily retroFamily)
        {
            _repo.SaveRetroFamily(retroFamily);

            RetroColumn retroColumn = _repo.RetroColumns.FirstOrDefault(x => x.Id == retroFamily.RetroColumnId);

            if (_hubContext.Clients != null)
            {
                try
                {
                    _hubContext.Clients.All.BroadcastMessage(true, retroColumn.RetrospectiveId);
                }
                catch (Exception e)
                {
                    _hubContext.Clients.All.BroadcastMessage(false, retroColumn.RetrospectiveId);
                }
            }

            return retroFamily;
        }

        /// <summary>
        /// Update a RetroFamily
        /// </summary>
        // PUT: api/RetroFamily/5
        [HttpPut]
        public void Put([FromBody] RetroFamily retroFamily)
        {
            _repo.SaveRetroFamily(retroFamily);

            RetroColumn retroColumn = _repo.RetroColumns.FirstOrDefault(x => x.Id == retroFamily.RetroColumnId);

            if (_hubContext.Clients != null)
            {
                try
                {
                    _hubContext.Clients.All.BroadcastMessage(true, retroColumn.RetrospectiveId);
                }
                catch (Exception e)
                {
                    _hubContext.Clients.All.BroadcastMessage(false, retroColumn.RetrospectiveId);
                }
            }
        }

        /// <summary>
        /// Delete a RetroFamily by id
        /// </summary>
        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public ActionResult<RetroFamily> Delete(int id)
        {
            var family = _repo.RetroFamilies.FirstOrDefault(x => x.Id == id);

            if (family == null)
                return NotFound();

            else
            {
                RetroColumn retroColumn = _repo.RetroColumns.FirstOrDefault(x => x.Id == family.RetroColumnId);

                _repo.RemoveRetroFamily(family);

                if (_hubContext.Clients != null)
                {
                    try
                    {
                        _hubContext.Clients.All.BroadcastMessage(true, retroColumn.RetrospectiveId);
                    }
                    catch (Exception e)
                    {
                        _hubContext.Clients.All.BroadcastMessage(false, retroColumn.RetrospectiveId);
                    }
                }

                return family;
            }
        }
    }
}