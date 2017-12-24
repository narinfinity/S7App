namespace S7Test.Core.Entity
{
    public interface IEntity<TKey>
    {
        TKey Id { get; set; }
    }
}
