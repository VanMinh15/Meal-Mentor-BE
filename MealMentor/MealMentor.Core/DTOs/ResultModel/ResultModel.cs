namespace MealMentor.Core.DTOs.ResultModel
{
    public class ResultModel
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public object? Data { get; set; }
        public int StatusCode { get; set; }
    }

    public class PaymentResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public int StatusCode { get; set; }
    }

}
