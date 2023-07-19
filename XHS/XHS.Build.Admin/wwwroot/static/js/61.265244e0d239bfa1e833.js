webpackJsonp([61],{J3FA:function(e,t,a){"use strict";Object.defineProperty(t,"__esModule",{value:!0});var i=a("mvHQ"),s=a.n(i),o=a("Xxa5"),n=a.n(o),r=a("woOf"),l=a.n(r),d=a("exGp"),c=a.n(d),u=a("P9l9"),m=a("G+2a"),p=a("djO7"),g=a("JLHL"),f=a("KxDp"),b={components:{Toolbar:p.a},mixins:[g.a],data:function(){return{buttonList:[],filters:{name:""},builds:[],total:0,page:1,pageSize:10,listLoading:!1,addDialogFormVisible:!1,editFormVisible:!1,editLoading:!1,editFormRules:{ID:[{required:!0,message:"请选择一条记录",trigger:"blur"}],BeginIP:[{required:!0,message:"请填写开始IP段",trigger:"blur"}],EndIP:[{required:!0,message:"请填写结束IP段",trigger:"blur"}]},editForm:{},addFormVisible:!1,addLoading:!1,addFormRules:{ID:[{required:!0,message:"请选择一条记录",trigger:"blur"}],BeginIP:[{required:!0,message:"请填写开始IP段",trigger:"blur"}],EndIP:[{required:!0,message:"请填写结束IP段",trigger:"blur"}]},addForm:{},canEdit:!1,canDelete:!1,mfogtype:f.b}},computed:{},methods:{callFunction:function(e){this.filters={name:e.search},this[e.Func].apply(this,e)},getPageList:function(e){var t=this;this.page=e||1;var a={keyword:this.filters.name,page:this.page,size:this.pageSize};this.listLoading=!0,Object(u._178)(a).then(function(e){if(e.success){var a=e.data.data||[];t.builds=a,t.total=e.data.dataCount}t.listLoading=!1})},handleCurrentChange:function(e){this.getPageList(e)},handleDel:function(e,t){var a=this,i=t;i?this.$confirm("确认删除该记录吗?","提示",{type:"warning"}).then(function(){a.listLoading=!0;var e={ID:i.ID};Object(u._22)(e).then(function(e){a.listLoading=!1,e.success?a.$message({message:"删除成功",type:"success"}):a.$message({message:e.msg,type:"error"}),a.getPageList(a.page)})}).catch(function(){}):this.$message({message:"请选择要删除的一行数据！",type:"error"})},handleEdit:function(e,t){var a=this;return c()(n.a.mark(function e(){var i;return n.a.wrap(function(e){for(;;)switch(e.prev=e.next){case 0:if(i=t){e.next=4;break}return a.$message({message:"请选择要编辑的一行数据！",type:"error"}),e.abrupt("return");case 4:a.editFormVisible=!0,a.editForm=l()({},i);case 6:case"end":return e.stop()}},e,a)}))()},handleAdd:function(){this.addFormVisible=!0,this.addForm={},this.$refs.addForm&&this.$refs.addForm.clearValidate()},editSubmit:function(){var e=this;this.$refs.editForm.validate(function(t){if(t){e.editLoading=!0;var a=JSON.parse(s()(e.editForm));Object(u._66)(a).then(function(t){t.success?(e.$message({message:"修改成功",type:"success"}),e.$refs.editForm.resetFields(),e.editFormVisible=!1,e.getPageList(e.page)):e.$message({message:t.msg,type:"error"}),e.editLoading=!1})}})},addSubmit:function(){var e=this;this.$refs.addForm.validate(function(t){if(t){e.addLoading=!0;var a=l()({},e.addForm);Object(u.T)(a).then(function(t){t.success?(e.$message({message:"新增成功",type:"success"}),e.$refs.addForm.resetFields(),e.addFormVisible=!1,e.getPageList(e.page)):e.$message({message:t.msg,type:"error"}),e.addLoading=!1})}})}},mounted:function(){var e=this;this.getPageList();var t=window.sessionStorage.router?JSON.parse(window.sessionStorage.router):[];Object(m.a)(this.$route.path,t).forEach(function(t){"handleEdit"==t.Func?e.canEdit=!0:"handleDel"==t.Func?e.canDelete=!0:e.buttonList.push(t)})}},v={render:function(){var e=this,t=e.$createElement,a=e._self._c||t;return a("el-container",[a("el-main",[a("toolbar",{attrs:{buttonList:e.buttonList,placeholder:"IP"},on:{callFunction:e.callFunction}}),e._v(" "),a("el-table",{directives:[{name:"loading",rawName:"v-loading",value:e.listLoading,expression:"listLoading"}],staticStyle:{width:"100%"},attrs:{data:e.builds,"row-class-name":e.$tableRowClassName}},[a("el-table-column",{attrs:{type:"index",width:"80"},scopedSlots:e._u([{key:"default",fn:function(t){return[a("div",{domProps:{textContent:e._s((e.page-1)*e.pageSize+1+t.$index)}})]}}])}),e._v(" "),a("el-table-column",{attrs:{prop:"BeginIP",label:"开始IP段"}}),e._v(" "),a("el-table-column",{attrs:{prop:"EndIP",label:"结束IP段"}}),e._v(" "),a("el-table-column",{attrs:{prop:"updatedate",label:"更新时间"}}),e._v(" "),a("el-table-column",{attrs:{prop:"iptype",label:"类型"}}),e._v(" "),e.canEdit||e.canDelete?a("el-table-column",{attrs:{label:"操作",width:"150"},scopedSlots:e._u([{key:"default",fn:function(t){return[e.canEdit?a("el-button",{attrs:{plain:"",type:"warning",size:"small"},on:{click:function(a){return e.handleEdit(t.$index,t.row)}}},[e._v("编辑")]):e._e(),e._v(" "),e.canDelete?a("el-button",{attrs:{plain:"",type:"danger",size:"small"},on:{click:function(a){return e.handleDel(t.$index,t.row)}}},[e._v("删除")]):e._e()]}}],null,!1,917012342)}):e._e()],1),e._v(" "),a("el-col",{staticClass:"toolbar",attrs:{span:24}},[a("el-pagination",{staticStyle:{float:"right"},attrs:{layout:"total, prev, pager, next, jumper","page-size":e.pageSize,"current-page":e.page,total:e.total},on:{"current-change":e.handleCurrentChange}})],1),e._v(" "),a("el-dialog",{staticClass:"file-dialog",attrs:{title:"编辑",visible:e.editFormVisible,"close-on-click-modal":!1},on:{"update:visible":function(t){e.editFormVisible=t}},model:{value:e.editFormVisible,callback:function(t){e.editFormVisible=t},expression:"editFormVisible"}},[a("el-form",{ref:"editForm",attrs:{model:e.editForm,"label-width":"120px",rules:e.editFormRules}},[a("el-col",{attrs:{span:12}},[a("el-form-item",{attrs:{label:"开始IP段",prop:"BeginIP"}},[a("el-input",{attrs:{"auto-complete":"off",maxlength:"40"},model:{value:e.editForm.BeginIP,callback:function(t){e.$set(e.editForm,"BeginIP",t)},expression:"editForm.BeginIP"}})],1)],1),e._v(" "),a("el-col",{attrs:{span:12}},[a("el-form-item",{attrs:{label:"结束IP段",prop:"EndIP"}},[a("el-input",{attrs:{"auto-complete":"off",maxlength:"50"},model:{value:e.editForm.EndIP,callback:function(t){e.$set(e.editForm,"EndIP",t)},expression:"editForm.EndIP"}})],1)],1),e._v(" "),a("el-col",{attrs:{span:12}},[a("el-form-item",{attrs:{label:"类型",prop:"iptype"}},[a("el-input",{attrs:{"auto-complete":"off",maxlength:"50"},model:{value:e.editForm.iptype,callback:function(t){e.$set(e.editForm,"iptype",t)},expression:"editForm.iptype"}})],1)],1)],1),e._v(" "),a("div",{staticClass:"dialog-footer",staticStyle:{"margin-top":"10px"},attrs:{slot:"footer"},slot:"footer"},[a("el-button",{nativeOn:{click:function(t){e.editFormVisible=!1}}},[e._v("取消")]),e._v(" "),a("el-button",{attrs:{type:"primary",loading:e.editLoading},nativeOn:{click:function(t){return e.editSubmit(t)}}},[e._v("提交")])],1)],1),e._v(" "),a("el-dialog",{staticClass:"file-dialog",attrs:{title:"新增",visible:e.addFormVisible,"close-on-click-modal":!1},on:{"update:visible":function(t){e.addFormVisible=t}},model:{value:e.addFormVisible,callback:function(t){e.addFormVisible=t},expression:"addFormVisible"}},[a("el-form",{ref:"addForm",attrs:{model:e.addForm,"label-width":"120px",rules:e.addFormRules}},[a("el-col",{attrs:{span:12}},[a("el-form-item",{attrs:{label:"开始IP段",prop:"BeginIP"}},[a("el-input",{attrs:{"auto-complete":"off",maxlength:"40"},model:{value:e.addForm.BeginIP,callback:function(t){e.$set(e.addForm,"BeginIP",t)},expression:"addForm.BeginIP"}})],1)],1),e._v(" "),a("el-col",{attrs:{span:12}},[a("el-form-item",{attrs:{label:"结束IP段",prop:"EndIP"}},[a("el-input",{attrs:{"auto-complete":"off",maxlength:"50"},model:{value:e.addForm.EndIP,callback:function(t){e.$set(e.addForm,"EndIP",t)},expression:"addForm.EndIP"}})],1)],1),e._v(" "),a("el-col",{attrs:{span:12}},[a("el-form-item",{attrs:{label:"类型",prop:"iptype"}},[a("el-input",{attrs:{"auto-complete":"off",maxlength:"50"},model:{value:e.addForm.iptype,callback:function(t){e.$set(e.addForm,"iptype",t)},expression:"addForm.iptype"}})],1)],1)],1),e._v(" "),a("div",{staticClass:"dialog-footer",staticStyle:{"margin-top":"10px"},attrs:{slot:"footer"},slot:"footer"},[a("el-button",{nativeOn:{click:function(t){e.addFormVisible=!1}}},[e._v("取消")]),e._v(" "),a("el-button",{attrs:{type:"primary",loading:e.addLoading},nativeOn:{click:function(t){return e.addSubmit(t)}}},[e._v("提交")])],1)],1)],1)],1)},staticRenderFns:[]},F=a("VU/8")(b,v,!1,null,null,null);t.default=F.exports}});
//# sourceMappingURL=61.265244e0d239bfa1e833.js.map