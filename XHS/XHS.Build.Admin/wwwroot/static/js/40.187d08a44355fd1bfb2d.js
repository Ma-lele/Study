webpackJsonp([40],{"7X2J":function(e,t){},Lshs:function(e,t,a){"use strict";Object.defineProperty(t,"__esModule",{value:!0});var i=a("+6Bu"),l=a.n(i),s=a("woOf"),r=a.n(s),o=a("Xxa5"),n=a.n(o),d=a("exGp"),c=a.n(d),u=a("P9l9"),m=a("ywL6"),p=a("G+2a"),g=a("djO7"),f=a("JLHL"),h=a("KxDp"),F={components:{leftgroup:m.a,Toolbar:g.a},mixins:[f.a],data:function(){return{groupCounts:[],buttonList:[],filters:{name:""},builds:[],total:0,page:1,pageSize:10,listLoading:!1,addDialogFormVisible:!1,editFormVisible:!1,editLoading:!1,editFormRules:{GROUPID:[{required:!0,message:"请选择监测对象分组",trigger:"blur"}],SITEID:[{required:!0,message:"请选择监测对象",trigger:"blur"}],HFWID:[{required:!0,message:"请选择高支模",trigger:"blur"}],hfwaname:[{required:!0,message:"请填写区域名称",trigger:"blur"}],imgurl:[{required:!0,message:"请上传图片",trigger:"blur"}]},editForm:{imgurl:[]},addFormVisible:!1,addLoading:!1,addFormRules:{GROUPID:[{required:!0,message:"请选择监测对象分组",trigger:"blur"}],SITEID:[{required:!0,message:"请选择监测对象",trigger:"blur"}],HFWID:[{required:!0,message:"请选择高支模",trigger:"blur"}],hfwaname:[{required:!0,message:"请填写区域名称",trigger:"blur"}],imgurl:[{required:!0,message:"请上传图片",trigger:"blur"}]},addForm:{imgurl:[]},canEdit:!1,canDelete:!1,groups:[],selectSites:[],hfws:[],selectGroupId:0,mfogtype:h.b}},computed:{uploadEditDisabled:function(){return this.editForm.imgurl.length>0},uploadAddDisabled:function(){return this.addForm.imgurl.length>0}},methods:{handleRemoveImgFile:function(){this.$refs.imgFileUpload.clearFiles(),this.addForm.imgurl=[],this.addForm.fileid=""},handleEditRemoveImgFile:function(){this.$refs.imgEditFileUpload.clearFiles(),this.editForm.imgurl=[],this.editForm.fileid=""},removeImgFile:function(e,t){this.addForm.imgurl=[],this.addForm.fileid=""},removeEditImgFile:function(e,t){this.editForm.imgurl=[],this.editForm.fileid=""},uploadImgFile:function(e){var t=this,a=e.file,i=new FormData;i.append("formFiles",a),Object(u._196)(i).then(function(e){t.addForm.fileid=e.data}).catch(function(e){})},uploadEditImgFile:function(e){var t=this,a=e.file,i=new FormData;i.append("formFiles",a),Object(u._196)(i).then(function(e){t.editForm.fileid=e.data}).catch(function(e){})},imgFileChanged:function(e,t){this.addForm.imgurl=t},imgEditFileChanged:function(e,t){this.editForm.imgurl=t},callbackGroup:function(e){this.selectGroupId=e.GROUPID,this.page=1,this.getPageList()},callFunction:function(e){this.filters={name:e.search},this[e.Func].apply(this,e)},getPageList:function(e){var t=this;this.page=e||1;var a={groupid:this.selectGroupId,keyword:this.filters.name,page:this.page,size:this.pageSize};this.listLoading=!0,Object(u._173)(a).then(function(e){e.success&&(t.builds=e.data.data,t.total=e.data.dataCount),t.listLoading=!1})},handleCurrentChange:function(e){this.getPageList(e)},getGroupCounts:function(){var e=this;Object(u._172)().then(function(t){t.success&&(e.groupCounts=t.data)})},groupChange:function(e){this.getGroupSites(e),this.$set(this.addForm,"SITEID",""),this.$set(this.editForm,"SITEID",""),this.selectSites=this.groupSites,this.$set(this.addForm,"HFWID",""),this.$set(this.editForm,"HFWID",""),this.hfws=[]},siteChange:function(e){var t=this;return c()(n.a.mark(function a(){var i;return n.a.wrap(function(a){for(;;)switch(a.prev=a.next){case 0:return t.$set(t.addForm,"HFWID",""),t.$set(t.editForm,"HFWID",""),t.hfws=[],a.next=5,Object(u._175)({siteid:e});case 5:i=a.sent,t.hfws=i.data;case 7:case"end":return a.stop()}},a,t)}))()},handleDel:function(e,t){var a=this,i=t;i?this.$confirm("确认删除该记录吗?","提示",{type:"warning"}).then(function(){a.listLoading=!0;var e=i;Object(u._223)(e).then(function(e){a.listLoading=!1,e.success?a.$message({message:"删除成功",type:"success"}):a.$message({message:e.msg,type:"error"}),a.getPageList(a.page),a.getGroupCounts()})}).catch(function(){}):this.$message({message:"请选择要删除的一行数据！",type:"error"})},handleEdit:function(e,t){var a=this;return c()(n.a.mark(function e(){var i,l;return n.a.wrap(function(e){for(;;)switch(e.prev=e.next){case 0:if(i=t){e.next=4;break}return a.$message({message:"请选择要编辑的一行数据！",type:"error"}),e.abrupt("return");case 4:return a.editFormVisible=!0,a.groupChange(i.GROUPID),a.editForm=r()({},i),a.$set(a.editForm,"bactive",i.bactive),a.$set(a.editForm,"imgurl",[{url:i.filepath}]),e.next=11,Object(u._175)({siteid:i.SITEID});case 11:l=e.sent,a.hfws=l.data;case 13:case"end":return e.stop()}},e,a)}))()},handleAdd:function(){this.addFormVisible=!0,this.addForm={imgurl:[]}},editSubmit:function(){var e=this;this.$refs.editForm.validate(function(t){if(t){e.editLoading=!0;var a=r()({},e.editForm);Object(u._64)(a).then(function(t){t.success?(e.$message({message:"修改成功",type:"success"}),e.$refs.editForm.resetFields(),e.editFormVisible=!1,e.getPageList(e.page),e.getGroupCounts()):e.$message({message:t.msg,type:"error"}),e.editLoading=!1})}})},addSubmit:function(){var e=this;this.$refs.addForm.validate(function(t){if(t){e.addLoading=!0;var a=r()({},e.addForm),i=(a.imgurl,l()(a,["imgurl"]));Object(u.R)(i).then(function(t){t.success?(e.$message({message:"新增成功",type:"success"}),e.$refs.addForm.resetFields(),e.addFormVisible=!1,e.getPageList(e.page),e.getGroupCounts()):e.$message({message:t.msg,type:"error"}),e.addLoading=!1})}})},handleOpenFile:function(e){window.open(e.url)}},mounted:function(){var e=this;this.getPageList(),this.getGroupCounts(),this.getGroups();var t=window.sessionStorage.router?JSON.parse(window.sessionStorage.router):[];Object(p.a)(this.$route.path,t).forEach(function(t){"handleEdit"==t.Func?e.canEdit=!0:"handleDel"==t.Func?e.canDelete=!0:e.buttonList.push(t)})}},b={render:function(){var e=this,t=e.$createElement,a=e._self._c||t;return a("el-container",[a("el-aside",{directives:[{name:"show",rawName:"v-show",value:e.$getUserGroupSee(),expression:"$getUserGroupSee()"}],staticClass:"left-group-aside"},[a("leftgroup",{attrs:{grouplist:e.groupCounts},on:{callGroup:e.callbackGroup}})],1),e._v(" "),a("el-main",[a("toolbar",{attrs:{buttonList:e.buttonList,placeholder:"监测对象、高支模名称、设备编号、区域名称过滤"},on:{callFunction:e.callFunction}}),e._v(" "),a("el-table",{directives:[{name:"loading",rawName:"v-loading",value:e.listLoading,expression:"listLoading"}],staticStyle:{width:"100%"},attrs:{data:e.builds,"row-class-name":e.$tableRowClassName}},[a("el-table-column",{attrs:{type:"index",width:"80"},scopedSlots:e._u([{key:"default",fn:function(t){return[a("div",{domProps:{textContent:e._s((e.page-1)*e.pageSize+1+t.$index)}})]}}])}),e._v(" "),a("el-table-column",{attrs:{prop:"siteshortname",label:"监测对象"}}),e._v(" "),a("el-table-column",{attrs:{prop:"bactive",label:"是否当前区域",align:"center"},scopedSlots:e._u([{key:"default",fn:function(t){return[a("el-tag",{attrs:{effect:"dark",type:"1"==t.row.bactive?"success":"danger","disable-transitions":""}},[e._v(e._s(1==t.row.bactive?"是":"否"))])]}}])}),e._v(" "),a("el-table-column",{attrs:{prop:"hfwname",label:"高支模名称"}}),e._v(" "),a("el-table-column",{attrs:{prop:"hfwcode",label:"设备编号"}}),e._v(" "),a("el-table-column",{attrs:{prop:"hfwaname",label:"区域名称"}}),e._v(" "),e.canEdit||e.canDelete?a("el-table-column",{attrs:{label:"操作",width:"150"},scopedSlots:e._u([{key:"default",fn:function(t){return[e.canEdit?a("el-button",{attrs:{plain:"",type:"warning",size:"small"},on:{click:function(a){return e.handleEdit(t.$index,t.row)}}},[e._v("编辑")]):e._e(),e._v(" "),e.canDelete?a("el-button",{attrs:{plain:"",type:"danger",size:"small"},on:{click:function(a){return e.handleDel(t.$index,t.row)}}},[e._v("删除")]):e._e()]}}],null,!1,917012342)}):e._e()],1),e._v(" "),a("el-col",{staticClass:"toolbar",attrs:{span:24}},[a("el-pagination",{staticStyle:{float:"right"},attrs:{layout:"total, prev, pager, next, jumper","page-size":e.pageSize,"current-page":e.page,total:e.total},on:{"current-change":e.handleCurrentChange}})],1),e._v(" "),a("el-dialog",{staticClass:"file-dialog",attrs:{title:"编辑",visible:e.editFormVisible,"close-on-click-modal":!1},on:{"update:visible":function(t){e.editFormVisible=t}},model:{value:e.editFormVisible,callback:function(t){e.editFormVisible=t},expression:"editFormVisible"}},[a("el-form",{ref:"editForm",attrs:{model:e.editForm,"label-width":"120px",rules:e.editFormRules}},[a("el-row",[a("el-col",{attrs:{span:12}},[a("el-form-item",{attrs:{label:"监测对象分组",prop:"GROUPID"}},[a("el-select",{attrs:{filterable:"",placeholder:"请选择"},on:{change:e.groupChange},model:{value:e.editForm.GROUPID,callback:function(t){e.$set(e.editForm,"GROUPID",t)},expression:"editForm.GROUPID"}},e._l(e.groups,function(e){return a("el-option",{key:e.GROUPID,attrs:{label:e.groupshortname,value:e.GROUPID}})}),1)],1)],1),e._v(" "),a("el-col",{attrs:{span:12}},[a("el-form-item",{attrs:{label:"监测对象",prop:"SITEID"}},[a("el-select",{attrs:{filterable:"",placeholder:"请选择"},on:{change:e.siteChange},model:{value:e.editForm.SITEID,callback:function(t){e.$set(e.editForm,"SITEID",t)},expression:"editForm.SITEID"}},e._l(e.selectSites,function(e){return a("el-option",{key:e.SITEID,attrs:{label:e.siteshortname,value:e.SITEID}})}),1)],1)],1),e._v(" "),a("el-col",{attrs:{span:12}},[a("el-form-item",{attrs:{label:"高支模",prop:"HFWID"}},[a("el-select",{attrs:{filterable:"",placeholder:"请选择"},model:{value:e.editForm.HFWID,callback:function(t){e.$set(e.editForm,"HFWID",t)},expression:"editForm.HFWID"}},e._l(e.hfws,function(e){return a("el-option",{key:e.HFWID,attrs:{label:e.hfwname,value:e.HFWID}})}),1)],1)],1),e._v(" "),a("el-col",{attrs:{span:12}},[a("el-form-item",{attrs:{label:"区域名称",prop:"hfwaname"}},[a("el-input",{attrs:{"auto-complete":"off",maxlength:"2    0"},model:{value:e.editForm.hfwaname,callback:function(t){e.$set(e.editForm,"hfwaname",t)},expression:"editForm.hfwaname"}})],1)],1),e._v(" "),a("el-col",{attrs:{span:12}},[a("el-form-item",{attrs:{label:"上传图片",prop:"imgurl"}},[a("el-upload",{ref:"imgEditFileUpload",staticClass:"upload-demo",class:{disabled:e.uploadEditDisabled},attrs:{action:"fakeaction",accept:".jpg","list-type":"picture-card",multiple:!1,limit:1,"file-list":e.editForm.imgurl,"on-change":e.imgEditFileChanged,"http-request":e.uploadEditImgFile,"on-remove":e.removeEditImgFile},scopedSlots:e._u([{key:"file",fn:function(t){var i=t.file;return a("div",{},[a("el-image",{staticClass:"\n                                            el-upload-list__item-thumbnail\n                                        ",attrs:{src:i.url,fit:"cover"}}),e._v(" "),a("span",{staticClass:"el-upload-list__item-actions"},[a("span",{staticClass:"\n                                                el-upload-list__item-preview\n                                            ",on:{click:function(t){return e.handleOpenFile(i)}}},[a("i",{staticClass:"el-icon-zoom-in"})]),e._v(" "),a("span",{staticClass:"\n                                                el-upload-list__item-delete\n                                            ",on:{click:function(t){return e.handleEditRemoveImgFile(i)}}},[a("i",{staticClass:"el-icon-delete"})])])],1)}}])},[a("i",{staticClass:"el-icon-plus",attrs:{slot:"default"},slot:"default"})])],1)],1),e._v(" "),a("el-col",{attrs:{span:12}},[a("el-form-item",{attrs:{label:"设为当前区域",prop:"bactive"}},[a("el-switch",{attrs:{"active-value":1,"inactive-value":0},model:{value:e.editForm.bactive,callback:function(t){e.$set(e.editForm,"bactive",t)},expression:"editForm.bactive"}})],1)],1)],1)],1),e._v(" "),a("div",{staticClass:"dialog-footer",attrs:{slot:"footer"},slot:"footer"},[a("el-button",{nativeOn:{click:function(t){e.editFormVisible=!1}}},[e._v("取消")]),e._v(" "),a("el-button",{attrs:{type:"primary",loading:e.editLoading},nativeOn:{click:function(t){return e.editSubmit(t)}}},[e._v("提交")])],1)],1),e._v(" "),a("el-dialog",{staticClass:"file-dialog",attrs:{title:"新增",visible:e.addFormVisible,"close-on-click-modal":!1},on:{"update:visible":function(t){e.addFormVisible=t}},model:{value:e.addFormVisible,callback:function(t){e.addFormVisible=t},expression:"addFormVisible"}},[a("el-form",{ref:"addForm",attrs:{model:e.addForm,"label-width":"120px",rules:e.addFormRules}},[a("el-row",[a("el-col",{attrs:{span:12}},[a("el-form-item",{attrs:{label:"监测对象分组",prop:"GROUPID"}},[a("el-select",{attrs:{filterable:"",placeholder:"请选择"},on:{change:e.groupChange},model:{value:e.addForm.GROUPID,callback:function(t){e.$set(e.addForm,"GROUPID",t)},expression:"addForm.GROUPID"}},e._l(e.groups,function(e){return a("el-option",{key:e.GROUPID,attrs:{label:e.groupshortname,value:e.GROUPID}})}),1)],1)],1),e._v(" "),a("el-col",{attrs:{span:12}},[a("el-form-item",{attrs:{label:"监测对象",prop:"SITEID"}},[a("el-select",{attrs:{filterable:"",placeholder:"请选择"},on:{change:e.siteChange},model:{value:e.addForm.SITEID,callback:function(t){e.$set(e.addForm,"SITEID",t)},expression:"addForm.SITEID"}},e._l(e.selectSites,function(e){return a("el-option",{key:e.SITEID,attrs:{label:e.siteshortname,value:e.SITEID}})}),1)],1)],1),e._v(" "),a("el-col",{attrs:{span:12}},[a("el-form-item",{attrs:{label:"高支模",prop:"HFWID"}},[a("el-select",{attrs:{filterable:"",placeholder:"请选择"},model:{value:e.addForm.HFWID,callback:function(t){e.$set(e.addForm,"HFWID",t)},expression:"addForm.HFWID"}},e._l(e.hfws,function(e){return a("el-option",{key:e.HFWID,attrs:{label:e.hfwname,value:e.HFWID}})}),1)],1)],1),e._v(" "),a("el-col",{attrs:{span:12}},[a("el-form-item",{attrs:{label:"区域名称",prop:"hfwaname"}},[a("el-input",{attrs:{"auto-complete":"off",maxlength:"20"},model:{value:e.addForm.hfwaname,callback:function(t){e.$set(e.addForm,"hfwaname",t)},expression:"addForm.hfwaname"}})],1)],1),e._v(" "),a("el-col",{attrs:{span:12}},[a("el-form-item",{attrs:{label:"上传图片",prop:"imgurl"}},[a("el-upload",{ref:"imgFileUpload",staticClass:"upload-demo",class:{disabled:e.uploadAddDisabled},attrs:{action:"fakeaction",accept:".jpg","list-type":"picture-card",multiple:!1,limit:1,"file-list":e.addForm.imgurl,"on-change":e.imgFileChanged,"http-request":e.uploadImgFile,"on-remove":e.removeImgFile},scopedSlots:e._u([{key:"file",fn:function(t){var i=t.file;return a("div",{},[a("el-image",{staticClass:"\n                                            el-upload-list__item-thumbnail\n                                        ",attrs:{src:i.url,fit:"cover"}}),e._v(" "),a("span",{staticClass:"el-upload-list__item-actions"},[a("span",{staticClass:"\n                                                el-upload-list__item-preview\n                                            ",on:{click:function(t){return e.handleOpenFile(i)}}},[a("i",{staticClass:"el-icon-zoom-in"})]),e._v(" "),a("span",{staticClass:"\n                                                el-upload-list__item-delete\n                                            ",on:{click:function(t){return e.handleRemoveImgFile(i)}}},[a("i",{staticClass:"el-icon-delete"})])])],1)}}])},[a("i",{staticClass:"el-icon-plus",attrs:{slot:"default"},slot:"default"})])],1)],1),e._v(" "),a("el-col",{attrs:{span:12}},[a("el-form-item",{attrs:{label:"设为当前区域",prop:"bactive"}},[a("el-switch",{attrs:{"active-value":"1","inactive-value":"0"},model:{value:e.addForm.bactive,callback:function(t){e.$set(e.addForm,"bactive",t)},expression:"addForm.bactive"}})],1)],1)],1)],1),e._v(" "),a("div",{staticClass:"dialog-footer",attrs:{slot:"footer"},slot:"footer"},[a("el-button",{nativeOn:{click:function(t){e.addFormVisible=!1}}},[e._v("取消")]),e._v(" "),a("el-button",{attrs:{type:"primary",loading:e.addLoading},nativeOn:{click:function(t){return e.addSubmit(t)}}},[e._v("提交")])],1)],1)],1)],1)},staticRenderFns:[]};var v=a("VU/8")(F,b,!1,function(e){a("7X2J")},"data-v-407fc0a9",null);t.default=v.exports}});
//# sourceMappingURL=40.187d08a44355fd1bfb2d.js.map