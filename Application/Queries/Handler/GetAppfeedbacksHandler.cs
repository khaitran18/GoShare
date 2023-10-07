using Application.Common;
using Application.Common.Dtos;
using AutoMapper;
using Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Queries.Handler
{
    public class GetAppfeedbacksHandler : IRequestHandler<GetAppfeedbacksQuery, BaseResponse<PaginatedResult<AppfeedbackDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        //private readonly ITokenService _tokenService;

        public GetAppfeedbacksHandler(IUnitOfWork unitOfWork, IMapper mapper/*, ITokenService tokenService*/)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            //_tokenService = tokenService;
        }

        public async Task<BaseResponse<PaginatedResult<AppfeedbackDto>>> Handle(GetAppfeedbacksQuery request, CancellationToken cancellationToken)
        {
            var response = new BaseResponse<PaginatedResult<AppfeedbackDto>>();

            try
            {
                //ClaimsPrincipal? claims = _tokenService.ValidateToken(request.Token ?? "");
                //if (claims != null)
                //{
                    //int.TryParse(claims.FindFirst("jti")?.Value, out int userId);

                    //var isLecturer = await _unitOfWork.UserRepository.IsUserLecturer(userId);

                    var (appfeedbacks, totalCount) = await _unitOfWork.AppfeedbackRepository.GetAppfeedbacks(
                        request.SortBy,
                        request.Page,
                        request.PageSize
                    );

                    var feedbackDtos = _mapper.Map<List<AppfeedbackDto>>(appfeedbacks);

                    var paginatedResult = new PaginatedResult<AppfeedbackDto>(
                        feedbackDtos,
                        totalCount,
                        request.Page,
                        request.PageSize
                    );

                    response.Result = paginatedResult;
                    response.Message = "Get feedbacks successfully!";
                //}
                //else
                //{
                //    response.Error = true;
                //    response.Exception = new BadRequestException("Invalid credentials");
                //}
            }
            catch (Exception ex)
            {
                response.Error = true;
                response.Message = ex.Message;
                response.Exception = ex;
            }

            return response;
        }
    }
}
