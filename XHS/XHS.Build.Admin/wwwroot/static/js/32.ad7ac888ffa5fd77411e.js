webpackJsonp([32],{"6Kdj":function(e,t,o){"use strict";Object.defineProperty(t,"__esModule",{value:!0});var a=o("woOf"),i=o.n(a),r=o("H84J"),s=o("P9l9"),l=o("G+2a"),n={components:{Toolbar:o("djO7").a},data:function(){return{buttonList:[],filters:{ApiUrl:""},systems:[],total:0,page:1,pageSize:20,listLoading:!1,addDialogFormVisible:!1,editFormVisible:!1,editLoading:!1,editFormRules:{Name:[{required:!0,message:"请输入系统名称",trigger:"blur"}],Secret:[{required:!0,message:"请输入系统Secret",trigger:"blur"}]},editForm:{Id:"0",Name:"",Url:"",Secret:"",sortno:0,Note:""},addFormVisible:!1,addLoading:!1,addFormRules:{Name:[{required:!0,message:"请输入系统名称",trigger:"blur"}],Secret:[{required:!0,message:"请输入系统Secret",trigger:"blur"}]},addForm:{Name:"",Url:"",Secret:"",sortno:0,Note:""},canEdit:!1,canDelete:!1}},methods:{callFunction:function(e){this.filters={name:e.search},this[e.Func].apply(this,e)},handleCurrentChange:function(e){this.page=e,this.getSystems()},getSystems:function(){var e=this,t={page:this.page,size:this.pageSize,keyword:this.filters.ApiUrl};this.listLoading=!0,Object(s._142)(t).then(function(t){e.total=t.data.dataCount,e.systems=t.data.data,e.listLoading=!1})},handleDel:function(e,t){var o=this,a=t;a?this.$confirm("确认删除该记录吗?","提示",{type:"warning"}).then(function(){o.listLoading=!0;var e={id:a.Id};Object(s._215)(e).then(function(e){r.a.isEmt.format(e)?o.listLoading=!1:(o.listLoading=!1,e.success?o.$message({message:"删除成功",type:"success"}):o.$message({message:e.msg,type:"error"}),o.getSystems())})}).catch(function(){}):this.$message({message:"请选择要删除的一行数据！",type:"error"})},handleEdit:function(e,t){var o=t;o?(this.editFormVisible=!0,this.editForm=i()({},o)):this.$message({message:"请选择要编辑的一行数据！",type:"error"})},handleAdd:function(){this.addFormVisible=!0,this.addForm={ApiUrl:"",Name:"",Enabled:"true"}},editSubmit:function(){var e=this;this.$refs.editForm.validate(function(t){if(t){e.editLoading=!0;var o=i()({},e.editForm);Object(s._51)(o).then(function(t){r.a.isEmt.format(t)?e.editLoading=!1:t.success?(e.editLoading=!1,e.$message({message:"修改成功",type:"success"}),e.$refs.editForm.resetFields(),e.editFormVisible=!1,e.getSystems()):e.$message({message:t.msg,type:"error"})})}})},addSubmit:function(){var e=this;this.$refs.addForm.validate(function(t){if(t){e.addLoading=!0;var o=i()({},e.addForm);Object(s.E)(o).then(function(t){r.a.isEmt.format(t)?e.addLoading=!1:t.success?(e.addLoading=!1,e.$message({message:"新增成功",type:"success"}),e.$refs.addForm.resetFields(),e.addFormVisible=!1,e.getSystems()):e.$message({message:t.msg,type:"error"})})}})}},mounted:function(){var e=this;this.getSystems();var t=window.sessionStorage.router?JSON.parse(window.sessionStorage.router):[];Object(l.a)(this.$route.path,t).forEach(function(t){"handleEdit"==t.Func?e.canEdit=!0:"handleDel"==t.Func?e.canDelete=!0:e.buttonList.push(t)})}},d={render:function(){var e=this,t=e.$createElement,o=e._self._c||t;return o("section",[o("toolbar",{attrs:{buttonList:e.buttonList},on:{callFunction:e.callFunction}}),e._v(" "),o("el-table",{directives:[{name:"loading",rawName:"v-loading",value:e.listLoading,expression:"listLoading"}],staticStyle:{width:"100%"},attrs:{data:e.systems,"row-class-name":e.$tableRowClassName}},[o("el-table-column",{attrs:{type:"index",width:"80"}}),e._v(" "),o("el-table-column",{attrs:{prop:"Name",label:"系统名称",width:"",sortable:""}}),e._v(" "),o("el-table-column",{attrs:{prop:"Url",label:"系统地址",width:"",sortable:""}}),e._v(" "),o("el-table-column",{attrs:{prop:"Note",label:"备注",width:""}}),e._v(" "),o("el-table-column",{attrs:{prop:"Secret",label:"密钥",width:"",sortable:""}}),e._v(" "),e.canEdit||e.canDelete?o("el-table-column",{attrs:{label:"操作",width:"150"},scopedSlots:e._u([{key:"default",fn:function(t){return[e.canEdit?o("el-button",{attrs:{plain:"",type:"warning",size:"small"},on:{click:function(o){return e.handleEdit(t.$index,t.row)}}},[e._v("编辑")]):e._e(),e._v(" "),e.canDelete?o("el-button",{attrs:{plain:"",type:"danger",size:"small"},on:{click:function(o){return e.handleDel(t.$index,t.row)}}},[e._v("删除")]):e._e()]}}],null,!1,917012342)}):e._e()],1),e._v(" "),o("el-col",{staticClass:"toolbar",attrs:{span:24}},[o("el-pagination",{staticStyle:{float:"right"},attrs:{layout:"total, prev, pager, next, jumper","page-size":e.pageSize,"current-page":e.page,total:e.total},on:{"current-change":e.handleCurrentChange}})],1),e._v(" "),o("el-dialog",{attrs:{title:"编辑",visible:e.editFormVisible,"close-on-click-modal":!1},on:{"update:visible":function(t){e.editFormVisible=t}},model:{value:e.editFormVisible,callback:function(t){e.editFormVisible=t},expression:"editFormVisible"}},[o("el-form",{ref:"editForm",attrs:{model:e.editForm,"label-width":"100px",rules:e.editFormRules}},[o("el-form-item",{attrs:{label:"系统名称",prop:"Name"}},[o("el-input",{attrs:{"auto-complete":"off"},model:{value:e.editForm.Name,callback:function(t){e.$set(e.editForm,"Name",t)},expression:"editForm.Name"}})],1),e._v(" "),o("el-form-item",{attrs:{label:"系统地址",prop:"Url"}},[o("el-input",{attrs:{"auto-complete":"off"},model:{value:e.editForm.Url,callback:function(t){e.$set(e.editForm,"Url",t)},expression:"editForm.Url"}})],1),e._v(" "),o("el-form-item",{attrs:{label:"系统Secret",prop:"Secret"}},[o("el-input",{attrs:{"auto-complete":"off"},model:{value:e.editForm.Secret,callback:function(t){e.$set(e.editForm,"Secret",t)},expression:"editForm.Secret"}})],1),e._v(" "),o("el-form-item",{attrs:{label:"排序",prop:"sortno"}},[o("el-input",{attrs:{"auto-complete":"off",type:"number"},model:{value:e.editForm.sortno,callback:function(t){e.$set(e.editForm,"sortno",t)},expression:"editForm.sortno"}})],1),e._v(" "),o("el-form-item",{attrs:{label:"备注",prop:"Note"}},[o("el-input",{attrs:{"auto-complete":"off"},model:{value:e.editForm.Note,callback:function(t){e.$set(e.editForm,"Note",t)},expression:"editForm.Note"}})],1)],1),e._v(" "),o("div",{staticClass:"dialog-footer",attrs:{slot:"footer"},slot:"footer"},[o("el-button",{nativeOn:{click:function(t){e.editFormVisible=!1}}},[e._v("取消")]),e._v(" "),o("el-button",{attrs:{type:"primary",loading:e.editLoading},nativeOn:{click:function(t){return e.editSubmit(t)}}},[e._v("提交")])],1)],1),e._v(" "),o("el-dialog",{attrs:{title:"新增",visible:e.addFormVisible,"close-on-click-modal":!1},on:{"update:visible":function(t){e.addFormVisible=t}},model:{value:e.addFormVisible,callback:function(t){e.addFormVisible=t},expression:"addFormVisible"}},[o("el-form",{ref:"addForm",attrs:{model:e.addForm,"label-width":"100px",rules:e.addFormRules}},[o("el-form-item",{attrs:{label:"系统名称",prop:"Name"}},[o("el-input",{attrs:{"auto-complete":"off"},model:{value:e.addForm.Name,callback:function(t){e.$set(e.addForm,"Name",t)},expression:"addForm.Name"}})],1),e._v(" "),o("el-form-item",{attrs:{label:"系统地址",prop:"Url"}},[o("el-input",{attrs:{"auto-complete":"off"},model:{value:e.addForm.Url,callback:function(t){e.$set(e.addForm,"Url",t)},expression:"addForm.Url"}})],1),e._v(" "),o("el-form-item",{attrs:{label:"系统Secret",prop:"Secret"}},[o("el-input",{attrs:{"auto-complete":"off"},model:{value:e.addForm.Secret,callback:function(t){e.$set(e.addForm,"Secret",t)},expression:"addForm.Secret"}})],1),e._v(" "),o("el-form-item",{attrs:{label:"排序",prop:"sortno"}},[o("el-input",{attrs:{"auto-complete":"off",type:"number"},model:{value:e.addForm.sortno,callback:function(t){e.$set(e.addForm,"sortno",t)},expression:"addForm.sortno"}})],1),e._v(" "),o("el-form-item",{attrs:{label:"备注",prop:"Note"}},[o("el-input",{attrs:{"auto-complete":"off"},model:{value:e.addForm.Note,callback:function(t){e.$set(e.addForm,"Note",t)},expression:"addForm.Note"}})],1)],1),e._v(" "),o("div",{staticClass:"dialog-footer",attrs:{slot:"footer"},slot:"footer"},[o("el-button",{nativeOn:{click:function(t){e.addFormVisible=!1}}},[e._v("取消")]),e._v(" "),o("el-button",{attrs:{type:"primary",loading:e.addLoading},nativeOn:{click:function(t){return e.addSubmit(t)}}},[e._v("提交")])],1)],1)],1)},staticRenderFns:[]};var m=o("VU/8")(n,d,!1,function(e){o("ouxi")},"data-v-69c59be6",null);t.default=m.exports},ouxi:function(e,t){}});
//# sourceMappingURL=32.ad7ac888ffa5fd77411e.js.map