using System.ComponentModel.DataAnnotations;
using System.Drawing;

namespace RestFullApiTest
{
    public class UserConfig
    {
        public int? Id { get; set; }
        public int? UserId { get; set; }
        public string? Name { get; set; }
        public string? Surename { get; set; }
        
        public string? Email { get; set; }

        public string? PhoneNumber { get; set; }
        public string? SelectedColor { get; set; }
        public byte[]? AvatarBytes { get; set; }
        public string? Theme { get; set; }
    }
}
