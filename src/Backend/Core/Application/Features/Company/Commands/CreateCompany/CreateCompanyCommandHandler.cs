using Application.Interfaces.Repositories;
using Mapster;
using MediatR;
using Shared;

namespace Application.Features.Company.Commands.CreateCompany
{
    internal sealed class CreateCompanyCommandHandler : IRequestHandler<CreateCompanyCommand, Result<CreatedCompanyDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CreateCompanyCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<CreatedCompanyDto>> Handle(CreateCompanyCommand request, CancellationToken cancellationToken)
        {
            var company = request.Adapt<Domain.Entities.Company>();
            await _unitOfWork.Repository<Domain.Entities.Company>().AddAsync(company);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var createdCompanyDto = company.Adapt<CreatedCompanyDto>();
            return await Result<CreatedCompanyDto>.SuccessAsync(createdCompanyDto);
        }
    }
}
