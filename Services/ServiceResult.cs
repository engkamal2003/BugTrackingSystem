namespace BugTrackingSystem.Services
{
    public class ServiceResult
    {
        public bool Success { get; private set; }
        public string Message { get; private set; }
        public object Data { get; private set; }
        public ServiceStatus Status { get; private set; }

        public static ServiceResult Ok(object data = null, string message = null)
            => new ServiceResult { Success = true, Data = data, Message = message, Status = ServiceStatus.Ok };

        public static ServiceResult Fail(string message, ServiceStatus status = ServiceStatus.BadRequest)
            => new ServiceResult { Success = false, Message = message, Status = status };
    }

    public enum ServiceStatus
    {
        Ok,
        BadRequest,
        NotFound,
        Forbidden
    }
}
