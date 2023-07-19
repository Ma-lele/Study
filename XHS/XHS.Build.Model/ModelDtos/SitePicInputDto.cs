using System;
using System.Collections.Generic;
using System.Text;

namespace XHS.Build.Model.ModelDtos
{
    public class SitePicInputDto
    {
        public int SITEID { get; set; }
        public List<PicInput> rendering { get; set; }
        public List<PicInput> dust { get; set; }
        public string[] renderingDel { get; set; }
        public string[] dustDel { get; set; }
    }

    public class PicInput
    {
        public string name { get; set; }
        public string status { get; set; }
        public string filename { get; set; }
    }
}
