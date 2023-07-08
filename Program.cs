int cashMoney = 0;
int cardMoney = 0;
bool adminMod = false;
int googsLimit = 10;
Dictionary<Good, int> Goods = new Dictionary<Good, int>()
{
    {new Good(1,"Picnic",100),5},
    {new Good(2,"Alpen gold",100),5},
    {new Good(3,"M&M",100),5},
    {new Good(4,"Milka",100),5},
    {new Good(5,"Добрый кола",100),5},
    {new Good(6,"Квас",100),5},
    {new Good(7,"Пельмени",100),5},
    {new Good(8,"Burn",100),5},
    {new Good(9,"Drive",100),5},
    {new Good(10,"Redbull",100),5},
};
void AddGood()
{
    if (googsLimit <= Goods.Count)
    {
        ErrorMessage("Нет мест для новых товаров");
        return;
    }
    int id;

    Console.WriteLine("Введите id товара");
    while (!int.TryParse(Console.ReadLine().Trim(), out id) || !(id > 0)) ErrorMessage("Введена не правильная сумма");
    foreach (Good good in Goods.Keys)
    {
        if (good.isThisId(id))
        {
            ErrorMessage("id уже существует");
            return;
        }
    }
    Console.WriteLine("Введите имя товара");
    string name = Console.ReadLine().Trim();
    Console.WriteLine("Введите цену");
    int price;
    while (!int.TryParse(Console.ReadLine().Trim(), out price) || !(price > 0)) ErrorMessage("Введена не правильная сумма");
    Console.WriteLine("Введите количество");
    int count;
    while (!int.TryParse(Console.ReadLine().Trim(), out count) || !(count > 0)) ErrorMessage("Введена не правильное число");
    Goods.Add(new Good(id, name, price), count);
    Console.WriteLine("Товар добавлен");

}
void ErrorMessage(string message)
{
    Console.BackgroundColor = ConsoleColor.Red;
    Console.WriteLine(message);
    Console.BackgroundColor = ConsoleColor.Black;
}
void AddMoney()
{
    while (true)
    {
        Console.WriteLine("Введите тип оплаты, Налиный или Карта: Cash/Card");
        string payType = Console.ReadLine().Trim().ToLower();
        int value=0;
        if (payType == "cash")
        {
            foreach (int i in new int[4] { 10, 5, 2, 1 })
            {
                int input;
                Console.WriteLine($"Введите сколько введено монет наминапом {i}");
                while (!int.TryParse(Console.ReadLine().Trim(), out input) || !(input>=0)) ErrorMessage("Введена не правильная сумма");
                value += input*i;
            }
            if (value <= 0) break;
            AddMoneyToDeposit(value, ref (payType == "cash" ? ref cashMoney : ref cardMoney));
            break;
        }
        else if (payType == "card")
        {
            Console.WriteLine("Введите сумму к пополнению");
            while (!int.TryParse(Console.ReadLine().Trim(), out value)) ErrorMessage("Введена не правильная сумма");
            if (value <= 0) break;
            AddMoneyToDeposit(value, ref cardMoney);
            break;
        }
        else ErrorMessage("Не верный тип платежа");
    }
}

void AddMoneyToDeposit(int value, ref int deposit)
{
    deposit += value;
    Console.WriteLine($"Общий депозит: {cashMoney + cardMoney}р.");
    Console.WriteLine($"Наличные: {cashMoney}р.");
    Console.WriteLine($"Картой: {cardMoney}р.");
}
void GetChange()
{
    Console.WriteLine("Возврат средств");
    if (cardMoney > 0) Console.WriteLine("Средства оплаченые картой будут возвращены");
    cardMoney = 0;
    if (cashMoney <= 0) return;
    Console.WriteLine($"Возвращено монетами: \n10 - {cashMoney / 10}");
    cashMoney -= cashMoney / 10 * 10;
    Console.WriteLine($"5 - {cashMoney / 5}");
    cashMoney -= cashMoney / 5 * 5;
    Console.WriteLine($"2 - {cashMoney / 2}");
    cashMoney -= cashMoney / 2 * 2;
    Console.WriteLine($"1 - {cashMoney}");
    cashMoney = 0;
}
void ShowGoods()
{
    Console.WriteLine("Все доступные товары");
    foreach (Good good in Goods.Keys) good.ShowInfo();
}
void BuyGood()
{
    while (true)
    {
        Console.WriteLine("Введите id товара, желаемого к покупке");
        int id;
        while (!int.TryParse(Console.ReadLine().Trim(), out id)) ErrorMessage("Введите число");
        bool goodIn = false;
        Good selectedGood = null;
        foreach (Good good in Goods.Keys)
        {
            goodIn = good.isThisId(id);
            if (goodIn)
            {
                selectedGood = good;
                break;
            }
        }
        if (!goodIn)
        {
            ErrorMessage("Такого товара не существует");
            continue;
        }
        Console.WriteLine($"Веддите сколько товаров хотите преобрести. Максимум {Goods[selectedGood]}");
        int count;
        while (!int.TryParse(Console.ReadLine().Trim(), out count) || !(count <= Goods[selectedGood]) || !(count > 0)) ErrorMessage($"Введите число товаров не превышающее {Goods[selectedGood]}");
        if (cashMoney + cardMoney >= count * selectedGood.Price)
        {
            int payCheck = count * selectedGood.Price;
            if (cashMoney > payCheck)
            {
                cashMoney -= payCheck;
                payCheck = 0;
            }
            else
            {
                payCheck -= cashMoney;
                cashMoney = 0;
                cardMoney -= payCheck;

            }
            Console.WriteLine("Товары преобретины");
            Console.WriteLine($"Общий депозит: {cashMoney + cardMoney}р.");
            Console.WriteLine($"Наличные: {cashMoney}р.");
            Console.WriteLine($"Картой: {cardMoney}р.");
            Goods[selectedGood] -= count;
            if (Goods[selectedGood] <= 0) Goods.Remove(selectedGood);
            break;
        }
        else
        {
            Console.WriteLine("Недостаточно средств");
            break;
        }
    }

}
void ChangeToAdmin()
{
    Console.WriteLine("Введите пароль");
    adminMod = Console.ReadLine().Trim() == "admin";
}

Action AddMoneyAction = AddMoney;
Action GetChangeAction = GetChange;
Action ShowGoodsAction = ShowGoods;
Action BuyGoodAction = BuyGood;
Action AddGoodAction = AddGood;
Action ChangeToAdminAction = ChangeToAdmin;

Dictionary<string, Action> userComands = new Dictionary<string, Action>()
{
    {"addmoney",AddMoneyAction},
    {"getchange",GetChangeAction},
    {"showgoods", ShowGoodsAction},
    {"buygood",BuyGoodAction},
    {"changetoadmin",ChangeToAdminAction}

};
Dictionary<string, Action> adminComands = new Dictionary<string, Action>()
{
    {"addgood",AddGoodAction}
};

void InputAnalyse(string input)
{
    foreach (string commandName in userComands.Keys) if (commandName == input) userComands[commandName].Invoke();
    if (!adminMod) return;
    foreach (string commandName in adminComands.Keys) if (commandName == input) adminComands[commandName].Invoke();

}

while (true)
{
    Console.WriteLine("Введите желаемое действие");
    InputAnalyse(Console.ReadLine().Trim().ToLower());
}
public class Good
{
    public int Id { get; private set; }
    public string Name { get; private set; }
    public int Price { get; private set; }
    public Good(int id, string name, int price)
    {
        Id = id;
        Name = name;
        Price = price;
    }
    public void ShowInfo()
    {
        Console.WriteLine($"id: {Id} Имя: {Name} Цена: {Price}р.");
    }
    public bool isThisId(int id)
    {
        return id == Id;
    }
}