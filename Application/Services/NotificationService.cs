using FirebaseAdmin.Messaging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    // Tạo một interface để định nghĩa các phương thức của service để gửi và nhận thông báo sử dụng Firebase Cloud Messaging
    //public interface INotificationService
    //{
    //    Task SendAsync(Message message); // Gửi một message đến một thiết bị sử dụng token của thiết bị
    //    Task SendAsync(string token, string title, string body); // Gửi một notification đến một thiết bị sử dụng token, title và body của notification
    //    Task<string> ReceiveAsync(string token, int timeout); // Nhận phản hồi từ một thiết bị sử dụng token của thiết bị và thời gian chờ tối đa
    //}

    //// Tạo một class để cài đặt interface trên
    //public class NotificationService : INotificationService
    //{
    //    private readonly FirebaseMessaging _firebaseMessaging; // Instance của FirebaseMessaging để gọi API của Firebase
    //    private readonly ConcurrentDictionary<string, TaskCompletionSource<string>> _responses; // Dictionary để lưu trữ các task completion source cho mỗi token

    //    public NotificationService()
    //    {
    //        _firebaseMessaging = FirebaseMessaging.DefaultInstance; // Lấy instance của FirebaseMessaging từ FirebaseApp đã được khởi tạo ở Startup.cs
    //        _responses = new ConcurrentDictionary<string, TaskCompletionSource<string>>(); // Khởi tạo dictionary rỗng
    //    }

    //    public async Task SendAsync(Message message)
    //    {
    //        // Gửi message đến thiết bị sử dụng token của thiết bị
    //        await _firebaseMessaging.SendAsync(message);
    //    }

    //    public async Task SendAsync(string token, string title, string body)
    //    {
    //        // Tạo một message từ token, title và body của notification
    //        var message = new Message
    //        {
    //            Notification = new Notification
    //            {
    //                Title = title,
    //                Body = body
    //            },
    //            Token = token
    //        };

    //        // Gửi message đến thiết bị sử dụng token của thiết bị
    //        await _firebaseMessaging.SendAsync(message);
    //    }

    //    public async Task<string> ReceiveAsync(string token, int timeout)
    //    {
    //        // Tạo một task completion source cho token này nếu chưa có sẵn trong dictionary
    //        var tcs = _responses.GetOrAdd(token, new TaskCompletionSource<string>());

    //        // Đợi cho đến khi task completion source được hoàn thành hoặc hết thời gian chờ
    //        var completedTask = await Task.WhenAny(tcs.Task, Task.Delay(timeout * 1000));

    //        // Nếu task completion source được hoàn thành, trả về kết quả là phản hồi của thiết bị ("accepted" hoặc "rejected")
    //        if (completedTask == tcs.Task)
    //        {
    //            return tcs.Task.Result;
    //        }
    //        // Nếu hết thời gian chờ, trả về kết quả là "timeout"
    //        else
    //        {
    //            return "timeout";
    //        }
    //    }

    //    // Phương thức này được gọi khi nhận được một message từ thiết bị sử dụng Firebase Cloud Messaging SDK trên client app
    //    public void OnMessageReceived(object sender, MessageReceivedEventArgs e)
    //    {
    //        // Lấy token và data của message
    //        var token = e.Message.From;
    //        var data = e.Message.Data;

    //        // Nếu data chứa key là "response", có nghĩa là đây là phản hồi của thiết bị cho một thông báo đã gửi trước đó
    //        if (data.ContainsKey("response"))
    //        {
    //            // Lấy giá trị của key "response" là phản hồi của thiết bị ("accepted" hoặc "rejected")
    //            var response = data["response"];

    //            // Nếu dictionary có task completion source cho token này, đánh dấu task completion source là hoàn thành và gán kết quả là phản hồi của thiết bị
    //            if (_responses.TryGetValue(token, out var tcs))
    //            {
    //                tcs.TrySetResult(response);
    //            }
    //        }
    //    }
    //}
}
