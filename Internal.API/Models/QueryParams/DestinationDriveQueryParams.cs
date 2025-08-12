namespace Distrupol.Bank.API.Models.QueryParams;

public class DestinationDriveQueryParams
{    
    
    public string BatchNumber { get; set; }
    public string SharedDrive { get; set; }
    public string DestinationDrive { get; set; }
    public bool Uppercase { get; set; }
}