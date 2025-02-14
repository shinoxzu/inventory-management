namespace InventoryManagement.Application.Errors;

public class ProvidedDataIsInvalidError(string message) : Exception(message);