webpackJsonp([60],{eVSH:function(e,t,o){"use strict";Object.defineProperty(t,"__esModule",{value:!0});var a=o("woOf"),r=o.n(a),s=o("H84J"),l=o("P9l9"),i=o("ywL6"),n=o("G+2a"),d=o("djO7"),m=o("JLHL"),u={components:{Toolbar:d.a,leftgroup:i.a},mixins:[m.a],data:function(){return{filters:{name:""},groupCounts:[],selectGroupId:0,users:[],roles:[],showRoles:[],total:0,buttonList:[],page:1,pageSize:50,listLoading:!1,addDialogFormVisible:!1,editFormVisible:!1,editLoading:!1,editFormRules:{LoginName:[{required:!0,message:"请输入登录名",trigger:"blur"}],UserName:[{required:!0,message:"请输入用户姓名",trigger:"blur"}],Mobile:[{required:!0,message:"请输入手机号码",trigger:"blur"},{validator:this.checkMobile,message:"请输入合法的手机号码"}],GroupId:[{required:!0,message:"请选择分组",trigger:"blur"}],RIDs:[{required:!0,message:"请选择角色",trigger:"blur"}]},editForm:{id:0,LoginName:"",UserName:"",Gender:-1,RIDs:[],Mobile:""},addFormVisible:!1,addLoading:!1,addFormRules:{LoginName:[{required:!0,message:"请输入登录名",trigger:"blur"}],UserName:[{required:!0,message:"请输入用户姓名",trigger:"blur"}],Password:[{required:!0,message:"请输入密码",trigger:"blur"}],Mobile:[{required:!0,message:"请输入手机号码",trigger:"blur"},{validator:this.checkMobile,message:"请输入合法的手机号码"}],GroupId:[{required:!0,message:"请选择分组",trigger:"blur"}],RIDs:[{required:!0,message:"请选择角色",trigger:"blur"}]},addForm:{name:"",LoginName:"",UserName:"",Password:"",Gender:-1,RIDs:[],Mobile:""},canEdit:!1,canDelete:!1,groups:[],globalUserGroupid:sessionStorage.user?JSON.parse(sessionStorage.user).GroupId:0}},methods:{checkMobile:function(e,t,o){return null==t||""===String(t).trim()?o(new Error("请输入正确的手机号码")):/^1(3|4|5|6|7|8|9)\d{9}$/.test(t)?o():o(new Error("请输入正确的手机号码"))},callFunction:function(e){this.filters={name:e.search},this[e.Func].apply(this,e)},formatSex:function(e,t){return 1==e.Gender?"男":2==e.Gender?"女":"未知"},formatBirth:function(e,t){return e.birth&&""!=e.birth?s.a.formatDate.format(new Date(e.birth.replace(/-/g,"/")),"yyyy-MM-dd"):""},handleCurrentChange:function(e){this.page=e,this.getUsers()},getUsers:function(){var e=this,t={groupid:this.selectGroupId,page:this.page,size:this.pageSize,keyword:this.filters.name};this.listLoading=!0,Object(l._149)(t).then(function(t){e.total=t.data.dataCount,e.users=t.data.data,e.listLoading=!1})},handleDel:function(e,t){var o=this,a=t;a?this.$confirm("确认删除该记录吗?","提示",{type:"warning"}).then(function(){o.listLoading=!0;var e={id:a.Id};Object(l._213)(e).then(function(e){s.a.isEmt.format(e)?o.listLoading=!1:(o.listLoading=!1,e.success?o.$message({message:"删除成功",type:"success"}):o.$message({message:e.msg,type:"error"}),o.getUsers(),o.getGroupCounts())})}).catch(function(){}):this.$message({message:"请选择要删除的一行数据！",type:"error"})},handleEdit:function(e,t){var o=t;o?(o.GroupId&&(this.showRoles=this.roles.filter(function(e){return e.GROUPID==o.GroupId})),this.editFormVisible=!0,o.Password="        ",this.editForm=r()({},o)):this.$message({message:"请选择要编辑的一行数据！",type:"error"})},handleAdd:function(){this.addFormVisible=!0,this.addForm={LoginName:"",UserName:"",Password:"",name:"",Gender:-1},this.globalUserGroupid>0&&(this.addForm.GROUPID=this.globalUserGroupid)},editSubmit:function(){var e=this;this.$refs.editForm.validate(function(t){if(t){e.editLoading=!0;var o=r()({},e.editForm);Object(l._54)(o).then(function(t){e.editLoading=!1,t.success?(e.$message({message:"修改成功",type:"success"}),e.cancelEditDialog(),e.getUsers(),e.getGroupCounts()):e.$message({message:t.msg,type:"error"})})}})},addSubmit:function(){var e=this;this.$refs.addForm.validate(function(t){if(t){e.addLoading=!0;var o=r()({},e.addForm);Object(l.J)(o).then(function(t){e.addLoading=!1,t.success?(e.$message({message:"新增成功",type:"success"}),e.cancelAddDialog(),e.getUsers(),e.getGroupCounts()):e.$message({message:t.msg,type:"error"})})}})},cancelAddDialog:function(){this.addFormVisible=!1,this.$refs.addForm.resetFields()},cancelEditDialog:function(){this.editFormVisible=!1,this.$refs.editForm.resetFields()},groupChange:function(e){this.$set(this.addForm,"RIDs",[]),this.$set(this.editForm,"RIDs",[]),this.showRoles=this.roles.filter(function(t){return t.GROUPID==e})},getGroupCounts:function(){var e=this;Object(l._148)().then(function(t){t.success&&(e.groupCounts=t.data)})},callbackGroup:function(e){this.selectGroupId=e.GROUPID,this.page=1,this.getUsers()}},mounted:function(){var e=this;this.getUsers(),Object(l._121)().then(function(t){e.roles=e.showRoles=t.data}),this.getGroupCounts();var t=window.sessionStorage.router?JSON.parse(window.sessionStorage.router):[];Object(n.a)(this.$route.path,t).forEach(function(t){"handleEdit"==t.Func?e.canEdit=!0:"handleDel"==t.Func?e.canDelete=!0:e.buttonList.push(t)})}},c={render:function(){var e=this,t=e.$createElement,o=e._self._c||t;return o("el-container",[o("el-aside",{directives:[{name:"show",rawName:"v-show",value:e.$getUserGroupSee(),expression:"$getUserGroupSee()"}],staticClass:"left-group-aside"},[o("leftgroup",{attrs:{grouplist:e.groupCounts},on:{callGroup:e.callbackGroup}})],1),e._v(" "),o("el-main",[o("toolbar",{attrs:{buttonList:e.buttonList},on:{callFunction:e.callFunction}}),e._v(" "),o("el-table",{directives:[{name:"loading",rawName:"v-loading",value:e.listLoading,expression:"listLoading"}],staticStyle:{width:"100%"},attrs:{data:e.users,"highlight-current-row":"","row-class-name":e.$tableRowClassName}},[o("el-table-column",{attrs:{type:"index",width:"80"},scopedSlots:e._u([{key:"default",fn:function(t){return[o("div",{domProps:{textContent:e._s((e.page-1)*e.pageSize+1+t.$index)}})]}}])}),e._v(" "),o("el-table-column",{attrs:{prop:"LoginName",label:"登录名",width:"",sortable:""}}),e._v(" "),o("el-table-column",{attrs:{prop:"UserName",label:"用户姓名",width:"",sortable:""}}),e._v(" "),o("el-table-column",{attrs:{prop:"Mobile",label:"手机号码",width:"",sortable:""}}),e._v(" "),o("el-table-column",{attrs:{prop:"RoleNames",label:"角色",width:""},scopedSlots:e._u([{key:"default",fn:function(t){return e._l(t.row.RoleNames,function(t){return o("el-tag",{key:t.Id},[e._v(e._s(t))])})}}])}),e._v(" "),o("el-table-column",{attrs:{prop:"CreateTime",label:"创建时间",width:"",sortable:""}}),e._v(" "),e.canEdit||e.canDelete?o("el-table-column",{attrs:{label:"操作",width:"150"},scopedSlots:e._u([{key:"default",fn:function(t){return[e.canEdit?o("el-button",{attrs:{plain:"",type:"warning",size:"small"},on:{click:function(o){return e.handleEdit(t.$index,t.row)}}},[e._v("编辑")]):e._e(),e._v(" "),e.canDelete?o("el-button",{key:t.row.Id,attrs:{plain:"",type:"danger",size:"small"},on:{click:function(o){return e.handleDel(t.$index,t.row)}}},[e._v("删除")]):e._e()]}}],null,!1,1656443290)}):e._e()],1),e._v(" "),o("el-col",{staticClass:"toolbar",attrs:{span:24}},[o("el-pagination",{staticStyle:{float:"right"},attrs:{layout:"total, prev, pager, next, jumper","page-size":e.pageSize,"current-page":e.page,total:e.total},on:{"current-change":e.handleCurrentChange}})],1),e._v(" "),o("el-dialog",{attrs:{title:"编辑",visible:e.editFormVisible,"close-on-click-modal":!1},on:{"update:visible":function(t){e.editFormVisible=t}},model:{value:e.editFormVisible,callback:function(t){e.editFormVisible=t},expression:"editFormVisible"}},[o("el-form",{ref:"editForm",attrs:{model:e.editForm,"label-width":"100px",rules:e.editFormRules}},[o("el-row",[o("el-col",{attrs:{span:12}},[o("el-form-item",{attrs:{label:"登录名",prop:"LoginName"}},[o("el-input",{attrs:{"auto-complete":"off"},model:{value:e.editForm.LoginName,callback:function(t){e.$set(e.editForm,"LoginName","string"==typeof t?t.trim():t)},expression:"editForm.LoginName"}})],1)],1),e._v(" "),o("el-col",{attrs:{span:12}},[o("el-form-item",{attrs:{label:"密码",prop:"Password"}},[o("el-input",{attrs:{"show-password":"","auto-complete":"off",clearable:""},model:{value:e.editForm.Password,callback:function(t){e.$set(e.editForm,"Password","string"==typeof t?t.trim():t)},expression:"editForm.Password"}})],1)],1),e._v(" "),o("el-col",{attrs:{span:12}},[o("el-form-item",{attrs:{label:"用户姓名",prop:"UserName"}},[o("el-input",{attrs:{"auto-complete":"off"},model:{value:e.editForm.UserName,callback:function(t){e.$set(e.editForm,"UserName","string"==typeof t?t.trim():t)},expression:"editForm.UserName"}})],1)],1),e._v(" "),o("el-col",{attrs:{span:12}},[o("el-form-item",{attrs:{label:"手机号码",prop:"Mobile"}},[o("el-input",{attrs:{maxlength:"11",type:"tel","auto-complete":"off"},model:{value:e.editForm.Mobile,callback:function(t){e.$set(e.editForm,"Mobile","string"==typeof t?t.trim():t)},expression:"editForm.Mobile"}})],1)],1),e._v(" "),0==e.globalUserGroupid?o("el-col",{attrs:{span:12}},[o("el-form-item",{attrs:{label:"所属分组",prop:"GroupId"}},[o("el-select",{staticClass:"selectrole",attrs:{clearable:"",filterable:"",placeholder:"请选择"},on:{change:e.groupChange},model:{value:e.editForm.GroupId,callback:function(t){e.$set(e.editForm,"GroupId",t)},expression:"editForm.GroupId"}},[o("el-option",{attrs:{label:"全部分组",value:0}}),e._v(" "),e._l(e.groups,function(e){return o("el-option",{key:e.GROUPID,attrs:{label:e.groupshortname,value:Number(e.GROUPID)}})})],2)],1)],1):e._e(),e._v(" "),o("el-col",{attrs:{span:12}},[o("el-form-item",{attrs:{label:"角色",prop:"RIDs"}},[o("el-select",{staticClass:"selectrole",attrs:{multiple:"",placeholder:"请选择角色"},model:{value:e.editForm.RIDs,callback:function(t){e.$set(e.editForm,"RIDs",t)},expression:"editForm.RIDs"}},e._l(e.showRoles,function(e){return o("el-option",{key:e.Id,attrs:{label:e.Name,value:e.Id}})}),1)],1)],1),e._v(" "),o("el-col",{attrs:{span:12}},[o("el-form-item",{attrs:{label:"性别"}},[o("el-radio-group",{model:{value:e.editForm.Gender,callback:function(t){e.$set(e.editForm,"Gender",t)},expression:"editForm.Gender"}},[o("el-radio",{staticClass:"radio",attrs:{label:"1"}},[e._v("男")]),e._v(" "),o("el-radio",{staticClass:"radio",attrs:{label:"2"}},[e._v("女")])],1)],1)],1),e._v(" "),o("el-col",{attrs:{span:12}},[o("el-form-item",{attrs:{label:"显示监测",prop:"IsShowinsite"}},[o("el-radio-group",{model:{value:e.editForm.IsShowinsite,callback:function(t){e.$set(e.editForm,"IsShowinsite",t)},expression:"editForm.IsShowinsite"}},[o("el-radio",{staticClass:"radio",attrs:{label:"True"}},[e._v("是")]),e._v(" "),o("el-radio",{staticClass:"radio",attrs:{label:"False"}},[e._v("否")])],1)],1)],1),e._v(" "),o("el-col",{attrs:{span:12}},[o("el-form-item",{attrs:{label:"单位名称",prop:"Company"}},[o("el-input",{attrs:{"auto-complete":"off"},model:{value:e.editForm.Company,callback:function(t){e.$set(e.editForm,"Company",t)},expression:"editForm.Company"}})],1)],1),e._v(" "),o("el-col",{attrs:{span:12}},[o("el-form-item",{attrs:{label:"职位",prop:"Position"}},[o("el-input",{attrs:{"auto-complete":"off"},model:{value:e.editForm.Position,callback:function(t){e.$set(e.editForm,"Position",t)},expression:"editForm.Position"}})],1)],1)],1)],1),e._v(" "),o("div",{staticClass:"dialog-footer",attrs:{slot:"footer"},slot:"footer"},[o("el-button",{nativeOn:{click:function(t){return e.cancelEditDialog(t)}}},[e._v("取消")]),e._v(" "),o("el-button",{attrs:{type:"primary",loading:e.editLoading},nativeOn:{click:function(t){return e.editSubmit(t)}}},[e._v("提交")])],1)],1),e._v(" "),o("el-dialog",{attrs:{title:"新增",visible:e.addFormVisible,"close-on-click-modal":!1},on:{"update:visible":function(t){e.addFormVisible=t}},model:{value:e.addFormVisible,callback:function(t){e.addFormVisible=t},expression:"addFormVisible"}},[o("el-form",{ref:"addForm",attrs:{model:e.addForm,"label-width":"100px",rules:e.addFormRules}},[o("el-row",[o("el-col",{attrs:{span:12}},[o("el-form-item",{attrs:{label:"登录名",prop:"LoginName"}},[o("el-input",{attrs:{"auto-complete":"off"},model:{value:e.addForm.LoginName,callback:function(t){e.$set(e.addForm,"LoginName","string"==typeof t?t.trim():t)},expression:"addForm.LoginName"}})],1)],1),e._v(" "),o("el-col",{attrs:{span:12}},[o("el-form-item",{attrs:{label:"密码",prop:"Password"}},[o("el-input",{attrs:{"show-password":"","auto-complete":"off"},model:{value:e.addForm.Password,callback:function(t){e.$set(e.addForm,"Password","string"==typeof t?t.trim():t)},expression:"addForm.Password"}})],1)],1),e._v(" "),o("el-col",{attrs:{span:12}},[o("el-form-item",{attrs:{label:"姓名",prop:"UserName"}},[o("el-input",{attrs:{"auto-complete":"off"},model:{value:e.addForm.UserName,callback:function(t){e.$set(e.addForm,"UserName","string"==typeof t?t.trim():t)},expression:"addForm.UserName"}})],1)],1),e._v(" "),o("el-col",{attrs:{span:12}},[o("el-form-item",{attrs:{label:"手机号码",prop:"Mobile"}},[o("el-input",{attrs:{maxlength:"11",type:"tel","auto-complete":"off"},model:{value:e.addForm.Mobile,callback:function(t){e.$set(e.addForm,"Mobile","string"==typeof t?t.trim():t)},expression:"addForm.Mobile"}})],1)],1),e._v(" "),0==e.globalUserGroupid?o("el-col",{attrs:{span:12}},[o("el-form-item",{attrs:{label:"所属分组",prop:"GroupId"}},[o("el-select",{staticClass:"selectrole",attrs:{clearable:"",filterable:"",placeholder:"请选择"},on:{change:e.groupChange},model:{value:e.addForm.GroupId,callback:function(t){e.$set(e.addForm,"GroupId",t)},expression:"addForm.GroupId"}},[o("el-option",{attrs:{label:"全部分组",value:0}}),e._v(" "),e._l(e.groups,function(e){return o("el-option",{key:e.GROUPID,attrs:{label:e.groupshortname,value:e.GROUPID}})})],2)],1)],1):e._e(),e._v(" "),o("el-col",{attrs:{span:12}},[o("el-form-item",{attrs:{label:"角色",prop:"RIDs"}},[o("el-select",{staticClass:"selectrole",attrs:{multiple:"",placeholder:"请选择角色"},model:{value:e.addForm.RIDs,callback:function(t){e.$set(e.addForm,"RIDs",t)},expression:"addForm.RIDs"}},e._l(e.showRoles,function(e){return o("el-option",{key:e.Id,attrs:{label:e.Name,value:e.Id}})}),1)],1)],1),e._v(" "),o("el-col",{attrs:{span:12}},[o("el-form-item",{attrs:{label:"性别"}},[o("el-radio-group",{model:{value:e.addForm.Gender,callback:function(t){e.$set(e.addForm,"Gender",t)},expression:"addForm.Gender"}},[o("el-radio",{staticClass:"radio",attrs:{label:1}},[e._v("男")]),e._v(" "),o("el-radio",{staticClass:"radio",attrs:{label:2}},[e._v("女")])],1)],1)],1),e._v(" "),o("el-col",{attrs:{span:12}},[o("el-form-item",{attrs:{label:"显示监测",prop:"IsShowinsite"}},[o("el-radio-group",{model:{value:e.addForm.IsShowinsite,callback:function(t){e.$set(e.addForm,"IsShowinsite",t)},expression:"addForm.IsShowinsite"}},[o("el-radio",{staticClass:"radio",attrs:{label:"True"}},[e._v("是")]),e._v(" "),o("el-radio",{staticClass:"radio",attrs:{label:"False"}},[e._v("否")])],1)],1)],1),e._v(" "),o("el-col",{attrs:{span:12}},[o("el-form-item",{attrs:{label:"单位名称",prop:"Company"}},[o("el-input",{attrs:{"auto-complete":"off"},model:{value:e.addForm.Company,callback:function(t){e.$set(e.addForm,"Company",t)},expression:"addForm.Company"}})],1)],1),e._v(" "),o("el-col",{attrs:{span:12}},[o("el-form-item",{attrs:{label:"职位",prop:"Position"}},[o("el-input",{attrs:{"auto-complete":"off"},model:{value:e.addForm.Position,callback:function(t){e.$set(e.addForm,"Position",t)},expression:"addForm.Position"}})],1)],1)],1)],1),e._v(" "),o("div",{staticClass:"dialog-footer",attrs:{slot:"footer"},slot:"footer"},[o("el-button",{nativeOn:{click:function(t){return e.cancelAddDialog(t)}}},[e._v("取消")]),e._v(" "),o("el-button",{attrs:{type:"primary",loading:e.addLoading},nativeOn:{click:function(t){return e.addSubmit(t)}}},[e._v("提交")])],1)],1)],1)],1)},staticRenderFns:[]};var p=o("VU/8")(u,c,!1,function(e){o("r1xd")},"data-v-04bdaa90",null);t.default=p.exports},r1xd:function(e,t){}});
//# sourceMappingURL=60.4beafeb3bbb5da9f908c.js.map