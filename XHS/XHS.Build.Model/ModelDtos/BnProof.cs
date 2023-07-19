using System;
using System.Collections.Generic;
using System.Text;

namespace XHS.Build.Model.ModelDtos
{
    public class BnProof
    {
        public ulong ROUNDID { get; set; }
        public string PROOFID { get; set; }
        public string filename { get; set; }
        public int filesize { get; set; }
        public int bsolved { get; set; }
        public DateTime createtime { get; set; }
    }
}
