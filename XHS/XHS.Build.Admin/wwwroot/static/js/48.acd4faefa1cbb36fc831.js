webpackJsonp([48],{LrBW:function(e,t){},YXq3:function(e,t,i){"use strict";Object.defineProperty(t,"__esModule",{value:!0});var a=i("Xxa5"),l=i.n(a),s=i("exGp"),o=i.n(s),r=i("woOf"),n=i.n(r),m=i("+6Bu"),c=i.n(m),d=i("P9l9"),p=i("ywL6"),u=i("G+2a"),f=i("djO7"),g=i("JLHL"),h=i("KxDp"),F={components:{leftgroup:p.a,Toolbar:f.a},mixins:[g.a],data:function(){return{groupCounts:[],buttonList:[],filters:{name:""},builds:[],total:0,page:1,pageSize:10,listLoading:!1,addDialogFormVisible:!1,editFormVisible:!1,editLoading:!1,editFormRules:{GROUPID:[{required:!0,message:"请选择监测对象分组",trigger:"blur"}],SITEID:[{required:!0,message:"请选择监测对象",trigger:"blur"}],sename:[{required:!0,message:"请填写设备名称",trigger:"blur"}],setype:[{required:!0,message:"请选择设备类型",trigger:"blur"}],secode:[{required:!0,message:"请填写设备编号",trigger:"blur"}]},editForm:{},addFormVisible:!1,addLoading:!1,addFormRules:{GROUPID:[{required:!0,message:"请选择监测对象分组",trigger:"blur"}],SITEID:[{required:!0,message:"请选择监测对象",trigger:"blur"}],sename:[{required:!0,message:"请填写设备名称",trigger:"blur"}],setype:[{required:!0,message:"请选择设备类型",trigger:"blur"}],secode:[{required:!0,message:"请填写设备编号",trigger:"blur"}]},addForm:{},canEdit:!1,canDelete:!1,groups:[],selectSites:[],selectGroupId:0,msetype:h.e,imgModalVisible:!1,eqpImgFormLoading:!1,eqpImgForm:{notify:[],monitor:[],licenses:[],repair:[],dismantle:[],notifyDel:[],monitorDel:[],licensesDel:[],repairDel:[],dismantleDel:[]},showpicrow:{}}},methods:{addPicSubmit:function(){var e=this;this.eqpImgFormLoading=!0;var t=n()({SEID:this.showpicrow.SEID},this.eqpImgForm),i=(t.imgurl,c()(t,["imgurl"]));Object(d._245)(i).then(function(t){t.success?(e.$message({message:"设置成功",type:"success"}),e.$refs.eqpImgForm.resetFields(),e.imgModalVisible=!1):e.$message({message:t.msg,type:"error"}),e.eqpImgFormLoading=!1})},dialogClose:function(){this.$refs.eqpImgForm.resetFields()},showPic:function(e,t){var i=this;this.imgModalVisible=!0,this.showpicrow=t,this.eqpImgForm.notify=[],this.eqpImgForm.monitor=[],this.eqpImgForm.licenses=[],this.eqpImgForm.repair=[],this.eqpImgForm.dismantle=[],this.eqpImgForm.notifyDel=[],this.eqpImgForm.monitorDel=[],this.eqpImgForm.licensesDel=[],this.eqpImgForm.repairDel=[],this.eqpImgForm.dismantleDel=[];var a={SEID:t.SEID};Object(d._183)(a).then(function(e){e.success&&e.data&&e.data.forEach(function(e){switch(e.filetype){case 1:i.eqpImgForm.notify.push(e);break;case 2:i.eqpImgForm.monitor.push(e);break;case 3:i.eqpImgForm.licenses.push(e);break;case 4:i.eqpImgForm.repair.push(e);break;case 5:i.eqpImgForm.dismantle.push(e)}})})},imgFileChanged:function(e,t,i){switch(i){case"notify":this.eqpImgForm.notify=t;break;case"monitor":this.eqpImgForm.monitor=t;break;case"licenses":this.eqpImgForm.licenses=t;break;case"repair":this.eqpImgForm.repair=t;break;case"dismantle":this.eqpImgForm.dismantle=t}},handleRemove:function(e,t,i){switch(i){case"notify":this.eqpImgForm.notify=t,this.eqpImgForm.notifyDel.push(e.url);break;case"monitor":this.eqpImgForm.monitor=t,this.eqpImgForm.monitorDel.push(e.url);break;case"licenses":this.eqpImgForm.licenses=t,this.eqpImgForm.licensesDel.push(e.url);break;case"repair":this.eqpImgForm.repair=t,this.eqpImgForm.repairDel.push(e.url);break;case"dismantle":this.eqpImgForm.dismantle=t,this.eqpImgForm.dismantleDel.push(e.url)}},handleRemoveImgFile:function(e,t){switch(t){case"notify":this.eqpImgForm.notify=this.eqpImgForm.notify.filter(function(t){return t.uid!==e.uid}),this.eqpImgForm.notifyDel.push(e.url);break;case"monitor":this.eqpImgForm.monitor=this.eqpImgForm.monitor.filter(function(t){return t.uid!==e.uid}),this.eqpImgForm.monitorDel.push(e.url);break;case"licenses":this.eqpImgForm.licenses=this.eqpImgForm.licenses.filter(function(t){return t.uid!==e.uid}),this.eqpImgForm.licensesDel.push(e.url);break;case"repair":this.eqpImgForm.repair=this.eqpImgForm.repair.filter(function(t){return t.uid!==e.uid}),this.eqpImgForm.repairDel.push(e.url);break;case"dismantle":this.eqpImgForm.dismantle=this.eqpImgForm.dismantle.filter(function(t){return t.uid!==e.uid}),this.eqpImgForm.dismantleDel.push(e.url)}},fileUpload:function(e){var t=this,i=e.file,a=new FormData;a.append("formFiles",i),Object(d._196)(a).then(function(i){switch(e.action){case"notify":t.eqpImgForm.notify[t.eqpImgForm.notify.length-1].filename=i.data;break;case"monitor":t.eqpImgForm.monitor[t.eqpImgForm.monitor.length-1].filename=i.data;break;case"licenses":t.eqpImgForm.licenses[t.eqpImgForm.licenses.length-1].filename=i.data;break;case"repair":t.eqpImgForm.repair[t.eqpImgForm.repair.length-1].filename=i.data;break;case"dismantle":t.eqpImgForm.dismantle[t.eqpImgForm.dismantle.length-1].filename=i.data}}).catch(function(e){})},callbackGroup:function(e){this.selectGroupId=e.GROUPID,this.page=1,this.getPageList()},callFunction:function(e){this.filters={name:e.search},this[e.Func].apply(this,e)},getPageList:function(e){var t=this;this.page=e||1;var i={groupid:this.selectGroupId,keyword:this.filters.name,page:this.page,size:this.pageSize};this.listLoading=!0,Object(d._188)(i).then(function(e){e.success&&(t.builds=e.data.data,t.total=e.data.dataCount),t.listLoading=!1})},handleCurrentChange:function(e){this.getPageList(e)},getGroupCounts:function(){var e=this;Object(d._187)().then(function(t){t.success&&(e.groupCounts=t.data)})},groupChange:function(e){this.getGroupSites(e),this.$set(this.addForm,"SITEID",""),this.$set(this.editForm,"SITEID",""),this.selectSites=this.groupSites},handleDel:function(e,t){var i=this,a=t;a?this.$confirm("确认删除该记录吗?","提示",{type:"warning"}).then(function(){i.listLoading=!0;var e={id:a.SEID};Object(d._226)(e).then(function(e){i.listLoading=!1,e.success?i.$message({message:"删除成功",type:"success"}):i.$message({message:e.msg,type:"error"}),i.getPageList(i.page),i.getGroupCounts()})}).catch(function(){}):this.$message({message:"请选择要删除的一行数据！",type:"error"})},handleEdit:function(e,t){var i=this;return o()(l.a.mark(function e(){var a;return l.a.wrap(function(e){for(;;)switch(e.prev=e.next){case 0:if(a=t){e.next=4;break}return i.$message({message:"请选择要编辑的一行数据！",type:"error"}),e.abrupt("return");case 4:i.editFormVisible=!0,i.groupChange(a.GROUPID),i.editForm=n()({},a);case 7:case"end":return e.stop()}},e,i)}))()},handleAdd:function(){this.addFormVisible=!0,this.addForm={}},editSubmit:function(){var e=this;this.$refs.editForm.validate(function(t){if(t){e.editLoading=!0;var i=n()({},e.editForm);Object(d._68)(i).then(function(t){t.success?(e.$message({message:"修改成功",type:"success"}),e.$refs.editForm.resetFields(),e.editFormVisible=!1,e.getPageList(e.page),e.getGroupCounts()):e.$message({message:t.msg,type:"error"}),e.editLoading=!1})}})},addSubmit:function(){var e=this;this.$refs.addForm.validate(function(t){if(t){e.addLoading=!0;var i=n()({},e.addForm);Object(d.V)(i).then(function(t){t.success?(e.$message({message:"新增成功",type:"success"}),e.$refs.addForm.resetFields(),e.addFormVisible=!1,e.getPageList(e.page),e.getGroupCounts()):e.$message({message:t.msg,type:"error"}),e.addLoading=!1})}})},handleOpenFile:function(e){window.open(e.url)}},mounted:function(){var e=this;this.getPageList(),this.getGroupCounts(),this.getGroups();var t=window.sessionStorage.router?JSON.parse(window.sessionStorage.router):[];Object(u.a)(this.$route.path,t).forEach(function(t){"handleEdit"==t.Func?e.canEdit=!0:"handleDel"==t.Func?e.canDelete=!0:e.buttonList.push(t)})}},b={render:function(){var e=this,t=e.$createElement,i=e._self._c||t;return i("el-container",[i("el-aside",{directives:[{name:"show",rawName:"v-show",value:e.$getUserGroupSee(),expression:"$getUserGroupSee()"}],staticClass:"left-group-aside"},[i("leftgroup",{attrs:{grouplist:e.groupCounts},on:{callGroup:e.callbackGroup}})],1),e._v(" "),i("el-main",[i("toolbar",{attrs:{buttonList:e.buttonList,placeholder:"监测对象、设备名称、设备编号过滤"},on:{callFunction:e.callFunction}}),e._v(" "),i("el-table",{directives:[{name:"loading",rawName:"v-loading",value:e.listLoading,expression:"listLoading"}],staticStyle:{width:"100%"},attrs:{data:e.builds,"row-class-name":e.$tableRowClassName}},[i("el-table-column",{attrs:{type:"index",width:"80"},scopedSlots:e._u([{key:"default",fn:function(t){return[i("div",{domProps:{textContent:e._s((e.page-1)*e.pageSize+1+t.$index)}})]}}])}),e._v(" "),i("el-table-column",{attrs:{prop:"siteshortname",label:"监测对象","min-width":"2"}}),e._v(" "),i("el-table-column",{attrs:{prop:"sename",label:"设备名称","min-width":"2"}}),e._v(" "),i("el-table-column",{attrs:{prop:"setype",label:"设备类型","min-width":"1"},scopedSlots:e._u([{key:"default",fn:function(t){return i("nobr",{},[e._v("\n                    "+e._s(t.row.setype?e.msetype[t.row.setype]:"")+"\n                ")])}}])}),e._v(" "),i("el-table-column",{attrs:{prop:"secode",label:"设备编号","show-overflow-tooltip":"","min-width":"2"}}),e._v(" "),i("el-table-column",{attrs:{prop:"liftovercode",label:"AI绑定","min-width":"2"}}),e._v(" "),i("el-table-column",{attrs:{prop:"checkintime",label:"最后上线时间","min-width":"2"}}),e._v(" "),i("el-table-column",{attrs:{prop:"",label:"信息图片",width:"90"},scopedSlots:e._u([{key:"default",fn:function(t){return[i("el-link",{attrs:{type:"primary"},on:{click:function(i){return e.showPic(t.$index,t.row)}}},[e._v("查看")])]}}])}),e._v(" "),e.canEdit||e.canDelete?i("el-table-column",{attrs:{label:"操作",width:"150"},scopedSlots:e._u([{key:"default",fn:function(t){return[e.canEdit?i("el-button",{attrs:{plain:"",type:"warning",size:"small"},on:{click:function(i){return e.handleEdit(t.$index,t.row)}}},[e._v("编辑")]):e._e(),e._v(" "),e.canDelete?i("el-button",{attrs:{plain:"",type:"danger",size:"small"},on:{click:function(i){return e.handleDel(t.$index,t.row)}}},[e._v("删除")]):e._e()]}}],null,!1,917012342)}):e._e()],1),e._v(" "),i("el-col",{staticClass:"toolbar",attrs:{span:24}},[i("el-pagination",{staticStyle:{float:"right"},attrs:{layout:"total, prev, pager, next, jumper","page-size":e.pageSize,"current-page":e.page,total:e.total},on:{"current-change":e.handleCurrentChange}})],1),e._v(" "),i("el-dialog",{staticClass:"file-dialog",attrs:{title:"图片",visible:e.imgModalVisible,"close-on-click-modal":!1,top:"5vh"},on:{"update:visible":function(t){e.imgModalVisible=t},close:e.dialogClose}},[i("el-form",{ref:"eqpImgForm",attrs:{"label-position":"top","label-width":"150px",model:e.eqpImgForm}},[i("el-form-item",{attrs:{label:"安装-告知书",prop:"notify"}},[i("el-upload",{ref:"uploadNotify",attrs:{action:"notify",limit:10,"on-remove":function(t,i){e.handleRemove(t,i,"notify")},accept:".jpg","auto-upload":!0,"http-request":e.fileUpload,"on-change":function(t,i){e.imgFileChanged(t,i,"notify")},"list-type":"picture-card","file-list":e.eqpImgForm.notify},scopedSlots:e._u([{key:"file",fn:function(t){var a=t.file;return i("div",{},[i("el-image",{staticClass:"el-upload-list__item-thumbnail",attrs:{src:a.url,fit:"cover"}}),e._v(" "),i("span",{staticClass:"el-upload-list__item-actions"},[i("span",{staticClass:"el-upload-list__item-preview",on:{click:function(t){return e.handleOpenFile(a)}}},[i("i",{staticClass:"el-icon-zoom-in"})]),e._v(" "),i("span",{staticClass:"el-upload-list__item-delete",on:{click:function(t){return e.handleRemoveImgFile(a,"notify")}}},[i("i",{staticClass:"el-icon-delete"})])])],1)}}])},[i("i",{staticClass:"el-icon-plus",attrs:{slot:"default"},slot:"default"})])],1),e._v(" "),i("el-form-item",{attrs:{label:"安装-监测报告",prop:"monitor"}},[i("el-upload",{ref:"uploadMonitor",attrs:{action:"monitor",limit:10,"on-remove":function(t,i){e.handleRemove(t,i,"monitor")},accept:".jpg","auto-upload":!0,"http-request":e.fileUpload,"on-change":function(t,i){e.imgFileChanged(t,i,"monitor")},"list-type":"picture-card","file-list":e.eqpImgForm.monitor},scopedSlots:e._u([{key:"file",fn:function(t){var a=t.file;return i("div",{},[i("el-image",{staticClass:"el-upload-list__item-thumbnail",attrs:{src:a.url,fit:"cover"}}),e._v(" "),i("span",{staticClass:"el-upload-list__item-actions"},[i("span",{staticClass:"el-upload-list__item-preview",on:{click:function(t){return e.handleOpenFile(a)}}},[i("i",{staticClass:"el-icon-zoom-in"})]),e._v(" "),i("span",{staticClass:"el-upload-list__item-delete",on:{click:function(t){return e.handleRemoveImgFile(a,"monitor")}}},[i("i",{staticClass:"el-icon-delete"})])])],1)}}])},[i("i",{staticClass:"el-icon-plus",attrs:{slot:"default"},slot:"default"})])],1),e._v(" "),i("el-form-item",{attrs:{label:"安装-使用许可证",prop:"licenses"}},[i("el-upload",{ref:"uploadLicenses",attrs:{action:"licenses",limit:10,"on-remove":function(t,i){e.handleRemove(t,i,"licenses")},accept:".jpg","auto-upload":!0,"http-request":e.fileUpload,"on-change":function(t,i){e.imgFileChanged(t,i,"licenses")},"list-type":"picture-card","file-list":e.eqpImgForm.licenses},scopedSlots:e._u([{key:"file",fn:function(t){var a=t.file;return i("div",{},[i("el-image",{staticClass:"el-upload-list__item-thumbnail",attrs:{src:a.url,fit:"cover"}}),e._v(" "),i("span",{staticClass:"el-upload-list__item-actions"},[i("span",{staticClass:"el-upload-list__item-preview",on:{click:function(t){return e.handleOpenFile(a)}}},[i("i",{staticClass:"el-icon-zoom-in"})]),e._v(" "),i("span",{staticClass:"el-upload-list__item-delete",on:{click:function(t){return e.handleRemoveImgFile(a,"licenses")}}},[i("i",{staticClass:"el-icon-delete"})])])],1)}}])},[i("i",{staticClass:"el-icon-plus",attrs:{slot:"default"},slot:"default"})])],1),e._v(" "),i("el-form-item",{attrs:{label:"安装-维修保养记录",prop:"repair"}},[i("el-upload",{ref:"uploadRepair",attrs:{action:"repair",limit:10,"on-remove":function(t,i){e.handleRemove(t,i,"repair")},accept:".jpg","auto-upload":!0,"http-request":e.fileUpload,"on-change":function(t,i){e.imgFileChanged(t,i,"repair")},"list-type":"picture-card","file-list":e.eqpImgForm.repair},scopedSlots:e._u([{key:"file",fn:function(t){var a=t.file;return i("div",{},[i("el-image",{staticClass:"el-upload-list__item-thumbnail",attrs:{src:a.url,fit:"cover"}}),e._v(" "),i("span",{staticClass:"el-upload-list__item-actions"},[i("span",{staticClass:"el-upload-list__item-preview",on:{click:function(t){return e.handleOpenFile(a)}}},[i("i",{staticClass:"el-icon-zoom-in"})]),e._v(" "),i("span",{staticClass:"el-upload-list__item-delete",on:{click:function(t){return e.handleRemoveImgFile(a,"repair")}}},[i("i",{staticClass:"el-icon-delete"})])])],1)}}])},[i("i",{staticClass:"el-icon-plus",attrs:{slot:"default"},slot:"default"})])],1),e._v(" "),i("el-form-item",{attrs:{label:"拆除-拆除告知",prop:"dismantle"}},[i("el-upload",{ref:"uploadDismantle",attrs:{action:"dismantle",limit:10,"on-remove":function(t,i){e.handleRemove(t,i,"dismantle")},accept:".jpg","auto-upload":!0,"http-request":e.fileUpload,"on-change":function(t,i){e.imgFileChanged(t,i,"dismantle")},"list-type":"picture-card","file-list":e.eqpImgForm.dismantle},scopedSlots:e._u([{key:"file",fn:function(t){var a=t.file;return i("div",{},[i("el-image",{staticClass:"el-upload-list__item-thumbnail",attrs:{src:a.url,fit:"cover"}}),e._v(" "),i("span",{staticClass:"el-upload-list__item-actions"},[i("span",{staticClass:"el-upload-list__item-preview",on:{click:function(t){return e.handleOpenFile(a)}}},[i("i",{staticClass:"el-icon-zoom-in"})]),e._v(" "),i("span",{staticClass:"el-upload-list__item-delete",on:{click:function(t){return e.handleRemoveImgFile(a,"dismantle")}}},[i("i",{staticClass:"el-icon-delete"})])])],1)}}])},[i("i",{staticClass:"el-icon-plus",attrs:{slot:"default"},slot:"default"})])],1)],1),e._v(" "),i("div",{staticClass:"dialog-footer",attrs:{slot:"footer"},slot:"footer"},[i("el-button",{nativeOn:{click:function(t){e.imgModalVisible=!1}}},[e._v("取消")]),e._v(" "),i("el-button",{attrs:{type:"primary",loading:e.eqpImgFormLoading},nativeOn:{click:function(t){return e.addPicSubmit(t)}}},[e._v("提交")])],1)],1),e._v(" "),i("el-dialog",{attrs:{title:"编辑",visible:e.editFormVisible,"close-on-click-modal":!1},on:{"update:visible":function(t){e.editFormVisible=t}},model:{value:e.editFormVisible,callback:function(t){e.editFormVisible=t},expression:"editFormVisible"}},[i("el-form",{ref:"editForm",attrs:{model:e.editForm,"label-width":"120px",rules:e.editFormRules}},[i("el-col",{attrs:{span:12}},[i("el-form-item",{attrs:{label:"监测对象分组",prop:"GROUPID"}},[i("el-select",{attrs:{filterable:"",placeholder:"请选择"},on:{change:e.groupChange},model:{value:e.editForm.GROUPID,callback:function(t){e.$set(e.editForm,"GROUPID",t)},expression:"editForm.GROUPID"}},e._l(e.groups,function(e){return i("el-option",{key:e.GROUPID,attrs:{label:e.groupshortname,value:e.GROUPID}})}),1)],1)],1),e._v(" "),i("el-col",{attrs:{span:12}},[i("el-form-item",{attrs:{label:"监测对象",prop:"SITEID"}},[i("el-select",{attrs:{filterable:"",placeholder:"请选择"},model:{value:e.editForm.SITEID,callback:function(t){e.$set(e.editForm,"SITEID",t)},expression:"editForm.SITEID"}},e._l(e.selectSites,function(e){return i("el-option",{key:e.SITEID,attrs:{label:e.siteshortname,value:e.SITEID}})}),1)],1)],1),e._v(" "),i("el-col",{attrs:{span:12}},[i("el-form-item",{attrs:{label:"特种设备名称",prop:"sename"}},[i("el-input",{attrs:{"auto-complete":"off",maxlength:"50"},model:{value:e.editForm.sename,callback:function(t){e.$set(e.editForm,"sename",t)},expression:"editForm.sename"}})],1)],1),e._v(" "),i("el-col",{attrs:{span:12}},[i("el-form-item",{attrs:{label:"特种设备类型",prop:"setype"}},[i("el-select",{attrs:{placeholder:"请选择"},model:{value:e.editForm.setype,callback:function(t){e.$set(e.editForm,"setype",t)},expression:"editForm.setype"}},e._l(e.msetype,function(e,t){return i("el-option",{key:t,attrs:{label:e,value:Number(t)}})}),1)],1)],1),e._v(" "),i("el-col",{attrs:{span:12}},[i("el-form-item",{attrs:{label:"设备编号",prop:"secode"}},[i("el-input",{attrs:{"auto-complete":"off",maxlength:"100"},model:{value:e.editForm.secode,callback:function(t){e.$set(e.editForm,"secode",t)},expression:"editForm.secode"}})],1)],1),e._v(" "),i("el-col",{attrs:{span:12}},[i("el-form-item",{attrs:{label:"AI绑定编号",prop:"liftovercode"}},[i("el-input",{attrs:{"auto-complete":"off",maxlength:"50"},model:{value:e.editForm.liftovercode,callback:function(t){e.$set(e.editForm,"liftovercode",t)},expression:"editForm.liftovercode"}})],1)],1),e._v(" "),i("el-col",{attrs:{span:24}},[i("el-form-item",{attrs:{label:"备注",prop:"remark"}},[i("el-input",{attrs:{type:"textarea",placeholder:"请输入内容",maxlength:"100","show-word-limit":""},model:{value:e.editForm.remark,callback:function(t){e.$set(e.editForm,"remark",t)},expression:"editForm.remark"}})],1)],1)],1),e._v(" "),i("div",{staticClass:"dialog-footer",attrs:{slot:"footer"},slot:"footer"},[i("el-button",{nativeOn:{click:function(t){e.editFormVisible=!1}}},[e._v("取消")]),e._v(" "),i("el-button",{attrs:{type:"primary",loading:e.editLoading},nativeOn:{click:function(t){return e.editSubmit(t)}}},[e._v("提交")])],1)],1),e._v(" "),i("el-dialog",{attrs:{title:"新增",visible:e.addFormVisible,"close-on-click-modal":!1},on:{"update:visible":function(t){e.addFormVisible=t}},model:{value:e.addFormVisible,callback:function(t){e.addFormVisible=t},expression:"addFormVisible"}},[i("el-form",{ref:"addForm",attrs:{model:e.addForm,"label-width":"120px",rules:e.addFormRules}},[i("el-col",{attrs:{span:12}},[i("el-form-item",{attrs:{label:"监测对象分组",prop:"GROUPID"}},[i("el-select",{attrs:{filterable:"",placeholder:"请选择"},on:{change:e.groupChange},model:{value:e.addForm.GROUPID,callback:function(t){e.$set(e.addForm,"GROUPID",t)},expression:"addForm.GROUPID"}},e._l(e.groups,function(e){return i("el-option",{key:e.GROUPID,attrs:{label:e.groupshortname,value:e.GROUPID}})}),1)],1)],1),e._v(" "),i("el-col",{attrs:{span:12}},[i("el-form-item",{attrs:{label:"监测对象",prop:"SITEID"}},[i("el-select",{attrs:{filterable:"",placeholder:"请选择"},model:{value:e.addForm.SITEID,callback:function(t){e.$set(e.addForm,"SITEID",t)},expression:"addForm.SITEID"}},e._l(e.selectSites,function(e){return i("el-option",{key:e.SITEID,attrs:{label:e.siteshortname,value:e.SITEID}})}),1)],1)],1),e._v(" "),i("el-col",{attrs:{span:12}},[i("el-form-item",{attrs:{label:"特种设备名称",prop:"sename"}},[i("el-input",{attrs:{"auto-complete":"off",maxlength:"50"},model:{value:e.addForm.sename,callback:function(t){e.$set(e.addForm,"sename",t)},expression:"addForm.sename"}})],1)],1),e._v(" "),i("el-col",{attrs:{span:12}},[i("el-form-item",{attrs:{label:"特种设备类型",prop:"setype"}},[i("el-select",{attrs:{placeholder:"请选择"},model:{value:e.addForm.setype,callback:function(t){e.$set(e.addForm,"setype",t)},expression:"addForm.setype"}},e._l(e.msetype,function(e,t){return i("el-option",{key:t,attrs:{label:e,value:Number(t)}})}),1)],1)],1),e._v(" "),i("el-col",{attrs:{span:12}},[i("el-form-item",{attrs:{label:"设备编号",prop:"secode"}},[i("el-input",{attrs:{"auto-complete":"off",maxlength:"100"},model:{value:e.addForm.secode,callback:function(t){e.$set(e.addForm,"secode",t)},expression:"addForm.secode"}})],1)],1),e._v(" "),i("el-col",{attrs:{span:12}},[i("el-form-item",{attrs:{label:"AI绑定编号",prop:"liftovercode"}},[i("el-input",{attrs:{"auto-complete":"off",maxlength:"50"},model:{value:e.addForm.liftovercode,callback:function(t){e.$set(e.addForm,"liftovercode",t)},expression:"addForm.liftovercode"}})],1)],1),e._v(" "),i("el-col",{attrs:{span:24}},[i("el-form-item",{attrs:{label:"备注",prop:"remark"}},[i("el-input",{attrs:{type:"textarea",placeholder:"请输入内容",maxlength:"100","show-word-limit":""},model:{value:e.addForm.remark,callback:function(t){e.$set(e.addForm,"remark",t)},expression:"addForm.remark"}})],1)],1)],1),e._v(" "),i("div",{staticClass:"dialog-footer",attrs:{slot:"footer"},slot:"footer"},[i("el-button",{nativeOn:{click:function(t){e.addFormVisible=!1}}},[e._v("取消")]),e._v(" "),i("el-button",{attrs:{type:"primary",loading:e.addLoading},nativeOn:{click:function(t){return e.addSubmit(t)}}},[e._v("提交")])],1)],1)],1)],1)},staticRenderFns:[]};var v=i("VU/8")(F,b,!1,function(e){i("LrBW")},"data-v-1d38b464",null);t.default=v.exports}});
//# sourceMappingURL=48.acd4faefa1cbb36fc831.js.map