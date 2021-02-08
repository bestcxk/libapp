using Mijin.Library.App.Model.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mijin.Library.App.Driver.PosPrint
{
    interface IPosPrint
    {
        MessageModel<bool> Init();

        MessageModel<bool> Print(PrintInfo print);
    }
}
