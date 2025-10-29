using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Service.Dto;

public class StoreService
{
    private readonly FinkDbContext _dbContext;

    public StoreService(FinkDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ReadStoreDto> CreateStoreAsync(CreateStoreDto dto)
    {
        if (dto == null)
        {
            throw new ArgumentNullException(nameof(dto));
        }

        var store = new Store
        {
            ChainName = dto.ChainName,
            Latitude = dto.Latitude,
            Longitude = dto.Longitude
        };

        await _dbContext.Stores.AddAsync(store);
        await _dbContext.SaveChangesAsync();

        return MapToReadDto(store);
    }

    public async Task<ReadStoreDto?> GetStoreByIdAsync(int id)
    {
        var store = await _dbContext.Stores.FirstOrDefaultAsync(s => s.Id == id);
        return store == null ? null : MapToReadDto(store);
    }

    public async Task<ReadStoreDto> UpdateStoreAsync(EditStoreDto dto)
    {
        if (dto == null)
        {
            throw new ArgumentNullException(nameof(dto));
        }

        var store = await _dbContext.Stores.FirstOrDefaultAsync(s => s.Id == dto.Id);
        if (store == null)
        {
            throw new InvalidOperationException($"Store with id '{dto.Id}' was not found.");
        }

        store.ChainName = dto.ChainName;
        store.Latitude = dto.Latitude;
        store.Longitude = dto.Longitude;

        await _dbContext.SaveChangesAsync();

        return MapToReadDto(store);
    }

    public async Task DeleteStoreAsync(int id)
    {
        var store = await _dbContext.Stores.FirstOrDefaultAsync(s => s.Id == id);

        if (store == null)
        {
            return;
        }

        _dbContext.Stores.Remove(store);
        await _dbContext.SaveChangesAsync();
    }

    public ShortStoreDto MapToShortDto(Store store)
    {
        if (store == null)
        {
            throw new ArgumentNullException(nameof(store));
        }

        return new ShortStoreDto
        {
            Id = store.Id,
            ChainName = store.ChainName
        };
    }

    private static ReadStoreDto MapToReadDto(Store store)
    {
        return new ReadStoreDto
        {
            Id = store.Id,
            ChainName = store.ChainName,
            Latitude = store.Latitude,
            Longitude = store.Longitude
        };
    }
}
