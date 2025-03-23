namespace GenericBaseRepository.Entities;

public interface IEntityAuthorizeds
{
    Guid CreatedBy { get; set; }
    Guid? ModifiedBy { get; set; }
    Guid? DeletedBy { get; set; }
}
