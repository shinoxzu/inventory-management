namespace InventoryManagement.Application.Errors;

public class AccessDeniedError(string message) : Exception(message);