using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace __To_DDS_From_PNG__DdsFileTypePlus_.JSON
{
    internal class Config
    {
        public DdsFileTypePlus.DdsFileFormat FileFormat { get; set; }
        public DdsFileTypePlus.DdsErrorMetric ErrorMetric { get; set; }
        public bool ErrorDiffusionDithering { get; set; }
    }
}
