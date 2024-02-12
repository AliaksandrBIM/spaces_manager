using System.IO;
using System.Windows;
using MessageBox = System.Windows.MessageBox;
using Res = Eneca.SpacesManager.Resources.Resources;
using System.Windows.Forms;
using SaveFileDialog = System.Windows.Forms.SaveFileDialog;
using Autodesk.Revit.DB;
using Eneca.SpacesManager.Core;
using EnecaUI.Controls.Eneca.UserControl;
using Document = Autodesk.Revit.DB.Document;

namespace Eneca.SpacesManager.Model
{
    public class ParameterCreator
    {
        private readonly Document _doc = RevitApi.Document;

        public bool CheckNeedParameter()
        {
            var parameterBindings = _doc.ParameterBindings;
            var forwardIterator = parameterBindings.ForwardIterator();

            List<string> needParameters = new();
            while (forwardIterator.MoveNext())
            {
                if (Res.Area_ParName == forwardIterator.Key.Name)
                {
                    needParameters .Add(forwardIterator.Key.Name);
                }
            }

            if (needParameters.Count > 0) return true;
            else return false;
        }

        public void SetExternalDefifnition(string filePath)
        {
            FileInfo fileInfo = new FileInfo(filePath);
            _doc.Application.SharedParametersFilename = fileInfo.FullName;
            var file = _doc.Application.OpenSharedParameterFile();
            var group = file.Groups.Create("EcecaParameters");

            #if  (R21)
            var option = new ExternalDefinitionCreationOptions(Res.Area_ParName, ParameterType.Area);
            #else
            var option = new ExternalDefinitionCreationOptions(Res.Area_ParName, SpecTypeId.Area);
            #endif
            
            group.Definitions.Create(option);
            var category = Category.GetCategory(_doc, BuiltInCategory.OST_MEPSpaces);
            var categoryset = _doc.Application.Create.NewCategorySet();
            categoryset.Insert(category);
            var definition = file.Groups.get_Item("EcecaParameters").Definitions.get_Item(Res.Area_ParName);
            var newTypeBinding = _doc.Application.Create.NewInstanceBinding(categoryset);
            using Transaction t = new Transaction(_doc, "Add need parameters");
            t.Start();
            _doc.ParameterBindings.Insert(definition, newTypeBinding);
            t.Commit();
        }

        public bool ConfigureSharedParameters()
        {
            string filePath = @"R:\2 - Библиотека\Revit 2021\1 - Общие ресурсы\Файлы общих параметров (ФОП)\ФОП [ENECA].txt";
            FileInfo fileInfo = new FileInfo(filePath);

            if (fileInfo.Exists)
            {
                _doc.Application.SharedParametersFilename = fileInfo.FullName;
                var category = Category.GetCategory(_doc, BuiltInCategory.OST_MEPSpaces);
                var categoryset = _doc.Application.Create.NewCategorySet();
                categoryset.Insert(category);
                var file = _doc.Application.OpenSharedParameterFile();
                var definition = file.Groups.get_Item("ADSK 10 Размеры").Definitions.get_Item("ADSK_Размер_Площадь");
                var newTypeBinding = _doc.Application.Create.NewInstanceBinding(categoryset);
                using Transaction t = new Transaction(_doc, "Add need parameters");
                t.Start();
                _doc.ParameterBindings.Insert(definition, newTypeBinding);
                t.Commit();
                return true;
            }
            else
            {
                return false;
            }
        }

        public string CreateSharedFile()
        {
            var addParamsDialogREsult = MessageBox.Show(Res.MSG_SharedParamsNotFound, Res.MSG_Title, MessageBoxButton.YesNo);
            if (addParamsDialogREsult != MessageBoxResult.Yes)
            {
                EnecaMessageBox.Show(Res.MSG_ErrorCreateSharedPar, Res.MSG_TitleError);
                return null;
            }
            else
            {
                SaveFileDialog dialog = new SaveFileDialog();
                // ReSharper disable once LocalizableElement
                dialog.Filter = ".txt files (*.txt)|*.txt";
                dialog.Title = Res.MSG_SelectPath;
                dialog.DefaultExt = ".txt";
                dialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                dialog.CheckPathExists = true;
                var result = dialog.ShowDialog();

                if (result == DialogResult.OK)
                    try
                    {
                        //Create new sharedParametersFile
                        FileStream fileStream = File.Create(dialog.FileName);
                        fileStream.Close();
                        if (File.Exists(dialog.FileName)) return dialog.FileName;
                    }
                    catch (Exception ex)
                    {
                        //Microsoft.AppCenter.Crashes.Crashes.TrackError(ex);
                        MessageBox.Show(string.Format(Res.MSG_UnableToCreateSharFile, ex.Message));
                        return null;
                    }
                return null;
            }
        }

    }
}
