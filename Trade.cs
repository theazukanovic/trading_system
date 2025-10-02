namespace App;

public class Trade
{
    public User Sender; //den som vill ha item
    public User Receiver; //den som äger item
    public Item RequestedItem; //det man vill ha
    public List<Item> OfferedItem; // flera erbjudna items
    public TradeStatus Status; //pending, accepted or denied.

    public Trade(
        User sender,
        User receiver,
        Item requestedItem,
        List<Item> offeredItem,
        TradeStatus status
    )
    {
        Sender = sender;
        Receiver = receiver;
        RequestedItem = requestedItem;
        OfferedItem = offeredItem;
        Status = status;
    }
}
