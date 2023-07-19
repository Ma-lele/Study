webpackJsonp([19],{eWVt:function(t,e,a){"use strict";Object.defineProperty(e,"__esModule",{value:!0});var r=a("Xxa5"),s=a.n(r),o=a("woOf"),i=a.n(o),n=a("exGp"),l=a.n(n),c=a("P9l9"),u=a("ywL6"),d=a("G+2a"),m=a("djO7"),p=a("JLHL"),g=a("H84J"),f={components:{leftgroup:u.a,Toolbar:m.a},mixins:[p.a],data:function(){return{groupCounts:[],buttonList:[],filters:{name:""},trucks:[],total:0,page:1,pageSize:10,listLoading:!1,editFormVisible:!1,editLoading:!1,editFormRules:{GROUPID:[{required:!0,message:"请选择监测对象分组",trigger:"blur"}],SITEID:[{required:!0,message:"请选择监测对象",trigger:"blur"}],truckno:[{required:!0,message:"请填写车牌号",trigger:"blur"}],truckstartdate:[{required:!0,message:"请填写有效期开始日",trigger:"blur"}],truckenddate:[{required:!0,message:"请填写有效期结束日",trigger:"blur"}]},editForm:{},canEdit:!1,canDelete:!1,groups:[],selectSites:[],selectGroupId:0,isEdit:!1}},methods:{callbackGroup:function(t){this.selectGroupId=t.GROUPID,this.page=1,this.getTrucks()},callFunction:function(t){this.filters={name:t.search},this[t.Func].apply(this,t)},getTrucks:function(){var t=this,e={keyword:this.filters.name,page:this.page,size:this.pageSize};this.listLoading=!0,Object(c._148)(e).then(function(e){e.success&&(t.trucks=e.data.data,t.total=e.data.dataCount),t.listLoading=!1})},dataFormat:function(t,e){var a=t[e.property];return null===a?"":g.a.formatDate.format(new Date(a.replace(/-/g,"/")),"yyyy-MM-dd")},getStatus:function(t,e){var a=!1,r=g.a.formatDate.format(new Date,"yyyy-MM-dd");return t&&e&&g.a.formatDate.format(new Date(t.replace(/-/g,"/")),"yyyy-MM-dd")<=r&&g.a.formatDate.format(new Date(e.replace(/-/g,"/")),"yyyy-MM-dd")>=r&&(a=!0),a},handleCurrentChange:function(t){this.page=t,this.getTrucks()},groupChange:function(t){this.getGroupSites(t),this.$set(this.editForm,"SITEID",""),this.selectSites=this.groupSites},handleDel:function(t,e){var a=this,r=e;r?this.$confirm("确认删除该记录吗?","提示",{type:"warning"}).then(function(){a.listLoading=!0;var t={STID:r.STID};Object(c._20)(t).then(function(t){a.listLoading=!1,t.success?a.$message({message:"删除成功",type:"success"}):a.$message({message:t.msg,type:"error"}),a.getTrucks(),a.getGroupCounts()})}).catch(function(){}):this.$message({message:"请选择要删除的一行数据！",type:"error"})},handleEdit:function(t,e){var a=this;return l()(s.a.mark(function t(){var r;return s.a.wrap(function(t){for(;;)switch(t.prev=t.next){case 0:if(r=e){t.next=4;break}return a.$message({message:"请选择要编辑的一行数据！",type:"error"}),t.abrupt("return");case 4:a.isEdit=!0,a.groupChange(r.GROUPID),a.editFormVisible=!0,a.editForm=i()({},r);case 8:case"end":return t.stop()}},t,a)}))()},handleAdd:function(){this.isEdit=!1,this.editFormVisible=!0,1==this.groups.length&&(this.editForm={GROUPID:this.groups[0].GROUPID},this.groupChange(this.groups[0].GROUPID))},editSubmit:function(){var t=this;this.$refs.editForm.validate(function(e){if(e){t.editLoading=!0;var a=i()({},t.editForm);t.isEdit?Object(c._55)(a).then(function(e){e.success?(t.$message({message:"修改成功",type:"success"}),t.$refs.editForm.resetFields(),t.editFormVisible=!1,t.editLoading=!1,t.getTrucks(),t.getGroupCounts()):(t.editLoading=!1,t.$message({message:e.msg,type:"error"}))}):Object(c.I)(a).then(function(e){e.success?(t.$message({message:"新增成功",type:"success"}),t.$refs.editForm.resetFields(),t.editFormVisible=!1,t.editLoading=!1,t.getTrucks(),t.getGroupCounts()):(t.editLoading=!1,t.$message({message:e.msg,type:"error"}))})}})}},mounted:function(){var t=this;this.getTrucks();var e=window.sessionStorage.router?JSON.parse(window.sessionStorage.router):[];Object(d.a)(this.$route.path,e).forEach(function(e){"handleEdit"==e.Func?t.canEdit=!0:"handleDel"==e.Func?t.canDelete=!0:t.buttonList.push(e)})}},h={render:function(){var t=this,e=t.$createElement,a=t._self._c||e;return a("el-container",[a("el-main",[a("toolbar",{attrs:{buttonList:t.buttonList},on:{callFunction:t.callFunction}}),t._v(" "),a("el-table",{directives:[{name:"loading",rawName:"v-loading",value:t.listLoading,expression:"listLoading"}],staticStyle:{width:"100%"},attrs:{data:t.trucks,"row-class-name":t.$tableRowClassName}},[a("el-table-column",{attrs:{type:"index",width:"80"},scopedSlots:t._u([{key:"default",fn:function(e){return[a("div",{domProps:{textContent:t._s((t.page-1)*t.pageSize+1+e.$index)}})]}}])}),t._v(" "),a("el-table-column",{attrs:{prop:"siteshortname",label:"监测对象"}}),t._v(" "),a("el-table-column",{attrs:{prop:"truckno",label:"车牌号"}}),t._v(" "),a("el-table-column",{attrs:{prop:"truckstartdate",formatter:t.dataFormat,label:"有效期开始日"}}),t._v(" "),a("el-table-column",{attrs:{prop:"truckenddate",formatter:t.dataFormat,label:"有效期结束日"}}),a("el-table-column",{attrs:{label:"状态"},scopedSlots:t._u([{key:"default",fn:function(e){return[t.getStatus(e.row.truckstartdate,e.row.truckenddate)?a("el-tag",{attrs:{type:"success","disable-transitions":""}},[t._v("有效")]):a("el-tag",{attrs:{type:"info","disable-transitions":""}},[t._v("无效")])]}}])}),t._v(" "),t.canEdit||t.canDelete?a("el-table-column",{attrs:{label:"操作",width:"150"},scopedSlots:t._u([{key:"default",fn:function(e){return[t.canEdit?a("el-button",{attrs:{plain:"",type:"warning",size:"small"},on:{click:function(a){return t.handleEdit(e.$index,e.row)}}},[t._v("编辑")]):t._e(),t._v(" "),t.canDelete?a("el-button",{attrs:{plain:"",type:"danger",size:"small"},on:{click:function(a){return t.handleDel(e.$index,e.row)}}},[t._v("删除")]):t._e()]}}],null,!1,917012342)}):t._e()],1),t._v(" "),a("el-col",{staticClass:"toolbar",attrs:{span:24}},[a("el-pagination",{staticStyle:{float:"right"},attrs:{layout:"total, prev, pager, next, jumper","page-size":t.pageSize,"current-page":t.page,total:t.total},on:{"current-change":t.handleCurrentChange}})],1),t._v(" "),a("el-dialog",{attrs:{title:"编辑",visible:t.editFormVisible,"close-on-click-modal":!1},on:{"update:visible":function(e){t.editFormVisible=e}},model:{value:t.editFormVisible,callback:function(e){t.editFormVisible=e},expression:"editFormVisible"}},[a("el-form",{ref:"editForm",attrs:{model:t.editForm,"label-width":"120px",rules:t.editFormRules}},[a("el-row",[a("el-col",{attrs:{span:12}},[a("el-form-item",{attrs:{label:"监测对象分组",prop:"GROUPID"}},[a("el-select",{attrs:{filterable:"",placeholder:"请选择"},on:{change:t.groupChange},model:{value:t.editForm.GROUPID,callback:function(e){t.$set(t.editForm,"GROUPID",e)},expression:"editForm.GROUPID"}},t._l(t.groups,function(t){return a("el-option",{key:t.GROUPID,attrs:{label:t.groupshortname,value:t.GROUPID}})}),1)],1)],1),t._v(" "),a("el-col",{attrs:{span:12}},[a("el-form-item",{attrs:{label:"监测对象",prop:"SITEID"}},[a("el-select",{attrs:{filterable:"",placeholder:"请选择"},model:{value:t.editForm.SITEID,callback:function(e){t.$set(t.editForm,"SITEID",e)},expression:"editForm.SITEID"}},t._l(t.selectSites,function(t){return a("el-option",{key:t.SITEID,attrs:{label:t.siteshortname,value:t.SITEID}})}),1)],1)],1),t._v(" "),a("el-col",{attrs:{span:12}},[a("el-form-item",{attrs:{label:"车牌号",prop:"truckno"}},[a("el-input",{attrs:{"auto-complete":"off",maxlength:"10"},model:{value:t.editForm.truckno,callback:function(e){t.$set(t.editForm,"truckno",e)},expression:"editForm.truckno"}})],1)],1),t._v(" "),a("el-col",{attrs:{span:12}},[a("el-form-item",{attrs:{label:"运输公司名称",prop:"transcomp"}},[a("el-input",{attrs:{"auto-complete":"off",maxlength:"50"},model:{value:t.editForm.transcomp,callback:function(e){t.$set(t.editForm,"transcomp",e)},expression:"editForm.transcomp"}})],1)],1),t._v(" "),a("el-col",{attrs:{span:12}},[a("el-form-item",{attrs:{label:"有效期开始日",prop:"truckstartdate"}},[a("el-date-picker",{attrs:{type:"date",placeholder:"选择开始日期"},model:{value:t.editForm.truckstartdate,callback:function(e){t.$set(t.editForm,"truckstartdate",e)},expression:"editForm.truckstartdate"}})],1)],1),t._v(" "),a("el-col",{attrs:{span:12}},[a("el-form-item",{attrs:{label:"有效期结束日",prop:"truckenddate"}},[a("el-date-picker",{attrs:{type:"date",placeholder:"选择结束日期"},model:{value:t.editForm.truckenddate,callback:function(e){t.$set(t.editForm,"truckenddate",e)},expression:"editForm.truckenddate"}})],1)],1),t._v(" "),a("el-col",{attrs:{span:12}},[a("el-form-item",{attrs:{label:"处置编号",prop:"disposeno"}},[a("el-input",{attrs:{"auto-complete":"off",maxlength:"15"},model:{value:t.editForm.disposeno,callback:function(e){t.$set(t.editForm,"disposeno",e)},expression:"editForm.disposeno"}})],1)],1)],1),t._v(" "),a("el-row",{staticClass:"form-button"},[a("el-col",{attrs:{span:24}},[a("el-button",{nativeOn:{click:function(e){t.editFormVisible=!1}}},[t._v("取消")]),t._v(" "),a("el-button",{attrs:{type:"primary",loading:t.editLoading},nativeOn:{click:function(e){return t.editSubmit(e)}}},[t._v("提交")])],1)],1)],1)],1)],1)],1)},staticRenderFns:[]};var b=a("VU/8")(f,h,!1,function(t){a("kS6L")},"data-v-aac0f378",null);e.default=b.exports},kS6L:function(t,e){}});
//# sourceMappingURL=19.a5d5461a420cbb69ae6e.js.map