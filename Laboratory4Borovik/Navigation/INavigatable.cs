using Laboratory4Borovik.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Laboratory4Borovik.Navigation
{
    internal interface INavigatable
    {
        NavigationTypes ViewType
        {
            get;
        }
    }
}
