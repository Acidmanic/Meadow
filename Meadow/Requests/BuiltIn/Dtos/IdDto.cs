namespace Meadow.Requests.BuiltIn.Dtos;

public struct IdDto<TId>
{
    public TId Id { get;}

    public IdDto(TId id)
    {
        Id = id;
    }
    public static implicit operator TId(IdDto<TId> value) => value.Id;
    
    public static implicit operator IdDto<TId>(TId value) => new IdDto<TId>(value);
}