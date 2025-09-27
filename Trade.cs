namespace App;

public class Trade
{
    public List<Item> Items;
    public User Sender;
    public User Receiver;
    public string Status;

    public Trade(List<Item> items, User sender, User receiver, string status)
    {
        Items = items;
        Sender = sender;
        Receiver = receiver;
        Status = status;
    }
}
