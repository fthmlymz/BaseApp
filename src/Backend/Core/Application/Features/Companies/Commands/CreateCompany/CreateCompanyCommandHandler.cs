using Application.Common.Exceptions;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Domain.Entities;
using Mapster;
using MediatR;
using Microsoft.Extensions.Logging;
using Shared;

namespace Application.Features.Companies.Commands.CreateCompany
{
    internal sealed class CreateCompanyCommandHandler : IRequestHandler<CreateCompanyCommand, Result<CreatedCompanyDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEasyCacheService _easyCacheService;
        private readonly ILogger<CreateCompanyCommandHandler> _logger;

        public CreateCompanyCommandHandler(IUnitOfWork unitOfWork, IEasyCacheService easyCacheService, ILogger<CreateCompanyCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _easyCacheService = easyCacheService;
            _logger = logger;
        }

        public async Task<Result<CreatedCompanyDto>> Handle(CreateCompanyCommand request, CancellationToken cancellationToken)
        {
            #region Easy Cache check
            var cacheKey = $"Company_{request.Name}";
            var cachedCompany = await _easyCacheService.GetAsync<Company>(cacheKey);
            if (cachedCompany != null)
            {
                // Found in cache, perform requested operations
                var createdDto = cachedCompany.Adapt<CreatedCompanyDto>();
                return await Result<CreatedCompanyDto>.SuccessAsync(createdDto);
            }
            #endregion

            #region Easycache - Not found in cache, go to database
            var companyExist = await _unitOfWork.Repository<Company>().AnyAsync(x => x.Name == request.Name);
            if (companyExist)
            {
                _logger.LogWarning("Already registered with this name: {RequestName}", request.Name);
                throw new BadRequestExceptionCustom($"{request.Name} isimli şirket daha önce kayıt edilmiş.");
            }
            #endregion


            var company = request.Adapt<Company>();
            await _unitOfWork.Repository<Company>().AddAsync(company);


            #region Easy cache save - Save entire table to cache
            cacheKey = $"Company_{company.Id}"; // Update cache key
            await _easyCacheService.SetAsync(cacheKey, company);
            #endregion

            var createdCompanyDto = company.Adapt<CreatedCompanyDto>();
            return await Result<CreatedCompanyDto>.SuccessAsync(createdCompanyDto);
        }
    }
}
