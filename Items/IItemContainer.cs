namespace OOD_Project.Items;

public interface IItemContainer
{
    bool TryAdd(Item item);
    Item ExtractLast();
}
