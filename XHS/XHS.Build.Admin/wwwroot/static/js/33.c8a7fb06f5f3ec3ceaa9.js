webpackJsonp([33],{AnLg:function(e,t){},vtNv:function(e,t,a){"use strict";Object.defineProperty(t,"__esModule",{value:!0});var r=a("mvHQ"),o=a.n(r),l=a("woOf"),s=a.n(l),i=a("P9l9"),n=a("ywL6"),c=a("G+2a"),d=a("djO7"),m=a("JLHL"),u=a("KxDp"),p={components:{leftgroup:n.a,Toolbar:d.a},mixins:[m.a],data:function(){return{groupCounts:[],buttonList:[],filters:{name:""},builds:[],total:0,page:1,pageSize:10,listLoading:!1,addDialogFormVisible:!1,editFormVisible:!1,editLoading:!1,editFormRules:{GROUPID:[{required:!0,message:"请选择监测对象分组",trigger:"blur"}],SITEID:[{required:!0,message:"请选择监测对象",trigger:"blur"}],cameratype:[{required:!0,message:"请选择设备类型",trigger:"blur"}],cameracode:[{required:!0,message:"请填写设备编号",trigger:"blur"}],cameraname:[{required:!0,message:"请填写设备名称",trigger:"blur"}]},editForm:{},addFormVisible:!1,addLoading:!1,addFormRules:{GROUPID:[{required:!0,message:"请选择监测对象分组",trigger:"blur"}],SITEID:[{required:!0,message:"请选择监测对象",trigger:"blur"}],cameratype:[{required:!0,message:"请选择设备类型",trigger:"blur"}],cameracode:[{required:!0,message:"请填写设备编号",trigger:"blur"}],cameraname:[{required:!0,message:"请填写设备名称",trigger:"blur"}]},addForm:{},canEdit:!1,canStatus:!1,groups:[],selectSites:[],selectGroupId:0,mcameratype:u.a,cameratypename:"设备编号"}},methods:{callbackGroup:function(e){this.selectGroupId=e.GROUPID,this.page=1,this.getPageList()},callFunction:function(e){this.filters={name:e.search},this[e.Func].apply(this,e)},getPageList:function(e){var t=this;this.page=e||1;var a={groupid:this.selectGroupId,keyword:this.filters.name,page:this.page,size:this.pageSize};this.listLoading=!0,Object(i._74)(a).then(function(e){e.success&&(t.builds=e.data.data,t.total=e.data.dataCount),t.listLoading=!1})},handleCurrentChange:function(e){this.getPageList(e)},getGroupCounts:function(){var e=this;Object(i._73)().then(function(t){t.success&&(e.groupCounts=t.data)})},groupChange:function(e){this.getGroupSites(e),this.$set(this.addForm,"SITEID",""),this.$set(this.editForm,"SITEID",""),this.selectSites=this.groupSites},handleEdit:function(e,t){var a=t;a?(this.groupChange(a.GROUPID),this.editFormVisible=!0,this.editForm=s()({},a)):this.$message({message:"请选择要编辑的一行数据！",type:"error"})},handleAdd:function(){this.addFormVisible=!0,this.addForm={}},handleStatus:function(e,t,a){var r=this,o={CAMERAID:a.CAMERAID,bdel:e};Object(i._247)(o).then(function(e){e.success?(r.$message({message:"操作成功",type:"success"}),r.getPageList(r.page),r.getGroupCounts()):(r.$message({message:"操作失败",type:"error"}),r.getPageList(r.page),r.getGroupCounts())})},editSubmit:function(){var e=this;this.$refs.editForm.validate(function(t){if(t){e.editLoading=!0;var a=s()({},e.editForm);console.log(o()(a)),Object(i._22)(a).then(function(t){t.success?(e.$message({message:"修改成功",type:"success"}),e.$refs.editForm.resetFields(),e.editFormVisible=!1,e.getPageList(e.page),e.getGroupCounts()):e.$message({message:t.msg,type:"error"}),e.editLoading=!1})}})},addSubmit:function(){var e=this;this.$refs.addForm.validate(function(t){if(t){e.addLoading=!0;var a=s()({},e.addForm);Object(i.c)(a).then(function(t){t.success?(e.$message({message:"新增成功",type:"success"}),e.$refs.addForm.resetFields(),e.addFormVisible=!1,e.getPageList(e.page),e.getGroupCounts()):e.$message({message:t.msg,type:"error"}),e.addLoading=!1})}})},cameratypeChange:function(e){this.cameratypename="4"==e?"流地址":"设备编号"}},mounted:function(){var e=this;this.getPageList(),this.getGroupCounts(),this.getGroups();var t=window.sessionStorage.router?JSON.parse(window.sessionStorage.router):[];Object(c.a)(this.$route.path,t).forEach(function(t){"handleEdit"==t.Func?e.canEdit=!0:"handleStatus"==t.Func?e.canStatus=!0:e.buttonList.push(t)})}},g={render:function(){var e=this,t=e.$createElement,a=e._self._c||t;return a("el-container",[a("el-aside",{directives:[{name:"show",rawName:"v-show",value:e.$getUserGroupSee(),expression:"$getUserGroupSee()"}],staticClass:"left-group-aside"},[a("leftgroup",{attrs:{grouplist:e.groupCounts},on:{callGroup:e.callbackGroup}})],1),e._v(" "),a("el-main",[a("toolbar",{attrs:{buttonList:e.buttonList,placeholder:"监测对象、设备名称、设备编号过滤"},on:{callFunction:e.callFunction}}),e._v(" "),a("div",{staticClass:"memo"},[e._v("注：冻结的摄像头，7天之后将被系统自动清理")]),e._v(" "),a("el-table",{directives:[{name:"loading",rawName:"v-loading",value:e.listLoading,expression:"listLoading"}],staticStyle:{width:"100%"},attrs:{data:e.builds,"row-class-name":e.$tableRowClassName}},[a("el-table-column",{attrs:{type:"index",width:"80"},scopedSlots:e._u([{key:"default",fn:function(t){return[a("div",{domProps:{textContent:e._s((e.page-1)*e.pageSize+1+t.$index)}})]}}])}),e._v(" "),a("el-table-column",{attrs:{prop:"siteshortname",label:"监测对象","min-width":"1"}}),e._v(" "),a("el-table-column",{attrs:{prop:"cameraname",label:"设备名称","min-width":"1"}}),e._v(" "),a("el-table-column",{attrs:{prop:"cameratype",label:"设备类型","min-width":"1"},scopedSlots:e._u([{key:"default",fn:function(t){return a("nobr",{},[e._v("\n                    "+e._s(e.mcameratype[t.row.cameratype])+"\n                ")])}}])}),e._v(" "),a("el-table-column",{attrs:{prop:"cameracode",label:"设备编号","show-overflow-tooltip":"","min-width":"2"}}),e._v(" "),a("el-table-column",{attrs:{prop:"channel",label:"信道",width:"80"}}),e._v(" "),a("el-table-column",{attrs:{prop:"devcode",label:"关联设备","show-overflow-tooltip":"","min-width":"1"}}),e._v(" "),a("el-table-column",{attrs:{prop:"status",label:"状态","min-width":"1"},scopedSlots:e._u([{key:"default",fn:function(t){return[e.canStatus?a("el-switch",{attrs:{"active-value":0,"active-text":"正常","inactive-value":1,"inactive-text":"冻结"},on:{change:function(a){return e.handleStatus(a,t.$index,t.row)}},model:{value:t.row.bdel,callback:function(a){e.$set(t.row,"bdel",a)},expression:"scope.row.bdel"}}):a("div",[e._v("\n                        "+e._s(0==t.row.bdel?"正常":"冻结")+"\n                    ")])]}}])}),e._v(" "),e.canEdit?a("el-table-column",{attrs:{label:"操作",width:"100"},scopedSlots:e._u([{key:"default",fn:function(t){return[e.canEdit?a("el-button",{attrs:{plain:"",type:"warning",size:"small"},on:{click:function(a){return e.handleEdit(t.$index,t.row)}}},[e._v("编辑")]):e._e()]}}],null,!1,3982963594)}):e._e()],1),e._v(" "),a("el-col",{staticClass:"toolbar",attrs:{span:24}},[a("el-pagination",{staticStyle:{float:"right"},attrs:{layout:"total, prev, pager, next, jumper","page-size":e.pageSize,"current-page":e.page,total:e.total},on:{"current-change":e.handleCurrentChange}})],1),e._v(" "),a("el-dialog",{attrs:{title:"编辑",visible:e.editFormVisible,"close-on-click-modal":!1},on:{"update:visible":function(t){e.editFormVisible=t}},model:{value:e.editFormVisible,callback:function(t){e.editFormVisible=t},expression:"editFormVisible"}},[a("el-form",{ref:"editForm",attrs:{model:e.editForm,"label-width":"120px",rules:e.editFormRules}},[a("el-row",[a("el-col",{attrs:{span:12}},[a("el-form-item",{attrs:{label:"监测对象分组",prop:"GROUPID"}},[a("el-select",{attrs:{filterable:"",placeholder:"请选择"},on:{change:e.groupChange},model:{value:e.editForm.GROUPID,callback:function(t){e.$set(e.editForm,"GROUPID",t)},expression:"editForm.GROUPID"}},e._l(e.groups,function(e){return a("el-option",{key:e.GROUPID,attrs:{label:e.groupshortname,value:e.GROUPID}})}),1)],1)],1),e._v(" "),a("el-col",{attrs:{span:12}},[a("el-form-item",{attrs:{label:"监测对象",prop:"SITEID"}},[a("el-select",{attrs:{filterable:"",placeholder:"请选择"},model:{value:e.editForm.SITEID,callback:function(t){e.$set(e.editForm,"SITEID",t)},expression:"editForm.SITEID"}},e._l(e.selectSites,function(e){return a("el-option",{key:e.SITEID,attrs:{label:e.siteshortname,value:e.SITEID}})}),1)],1)],1),e._v(" "),a("el-col",{attrs:{span:12}},[a("el-form-item",{attrs:{label:"设备类型",prop:"cameratype"}},[a("el-select",{attrs:{filterable:"",placeholder:"请选择"},on:{change:e.cameratypeChange},model:{value:e.editForm.cameratype,callback:function(t){e.$set(e.editForm,"cameratype",t)},expression:"editForm.cameratype"}},e._l(e.mcameratype,function(e,t){return a("el-option",{key:t,attrs:{label:e,value:Number(t)}})}),1)],1)],1),e._v(" "),a("el-col",{attrs:{span:12}},[a("el-form-item",{attrs:{label:"设备名称",prop:"cameraname"}},[a("el-input",{attrs:{"auto-complete":"off",maxlength:"20"},model:{value:e.editForm.cameraname,callback:function(t){e.$set(e.editForm,"cameraname",t)},expression:"editForm.cameraname"}})],1)],1),e._v(" "),a("el-col",{attrs:{span:12}},[a("el-form-item",{attrs:{label:e.cameratypename,prop:"cameracode"}},[a("el-input",{attrs:{"auto-complete":"off",maxlength:"200"},model:{value:e.editForm.cameracode,callback:function(t){e.$set(e.editForm,"cameracode",t)},expression:"editForm.cameracode"}})],1)],1),e._v(" "),a("el-col",{attrs:{span:12}},[a("el-form-item",{attrs:{label:"信道",prop:"channel"}},[a("el-input",{attrs:{"auto-complete":"off",maxlength:"5"},model:{value:e.editForm.channel,callback:function(t){e.$set(e.editForm,"channel",t)},expression:"editForm.channel"}})],1)],1),e._v(" "),a("el-col",{attrs:{span:12}},[a("el-form-item",{attrs:{label:"备选参数",prop:"cameraparam"}},[a("el-input",{attrs:{"auto-complete":"off",maxlength:"500"},model:{value:e.editForm.cameraparam,callback:function(t){e.$set(e.editForm,"cameraparam",t)},expression:"editForm.cameraparam"}})],1)],1),e._v(" "),a("el-col",{attrs:{span:12}},[a("el-form-item",{attrs:{label:"关联设备编号",prop:"devcode"}},[a("el-input",{attrs:{"auto-complete":"off",maxlength:"100"},model:{value:e.editForm.devcode,callback:function(t){e.$set(e.editForm,"devcode",t)},expression:"editForm.devcode"}})],1)],1)],1)],1),e._v(" "),a("div",{staticClass:"dialog-footer",attrs:{slot:"footer"},slot:"footer"},[a("el-button",{nativeOn:{click:function(t){e.editFormVisible=!1}}},[e._v("取消")]),e._v(" "),a("el-button",{attrs:{type:"primary",loading:e.editLoading},nativeOn:{click:function(t){return e.editSubmit(t)}}},[e._v("提交")])],1)],1),e._v(" "),a("el-dialog",{attrs:{title:"新增",visible:e.addFormVisible,"close-on-click-modal":!1},on:{"update:visible":function(t){e.addFormVisible=t}},model:{value:e.addFormVisible,callback:function(t){e.addFormVisible=t},expression:"addFormVisible"}},[a("el-form",{ref:"addForm",attrs:{model:e.addForm,"label-width":"120px",rules:e.addFormRules}},[a("el-row",[a("el-col",{attrs:{span:12}},[a("el-form-item",{attrs:{label:"监测对象分组",prop:"GROUPID"}},[a("el-select",{attrs:{filterable:"",placeholder:"请选择"},on:{change:e.groupChange},model:{value:e.addForm.GROUPID,callback:function(t){e.$set(e.addForm,"GROUPID",t)},expression:"addForm.GROUPID"}},e._l(e.groups,function(e){return a("el-option",{key:e.GROUPID,attrs:{label:e.groupshortname,value:e.GROUPID}})}),1)],1)],1),e._v(" "),a("el-col",{attrs:{span:12}},[a("el-form-item",{attrs:{label:"监测对象",prop:"SITEID"}},[a("el-select",{attrs:{filterable:"",placeholder:"请选择"},model:{value:e.addForm.SITEID,callback:function(t){e.$set(e.addForm,"SITEID",t)},expression:"addForm.SITEID"}},e._l(e.selectSites,function(e){return a("el-option",{key:e.SITEID,attrs:{label:e.siteshortname,value:e.SITEID}})}),1)],1)],1),e._v(" "),a("el-col",{attrs:{span:12}},[a("el-form-item",{attrs:{label:"设备类型",prop:"cameratype"}},[a("el-select",{attrs:{filterable:"",placeholder:"请选择"},on:{change:e.cameratypeChange},model:{value:e.addForm.cameratype,callback:function(t){e.$set(e.addForm,"cameratype",t)},expression:"addForm.cameratype"}},e._l(e.mcameratype,function(e,t){return a("el-option",{key:t,attrs:{label:e,value:Number(t)}})}),1)],1)],1),e._v(" "),a("el-col",{attrs:{span:12}},[a("el-form-item",{attrs:{label:"设备名称",prop:"cameraname"}},[a("el-input",{attrs:{"auto-complete":"off",maxlength:"20"},model:{value:e.addForm.cameraname,callback:function(t){e.$set(e.addForm,"cameraname",t)},expression:"addForm.cameraname"}})],1)],1),e._v(" "),a("el-col",{attrs:{span:12}},[a("el-form-item",{attrs:{label:e.cameratypename,prop:"cameracode"}},[a("el-input",{attrs:{"auto-complete":"off",maxlength:"200"},model:{value:e.addForm.cameracode,callback:function(t){e.$set(e.addForm,"cameracode",t)},expression:"addForm.cameracode"}})],1)],1),e._v(" "),a("el-col",{attrs:{span:12}},[a("el-form-item",{attrs:{label:"信道",prop:"channel"}},[a("el-input",{attrs:{"auto-complete":"off",maxlength:"5"},model:{value:e.addForm.channel,callback:function(t){e.$set(e.addForm,"channel",t)},expression:"addForm.channel"}})],1)],1),e._v(" "),a("el-col",{attrs:{span:12}},[a("el-form-item",{attrs:{label:"备选参数",prop:"cameraparam"}},[a("el-input",{attrs:{"auto-complete":"off",maxlength:"500"},model:{value:e.addForm.cameraparam,callback:function(t){e.$set(e.addForm,"cameraparam",t)},expression:"addForm.cameraparam"}})],1)],1),e._v(" "),a("el-col",{attrs:{span:12}},[a("el-form-item",{attrs:{label:"关联设备编号",prop:"devcode"}},[a("el-input",{attrs:{"auto-complete":"off",maxlength:"100"},model:{value:e.addForm.devcode,callback:function(t){e.$set(e.addForm,"devcode",t)},expression:"addForm.devcode"}})],1)],1)],1)],1),e._v(" "),a("div",{staticClass:"dialog-footer",attrs:{slot:"footer"},slot:"footer"},[a("el-button",{nativeOn:{click:function(t){e.addFormVisible=!1}}},[e._v("取消")]),e._v(" "),a("el-button",{attrs:{type:"primary",loading:e.addLoading},nativeOn:{click:function(t){return e.addSubmit(t)}}},[e._v("提交")])],1)],1)],1)],1)},staticRenderFns:[]};var f=a("VU/8")(p,g,!1,function(e){a("AnLg")},"data-v-62a4123e",null);t.default=f.exports}});
//# sourceMappingURL=33.c8a7fb06f5f3ec3ceaa9.js.map