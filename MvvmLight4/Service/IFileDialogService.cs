namespace MvvmLight4.Service
{
    interface IFileDialogService
    {
        string OpenFileDialog(string srcFilter = "");
        string OpenFolderBrowserDialog();
        string OpenSaveFileDialog();
    }
}
