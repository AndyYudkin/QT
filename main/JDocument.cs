using System.Collections.Generic;

namespace main
{
    public class JProduct
    {
        public string id { get; set; }
        public int num { get; set; }
    }

    public class JСomponent
    {
        public string id { get; set; }
        public int num { get; set; }
    }

    public class JRecipe
    {
        public string name { get; set; }
        public IList<JСomponent> components { get; set; }

        public JProduct product { get; set; }
    }

    public class JAbility
    {
        public string name { get; set; }
        public int duration { get; set; }
    }

    public class JProject
    {
        public string name { get; set; }
        public IList<JAbility> abilities { get; set; }
    }
    
    public class JBuilding
    {
        public string name { get; set; }
        public string project { get; set; }
    }
    
    public class JDocument
    {
        public IList<JProduct> products { get; set; }
        public IList<JRecipe> recipes { get; set; }
        public IList<JProject> projects { get; set; }
        public IList<JBuilding> buildings { get; set; }
    }
}