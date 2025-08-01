namespace MealMentor.Core.DTOs.UserDTO
{
    public class UserProfileDTO
    {

    }

    public class EditProfileRequestDTO
    {
        public string Id { get; set; }
        public string Username { get; set; }
        public double? Height { get; set; }
        public double? Weight { get; set; }
        public DateTime? BirthDate { get; set; }
    }
    public class EditProfileResponseDTO
    {
        public string Username { get; set; }
        public double? Height { get; set; }
        public double? Weight { get; set; }
        public DateTime? BirthDate { get; set; }
        public double BMI { get; set; }
    }
    public class UserSearchtDTO
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public DateTime? BirthDate { get; set; }
    }
}
