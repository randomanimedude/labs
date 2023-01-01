using System.Collections;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace MyProject;
class Program
{
    private const int NUMBER_OF_ITEMS = 40;
    private static Item[] items = new Item[NUMBER_OF_ITEMS];
    private const double MIN_ITEM_PRICE = 3.0;
    private const double MAX_ITEM_PRICE = 7.0;
    private const double MIN_ITEM_WEIGHT = 1.0;
    private const double MAX_ITEM_WEIGHT = 5.0;
    private const double MAX_WEIGHT = 20.0;
    private const int NUMBER_OF_BAGS = 10;
    private const int MAX_BAGS = 20;
    private const int MIN_BAGS = 3;
    private const int MAX_ITERATIONS = 1000000000;
    private const double MUTATION_CHANCE = 0.15;


    private struct Item
    {
        public double price;
        public double weight;

        public Item(double price, double weight)
        {
            this.price = price; 
            this.weight = weight;
        }
    }

    public class BagsComparer : IComparer<bool[]>
    {
        public int Compare(bool[] x, bool[] y)
        {
            return f(x).CompareTo(f(y));
        }
    }

    static void Main(string[] args)
    {
        var rand = new Random();
        Console.WriteLine("Items: ");
        for(int i = 0; i < NUMBER_OF_ITEMS; ++i)
        {
            var price = MIN_ITEM_PRICE + (rand.NextDouble() * (MIN_ITEM_PRICE + MAX_ITEM_PRICE));
            var weight = MIN_ITEM_WEIGHT + (rand.NextDouble() * (MIN_ITEM_WEIGHT + MAX_ITEM_WEIGHT));
            price = Math.Round(price, 2);
            weight = Math.Round(weight, 2);
            items[i] = new Item(price, weight);

            Console.WriteLine($"{i + 1})\tPrice: {price}\n\tWeight: {weight}\n");
        }
        List<bool[]> bags = new List<bool[]>();
        for(int i = 0; i < NUMBER_OF_BAGS; ++i)
        {
            bags.Add(GenerateBag());
        }

        uint iteration = 0;

        bags.Sort(new BagsComparer());

        bool[] bestBag = new bool[NUMBER_OF_ITEMS];
        for(int i = 0; i < NUMBER_OF_ITEMS; ++i)
        {
            bestBag[i] = bags[bags.Count - 1][i];
        }
        Console.WriteLine($"Новий найкращий рюкзак з вартiстю {f(bestBag)} на iтерацiї {iteration}");
        for(int i = 0; i < bestBag.Length; ++i)
        {
            Console.Write(bestBag[i] ? "1  " : "0  ");
        }
        Console.WriteLine();

        while (iteration < MAX_ITERATIONS)
        {
            ++iteration;

            //int half = bags.Count / 2;

            //// Видалення поганих рюкзаків
            //for (int i = 0; i < half; ++i)
            //{
            //    bags.Remove(bags[0]);
            //}

            while (bags.Count > MAX_BAGS)
            {
                if (bags.Count <= MIN_BAGS)
                {
                    break;
                }
                bags.Remove(bags[0]);
            }

            // Мутація старих рюкзаків з ймовірністю MUTATION_CHANCE
            var mutatedBags = MutateBags(bags);
            // Перехресне схрещування сусідніх рюкзаків
            var crossedBags = Cross(bags);

            bags.AddRange(mutatedBags);
            bags.AddRange(crossedBags);
            bags.Sort(new BagsComparer());

            if (f(bags[bags.Count - 1]) > f(bestBag)){
                //Console.WriteLine($"old - {f(bestBag)}   new - {f(bags[bags.Count - 1])}");
                bestBag = new bool[NUMBER_OF_ITEMS];
                for (int i = 0; i < NUMBER_OF_ITEMS; ++i)
                {
                    bestBag[i] = bags[bags.Count - 1][i];
                }
                Console.WriteLine($"\nНовий найкращий рюкзак з вартiстю {f(bestBag)} на iтерацiї {iteration}");
                for (int i = 0; i < bestBag.Length; ++i)
                {
                    Console.Write(bestBag[i] ? "1  " : "0  ");
                }
                Console.WriteLine();
            }
        }

        Console.WriteLine($"\nОбчислення завершено, за {MAX_ITERATIONS} iтерацiй знайдено рюкзак з вартiстю {f(bestBag)}: ");
        for (int i = 0; i < bestBag.Length; ++i)
        {
            Console.Write(bestBag[i] ? "1  " : "0  ");
        }
        Console.WriteLine();

    }

    // Функцiя оцiнки
    private static double f(bool[] selection)
    {
        double sumPrice = 0;
        double sumWeight = 0;
        for(int i = 0; i < selection.Length; ++i)
        {
            if (selection[i])
            {
                sumPrice += items[i].price;
                sumWeight += items[i].weight;
            }
        }

        if (sumWeight > MAX_WEIGHT)
        {
            sumPrice = 0.0;
        }

        return Math.Round(sumPrice, 2);
    }

    private static bool[] GenerateBag()
    {
        var rand = new Random();
        bool[] itemsInBag = new bool[NUMBER_OF_ITEMS];
        double sumWeight = 0;
        for (int i = 0; i < NUMBER_OF_ITEMS; ++i)
        {
            itemsInBag[i] = rand.Next() % 2 == 0;
            if (itemsInBag[i])
            {
                sumWeight += items[i].weight;
            }
        }

        while(sumWeight > MAX_WEIGHT)
        {
            var index = rand.Next() % (NUMBER_OF_ITEMS - 1);

            if (itemsInBag[index])
            {
                itemsInBag[index] = false;
                sumWeight -= items[index].weight;
            }
        }

        return itemsInBag;
    } 

    private static List<bool[]> MutateBags(List<bool[]> oldBags)
    {
        List<bool[]> newBags = new List<bool[]>();
        var rand = new Random();
        foreach (bool[] bag in oldBags)
        {
            if(rand.NextDouble() <= MUTATION_CHANCE)
            {
                bool[] newBag = new bool[NUMBER_OF_ITEMS];
                for (int i = 0; i < NUMBER_OF_ITEMS; ++i)
                {
                    newBag[i] = bag[i];
                }
                int index = rand.Next() % (bag.Length - 1);
                newBag[index] = !newBag[index];
                newBags.Add(newBag);
            }
        }

        return newBags;
    }

    private static List<bool[]> Cross(List<bool[]> oldBags)
    {
        List<bool[]> newBags = new List<bool[]>();
        var rand = new Random();
        for(int i = 0; i < oldBags.Count - 1; ++i)
        {
            int crossingPoint = rand.Next() % (NUMBER_OF_ITEMS - 1);
            bool[] newBag = new bool[NUMBER_OF_ITEMS];
            for (int j = 0; j < NUMBER_OF_ITEMS; ++j)
            {
                if(j < crossingPoint)
                {
                    newBag[j] = oldBags[i][j];
                }
                else
                {
                    newBag[j] = oldBags[i + 1][j];
                }
            }
            newBags.Add(newBag);
        }

        return newBags;
    }
}