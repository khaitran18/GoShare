using Application.Common.Utilities.Google;
using Domain.DataModels;
using Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Hosting;
using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Behaviours
{
    //// Tạo một request model cho việc tạo chuyến đi mới
    //public class CreateTripRequest
    //{
    //    public string Destination { get; set; } // Điểm đến của chuyến đi
    //    public int NumberOfPeople { get; set; } // Số người tham gia chuyến đi
    //}

    //// Tạo một response model cho việc tạo chuyến đi mới
    //public class CreateTripResponse
    //{
    //    public int TripId { get; set; } // Id của chuyến đi mới được tạo
    //    public string Status { get; set; } // Trạng thái của chuyến đi (pending, going, done, canceled)
    //}

    //// Tạo một command cho MediatR để xử lý logic của việc tạo chuyến đi mới
    //public class CreateTripCommand : IRequest<CreateTripResponse>
    //{
    //    public CreateTripRequest Request { get; set; } // Request model được gửi từ client

    //    public CreateTripCommand(CreateTripRequest request)
    //    {
    //        Request = request;
    //    }
    //}

    //// Tạo một handler cho command trên để thực hiện các nhiệm vụ sau:
    //// - Validate request model
    //// - Tạo một entity Trip mới và lưu vào database sử dụng repository pattern
    //// - Trả về response model chứa id và status của chuyến đi mới
    //public class CreateTripHandler : IRequestHandler<CreateTripCommand, CreateTripResponse>
    //{
    //    private readonly ITripRepository _tripRepository; // Interface của repository để thao tác với database

    //    public CreateTripHandler(ITripRepository tripRepository)
    //    {
    //        _tripRepository = tripRepository;
    //    }

    //    public async Task<CreateTripResponse> Handle(CreateTripCommand request, CancellationToken cancellationToken)
    //    {
    //        // Validate request model
    //        if (request.Request == null)
    //        {
    //            throw new ArgumentNullException(nameof(request.Request));
    //        }

    //        if (string.IsNullOrEmpty(request.Request.Destination))
    //        {
    //            throw new ArgumentException("Destination is required");
    //        }

    //        if (request.Request.NumberOfPeople <= 0)
    //        {
    //            throw new ArgumentException("Number of people must be positive");
    //        }

    //        // Tạo một entity Trip mới và lưu vào database sử dụng repository pattern
    //        var trip = new Trip
    //        {
    //            // Origin
    //            Destination = request.Request.Destination,
    //            NumberOfPeople = request.Request.NumberOfPeople,
    //            Status = "pending" // Trạng thái ban đầu là pending
    //        };

    //        await _tripRepository.AddAsync(trip); // Thêm trip vào database

    //        // Trả về response model chứa id và status của chuyến đi mới
    //        return new CreateTripResponse
    //        {
    //            TripId = trip.Id,
    //            Status = trip.Status
    //        };
    //    }
    //}

    //// Tạo một controller để nhận request từ client và gọi MediatR để xử lý command
    //[ApiController]
    //[Route("api/[controller]")]
    //public class TripsController : ControllerBase
    //{
    //    private readonly IMediator _mediator; // Interface của MediatR để gọi command

    //    public TripsController(IMediator mediator)
    //    {
    //        _mediator = mediator;
    //    }

    //    [HttpPost] // Phương thức POST để tạo chuyến đi mới
    //    public async Task<IActionResult> CreateTrip([FromBody] CreateTripRequest request) // Nhận request model từ client
    //    {
    //        var command = new CreateTripCommand(request); // Tạo command từ request model
    //        var response = await _mediator.Send(command); // Gọi MediatR để xử lý command và trả về response model
    //        return Ok(response); // Trả về http response với status code 200 (OK) và response model
    //    }
    //}

    ////----------------------------------------------------------------------------

    //// Tạo một class kế thừa từ IJob interface của Quartz để định nghĩa công việc cần làm khi background service được kích hoạt
    //public class TripScannerJob : IJob
    //{
    //    private readonly ITripRepository _tripRepository; // Interface của repository để thao tác với database
    //    private readonly IDriverService _driverService; // Interface của service để tìm người tài xế gần nhất

    //    public TripScannerJob(ITripRepository tripRepository, IDriverService driverService)
    //    {
    //        _tripRepository = tripRepository;
    //        _driverService = driverService;
    //    }

    //    public async Task Execute(IJobExecutionContext context) // Phương thức Execute sẽ được gọi khi background service được kích hoạt
    //    {
    //        // Lấy tất cả các chuyến đi có trạng thái là pending từ database
    //        var pendingTrips = await _tripRepository.GetByStatusAsync("pending");

    //        // Đối với mỗi chuyến đi đang chờ, tìm người tài xế gần nhất với điểm xuất phát của chuyến đi sử dụng một công thức như Haversine để tính khoảng cách giữa hai điểm trên bản đồ.
    //        foreach (var trip in pendingTrips)
    //        {
    //            var nearestDriver = await _driverService.FindNearestDriverAsync(trip.Origin);

    //            // Gửi một thông báo đến người tài xế gần nhất để hỏi họ có muốn nhận chuyến đi hay không sử dụng SignalR hoặc Firebase Cloud Messaging
    //            await _driverService.SendNotificationAsync(nearestDriver, trip);

    //            // Cập nhật trạng thái của chuyến đi là going và id của tài xế vào database nếu tài xế đồng ý
    //            // Hoặc tìm người tài xế tiếp theo gần nhất và lặp lại quá trình trên nếu người tài xế hủy, hoặc không phản hồi trong một khoảng thời gian nhất định
    //            await _driverService.HandleDriverResponseAsync(nearestDriver, trip);
    //        }
    //    }
    //}

    //// Tạo một class kế thừa từ BackgroundService interface của ASP.NET Core để đăng ký background service với dependency injection
    //public class TripScannerService : BackgroundService
    //{
    //    private readonly IServiceProvider _serviceProvider; // Interface của service provider để lấy các dependency cần thiết

    //    public TripScannerService(IServiceProvider serviceProvider)
    //    {
    //        _serviceProvider = serviceProvider;
    //    }

    //    protected override async Task ExecuteAsync(CancellationToken stoppingToken) // Phương thức ExecuteAsync sẽ được gọi khi background service được khởi động
    //    {
    //        // Tạo một lịch trình cho background service để chạy mỗi phút sử dụng Quartz
    //        var schedulerFactory = new StdSchedulerFactory();
    //        var scheduler = await schedulerFactory.GetScheduler(stoppingToken);
    //        await scheduler.Start(stoppingToken);

    //        var jobDetail = JobBuilder.Create<TripScannerJob>()
    //            .WithIdentity("TripScannerJob")
    //            .Build();

    //        var trigger = TriggerBuilder.Create()
    //            .WithIdentity("TripScannerTrigger")
    //            .StartNow()
    //            .WithSimpleSchedule(x => x.WithIntervalInMinutes(1).RepeatForever())
    //            .Build();

    //        await scheduler.ScheduleJob(jobDetail, trigger, stoppingToken);

    //        // Đợi cho đến khi background service được dừng lại
    //        await Task.Delay(Timeout.Infinite, stoppingToken);
    //    }
    //}

    //// Đăng ký background service với dependency injection trong Startup class
    //public class Startup
    //{
    //    public void ConfigureServices(IServiceCollection services)
    //    {
    //        // Các dependency khác

    //        services.AddHostedService<TripScannerService>(); // Đăng ký background service
    //    }

    //    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    //    {
    //        // Các middleware khác
    //    }
    //}

    ////----------------------------------------------------------

    //// Tạo một class để lưu trữ thông tin của người tài xế
    //public class Driver
    //{
    //    public int Id { get; set; } // Id của người tài xế
    //    public string Name { get; set; } // Tên của người tài xế
    //    public double Latitude { get; set; } // Vĩ độ của vị trí hiện tại của người tài xế
    //    public double Longitude { get; set; } // Kinh độ của vị trí hiện tại của người tài xế
    //}

    //// Tạo một interface để định nghĩa các phương thức của service để tìm người tài xế gần nhất
    //public interface IUserService
    //{
    //    Task<User?> FindNearestDriverAsync(string origin); // Tìm người tài xế gần nhất với điểm xuất phát của chuyến đi
    //    Task SendNotificationAsync(User driver, Trip trip); // Gửi thông báo đến người tài xế để hỏi họ có muốn nhận chuyến đi hay không
    //    Task HandleDriverResponseAsync(User driver, Trip trip); // Xử lý phản hồi của người tài xế và cập nhật trạng thái của chuyến đi
    //}

    //// Tạo một class để cài đặt interface trên
    //public class UserService : IUserService
    //{
    //    private readonly IUnitOfWork _unitOfWork;
    //    private readonly INotificationService _notificationService; // Interface của service để gửi thông báo sử dụng SignalR hoặc Firebase Cloud Messaging

    //    public UserService(IUnitOfWork unitOfWork, INotificationService notificationService)
    //    {
    //        _unitOfWork = unitOfWork;
    //        _notificationService = notificationService;
    //    }

    //    //public async Task<Driver> FindNearestDriverAsync(string origin)
    //    //{
    //    //    // Lấy tất cả các người tài xế từ database
    //    //    var drivers = await _driverRepository.GetAllAsync();

    //    //    // Tách chuỗi origin thành latitude và longitude
    //    //    var originParts = origin.Split(",");
    //    //    var originLat = double.Parse(originParts[0]);
    //    //    var originLng = double.Parse(originParts[1]);

    //    //    // Khởi tạo biến để lưu kết quả và khoảng cách nhỏ nhất
    //    //    Driver nearestDriver = null;
    //    //    double minDistance = double.MaxValue;

    //    //    // Đối với mỗi người tài xế, tính khoảng cách giữa vị trí hiện tại của họ và điểm xuất phát của chuyến đi sử dụng công thức Haversine
    //    //    foreach (var driver in drivers)
    //    //    {
    //    //        var driverLat = driver.Latitude;
    //    //        var driverLng = driver.Longitude;

    //    //        var distance = CalculateDistance(originLat, originLng, driverLat, driverLng);

    //    //        // Nếu khoảng cách nhỏ hơn minDistance, cập nhật kết quả và minDistance
    //    //        if (distance < minDistance)
    //    //        {
    //    //            nearestDriver = driver;
    //    //            minDistance = distance;
    //    //        }
    //    //    }

    //    //    // Trả về kết quả
    //    //    return nearestDriver;
    //    //}

    //    public async Task<User?> FindNearestDriverAsync(string origin)
    //    {
    //        // Khởi tạo biến để lưu kết quả và khoảng cách nhỏ nhất
    //        int? nearestDriverId = null;
    //        double minDistance = double.MaxValue;

    //        // Khởi tạo biến để lưu bán kính tìm kiếm hiện tại và bước nhảy
    //        double radius = 1; // Bán kính ban đầu là 1 km
    //        double step = 1; // Mỗi lần không tìm thấy tài xế, tăng bán kính lên 1 km

    //        // Khởi tạo biến để lưu số lượng tài xế tối đa được truy vấn mỗi lần
    //        int limit = 10;

    //        // Lặp cho đến khi tìm thấy một vị trí hoặc vượt quá giới hạn bán kính
    //        while (nearestDriverId == null && radius <= 5)
    //        {
    //            // Lấy các vị trí của các tài xế đang online từ database theo bán kính và giới hạn số lượng
    //            var driverLocations = await _unitOfWork.LocationRepository.GetOnlineDriverLocationByRadiusAsync(origin, radius, limit);

    //            // Nếu không có vị trí nào trong bán kính hiện tại, tăng bán kính lên và tiếp tục vòng lặp
    //            if (driverLocations.Count == 0)
    //            {
    //                radius += step;
    //                continue;
    //            }

    //            // Đối với mỗi vị trí, tạo một điểm đến từ lat và long của nó và tính khoảng cách giữa điểm xuất phát và điểm đến sử dụng Routes API của Google
    //            foreach (var driverLocation in driverLocations)
    //            {
    //                var destination = driverLocation.Latitude + "," + driverLocation.Longitude;
    //                var distance = await GoogleMapsApiUtilities.ComputeDistanceMatrixAsync(origin, destination);

    //                // Nếu khoảng cách nhỏ hơn minDistance, cập nhật minDistance
    //                if (distance < minDistance)
    //                {
    //                    nearestDriverId = driverLocation.UserId;
    //                    minDistance = distance;
    //                }
    //            }

    //            // Tăng bán kính lên cho lần tìm kiếm tiếp theo
    //            radius += step;
    //        }

    //        var nearestDriver = await _unitOfWork.UserRepository.GetUserByIdAsync(nearestDriverId);

    //        // Trả về kết quả hoặc null nếu không tìm thấy
    //        return nearestDriver;
    //    }

    //    //private double CalculateDistance(double lat1, double lng1, double lat2, double lng2)
    //    //{
    //    //    // Công thức Haversine để tính khoảng cách giữa hai điểm trên bản đồ theo km
    //    //    const double R = 6371; // Bán kính Trái Đất theo km

    //    //    var dLat = ToRadians(lat2 - lat1);
    //    //    var dLng = ToRadians(lng2 - lng1);

    //    //    var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
    //    //            Math.Cos(ToRadians(lat1)) * Math.Cos(ToRadians(lat2)) *
    //    //            Math.Sin(dLng / 2) * Math.Sin(dLng / 2);

    //    //    var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

    //    //    var d = R * c;

    //    //    return d;
    //    //}

    //    //private double ToRadians(double angle)
    //    //{
    //    //    // Chuyển đổi góc từ độ sang radian
    //    //    return angle * Math.PI / 180;
    //    //}



    //    public async Task SendNotificationAsync(User driver, Trip trip)
    //    {
    //        var notification = new Notification
    //        {
    //            TripId = trip.Id,
    //            Destination = trip.Destination,
    //            NumberOfPeople = trip.NumberOfPeople,
    //            DriverId = driver.Id,
    //            DriverName = driver.Name
    //        };

    //        // Gửi thông báo đến người tài xế sử dụng SignalR hoặc Firebase Cloud Messaging
    //        await _notificationService.SendAsync(message);
    //    }

    //    // Tiếp tục cài đặt phương thức HandleDriverResponseAsync của class DriverService
    //    public async Task HandleDriverResponseAsync(User driver, Trip trip)
    //    {
    //        // Đợi cho đến khi nhận được phản hồi từ người tài xế hoặc hết thời gian chờ (ví dụ 30 giây)
    //        var response = await _notificationService.ReceiveAsync(driver.Id, 30);

    //        // Nếu tài xế đồng ý, cập nhật trạng thái của chuyến đi là going và id của tài xế vào database
    //        if (response == "accepted")
    //        {
    //            trip.Status = "going";
    //            trip.DriverId = driver.Id;
    //            await _tripRepository.UpdateAsync(trip);

    //            // Gửi một thông báo đến user để cho họ biết người tài xế đã nhận chuyến đi và thông tin của người tài xế
    //            await _notificationService.SendAsync(trip.UserId, $"Chúc mừng! Người tài xế {driver.Name} đã nhận chuyến đi của bạn. Hãy chuẩn bị để khởi hành!");
    //        }
    //        // Nếu người tài xế hủy, hoặc không phản hồi trong một khoảng thời gian nhất định, tìm người tài xế tiếp theo gần nhất và lặp lại quá trình trên.
    //        else
    //        {
    //            var nextDriver = await FindNearestDriverAsync(trip.Origin);
    //            await SendNotificationAsync(nextDriver, trip);
    //            await HandleDriverResponseAsync(nextDriver, trip);
    //        }
    //    }
    //}

    //public class Message
    //{
    //    public int TripId { get; set; } // Id của chuyến đi
    //    public string Destination { get; set; } // Điểm đến của chuyến đi
    //    public int NumberOfPeople { get; set; } // Số người tham gia chuyến đi
    //    public int DriverId { get; set; } // Id của người tài xế
    //    public string DriverName { get; set; } // Tên của người tài xế
    //}
}
