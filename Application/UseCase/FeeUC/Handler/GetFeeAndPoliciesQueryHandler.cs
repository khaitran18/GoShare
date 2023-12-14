using Application.Common.Dtos;
using Application.UseCase.FeeUC.Queries;
using AutoMapper;
using Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCase.FeeUC.Handler
{
    public class GetFeeAndPoliciesQueryHandler : IRequestHandler<GetFeeAndPoliciesQuery, List<FeeDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetFeeAndPoliciesQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<List<FeeDto>> Handle(GetFeeAndPoliciesQuery request, CancellationToken cancellationToken)
        {
            var response = new List<FeeDto>();
            var fees = (await _unitOfWork.FeeRepository.GetAllWithPoliciesAsync()).ToList();
            foreach (var fe in fees)
            {
                var feepolicies = _mapper.Map<List<FeepolicyDto>>(fe.Feepolicies.ToList());
                FeeDto dto = _mapper.Map<FeeDto>(fe);
                dto.policies = feepolicies;
                response.Add(dto);
            }
            return response.OrderBy(u=>u.CarType).ToList();
        }
    }
}
