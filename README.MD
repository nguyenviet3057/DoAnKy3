# Cấu hình chạy dự án
- Import data bằng cách chạy file `QLNS.sql` bằng SQL Server (SSMS)
- Mở file `DoAnKy3.sln` bằng Visual Studio
- Trong Visual Studio, mở file Web.config, ở dòng `<add name="QLNSConnectionString" connectionString="Data Source=`**`Darkin-Laptop\DARKIN`**`;Initial Catalog=QLNS;Persist Security Info=True;User ID=`**`sa`**`;Password=`**`12345678`**`" providerName="System.Data.SqlClient" />` đổi tên **`Darkin-Laptop\DARKIN`** thành tên ***Server name*** trong SSMS khi mới mở lên, tương tự với **`User ID`** và **`Password`** đối với tài khoản đăng nhập SQL Server Authentication