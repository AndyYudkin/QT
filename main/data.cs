using System;
using System.Collections.Generic;

namespace main
{
    public class Product
    {
        public string Id { get; set; }
        public int Quantity { get; set; }
        public bool IsComposite { get; set; }

        public Product(JProduct product, bool isComposite = false)
        {
            Id = product.id;
            Quantity = product.num;
            IsComposite = isComposite;
        }
        
        public Product(Product product)
        {
            Id = product.Id;
            Quantity = product.Quantity;
            IsComposite = product.IsComposite;
        }
    }

    public class Recipe
    {
        public string Id { get; set; }
        public int Duration { get; set; }
        public IList<JСomponent> ComponentList { get; set; }
        public Product CompositeProduct { get; set; }

        public Recipe(string id, int duration, IList<JСomponent> componentList, Product compositeProduct)
        {
            Id = id;
            Duration = duration;
            ComponentList = componentList;
            CompositeProduct = compositeProduct;
        }
    }


    public class Factory
    {
        private string Id { get; set; }
        
        private int TickOfFinishProducingProduct { get; set; }

        private IList<Recipe> RecipeList { get; set; }
        
        private Recipe ActiveRecipe { get; set; }

        public bool IsProducing()
        {
            return ActiveRecipe != null;
        }
        
        public bool IsProductProduced(int tick)
        {
            return TickOfFinishProducingProduct == tick;
        }

        public void PutProductInDictionary(Dictionary<string, Product> productDictionary)
        {

            Product product;
            if (productDictionary.TryGetValue(ActiveRecipe.CompositeProduct.Id, out product))
            {
                product.Quantity += ActiveRecipe.CompositeProduct.Quantity;
            }
            else
            {
                productDictionary.Add(ActiveRecipe.CompositeProduct.Id, new Product(ActiveRecipe.CompositeProduct));
            }

            ActiveRecipe = null;
        }

        private bool CheckAvailableComponent(Recipe recipe, Dictionary<string, Product> productDictionary)
        {
            bool result = true;
            foreach (var component in recipe.ComponentList)
            {
                Product product;
                if (!productDictionary.TryGetValue(component.id, out product) ||
                    product.Quantity < component.num)
                {
                    result = false;
                    break;
                }
            }
            return result;
        }

        public bool TryStartProducingProduct(Dictionary<string, Product> productDictionary, int tick)
        {
            bool result = false;
            foreach (var recipe in RecipeList)
            {
                if (ActiveRecipe == null && CheckAvailableComponent(recipe, productDictionary))
                {
                    foreach (var component in recipe.ComponentList)
                    {
                        Product product;
                        if (productDictionary.TryGetValue(component.id, out product))
                        {
                            product.Quantity -= component.num;
                        }

                    }

                    TickOfFinishProducingProduct = tick + recipe.Duration;
                    ActiveRecipe = recipe;
                    Console.WriteLine($"time: {tick}, factory: {Id}, recipe: {recipe.Id}");
                    result = true;
                }

            }


            return result;
        }

        public Factory(JBuilding building, IList<JAbility> abilityList, IList<JRecipe> recipeList)
        {
            Id = building.name;
            RecipeList = new List<Recipe>();
            ActiveRecipe = null;
            foreach (var ability in abilityList)
            {
                foreach (var recipe in recipeList)
                {
                    if (ability.name == recipe.name)
                    {
                        RecipeList.Add(new Recipe(ability.name, ability.duration, recipe.components,
                            new Product(recipe.product, true)));
                        break;
                    }
                }
            }
        }
    }
}