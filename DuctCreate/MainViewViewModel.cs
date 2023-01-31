using Autodesk.Revit.DB.Mechanical;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Selection;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Commands;
using RevitAPITrainingLibrary;

namespace DuctCreate
{
    public class MainViewViewModel
    {
        private ExternalCommandData _commandData;
        private DuctType selectedDuctType;

        public List<DuctType> DuctTypes { get; } = new List<DuctType>();
        public object Levels { get; } = new List<Level>();
        public DelegateCommand SaveCommand { get; }
        public double DuctHeight { get; set; }            //отметка оси трубы
        public DuctType SelectedDuctType { get => selectedDuctType; set => selectedDuctType = value; }
        public MEPSystemType SelectedSystemType { get; set; } //тип системы
        public Level SelectedLevel { get; set; }
        public List<XYZ> Points { get; } = new List<XYZ>();

        public MainViewViewModel(ExternalCommandData commandData)
        {
            _commandData = commandData;
            DuctTypes = DuctsUtils.GetDuctTypes(commandData);
            Levels = LevelsUtils.GetLevels(commandData);
            SelectedSystemType = SystemType.GetSystemType(commandData);
            SaveCommand = new DelegateCommand(OnSaveCommand);
            DuctHeight = 0;                                     //отметка оси трубы
            Points = SelectionUtils.GetPoints(_commandData, "Выберите точки", ObjectSnapTypes.Endpoints);
        }

        private void OnSaveCommand()
        {
            UIApplication uiapp = _commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;

            if (Points.Count < 2 || SelectedDuctType == null || SelectedLevel == null)
                return;

            var curves = new List<Curve>();
            for (int i = 0; i < Points.Count; i++)
            {
                if (i == 0)
                    continue;

                XYZ prevPoint = Points[i - 1];
                XYZ currentPoint = Points[i];

                Curve curve = Line.CreateBound(prevPoint, currentPoint);
                curves.Add(curve);


                using (var ts = new Transaction(doc, "Create duct"))
                {
                    ts.Start();

                    foreach (var point in curves)      
                    {
                        Duct duct = Duct.Create(doc, SelectedSystemType.Id, SelectedDuctType.Id, SelectedLevel.Id, curve.GetEndPoint(0), curve.GetEndPoint(1)); //создание воздуховода

                        Parameter ductHight = duct.get_Parameter(BuiltInParameter.RBS_OFFSET_PARAM);        //смещение от уровня
                        ductHight.Set(UnitUtils.ConvertToInternalUnits(DuctHeight, UnitTypeId.Millimeters));
                    }
                    ts.Commit();
                }
                RaiseCloseRequest();
            }
        }
        public event EventHandler CloseRequest;
        private void RaiseCloseRequest()
        {
            CloseRequest?.Invoke(this, EventArgs.Empty);
        }
    }
}
