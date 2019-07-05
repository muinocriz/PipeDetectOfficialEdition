using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvvmLight4.Service
{
    interface IFileDialogService
    {
        string OpenFileDialog(string srcFilter = "");
        string OpenFolderBrowserDialog();
        string OpenSaveFileDialog();
    }
}
