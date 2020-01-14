namespace ItspServices.pServer.Abstraction.Models.UseCase.Response
{
    public class UseCaseResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }

        public UseCaseResponse(bool success, string message)
        {
            Success = success;
            Message = message;
        }
    }
}
