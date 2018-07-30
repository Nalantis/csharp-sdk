using System.Collections.ObjectModel;
using System.Collections.Generic;

namespace Nalantis.Models
{
    public class Standards
    {
      public static readonly IList<string> Categories = new ReadOnlyCollection<string>
            (new List<string>
            {
                "Experience",
                "Language",
                "About_Me",
                "Education",
                "Competences"
            });    
    }
}