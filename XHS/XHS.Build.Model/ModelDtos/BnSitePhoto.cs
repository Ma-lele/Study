﻿using System;
using System.Collections.Generic;
using System.Text;

namespace XHS.Build.Model.ModelDtos
{
    public class BnSitePhoto
    {
        
        public int PHOTOID { get; set; }
        
        public int SITEID { get; set; }
        
        public float latitude { get; set; }
        
        public float longitude { get; set; }
        
        public float altitude { get; set; }
        
        public string path { get; set; }
        
        public string filename { get; set; }
        
        public int filesize { get; set; }
        
        public DateTime createddate { get; set; }
        
        public string username { get; set; }
        
        public DateTime operatedate { get; set; }
    }
}
