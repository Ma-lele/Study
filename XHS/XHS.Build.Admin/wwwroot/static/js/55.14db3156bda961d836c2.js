webpackJsonp([55],{IWIO:function(e,t){},qY9A:function(e,t,i){"use strict";Object.defineProperty(t,"__esModule",{value:!0});var a=i("mvHQ"),r=i.n(a),n=i("Xxa5"),s=i.n(n),o=i("woOf"),l=i.n(o),c=i("exGp"),d=i.n(c),u=i("H84J"),m=i("P9l9"),p=i("G+2a"),f=i("djO7"),g=i("4iyM"),b={components:{Toolbar:f.a,MarkMap:g.a},data:function(){return{buttonList:[],filters:{name:""},Groups:[],total:0,page:1,pageSize:10,listLoading:!1,editFormVisible:!1,editLineFormVisible:!1,editLoading:!1,editLineLoading:!1,editFormRules:{groupname:[{required:!0,message:"请输入分组名称",trigger:"blur"}],groupshortname:[{required:!0,message:"请输入分组简称",trigger:"blur"}],city:[{required:!0,message:"请选择城市",trigger:"blur"}]},editLineFormRules:{pm10warnline:[{required:!0,message:"请输入此项",trigger:"blur"}],pm2_5warnline:[{required:!0,message:"请输入此项",trigger:"blur"}],tspwarnline:[{required:!0,message:"请输入此项",trigger:"blur"}]},editForm:{GroupId:""},editLineForm:{GroupId:"",pm10warnline:"",pm2_5warnline:"",tspwarnline:""},canEdit:!1,canDelete:!1,citys:[],districts:[],isEdit:!1,canEditLine:!1}},methods:{callFunction:function(e){this.filters={name:e.search},this[e.Func].apply(this,e)},handleCurrentChange:function(e){this.getGroups(e)},getGroups:function(e){var t=this;this.page=e||1;var i={page:this.page,size:this.pageSize,keyword:this.filters.name};this.listLoading=!0,Object(m._103)(i).then(function(e){t.total=e.data.dataCount,t.Groups=e.data.data,t.listLoading=!1})},handleDel:function(e,t){var i=this,a=t;a?this.$confirm("确认删除该记录吗?","提示",{type:"warning"}).then(function(){i.listLoading=!0;var e={GroupId:a.GroupId};Object(m._205)(e).then(function(e){u.a.isEmt.format(e)?i.listLoading=!1:(i.listLoading=!1,e.success?(i.$message({message:"删除成功",type:"success"}),i.clearGroupSites()):i.$message({message:e.msg,type:"error"}),i.getGroups())})}).catch(function(){}):this.$message({message:"请选择要删除的一行数据！",type:"error"})},handleEdit:function(e,t){var i=this;return d()(s.a.mark(function e(){var a;return s.a.wrap(function(e){for(;;)switch(e.prev=e.next){case 0:if(a=t){e.next=4;break}return i.$message({message:"请选择要编辑的一行数据！",type:"error"}),e.abrupt("return");case 4:if(i.isEdit=!0,!(a.city>0)){e.next=11;break}return e.next=8,i.getAreas(a.city);case 8:i.editForm=l()({},a),e.next=12;break;case 11:i.editForm=l()({},a);case 12:0==i.editForm.district&&(i.editForm.district=""),i.editFormVisible=!0;case 14:case"end":return e.stop()}},e,i)}))()},handleEditLine:function(e,t){var i=this;return d()(s.a.mark(function e(){var a,r,n;return s.a.wrap(function(e){for(;;)switch(e.prev=e.next){case 0:if(a=t){e.next=4;break}return i.$message({message:"请选择要编辑的一行数据！",type:"error"}),e.abrupt("return");case 4:return i.editLineForm={},r={groupid:a.GROUPID},e.next=8,Object(m._104)(r);case 8:n=e.sent,i.isEdit=!0,i.editLineFormVisible=!0,i.$set(i.editLineForm,"GroupId",a.GROUPID),n.data.forEach(function(e){i.$set(i.editLineForm,e.key,e.value)});case 13:case"end":return e.stop()}},e,i)}))()},handleAdd:function(){this.isEdit=!1,this.editForm={},this.editFormVisible=!0},callEditMap:function(e){this.$set(this.editForm,"longitude",e.point.lng),this.$set(this.editForm,"latitude",e.point.lat)},editSubmit:function(){var e=this;this.$refs.editForm.validate(function(t){if(t){e.editLoading=!0;var i=l()({},e.editForm);i.district||(i.district=0),e.isEdit?Object(m._35)(i).then(function(t){if(u.a.isEmt.format(t))e.editLoading=!1;else{if(t.success)e.$message({message:"保存成功",type:"success"}),e.$refs.editForm.resetFields(),e.editFormVisible=!1,e.clearGroupSites();else{var i="保存失败";t.msg&&(i=t.msg),e.$message({message:i,type:"error"})}e.editLoading=!1}}):(i.status=0,Object(m.n)(i).then(function(t){if(u.a.isEmt.format(t))e.editLoading=!1;else if(t.success)e.editLoading=!1,e.$message({message:"新增成功",type:"success"}),e.$refs.editForm.resetFields(),e.editFormVisible=!1,e.clearGroupSites();else{var i="保存失败";t.msg&&(i=t.msg),e.$message({message:i,type:"error"})}}))}})},editLineSubmit:function(){var e=this;this.$refs.editLineForm.validate(function(t){if(t){e.editLineLoading=!0;var i=l()({},e.editLineForm);Object(m._36)(i).then(function(t){if(u.a.isEmt.format(t))e.editLineLoading=!1;else{if(t.success)e.$message({message:"保存成功",type:"success"}),e.$refs.editLineForm.resetFields(),e.editLineFormVisible=!1,e.getGroups();else{var i="保存失败";t.msg&&(i=t.msg),e.$message({message:i,type:"error"})}e.editLineLoading=!1}})}})},addSubmit:function(){alert(r()(valid)),this.$refs.editForm.validate(function(e){})},changeSwitch:function(e,t){var i=this,a=t.GROUPID;Object(m._255)(a).then(function(a){if(a.success)i.$message({message:"更新成功",type:"success"}),i.clearGroupSites();else{var r=t;r.status=0==t.status?1:0,i.Groups[e]=r,i.$message({message:"更新失败",type:"error"})}})},getCitys:function(e){var t=this,i={pid:e};Object(m.b)(i).then(function(e){e.success&&(t.citys=e.data)})},getAreas:function(e){var t=this;return d()(s.a.mark(function i(){var a;return s.a.wrap(function(i){for(;;)switch(i.prev=i.next){case 0:a={pid:e},Object(m.b)(a).then(function(e){e.success&&(t.districts=e.data)});case 2:case"end":return i.stop()}},i,t)}))()},changeCity:function(e){this.districts=[],""!=e&&this.getAreas(e),this.$set(this.editForm,"district","")}},mounted:function(){var e=this;this.getGroups(),this.getCitys(0);var t=window.sessionStorage.router?JSON.parse(window.sessionStorage.router):[];Object(p.a)(this.$route.path,t).forEach(function(t){"handleEdit"==t.Func?e.canEdit=!0:"handleDel"==t.Func?e.canDelete=!0:"handleEditLine"==t.Func?e.canEditLine=!0:e.buttonList.push(t)})}},h={render:function(){var e=this,t=e.$createElement,i=e._self._c||t;return i("section",[i("toolbar",{attrs:{buttonList:e.buttonList,placeholder:"分组名称、分组简称过滤"},on:{callFunction:e.callFunction}}),e._v(" "),i("el-table",{directives:[{name:"loading",rawName:"v-loading",value:e.listLoading,expression:"listLoading"}],staticStyle:{width:"100%"},attrs:{data:e.Groups,"row-class-name":e.$tableRowClassName}},[i("el-table-column",{attrs:{type:"index",width:"80"},scopedSlots:e._u([{key:"default",fn:function(t){return[i("div",{domProps:{textContent:e._s((e.page-1)*e.pageSize+1+t.$index)}})]}}])}),e._v(" "),i("el-table-column",{attrs:{prop:"groupname",label:"分组名称","min-width":"350"}}),e._v(" "),i("el-table-column",{attrs:{prop:"groupshortname",label:"分组简称","min-width":"200"}}),e._v(" "),i("el-table-column",{attrs:{prop:"CityName","min-width":"150",label:"城市"}}),e._v(" "),i("el-table-column",{attrs:{prop:"DistrictName","min-width":"150",label:"区"}}),e._v(" "),i("el-table-column",{attrs:{prop:"hastsp",label:"TSP模块","min-width":"150"},scopedSlots:e._u([{key:"default",fn:function(t){return[i("el-tag",{attrs:{effect:"dark",type:1==t.row.hastsp?"success":"info"}},[e._v("TSP")])]}}])}),e._v(" "),e.canEditLine?i("el-table-column",{attrs:{label:"扬尘超标线管理","min-width":"150"},scopedSlots:e._u([{key:"default",fn:function(t){return[e.canEditLine?i("el-button",{attrs:{plain:"",size:"small"},on:{click:function(i){return e.handleEditLine(t.$index,t.row)}}},[e._v("扬尘超标线")]):e._e()]}}],null,!1,3387671488)}):e._e(),e._v(" "),i("el-table-column",{attrs:{prop:"status",label:"状态","min-width":"180"},scopedSlots:e._u([{key:"default",fn:function(t){return[i("el-switch",{attrs:{"active-value":0,"active-text":"正常","inactive-value":1,"inactive-text":"冻结"},on:{change:function(i){return e.changeSwitch(t.$index,t.row)}},model:{value:t.row.status,callback:function(i){e.$set(t.row,"status",i)},expression:"scope.row.status"}})]}}])}),e._v(" "),e.canEdit||e.canDelete?i("el-table-column",{attrs:{label:"操作",width:"150"},scopedSlots:e._u([{key:"default",fn:function(t){return[e.canEdit?i("el-button",{attrs:{plain:"",type:"warning",size:"small"},on:{click:function(i){return e.handleEdit(t.$index,t.row)}}},[e._v("编辑")]):e._e()]}}],null,!1,3982963594)}):e._e()],1),e._v(" "),i("el-col",{staticClass:"toolbar",attrs:{span:24}},[i("el-pagination",{staticStyle:{float:"right"},attrs:{layout:"total, prev, pager, next, jumper","page-size":e.pageSize,"current-page":e.page,total:e.total},on:{"current-change":e.handleCurrentChange}})],1),e._v(" "),i("el-dialog",{attrs:{title:"编辑",visible:e.editFormVisible,"close-on-click-modal":!1},on:{"update:visible":function(t){e.editFormVisible=t}},model:{value:e.editFormVisible,callback:function(t){e.editFormVisible=t},expression:"editFormVisible"}},[i("el-form",{ref:"editForm",attrs:{model:e.editForm,"label-width":"100px",rules:e.editFormRules}},[i("el-row",[i("el-col",{attrs:{span:12}},[i("el-form-item",{attrs:{label:"分组名称",prop:"groupname"}},[i("el-input",{attrs:{maxlength:"40","auto-complete":"off"},model:{value:e.editForm.groupname,callback:function(t){e.$set(e.editForm,"groupname",t)},expression:"editForm.groupname"}})],1)],1),e._v(" "),i("el-col",{attrs:{span:12}},[i("el-form-item",{attrs:{label:"分组简称",prop:"groupshortname"}},[i("el-input",{attrs:{maxlength:"6","auto-complete":"off"},model:{value:e.editForm.groupshortname,callback:function(t){e.$set(e.editForm,"groupshortname",t)},expression:"editForm.groupshortname"}})],1)],1)],1),e._v(" "),i("el-row",[i("el-col",{attrs:{span:12}},[i("el-col",{attrs:{span:24}},[i("el-form-item",{attrs:{label:"所属机构",prop:"belongto"}},[i("el-input",{attrs:{maxlength:"50","auto-complete":"off",placeholder:"示范片区必填"},model:{value:e.editForm.belongto,callback:function(t){e.$set(e.editForm,"belongto",t)},expression:"editForm.belongto"}})],1)],1),e._v(" "),i("el-col",{attrs:{span:24}},[i("el-form-item",{attrs:{label:"城市",prop:"city"}},[i("el-select",{attrs:{filterable:"",placeholder:"请选择"},on:{change:e.changeCity},model:{value:e.editForm.city,callback:function(t){e.$set(e.editForm,"city",t)},expression:"editForm.city"}},e._l(e.citys,function(e){return i("el-option",{key:e.RegionId,attrs:{label:e.RegionName,value:e.RegionId}})}),1)],1)],1),e._v(" "),i("el-col",{attrs:{span:24}},[i("el-form-item",{attrs:{label:"区",prop:"district"}},[i("el-select",{attrs:{filterable:"",clearable:"",placeholder:"请选择"},model:{value:e.editForm.district,callback:function(t){e.$set(e.editForm,"district",t)},expression:"editForm.district"}},e._l(e.districts,function(e){return i("el-option",{key:e.RegionId,attrs:{label:e.RegionName,value:e.RegionId}})}),1)],1)],1),e._v(" "),i("el-col",{attrs:{span:24}},[i("el-row",[i("el-col",{attrs:{span:16}},[i("el-form-item",{attrs:{label:"经纬度",prop:"longitude"}},[i("el-input",{attrs:{"auto-complete":"off",maxlength:"30",placeholder:"示范片区必填"},model:{value:e.editForm.longitude,callback:function(t){e.$set(e.editForm,"longitude",t)},expression:"editForm.longitude"}})],1)],1),e._v(" "),i("el-col",{attrs:{span:8}},[i("el-form-item",{attrs:{prop:"latitude","label-width":"0"}},[i("el-input",{attrs:{"auto-complete":"off",maxlength:"30"},model:{value:e.editForm.latitude,callback:function(t){e.$set(e.editForm,"latitude",t)},expression:"editForm.latitude"}})],1)],1)],1)],1),e._v(" "),i("el-col",{attrs:{span:24}},[i("el-form-item",{attrs:{label:"TSP模块",prop:"hastsp"}},[i("el-switch",{attrs:{"active-text":"是","inactive-text":"否","active-value":1,"inactive-value":0},model:{value:e.editForm.hastsp,callback:function(t){e.$set(e.editForm,"hastsp",t)},expression:"editForm.hastsp"}})],1)],1)],1),e._v(" "),i("el-col",{attrs:{span:12}},[e.editFormVisible?i("mark-map",{ref:"myMap",attrs:{markposition:{lng:e.editForm.longitude,lat:e.editForm.latitude}},on:{callMap:e.callEditMap}}):e._e()],1)],1),e._v(" "),i("el-row"),e._v(" "),i("el-form-item",{attrs:{label:"网站名称",prop:"logo"}},[i("el-input",{attrs:{"auto-complete":"off",maxlength:"100"},model:{value:e.editForm.logo,callback:function(t){e.$set(e.editForm,"logo",t)},expression:"editForm.logo"}})],1),e._v(" "),i("el-form-item",{attrs:{label:"简介",prop:"summary"}},[i("el-input",{attrs:{type:"textarea","auto-complete":"off",maxlength:"2000",rows:"7"},model:{value:e.editForm.summary,callback:function(t){e.$set(e.editForm,"summary",t)},expression:"editForm.summary"}})],1)],1),e._v(" "),i("div",{staticClass:"dialog-footer",attrs:{slot:"footer"},slot:"footer"},[i("el-button",{nativeOn:{click:function(t){e.editFormVisible=!1}}},[e._v("取消")]),e._v(" "),i("el-button",{attrs:{type:"primary",loading:e.editLoading},nativeOn:{click:function(t){return e.editSubmit(t)}}},[e._v("提交")])],1)],1),e._v(" "),i("el-dialog",{attrs:{title:"扬尘告警线设置",visible:e.editLineFormVisible,"close-on-click-modal":!1,width:"30%"},on:{"update:visible":function(t){e.editLineFormVisible=t}},model:{value:e.editLineFormVisible,callback:function(t){e.editLineFormVisible=t},expression:"editLineFormVisible"}},[i("el-form",{ref:"editLineForm",attrs:{model:e.editLineForm,rules:e.editLineFormRules}},[i("el-row",[i("el-col",{attrs:{span:24}},[i("el-form-item",{attrs:{label:"PM2.5超标线",prop:"pm2_5warnline"}},[i("el-input",{attrs:{maxlength:"50","auto-complete":"off",type:"number"},model:{value:e.editLineForm.pm2_5warnline,callback:function(t){e.$set(e.editLineForm,"pm2_5warnline",t)},expression:"editLineForm.pm2_5warnline"}})],1)],1)],1),e._v(" "),i("el-row",[i("el-col",{attrs:{span:24}},[i("el-form-item",{attrs:{label:"PM10超标线",prop:"pm10warnline"}},[i("el-input",{attrs:{maxlength:"50","auto-complete":"off",type:"number"},model:{value:e.editLineForm.pm10warnline,callback:function(t){e.$set(e.editLineForm,"pm10warnline",t)},expression:"editLineForm.pm10warnline"}})],1)],1)],1),e._v(" "),i("el-row",[i("el-col",{attrs:{span:24}},[i("el-form-item",{attrs:{label:"TSP超标线",prop:"tspwarnline"}},[i("el-input",{attrs:{maxlength:"50","auto-complete":"off",type:"number"},model:{value:e.editLineForm.tspwarnline,callback:function(t){e.$set(e.editLineForm,"tspwarnline",t)},expression:"editLineForm.tspwarnline"}})],1)],1)],1)],1),e._v(" "),i("div",{staticClass:"dialog-footer",attrs:{slot:"footer"},slot:"footer"},[i("el-button",{nativeOn:{click:function(t){e.editLineFormVisible=!1}}},[e._v("取消")]),e._v(" "),i("el-button",{attrs:{type:"primary",loading:e.editLineLoading},nativeOn:{click:function(t){return e.editLineSubmit(t)}}},[e._v("提交")])],1)],1)],1)},staticRenderFns:[]};var v=i("VU/8")(b,h,!1,function(e){i("IWIO")},"data-v-0eac5514",null);t.default=v.exports}});
//# sourceMappingURL=55.14db3156bda961d836c2.js.map