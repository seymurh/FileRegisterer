using System.Collections.Generic;

namespace FileRegisterer.Models
{
    public class EmployeeMedicalDataResult
    {
        public List<MedicalLogData> medicalRecord { get; set; }
        public List<OorzaakcodeModel> oorzaakCode { get; set; }
        public List<CAScodeModel> casCode { get; set; }
    }

    public class MedicalLogData : IDocument
    {
        public string Document { get; set; }
    }

    public class OorzaakcodeModel : IDocument
    {
        public string Document { get; set; }
    }

    public class CAScodeModel : IDocument
    {
        public string Document { get; set; }
    }
}
