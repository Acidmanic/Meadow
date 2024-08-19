namespace Meadow.Requests.BuiltIn.Dtos;

public record FindPagedDto(long Offset, long Size, string FilterExpression, string SearchExpression, string OrderExpression);