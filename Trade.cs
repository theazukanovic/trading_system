namespace App;

public class Trade
{
    public User Sender;
    public User Receiver;
    public List<Item> Items;
    public TradeStatus Status;

    public Trade(User sender, User receiver, List<Item> items, TradeStatus status)
    {
        Sender = sender;
        Receiver = receiver;
        Items = items;
        Status = status;
    }

    public void Accept()
    {
        foreach (Item item in Items)
        {
            if (item.Owner == Receiver)
            {
                Receiver.Items.Remove(item);
                Sender.Items.Add(item);
                item.Owner = Sender;
            }
            else if (item.Owner == Sender)
            {
                Sender.Items.Remove(item);
                Receiver.Items.Add(item);
                item.Owner = Receiver;
            }
        }

        Status = TradeStatus.Accepted;
    }
}
