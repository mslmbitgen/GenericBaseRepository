namespace GenericBaseRepository.Entities;

public interface IEntity<T>
{
    T Id { get; set; }
}
