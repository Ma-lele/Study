webpackJsonp([12],{PRdW:function(e,t){},VwQn:function(e,t,a){"use strict";Object.defineProperty(t,"__esModule",{value:!0});var r=a("woOf"),s=a.n(r),o=a("Xxa5"),i=a.n(o),n=a("exGp"),l=a.n(n),d=a("P9l9"),u=a("ywL6"),c=a("G+2a"),m=a("djO7"),p=a("JLHL"),g={components:{leftgroup:u.a,Toolbar:m.a},mixins:[p.a],data:function(){var e,t,a=this;return{groupCounts:[],buttonList:[],filters:{name:""},builds:[],total:0,page:1,pageSize:10,listLoading:!1,addDialogFormVisible:!1,editFormVisible:!1,editLoading:!1,editFormRules:{GROUPID:[{required:!0,message:"请选择监测对象分组",trigger:"blur"}],SITEID:[{required:!0,message:"请选择监测对象",trigger:"blur"}],name:[{required:!0,message:"请填写设备名称",trigger:"blur"}],deviceId:[{required:!0,message:"请填写设备编号",trigger:"blur"},,{message:"设备编号不能重复",trigger:"blur",validator:(t=l()(i.a.mark(function e(t,r,s){return i.a.wrap(function(e){for(;;)switch(e.prev=e.next){case 0:return e.next=2,Object(d._2)({code:a.editForm.deviceId,FPDID:a.editForm.FPDID});case 2:e.sent.data?s():s(new Error(""));case 4:case"end":return e.stop()}},e,a)})),function(e,a,r){return t.apply(this,arguments)})}]},editForm:{},addFormVisible:!1,addLoading:!1,addFormRules:{GROUPID:[{required:!0,message:"请选择监测对象分组",trigger:"blur"}],SITEID:[{required:!0,message:"请选择监测对象",trigger:"blur"}],name:[{required:!0,message:"请填写设备名称",trigger:"blur"}],deviceId:[{required:!0,message:"请填写设备编号",trigger:"blur"},,{message:"设备编号不能重复",trigger:"blur",validator:(e=l()(i.a.mark(function e(t,r,s){return i.a.wrap(function(e){for(;;)switch(e.prev=e.next){case 0:return e.next=2,Object(d._2)({code:a.addForm.deviceId});case 2:e.sent.data?s():s(new Error(""));case 4:case"end":return e.stop()}},e,a)})),function(t,a,r){return e.apply(this,arguments)})}]},addForm:{},canEdit:!1,canDelete:!1,groups:[],selectSites:[],selectGroupId:0,mfloor:[],mzone:["A区","B区","C区","D区","E区","F区","G区","H区","I区","J区","K区","L区","M区","N区","O区","P区","Q区","R区","S区","T区"],mdfstatus:{0:"离线",1:"在线"}}},methods:{callbackGroup:function(e){this.selectGroupId=e.GROUPID,this.page=1,this.getPageList()},callFunction:function(e){this.filters={name:e.search},this[e.Func].apply(this,e)},getPageList:function(e){var t=this;this.page=e||1;var a={groupid:this.selectGroupId,keyword:this.filters.name,page:this.page,size:this.pageSize};this.listLoading=!0,Object(d._95)(a).then(function(e){e.success&&(t.builds=e.data.data,t.total=e.data.dataCount),t.listLoading=!1})},handleCurrentChange:function(e){this.getPageList(e)},getGroupCounts:function(){var e=this;Object(d._94)().then(function(t){t.success&&(e.groupCounts=t.data)})},groupChange:function(e){this.getGroupSites(e),this.$set(this.addForm,"SITEID",""),this.$set(this.editForm,"SITEID",""),this.selectSites=this.groupSites},handleDel:function(e,t){var a=this,r=t;r?this.$confirm("确认删除该记录吗?","提示",{type:"warning"}).then(function(){a.listLoading=!0;var e={FPDID:r.FPDID};Object(d._7)(e).then(function(e){a.listLoading=!1,e.success?a.$message({message:"删除成功",type:"success"}):a.$message({message:e.msg,type:"error"}),a.getPageList(a.page),a.getGroupCounts()})}).catch(function(){}):this.$message({message:"请选择要删除的一行数据！",type:"error"})},handleEdit:function(e,t){var a=this;return l()(i.a.mark(function e(){var r;return i.a.wrap(function(e){for(;;)switch(e.prev=e.next){case 0:if(r=t){e.next=4;break}return a.$message({message:"请选择要编辑的一行数据！",type:"error"}),e.abrupt("return");case 4:a.editFormVisible=!0,r.dfzone&&(r.mblock=r.dfzone.split(" ")[0],r.mfloor=r.dfzone.split(" ")[1],t.mzone=r.dfzone.split(" ")[2]),a.groupChange(r.GROUPID),a.editForm=s()({},r);case 8:case"end":return e.stop()}},e,a)}))()},handleAdd:function(){this.addFormVisible=!0,this.addForm={}},editSubmit:function(){var e=this;this.$refs.editForm.validate(function(t){if(t){e.editLoading=!0;var a=s()({},e.editForm);Object(d._32)(a).then(function(t){t.success?(e.$message({message:"修改成功",type:"success"}),e.$refs.editForm.resetFields(),e.editFormVisible=!1,e.getPageList(e.page),e.getGroupCounts()):e.$message({message:t.msg,type:"error"}),e.editLoading=!1})}})},addSubmit:function(){var e=this;this.$refs.addForm.validate(function(t){if(t){e.addLoading=!0;var a=s()({},e.addForm);a.dfzone=a.mblock+" "+a.mfloor+" "+a.mzone,Object(d.k)(a).then(function(t){t.success?(e.$message({message:"新增成功",type:"success"}),e.$refs.addForm.resetFields(),e.addFormVisible=!1,e.getPageList(e.page),e.getGroupCounts()):e.$message({message:t.msg,type:"error"}),e.addLoading=!1})}})}},created:function(){for(var e=-3;e<=50;e++)e<0?this.mfloor.push("B"+-1*e):this.mfloor.push(e+"F"),0==e&&this.mfloor.pop()},mounted:function(){var e=this;this.getPageList(),this.getGroupCounts(),this.getGroups();var t=window.sessionStorage.router?JSON.parse(window.sessionStorage.router):[];Object(c.a)(this.$route.path,t).forEach(function(t){"handleEdit"==t.Func?e.canEdit=!0:"handleDel"==t.Func?e.canDelete=!0:e.buttonList.push(t)})}},f={render:function(){var e=this,t=e.$createElement,a=e._self._c||t;return a("el-container",[a("el-aside",{directives:[{name:"show",rawName:"v-show",value:e.$getUserGroupSee(),expression:"$getUserGroupSee()"}],staticClass:"left-group-aside"},[a("leftgroup",{attrs:{grouplist:e.groupCounts},on:{callGroup:e.callbackGroup}})],1),e._v(" "),a("el-main",[a("toolbar",{attrs:{buttonList:e.buttonList,placeholder:"设备编号、设备名称、所属区域过滤"},on:{callFunction:e.callFunction}}),e._v(" "),a("el-table",{directives:[{name:"loading",rawName:"v-loading",value:e.listLoading,expression:"listLoading"}],staticStyle:{width:"100%"},attrs:{data:e.builds,"row-class-name":e.$tableRowClassName}},[a("el-table-column",{attrs:{type:"index",width:"60"},scopedSlots:e._u([{key:"default",fn:function(t){return[a("div",{domProps:{textContent:e._s((e.page-1)*e.pageSize+1+t.$index)}})]}}])}),e._v(" "),a("el-table-column",{attrs:{prop:"siteshortname",label:"监测对象","min-width":"2"}}),e._v(" "),a("el-table-column",{attrs:{prop:"deviceId",label:"设备编号","min-width":"2"}}),e._v(" "),a("el-table-column",{attrs:{prop:"name",label:"设备名称","min-width":"2"}}),e._v(" "),a("el-table-column",{attrs:{prop:"battery",label:"电池电量","min-width":"2"}}),e._v(" "),a("el-table-column",{attrs:{prop:"onlinestatus",label:"设备状态",width:"80"},scopedSlots:e._u([{key:"default",fn:function(t){return[a("el-tag",{directives:[{name:"show",rawName:"v-show",value:e.mdfstatus[t.row.onlinestatus],expression:"mdfstatus[scope.row.onlinestatus]"}],attrs:{"disable-transitions":"",type:"0"==t.row.onlinestatus?"danger":"success"}},[e._v("\n                        "+e._s(e.mdfstatus[t.row.onlinestatus])+"\n                    ")])]}}])}),e._v(" "),a("el-table-column",{attrs:{prop:"alarmId",label:"报警状态",width:"80"},scopedSlots:e._u([{key:"default",fn:function(t){return[a("el-tag",{attrs:{"disable-transitions":"",type:t.row.alarmId?"danger":"success"}},[e._v("\n                        "+e._s(t.row.alarmId?t.row.alarmId:"正常")+"\n                    ")])]}}])}),e._v(" "),a("el-table-column",{attrs:{prop:"long",label:"经度","min-width":"3"}}),e._v(" "),a("el-table-column",{attrs:{prop:"lat",label:"纬度","min-width":"3"}}),e._v(" "),e.canEdit||e.canDelete?a("el-table-column",{attrs:{label:"操作",width:"150"},scopedSlots:e._u([{key:"default",fn:function(t){return[e.canEdit?a("el-button",{attrs:{plain:"",type:"warning",size:"small"},on:{click:function(a){return e.handleEdit(t.$index,t.row)}}},[e._v("编辑")]):e._e(),e._v(" "),e.canDelete?a("el-button",{attrs:{plain:"",type:"danger",size:"small"},on:{click:function(a){return e.handleDel(t.$index,t.row)}}},[e._v("删除")]):e._e()]}}],null,!1,917012342)}):e._e()],1),e._v(" "),a("el-col",{staticClass:"toolbar",attrs:{span:24}},[a("el-pagination",{staticStyle:{float:"right"},attrs:{layout:"total, prev, pager, next, jumper","page-size":e.pageSize,"current-page":e.page,total:e.total},on:{"current-change":e.handleCurrentChange}})],1),e._v(" "),a("el-dialog",{attrs:{title:"编辑",visible:e.editFormVisible,"close-on-click-modal":!1},on:{"update:visible":function(t){e.editFormVisible=t}},model:{value:e.editFormVisible,callback:function(t){e.editFormVisible=t},expression:"editFormVisible"}},[a("el-form",{ref:"editForm",attrs:{model:e.editForm,"label-width":"120px",rules:e.editFormRules}},[a("el-col",{attrs:{span:12}},[a("el-form-item",{attrs:{label:"监测对象分组",prop:"GROUPID"}},[a("el-select",{attrs:{filterable:"",placeholder:"请选择"},on:{change:e.groupChange},model:{value:e.editForm.GROUPID,callback:function(t){e.$set(e.editForm,"GROUPID",t)},expression:"editForm.GROUPID"}},e._l(e.groups,function(e){return a("el-option",{key:e.GROUPID,attrs:{label:e.groupshortname,value:e.GROUPID}})}),1)],1)],1),e._v(" "),a("el-col",{attrs:{span:12}},[a("el-form-item",{attrs:{label:"监测对象",prop:"SITEID"}},[a("el-select",{attrs:{filterable:"",placeholder:"请选择"},model:{value:e.editForm.SITEID,callback:function(t){e.$set(e.editForm,"SITEID",t)},expression:"editForm.SITEID"}},e._l(e.selectSites,function(e){return a("el-option",{key:e.SITEID,attrs:{label:e.siteshortname,value:e.SITEID}})}),1)],1)],1),e._v(" "),a("el-col",{attrs:{span:12}},[a("el-form-item",{attrs:{label:"设备名称(位置)",prop:"name"}},[a("el-input",{attrs:{"auto-complete":"off",maxlength:"50"},model:{value:e.editForm.name,callback:function(t){e.$set(e.editForm,"name",t)},expression:"editForm.name"}})],1)],1),e._v(" "),a("el-col",{attrs:{span:12}},[a("el-form-item",{attrs:{label:"设备编号",prop:"deviceId"}},[a("el-input",{attrs:{"auto-complete":"off",maxlength:"50"},model:{value:e.editForm.deviceId,callback:function(t){e.$set(e.editForm,"deviceId",t)},expression:"editForm.deviceId"}})],1)],1)],1),e._v(" "),a("div",{staticClass:"dialog-footer",attrs:{slot:"footer"},slot:"footer"},[a("el-button",{nativeOn:{click:function(t){e.editFormVisible=!1}}},[e._v("取消")]),e._v(" "),a("el-button",{attrs:{type:"primary",loading:e.editLoading},nativeOn:{click:function(t){return e.editSubmit(t)}}},[e._v("提交")])],1)],1),e._v(" "),a("el-dialog",{attrs:{title:"新增",visible:e.addFormVisible,"close-on-click-modal":!1},on:{"update:visible":function(t){e.addFormVisible=t}},model:{value:e.addFormVisible,callback:function(t){e.addFormVisible=t},expression:"addFormVisible"}},[a("el-form",{ref:"addForm",attrs:{model:e.addForm,"label-width":"120px",rules:e.addFormRules}},[a("el-col",{attrs:{span:12}},[a("el-form-item",{attrs:{label:"监测对象分组",prop:"GROUPID"}},[a("el-select",{attrs:{filterable:"",placeholder:"请选择"},on:{change:e.groupChange},model:{value:e.addForm.GROUPID,callback:function(t){e.$set(e.addForm,"GROUPID",t)},expression:"addForm.GROUPID"}},e._l(e.groups,function(e){return a("el-option",{key:e.GROUPID,attrs:{label:e.groupshortname,value:e.GROUPID}})}),1)],1)],1),e._v(" "),a("el-col",{attrs:{span:12}},[a("el-form-item",{attrs:{label:"监测对象",prop:"SITEID"}},[a("el-select",{attrs:{filterable:"",placeholder:"请选择"},model:{value:e.addForm.SITEID,callback:function(t){e.$set(e.addForm,"SITEID",t)},expression:"addForm.SITEID"}},e._l(e.selectSites,function(e){return a("el-option",{key:e.SITEID,attrs:{label:e.siteshortname,value:e.SITEID}})}),1)],1)],1),e._v(" "),a("el-col",{attrs:{span:12}},[a("el-form-item",{attrs:{label:"设备名称(位置)",prop:"name"}},[a("el-input",{attrs:{"auto-complete":"off",maxlength:"50"},model:{value:e.addForm.name,callback:function(t){e.$set(e.addForm,"name",t)},expression:"addForm.name"}})],1)],1),e._v(" "),a("el-col",{attrs:{span:12}},[a("el-form-item",{attrs:{label:"设备编号",prop:"deviceId"}},[a("el-input",{attrs:{"auto-complete":"off",maxlength:"50"},model:{value:e.addForm.deviceId,callback:function(t){e.$set(e.addForm,"deviceId",t)},expression:"addForm.deviceId"}})],1)],1)],1),e._v(" "),a("div",{staticClass:"dialog-footer",attrs:{slot:"footer"},slot:"footer"},[a("el-button",{nativeOn:{click:function(t){e.addFormVisible=!1}}},[e._v("取消")]),e._v(" "),a("el-button",{attrs:{type:"primary",loading:e.addLoading},nativeOn:{click:function(t){return e.addSubmit(t)}}},[e._v("提交")])],1)],1)],1)],1)},staticRenderFns:[]};var b=a("VU/8")(g,f,!1,function(e){a("PRdW")},"data-v-e65d565c",null);t.default=b.exports}});
//# sourceMappingURL=12.33f76d2bf24562ac2b43.js.map