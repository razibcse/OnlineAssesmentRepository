using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Frontend.Models
{
    public class PostCreateViewModel
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public bool IsChangeable { get; set; }
    }
}
