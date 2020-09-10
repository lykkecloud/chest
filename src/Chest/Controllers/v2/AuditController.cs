using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Chest.Client.Api;
using Chest.Client.Models;
using Chest.Client.Models.Requests;
using Chest.Client.Models.Responses.Chest.Client.Models.Responses;
using Chest.Models.v2;
using Chest.Models.v2.Audit;
using Chest.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Chest.Controllers.v2
{
    [Route("api/v2/audit")]
    [Authorize]
    [ApiController]
    public class AuditController : ControllerBase, IAuditApi
    {
        private readonly IAuditService _auditService;
        private readonly IMapper _mapper;

        public AuditController(IAuditService auditService, IMapper mapper)
        {
            _auditService = auditService;
            _mapper = mapper;
        }

        /// <summary>
        /// Get audit logs
        /// </summary>
        /// <param name="request"></param>
        /// <param name="skip"></param>
        /// <param name="take"></param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(PaginatedResponseContract<AuditContract>), (int)HttpStatusCode.OK)]
        public async Task<PaginatedResponseContract<AuditContract>> GetAuditTrailAsync([FromQuery] GetAuditLogsRequest request, int? skip = null, int? take = null)
        {
            var filter = _mapper.Map<GetAuditLogsRequest, AuditLogsFilterDto>(request);
            var result = await _auditService.GetAll(filter, skip, take);

            return new PaginatedResponseContract<AuditContract>(
                contents: result.Contents.Select(i => _mapper.Map<IAuditModel, AuditContract>(i)).ToList(),
                start: result.Start,
                size: result.Size,
                totalSize: result.TotalSize
            );
        }
    }
}