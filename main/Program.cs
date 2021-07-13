using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;


namespace main
{
    internal class Program
    {
        private static JDocument GetInputDocument()
        {
            const string jsonFilePath = @"../../input.json";
            return JsonSerializer.Deserialize<JDocument>(File.ReadAllBytes(jsonFilePath));
        }

        private static void ParseDocument(JDocument inputDoc, Dictionary<string, Product> productDictionary,
            IList<Factory> factoryList)
        {
            foreach (var product in inputDoc.products)
            {
                Product existProduct;
                if (productDictionary.TryGetValue(product.id, out existProduct))
                {
                    existProduct.Quantity += product.num;
                }
                else
                {
                    productDictionary.Add(product.id, new Product(product));
                }
            }

            foreach (var building in inputDoc.buildings)
            {
                foreach (var project in inputDoc.projects)
                {
                    if (building.project == project.name)
                    {
                        factoryList.Add(new Factory(building, project.abilities, inputDoc.recipes));
                        break;
                    }
                }
            }
        }

        private static void WriteOutLog(int tick,  Dictionary<string, Product> productDictionary)
        {
            Console.WriteLine($"totalTime: {tick}");
            Console.WriteLine($"products: ");
            foreach (var product in productDictionary.Values)
            {
                if (product.IsComposite)
                {
                    Console.Write($"{product.Id} = {product.Quantity}; ");
                }
            }
        }

        private static int UpdateProducingFactoryList(int activeFactoryCount, IList<Factory> factoryList, int tick,  Dictionary<string, Product> productDictionary)
        {
            foreach (var factory in factoryList)
            {
                if (factory.IsProducing())
                {
                    var isFinished = factory.IsProductProduced(tick);
                    if (isFinished)
                    {
                        factory.PutProductInDictionary(productDictionary);
                        --activeFactoryCount;
                    }
                }
            }

            return activeFactoryCount;
        }

        private static int TryStartNewProducing(int activeFactoryCount, IList<Factory> factoryList, int tick,  Dictionary<string, Product> productDictionary)
        {
            int i = 0;
            int length = factoryList.Count;
            while (i < length)
            {
                var factory = factoryList[i];
                var isStarted = false;
                if (!factory.IsProducing())
                {
                    isStarted = factory.TryStartProducingProduct(productDictionary, tick);
                }
                if (isStarted)
                {
                    factoryList.RemoveAt(i);
                    factoryList.Add(factory);
                    --length;
                    ++activeFactoryCount;
                }
                else
                {
                    ++i;
                }
            }

            return activeFactoryCount;
        }

        public static void Main(string[] args)
        {
            Dictionary<string, Product> productDictionary = new Dictionary<string, Product>();
            IList<Factory> factoryList = new List<Factory>();
            var inputDoc = GetInputDocument();
            ParseDocument(inputDoc, productDictionary, factoryList);

            int tick = 0;
            Console.WriteLine("productionStartLog:");
            int activeFactoryCount = 0;
            while (true)
            {
                activeFactoryCount =
                    UpdateProducingFactoryList(activeFactoryCount, factoryList, tick, productDictionary);
               
                activeFactoryCount =
                    TryStartNewProducing(activeFactoryCount, factoryList, tick, productDictionary);

                if (activeFactoryCount > 0)
                {
                    ++tick;
                }
                else
                {
                    WriteOutLog(tick, productDictionary);
                    break;
                }
            }
        }
    }
}