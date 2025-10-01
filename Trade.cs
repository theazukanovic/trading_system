namespace App;

public class Trade
{
    public User Sender; //den som vill ha item
    public User Receiver; //den som äger item
    public Item Item; //vilken item bytet gäller
    public TradeStatus Status; //pending, accepted or denied.

    public Trade(User sender, User receiver, Item item, TradeStatus status)
    {
        Sender = sender;
        Receiver = receiver;
        Item = item;
        Status = status;
    }
}
