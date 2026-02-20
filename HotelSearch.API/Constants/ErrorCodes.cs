namespace HotelSearch.API.Constants;

public static class ErrorCodes
{
    // Validation (1000–1099)
    public const string Required = "1000";
    public const string NameTooLong = "1001";
    public const string OutOfRange = "1002";
    public const string AlreadyExists = "1003";
    
    // Not Found / State (1200–1299)
    public const string NotFound = "1200";

    // System / Infrastructure (1400–1499)
    public const string SavingError = "1400";
}