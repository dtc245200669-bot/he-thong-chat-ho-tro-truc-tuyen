\# HỆ THỐNG CHAT HỖ TRỢ TRỰC TUYẾN



\## 1. Giới thiệu



Dự án xây dựng hệ thống chat hỗ trợ giữa khách hàng và nhân viên tư vấn, sử dụng kiến trúc ứng dụng phân tán.



\## 2. Công nghệ sử dụng



\- C# và .NET 10

\- ASP.NET Core Minimal API

\- SignalR

\- RabbitMQ

\- Worker Service

\- PostgreSQL

\- Entity Framework Core

\- Docker và Docker Compose

\- React và TypeScript (frontend dự kiến)



\## 3. Kiến trúc hệ thống



\- Customer Client: giao diện khách hàng.

\- Agent Client: giao diện nhân viên tư vấn.

\- ChatService: xử lý API và nghiệp vụ chat.

\- SignalR Hub: giao tiếp thời gian thực.

\- PostgreSQL: lưu dữ liệu.

\- RabbitMQ: truyền sự kiện qua hàng đợi.

\- HistoryWorker: xử lý lịch sử tin nhắn.



\## 4. Cấu trúc dự án



```text

HeThongChatHoTroTrucTuyen/

├── src/

│   ├── ChatService/

│   └── HistoryWorker/

├── .gitignore

├── docker-compose.yml

└── HeThongChatHoTroTrucTuyen.slnx

```



\## 5. Yêu cầu



\- Windows 10/11

\- .NET SDK 10

\- Docker Desktop

\- Git

\- Visual Studio Code



\## 6. Cấu hình môi trường



Tạo file `.env` ở thư mục gốc, khai báo thông tin PostgreSQL và RabbitMQ.



Không đưa file `.env` chứa mật khẩu thật lên GitHub.



\## 7. Khởi chạy PostgreSQL và RabbitMQ



```powershell

docker compose up -d

docker compose ps

```



\## 8. Cập nhật database



```powershell

dotnet ef database update --project .\\src\\ChatService\\ChatService.csproj

```



\## 9. Chạy ChatService



Cấu hình chuỗi kết nối PostgreSQL:



```powershell

$env:ConnectionStrings\_\_DefaultConnection = "Host=localhost;Port=5433;Database=chat\_support;Username=chat\_admin;Password=MAT\_KHAU\_POSTGRES"

```



Thay `MAT\_KHAU\_POSTGRES` bằng mật khẩu PostgreSQL của bạn.



Chạy dịch vụ:



```powershell

dotnet run --project .\\src\\ChatService\\ChatService.csproj --urls http://localhost:5000

```



\## 10. Các API đã triển khai



| Phương thức | Endpoint | Chức năng |

|---|---|---|

| GET | / | Kiểm tra dịch vụ |

| GET | /health/database | Kiểm tra database |

| POST | /conversations | Tạo cuộc hội thoại |

| GET | /conversations | Danh sách hội thoại |

| GET | /conversations/{id} | Chi tiết hội thoại |

| GET | /conversations/{id}/messages | Lịch sử tin nhắn |

| POST | /conversations/{id}/close | Đóng hội thoại |



\## 11. Tiến độ



Đã thực hiện:

\- Khởi tạo solution .NET 10.

\- Tạo ChatService và HistoryWorker.

\- Cấu hình Docker Compose cho PostgreSQL và RabbitMQ.

\- Xây dựng model Conversation và Message.

\- Tạo migration và bảng database.

\- Xây dựng và kiểm thử các REST API cơ bản.



Cần tiếp tục:

\- SignalR Hub và chat thời gian thực.

\- Tích hợp RabbitMQ.

\- Hoàn thiện HistoryWorker.

\- Xây dựng giao diện React.

\- Hoàn thiện Docker Compose cho toàn hệ thống.



\---



Dự án học tập môn Phát triển ứng dụng phân tán.
