using System.ComponentModel.DataAnnotations;

namespace Service.Dto;

public class CreateStoreDto
{
    [Required]
    public ChainType ChainName { get; set; }

    [Range(-90, 90)]
    public double Latitude { get; set; }

    [Range(-180, 180)]
    public double Longitude { get; set; }
}

public class ReadStoreDto
{
    public int Id { get; set; }

    public ChainType ChainName { get; set; }

    public double Latitude { get; set; }

    public double Longitude { get; set; }
}

public class EditStoreDto
{
    [Required]
    public int Id { get; set; }

    [Required]
    public ChainType ChainName { get; set; }

    [Range(-90, 90)]
    public double Latitude { get; set; }

    [Range(-180, 180)]
    public double Longitude { get; set; }
}

public class ShortStoreDto
{
    public int Id { get; set; }

    [Required]
    public ChainType ChainName { get; set; }
}
