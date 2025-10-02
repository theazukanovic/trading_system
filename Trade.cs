namespace App;

public class Trade
{
    public User Sender; //den som vill ha item
    public User Receiver; //den som äger item
    public Item RequestedItem; //det man vill ha
    public Item OfferedItem; // det item man erbjuder i utbyte
    public TradeStatus Status; //pending, accepted or denied.

    public Trade(
        User sender,
        User receiver,
        Item requestedItem,
        Item offeredItem,
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
