using App;

List<User> users = new List<User>();
List<Trade> trades = new List<Trade>();
List<Item> items = new List<Item>();

users.Add(new User("Thea", "thea", "thea")); //testanvändare
users.Add(new User("Manuel", "manuel", "manuel")); //testanavändare2
users.Add(new User("Marcus", "marcus", "marcus")); // test3
users.Add(new User("Malin", "malin", "malin")); // test 4

//filhantering: läs in users om filen finns
if (File.Exists("users.txt"))
{
    string[] userLines = File.ReadAllLines("users.txt");
    foreach (string userRow in userLines)
    {
        string[] parts = userRow.Split(','); //Name, email, password
        if (parts.Length == 3)
        {
            string name = parts[0];
            string email = parts[1];
            string password = parts[2];
            users.Add(new User(name, email, password));
        }
    }
}

//filhantering:läsa in items
if (File.Exists("items.txt"))
{
    string[] itemLines = File.ReadAllLines("items.txt");
    foreach (string itemRow in itemLines)
    {
        string[] parts = itemRow.Split(',');
        if (parts.Length == 3)
        {
            string ownerEmail = parts[0];
            string itemName = parts[1];
            string desc = parts[2];

            // hitta ägaren via email
            User owner = null;
            foreach (User user in users)
            {
                if (user.Email == ownerEmail)
                {
                    owner = user;
                    break;
                }
            }
            if (owner != null)
            {
                Item item = new Item(itemName, desc, owner);
                owner.Items.Add(item); //lägg i ägarens invetory
                items.Add(item);
            }
        }
    }
}

//filhantering: läsa in trades
if (File.Exists("trades.txt"))
{
    string[] tradeLines = File.ReadAllLines("trades.txt");
    foreach (string tradeRow in tradeLines)
    {
        if (tradeRow != null && tradeRow.Length > 0)
        {
            string[] parts = tradeRow.Split(',');
            if (parts.Length == 5)
            {
                string senderEmail = parts[0];
                string receiverEmail = parts[1];
                string requestedName = parts[2];
                string offeredNames = parts[3];
                string statusText = parts[4];
                //hitta sender
                User sender = null;
                foreach (User user in users)
                {
                    if (user.Email == senderEmail)
                    {
                        sender = user;
                        break;
                    }
                }
                //hitta receiver
                User receiver = null;
                foreach (User user in users)
                {
                    if (user.Email == receiverEmail)
                    {
                        receiver = user;
                        break;
                    }
                }
                //hitta item hos receiver
                Item requestedItem = null;
                if (receiver != null)
                {
                    foreach (Item it in receiver.Items)
                    {
                        if (it.Name == requestedName)
                        {
                            requestedItem = it;
                            break;
                        }
                    }
                }
                //Erbjudna namn/items = bygg lista av items hos sender
                List<Item> offeredItems = new List<Item>();
                if (sender != null)
                {
                    string[] offeredNameList = offeredNames.Split('|');
                    foreach (string oneName in offeredNameList)
                    {
                        Item found = null;
                        foreach (Item it in sender.Items)
                        {
                            if (it.Name == oneName)
                            {
                                offeredItems.Add(it);
                                break;
                            }
                        }
                    }
                }

                //status
                TradeStatus status = TradeStatus.Pending;
                if (statusText == "Accepted")
                {
                    status = TradeStatus.Accepted;
                }
                else if (statusText == "Denied")
                {
                    status = TradeStatus.Denied;
                }
                //lägg bara till om allt hittades
                bool hasAnyOffered = false;
                foreach (Item it in offeredItems)
                {
                    hasAnyOffered = true;
                    break;
                }
                if (sender != null && receiver != null && requestedItem != null && hasAnyOffered)
                {
                    trades.Add(new Trade(sender, receiver, requestedItem, offeredItems, status));
                }
            }
        }
    }
}

User? active_user = null; //om man sätter null så betyder det att är det ingen användare som är selected = utloggad

bool running = true;
while (running)
{
    if (active_user == null)
    {
        Console.Clear();
        Console.WriteLine("---Welcome to trading system!---"); //// kommentar till mig själv:måste lägga till att registrera account
        Console.WriteLine("1) Log in");
        Console.WriteLine("2) Make an account");

        string pickoption = Console.ReadLine();
        switch (pickoption)
        {
            case "1":
                Console.Write("Email: ");
                string email = Console.ReadLine();
                Console.Clear();

                Console.Write("Password: ");
                string password = Console.ReadLine();
                Console.Clear();

                foreach (User user in users) //kollar igenom alla användare
                {
                    if (user.TryLogin(email, password))
                    {
                        active_user = user;
                        break; //sluta leta om vi hittar rätt
                    }
                }
                if (active_user == null) // om ingen användare hittas
                {
                    Console.WriteLine("Wrong email or password");
                    Console.WriteLine("Press enter to try again or make an account..");
                    Console.ReadLine();
                }
                break;

            case "2":
                Console.Clear();
                Console.WriteLine("---Lets create an account!---");
                Console.WriteLine("Firstname: ");
                string newName = Console.ReadLine();
                Console.WriteLine("Email:  ");
                string newEmail = Console.ReadLine();
                Console.WriteLine("Password:  ");
                string newPassword = Console.ReadLine();
                //lägg till i minnet
                User newUser = new User(newName, newEmail, newPassword);
                users.Add(newUser);

                //Skapar en lista som ska sparas till filen
                List<string> newUserCreated = new List<string>();
                //läser in gamla rader (om filen finns) och kopiera in den i listan
                if (File.Exists("users.txt"))
                {
                    string[] oldLines = File.ReadAllLines("users.txt");
                    foreach (string line in oldLines)
                    {
                        newUserCreated.Add(line);
                    }
                }
                //lägg till den nya användaren sist
                newUserCreated.Add(newName + "," + newEmail + "," + newPassword);
                //skriv hela listan tillbaka till filen
                File.WriteAllLines("users.txt", newUserCreated);

                Console.WriteLine("Account created! Press enter to log in");
                Console.ReadLine();
                break;
        }
    }
    else
    { // kommentar till mig själv: borde man kunna se inventory?
        Console.WriteLine("-----Trading system-----");
        Console.WriteLine("1) Upload item to trade");
        Console.WriteLine("2) Browse items from other users");
        Console.WriteLine("3) Request trade");
        Console.WriteLine("4) Browse trade requests");
        Console.WriteLine("5) Handle trade requests");
        Console.WriteLine("6) Browse completed requests");
        Console.WriteLine("7) Log out");

        string choice = Console.ReadLine();

        switch (choice)
        {
            case "1": //Lägg till item på den inloggade användaren
                Console.Clear();
                Console.WriteLine("---Upload item to trade---");
                Console.Write("Item name:  ");
                string name = Console.ReadLine();
                Console.Write("Description:  ");
                string desc = Console.ReadLine();
                // skapa ett nytt item objekt med ägare = active_user:
                Item newItem = new Item(name, desc, active_user);
                //lägg in item i den inloggade användarens inventory:
                active_user.Items.Add(newItem);
                items.Add(newItem);

                //läs in gamla rader i fil om det finns
                List<string> itemLines = new List<string>();
                if (File.Exists("items.txt"))
                {
                    string[] existingItemRows = File.ReadAllLines("items.txt");
                    foreach (string row in existingItemRows)
                    {
                        itemLines.Add(row);
                    }
                }
                //lägg till den nya item sist
                itemLines.Add(active_user.Email + "," + name + "," + desc);
                File.WriteAllLines("items.txt", itemLines);

                Console.WriteLine("Item added!");
                Console.WriteLine("Press enter to go back to the menu..");
                Console.ReadLine();
                break;

            case "2":

                Console.Clear();
                Console.WriteLine("---Browse items from other users---");
                foreach (User user in users)
                {
                    if (user != active_user) //visa bara andra användare
                    {
                        Console.WriteLine("User " + user.Name + " inventory:");
                        bool found = false; //utgå ifrån att vi inte hittar några items
                        foreach (Item item in user.Items)
                        {
                            Console.WriteLine(item.Name + ": " + item.Description);
                            found = true; //så fort vi hittar ett item ändra till true
                        }
                        if (found == false) //om vi inte hittar några
                        {
                            Console.WriteLine("[no items]");
                        }
                        Console.WriteLine();
                    }
                }
                Console.WriteLine("Press enter to go back to menu");
                Console.ReadLine();
                break;

            case "3":

                Console.Clear();
                Console.WriteLine("---Request a trade---");
                //fråga vem man vill skicka en request till
                Console.Write("Owner name: ");
                string ownerName = Console.ReadLine();

                //leta upp användaren med det namnet som inte är inloggad
                User owner = null;
                foreach (User user in users)
                {
                    if (user != null && user != active_user && user.Name == ownerName)
                    {
                        owner = user;
                        break;
                    }
                }
                if (owner == null) //om ingen användare hittades
                {
                    Console.WriteLine("Owner not found, press enter to go back");
                    Console.ReadLine();
                    break;
                }
                //kolla om owner har några items
                bool hasItems = false;
                foreach (Item item in owner.Items)
                {
                    hasItems = true;
                    break; //om vi hittar minst ett item så stopp
                }
                if (hasItems == false)
                {
                    Console.WriteLine(owner.Name + " has no items. Press enter to go back");
                    Console.ReadLine();
                    break;
                }
                //visa ägarens items
                Console.WriteLine("User " + owner.Name + " inventory:");
                foreach (Item it in owner.Items)
                {
                    Console.WriteLine(it.Name + ": " + it.Description);
                }
                //Välj vilket item du vill ha
                Console.Write("Enter the item you want: ");
                string wantedName = Console.ReadLine();
                string wantedNameLower = "";
                if (wantedName != null)
                {
                    wantedNameLower = wantedName.ToLower();
                }
                Item wantedItem = null;
                foreach (Item it in owner.Items)
                {
                    if (it.Name != null && it.Name.ToLower() == wantedNameLower)
                    {
                        wantedItem = it;
                        break;
                    }
                }
                if (wantedItem == null)
                {
                    Console.WriteLine("Item not found, press enter to go back");
                    Console.ReadLine();
                    break;
                }
                //visa din invenotry med index
                Console.WriteLine("---YOUR ITEMS---");
                Console.WriteLine("Type a number to add, empty line to finish: ");

                bool haveItems = false;
                foreach (Item it in active_user.Items)
                {
                    haveItems = true;
                    break;
                }
                if (haveItems == false)
                {
                    Console.WriteLine("You have no items to offer. Press enter to go back");
                    Console.ReadLine();
                    break;
                }
                //skriva ut dina items med index
                int showIdx = 1;
                foreach (Item it in active_user.Items)
                {
                    Console.WriteLine(showIdx + ") " + it.Name);
                    showIdx = showIdx + 1;
                }
                //läs valen ett i taget. Tom rad = klart
                List<Item> offeredItems = new List<Item>();
                while (true)
                {
                    Console.WriteLine("Pick number (enter to finish): ");
                    string pick = Console.ReadLine();

                    if (pick == null || pick == "")
                    {
                        break; //tom rad = klart
                    }
                    int i = 0;
                    bool pickedNumber = int.TryParse(pick, out i);

                    if (pickedNumber)
                    {
                        //leta upp item med index i
                        Item selectedItem = null;
                        int idxCount = 1;
                        foreach (Item it in active_user.Items)
                        {
                            if (idxCount == i)
                            {
                                selectedItem = it;
                                break;
                            }
                            idxCount = idxCount + 1;
                        }
                        if (selectedItem == null)
                        {
                            Console.WriteLine("No item at that number.");
                        }
                        else
                        {
                            //undvika dubbla
                            bool doublette = false;
                            foreach (Item itof in offeredItems)
                            {
                                if (itof == selectedItem)
                                {
                                    doublette = true;
                                    break;
                                }
                            }
                            if (doublette)
                            {
                                Console.WriteLine("That item is already selected..");
                            }
                            else
                            {
                                offeredItems.Add(selectedItem);
                                Console.WriteLine("Added:  " + selectedItem.Name);
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine("Pleade enter a valid number or press enter to finish");
                    }
                }
                Trade trade = new Trade(
                    active_user,
                    owner,
                    wantedItem,
                    offeredItems,
                    TradeStatus.Pending
                );
                trades.Add(trade);
                //spara alla offered names. om listan är tom blir det en tom sträng
                string offeredNamesStr = "";
                bool firstName = true;
                foreach (Item itof in trade.OfferedItem)
                {
                    if (firstName)
                    {
                        offeredNamesStr = itof.Name;
                        firstName = false;
                    }
                    else
                    {
                        offeredNamesStr = offeredNamesStr + "|" + itof.Name;
                    }
                }
                string newTradeRow =
                    trade.Sender.Email
                    + ","
                    + trade.Receiver.Email
                    + ","
                    + trade.RequestedItem.Name
                    + ","
                    + offeredNamesStr
                    + ","
                    + trade.Status.ToString();

                File.AppendAllText("trades.txt", newTradeRow + "\n");

                Console.WriteLine("Trade request sent...");
                Console.WriteLine("Status: [pending..]");
                Console.WriteLine("Press enter to go back to menu");
                Console.ReadLine();
                break;

            case "4":
                Console.Clear();
                Console.WriteLine("--Incoming trade requests---");

                bool incomingTr = false;
                foreach (Trade t in trades) // loopa igenom alla trades
                {
                    if (
                        t != null
                        && t.Receiver != null
                        && t.Receiver.Email == active_user.Email
                        && t.Status == TradeStatus.Pending
                    )
                    { //alla erbjudna items
                        string offeredList = "";
                        bool first = true;
                        foreach (Item itof in t.OfferedItem)
                        {
                            if (first)
                            {
                                offeredList = itof.Name;
                                first = false;
                            }
                            else
                            {
                                offeredList = offeredList + ", " + itof.Name;
                            }
                        }
                        Console.WriteLine(t.Sender.Name + " wants your " + t.RequestedItem.Name);
                        if (offeredList != "")
                        {
                            Console.WriteLine("They offer: " + offeredList);
                        }
                        Console.WriteLine("Status: " + t.Status);
                        Console.WriteLine(); // tom rad för kosmetisk anledning hehe
                        incomingTr = true;
                    }
                }
                if (incomingTr == false)
                {
                    Console.WriteLine("[no requests to show]");
                }
                Console.WriteLine("\n---Your outgoing requests---");
                //outgoing spm du har skickat
                bool outgoingTr = false;
                foreach (Trade t in trades)
                {
                    if (t != null && t.Sender != null && t.Sender.Email == active_user.Email)
                    {
                        string offeredList = "";
                        bool first = true;
                        foreach (Item itof in t.OfferedItem)
                        {
                            if (first)
                            {
                                offeredList = itof.Name;
                                first = false;
                            }
                            else
                            {
                                offeredList = offeredList + ", " + itof.Name;
                            }
                        }

                        Console.WriteLine("Sent to " + t.Receiver.Name);
                        Console.WriteLine("Wants: " + t.RequestedItem.Name);

                        if (offeredList != "")
                        {
                            Console.WriteLine("You offered: " + offeredList);
                        }
                        Console.WriteLine("Status: " + t.Status);
                        Console.WriteLine();
                        outgoingTr = true;
                    }
                }
                if (outgoingTr == false)
                {
                    Console.WriteLine("[no outgoing requests]");
                }

                Console.WriteLine("\nPress enter to go back to menu");
                Console.ReadLine();
                break;

            case "5":
            {
                Console.Clear();
                Console.WriteLine("--Handle trade requests--");
                //flagga för att kolla om vi hittade några trades
                bool foundAny = false;
                //loopa igenom alla trades i systemet
                foreach (Trade t in trades)
                {
                    //kolla om traden är till den inloggade användaren och fortfarande pending
                    if (
                        t != null
                        && t.Receiver != null
                        && t.Receiver.Email == active_user.Email
                        && t.Status == TradeStatus.Pending
                    )
                    {
                        foundAny = true; //vi gittade minst en pending
                        //visa vilken trade det gäller alltså vem, vad de vill ha och erbjuder
                        string offeredList = "";
                        bool printFirst = true;
                        foreach (Item itof in t.OfferedItem)
                        {
                            if (printFirst)
                            {
                                offeredList = itof.Name;
                                printFirst = false;
                            }
                            else
                            {
                                offeredList = offeredList + ", " + itof.Name;
                            }
                        }
                        // skriv vad trade gäller

                        Console.WriteLine("From: " + t.Sender.Name);
                        Console.WriteLine("Wants: " + t.RequestedItem.Name);
                        if (offeredList != "")
                        {
                            Console.WriteLine("They offer: " + offeredList);
                        }
                        else
                        {
                            Console.WriteLine("They offer: [nothing]");
                        }
                        //fråga användaren vad den vill glra
                        Console.WriteLine("Accept (A) or Deny (D)?");
                        string decision = Console.ReadLine();
                        if (decision != null)
                        {
                            decision = decision.ToLower();
                        }
                        else
                        {
                            decision = "";
                        }
                        if (decision == "a")
                        {
                            //om man accepterar = byt ägare på båda items
                            //ta bort requested item från receiver
                            t.Receiver.Items.Remove(t.RequestedItem);
                            //ta bort offered item från sender (den andra användare)
                            t.Sender.Items.Add(t.RequestedItem);

                            foreach (Item itof in t.OfferedItem)
                            {
                                t.Sender.Items.Remove(itof);
                                t.Receiver.Items.Add(itof);
                            }
                            t.Status = TradeStatus.Accepted;
                            Console.WriteLine("Trade [accepted] and items swapped");
                        }
                        else if (decision == "d")
                        {
                            t.Status = TradeStatus.Denied;
                            Console.WriteLine("Trade denied.");
                        }
                        else
                        {
                            //om man skriver någor annat än A eller D
                            Console.WriteLine("No action taken..");
                        }
                        List<string> tradeLines = new List<string>();
                        foreach (Trade tr in trades)
                        {
                            string offeredNames = "";
                            bool first = true;
                            foreach (Item itof in tr.OfferedItem)
                            {
                                if (first)
                                {
                                    offeredNames = itof.Name;
                                    first = false;
                                }
                                else
                                {
                                    offeredNames = offeredNames + "|" + itof.Name;
                                }
                            }
                            string row =
                                tr.Sender.Email
                                + ","
                                + tr.Receiver.Email
                                + ","
                                + tr.RequestedItem.Name
                                + ","
                                + offeredNames
                                + ","
                                + tr.Status.ToString();
                            tradeLines.Add(row);
                        }
                        File.WriteAllLines("trades.txt", tradeLines);
                        Console.WriteLine();
                    }
                }
                if (foundAny == false)
                {
                    Console.WriteLine("You have no pending trade requests.");
                }
                Console.WriteLine("Press enter to go back");
                Console.ReadLine();
                break;

                // case "6":
            }
        }
    }
}
