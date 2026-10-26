# Bài Tập An Toàn Và Bảo Mật Thông Tin

## Thông tin sinh viên
- Họ tên: Dương Đình Kiền
- Lớp: K59KMT.K01
- MSSV: K235480106109
- Môn học: An toàn và bảo mật thông tin

## Mô tả
Bài tập thực hành mã hóa đối xứng (AES) và mã hóa bất đối xứng (RSA) 
sử dụng ngôn ngữ C# trên nền tảng .NET.

## Cấu trúc thư mục dự án

BaiTapAES_RSA/
├── Program.cs                    File code chính chứa toàn bộ logic
├── BaiTapAES_RSA.csproj          File cấu hình project .NET
├── khoa_aes.json                 File lưu khóa AES (Key + IV)
── public_key.pem                Public Key RSA định dạng PEM
├── private_key.pem               Private Key RSA định dạng PEM
├── du_lieu_ma_hoa_rsa.txt        Dữ liệu đã mã hóa bằng RSA (Base64)
── file_goc.dat                  File gốc dùng để test mã hóa AES
├── file_ma_hoa.dat               File đã được mã hóa AES
├── file_giai_ma.dat              File đã được giải mã AES
└── README.md                     File hướng dẫn này

## Yêu cầu hệ thống
- .NET SDK 6.0 hoặc cao hơn (máy mình dùng .NET 10.0.402)
- Visual Studio Code hoặc Visual Studio

## Cách chạy chương trình

Bước 1: Mở terminal trong thư mục dự án
Bước 2: Gõ lệnh: dotnet run
Bước 3: Xem kết quả trên màn hình console

Lưu ý: Mỗi bước trong bài tập là 1 phiên bản Program.cs riêng.
Cần thay thế code Program.cs tương ứng với từng bước.

## Nội dung bài tập

### Bài 1: Mã hóa AES (Mã hóa đối xứng)

Mã hóa đối xứng sử dụng cùng 1 khóa cho cả mã hóa và giải mã.

Các bước thực hiện:
1. Sinh khóa AES-256 ngẫu nhiên (Key 32 bytes + IV 16 bytes)
2. Lưu khóa ra file JSON (khoa_aes.json)
3. Import khóa từ file để sử dụng
4. Mã hóa và giải mã một đoạn văn bản
5. Mã hóa và giải mã file lớn
6. So sánh file gốc và file giải mã theo từng byte
7. Đo thời gian và rút ra kết luận

Kết quả thử nghiệm với file 10 MB:
- Thuật toán: AES-256 (chế độ CBC)
- Kích thước file: 10 MB (10,485,760 bytes)
- Tốc độ mã hóa: 318.43 MB/s
- Tốc độ giải mã: 629.05 MB/s
- Kết quả so sánh: File giải mã GIỐNG HOÀN TOÀN file gốc theo từng byte

Nhận xét:
- AES có tốc độ mã hóa/giải mã rất nhanh (hàng trăm MB/s)
- Giải mã nhanh hơn mã hóa khoảng 2 lần
- Sử dụng CryptoStream (stream mode) giúp xử lý file lớn 
  mà không cần load toàn bộ vào RAM
- Phù hợp để mã hóa dữ liệu lớn (hàng GB)

### Bài 2: Mã hóa RSA (Mã hóa bất đối xứng)

Mã hóa bất đối xứng sử dụng cặp khóa: Public Key để mã hóa, 
Private Key để giải mã.

Các bước thực hiện:
1. Sinh cặp khóa RSA 2048-bit
2. Xuất cặp khóa ra file định dạng PEM
3. View file PEM và giải thích ý nghĩa các byte
4. Import PEM để lấy các tham số n, e, d
5. Mã hóa dữ liệu (tối đa 190 bytes)
6. Giải mã và so sánh với dữ liệu gốc
7. Đánh giá tốc độ mã hóa/giải mã
8. Băm SHA256 và hiệu ứng tuyết lở
9. Chữ ký số RSA

#### Kết quả tham số RSA
- Modulus (n): 2048 bits
- Public Exponent (e): 65537 (0x010001)
- Private Exponent (d): 2048 bits
- Prime P (p): 1024 bits
- Prime Q (q): 1024 bits

#### Kết quả đánh giá tốc độ (10,000 lần mã hóa)

Phương pháp                          | Thời gian (ms) | Trung bình/lần (ms)
-------------------------------------|----------------|---------------------
Mã hóa bằng Public Key (e nhỏ)       | 277            | 0.0277
Mã hóa bằng Private Key (d lớn)      | 289            | 0.0289

Tỷ lệ: Private Key chậm hơn Public Key khoảng 1.04 lần

Giải thích:
- Trên máy tính hiện đại với .NET tối ưu, sự chênh lệch không lớn
- Lý thuyết: e = 65537 chỉ có 2 bit 1 trong binary nên tính nhanh
- d là số lớn 2048 bits nên tính chậm hơn
- Trong thực tế với khóa lớn hơn (4096 bits) hoặc không dùng 
  CRT optimization, Private Key sẽ chậm hơn 5-20 lần

#### Hiệu ứng tuyết lở SHA256

Test với dữ liệu: "Toi yeu Viet Nam"
Thay đổi nhỏ: thêm 1 dấu chấm "!" thành "Toi yeu Viet Nam!"

Kết quả:
- Số bit khác nhau: 144 / 256 bits
- Tỷ lệ: 56.2%
- Lý thuyết: ~50% (128/256 bits)

Kết luận: Chỉ thay đổi 1 bit đầu vào làm thay đổi khoảng 50% 
bit đầu ra. Đây là hiệu ứng tuyết lở (avalanche effect), 
đảm bảo SHA256 chống đạo ngược và chống xung đột.

#### Chữ ký số - Phương pháp đề xuất

Yêu cầu đặc biệt của thầy:
Cùng 1 dữ liệu + Cùng 1 Private Key 
=> Chữ ký ở các lần ký khác nhau phải KHÁC NHAU

Phương pháp đề xuất: Sử dụng RSA-PSS 
(Probabilistic Signature Scheme)

Tại sao PSS đạt được điều này?
1. PSS thêm salt ngẫu nhiên (random salt) vào quá trình ký
2. Mỗi lần ký, salt khác nhau => chữ ký khác nhau
3. Nhưng vẫn verify được bằng Public Key 
   (vì salt được mã hóa cùng chữ ký)

So sánh PKCS1v1.5 và PSS:

Tiêu chí              | PKCS1v1.5              | PSS
----------------------|------------------------|---------------------------
Tính deterministic    | Cùng input = cùng output | Cùng input = output khác nhau
Salt ngẫu nhiên       | Không có               | Có
Mức độ an toàn        | Tốt                    | Tốt hơn
Tiêu chuẩn            | Cũ (1993)              | Mới hơn (2002, RFC 3447)

Kết quả thử nghiệm:
- PKCS1v1.5: 3 lần ký cho ra CÙNG 1 chữ ký
- PSS: 3 lần ký cho ra 3 chữ ký KHÁC NHAU
- Cả 3 chữ ký PSS đều HỢP LỆ khi verify bằng Public Key
- Khi thay đổi dữ liệu (sai 1 ngày): chữ ký KHÔNG HỢP LỆ

Ứng dụng thực tế:
- Chữ ký số trong chứng thư số (digital certificate)
- Ký hợp đồng điện tử, văn bản pháp lý
- Bảo đảm tính toàn vẹn, chống giả mạo

## Kết luận chung

1. Về AES (Mã hóa đối xứng):
   - Tốc độ rất nhanh (hàng trăm MB/s)
   - Phù hợp mã hóa dữ liệu lớn
   - Nên dùng để mã hóa nội dung thực tế

2. Về RSA (Mã hóa bất đối xứng):
   - Chỉ mã hóa được dữ liệu nhỏ (≤190 bytes với 2048-bit)
   - Chậm hơn AES rất nhiều
   - Phù hợp để mã hóa khóa AES hoặc tạo chữ ký số

3. Về SHA256:
   - Hiệu ứng tuyết lở đảm bảo tính an toàn
   - Thay đổi nhỏ ở đầu vào => thay đổi lớn ở đầu ra

4. Về chữ ký số:
   - RSA-PSS là phương pháp tốt nhất để tạo chữ ký 
     khác nhau mỗi lần ký
   - Kết hợp SHA256 + RSA-PSS tạo chữ ký số an toàn

Trong thực tế, người ta thường kết hợp cả AES và RSA:
- Dùng AES mã hóa dữ liệu lớn (nhanh)
- Dùng RSA mã hóa khóa AES (an toàn khi truyền khóa)
- Dùng SHA256 + RSA-PSS tạo chữ ký số (đảm bảo toàn vẹn)

