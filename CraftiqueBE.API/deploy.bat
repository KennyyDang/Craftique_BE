@echo off
echo 🚀 Bắt đầu triển khai Craftique API...

REM Chuyển vào thư mục docker-compose
cd docker-compose

REM Dừng và xóa container cũ nếu có
echo 📦 Dọn dẹp container cũ...
docker-compose down

REM Xóa image cũ để build lại từ đầu
echo 🗑️ Xóa image cũ...
docker rmi craftiquebeapi 2>nul

REM Build lại image với cache mới
echo 🔨 Build lại Docker image...
docker-compose build --no-cache

REM Khởi động lại services
echo ▶️ Khởi động services...
docker-compose up -d

REM Kiểm tra trạng thái
echo ✅ Kiểm tra trạng thái container...
docker-compose ps

REM Quay lại thư mục gốc
cd ..

echo 🎉 Triển khai hoàn tất!
echo 📋 API sẽ có sẵn tại: http://localhost:5000
echo 📋 Swagger UI tại: http://localhost:5000/swagger
echo.
echo 📝 Để xem logs realtime, chạy lệnh:
echo cd docker-compose ^&^& docker-compose logs -f craftiquebe.api

pause