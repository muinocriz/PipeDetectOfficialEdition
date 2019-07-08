using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvvmLight4.Common
{
    public class FrameTime : ObservableCollection<int>
    {
        public FrameTime()
        {
            Add(10);
            Add(15);
            Add(20);
        }
    }
}
