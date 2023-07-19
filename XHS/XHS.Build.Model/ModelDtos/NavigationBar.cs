using System;
using System.Collections.Generic;
using System.Text;

namespace XHS.Build.Model.ModelDtos
{
    public class NavigationBar
    {
        public string id { get; set; }
        public string pid { get; set; }
        public int order { get; set; }
        public string name { get; set; }
        public bool IsHide { get; set; } = false;
        public bool IsButton { get; set; } = false;
        public string path { get; set; }
        public string Func { get; set; }
        public string iconCls { get; set; }
        public NavigationBarMeta meta { get; set; }
        public List<NavigationBar> children { get; set; }
    }

    public class NavigationBarMeta
    {
        public string title { get; set; }
        public bool requireAuth { get; set; } = true;
        public bool NoTabPage { get; set; } = false;
        public bool keepAlive { get; set; } = false;


    }

    public class PermissionTree
    {
        public string value { get; set; }
        public string Pid { get; set; }
        public string label { get; set; }
        public int SortNo { get; set; }
        public bool isbtn { get; set; }
        public bool disabled { get; set; }
        public List<PermissionTree> children { get; set; }
        public List<PermissionTree> btns { get; set; }
    }


    public class AssignView
    {
        public List<string> pids { get; set; }
        public string rid { get; set; }
    }
    public class AssignShow
    {
        public List<string> permissionids { get; set; }
        public List<string> assignbtns { get; set; }
    }
}
