namespace July_Team.Exceptions
{
    /// <summary>
    /// Base exception class for application-specific errors.
    /// Provides structured error handling with error codes.
    /// </summary>
    public class AppException : Exception
    {
        public string ErrorCode { get; }
        public int StatusCode { get; }

        public AppException(string message, string errorCode = "GENERAL_ERROR", int statusCode = 500)
            : base(message)
        {
            ErrorCode = errorCode;
            StatusCode = statusCode;
        }
    }

    /// <summary>
    /// Thrown when a requested resource cannot be found.
    /// Maps to HTTP 404 Not Found.
    /// </summary>
    public class NotFoundException : AppException
    {
        public NotFoundException(string resourceName, int id)
            : base($"{resourceName} with ID {id} was not found.", "NOT_FOUND", 404)
        {
        }

        public NotFoundException(string message)
            : base(message, "NOT_FOUND", 404)
        {
        }
    }

    /// <summary>
    /// Thrown when input validation fails.
    /// Maps to HTTP 400 Bad Request.
    /// </summary>
    public class ValidationException : AppException
    {
        public IDictionary<string, string[]> Errors { get; }

        public ValidationException(string message)
            : base(message, "VALIDATION_ERROR", 400)
        {
            Errors = new Dictionary<string, string[]>();
        }

        public ValidationException(IDictionary<string, string[]> errors)
            : base("One or more validation errors occurred.", "VALIDATION_ERROR", 400)
        {
            Errors = errors;
        }
    }

    /// <summary>
    /// Thrown when a business rule is violated.
    /// Example: Insufficient stock for an order.
    /// </summary>
    public class BusinessRuleException : AppException
    {
        public BusinessRuleException(string message)
            : base(message, "BUSINESS_RULE_VIOLATION", 422)
        {
        }
    }

    /// <summary>
    /// Thrown when user lacks permission for an operation.
    /// Maps to HTTP 403 Forbidden.
    /// </summary>
    public class UnauthorizedAccessException : AppException
    {
        public UnauthorizedAccessException(string message = "You do not have permission to perform this action.")
            : base(message, "UNAUTHORIZED", 403)
        {
        }
    }
}
