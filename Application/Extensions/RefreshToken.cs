using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Application.Extensions
{
    public static class RefreshToken
    {
        public static string GenerateRefreshToken(int size = 32) // Kích thước mặc định là 32 byte
        {
            var randomBytes = new byte[size];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomBytes); // Tạo chuỗi ngẫu nhiên
            }
            return Convert.ToBase64String(randomBytes).Replace("+", "").Replace("/", "").Replace("=", ""); // Chuyển đổi thành chuỗi Base64 và loại bỏ ký tự không mong muốn
        }
    }
}
