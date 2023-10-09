using Application.Common;
using Application.Common.Dtos;
using Application.Common.Exceptions;
using AutoMapper;
using Domain.DataModels;
using Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Queries.Handler
{
    public class TestQueryHandler : IRequestHandler<TestQuery, TestDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public TestQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<TestDto> Handle(TestQuery request, CancellationToken cancellationToken)
        {
            TestDto response = new TestDto();
            TestDto testdto = new TestDto();
            var userList = await _unitOfWork.UserRepository.GetAllAsync();
            testdto.IsSuccess = userList.Any(u => u.Phone.Equals(request.phone));
            if (testdto.IsSuccess)
            {
                User? u = userList.FirstOrDefault(u => u.Phone.Equals(request.phone));
                _mapper.Map(u, testdto);
                response = testdto;
            }
            else
            {
                if (!testdto.IsSuccess)
                {
                    //response.Exception = new NotFoundException();
                    throw new NotFoundException("Phone number is not found");
                }
                if (userList.Count() == 0)
                {
                    //response.Exception = new BadRequestException();
                    throw new BadRequestException("Wrong input");
                }
            }
            return response;
        }
    }
}
