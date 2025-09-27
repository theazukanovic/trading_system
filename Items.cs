namespace App;

public class Item
{
    public User Owner;
    public string Name;
    public string Description;

    public Item(string name, string description, User owner)
    {
        Name = name;
        Description = description;
        Owner = owner;
    }
}
