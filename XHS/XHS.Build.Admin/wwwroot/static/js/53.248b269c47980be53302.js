webpackJsonp([53],{Dgkw:function(t,e,s){"use strict";Object.defineProperty(e,"__esModule",{value:!0});var a=s("mvHQ"),n=s.n(a),o=s("P9l9"),r={components:{},data:function(){return{userSiteListData:[],multipleSelection:[],roles:[],companys:[],positions:[],tableForm:{},listLoading:!1,saveLoading:!1}},mounted:function(){this.getUserSiteList(this.$route.params.id)},methods:{getUserSiteList:function(t){var e=this;this.listLoading=!0,Object(o._153)({USERID:t}).then(function(t){t.success?(e.tableForm.listData=t.data,e.listLoading=!1):e.listLoading=!1})},handleSelectionChange:function(t){this.multipleSelection=t;for(var e=0;e<this.tableForm.listData.length;e++)if(t.length>0&&this.tableForm.listData[e].USERID==t[t.length-1]){this.$set(this.tableForm.listData[e],"bstartfog",0);break}},filterHandler:function(t,e,s){return e[s.property]===t},handleCheckChange:function(t,e){t||(this.tableForm.listData[e].bstartfog=0,this.tableForm.listData[e].bsms=0,this.tableForm.listData[e].bwarn=0)},changeFog:function(t){var e=this.tableForm.listData[t].bstartfog;this.tableForm.listData[t].bstartfog=1==e?0:1},changeSms:function(t){var e=this.tableForm.listData[t].bsms;this.tableForm.listData[t].bsms=1==e?0:1},changeWarn:function(t){var e=this.tableForm.listData[t].bwarn;this.tableForm.listData[t].bwarn=1==e?0:1},saveUserSite:function(){var t=this;this.$refs.userSiteForm.validate(function(e){if(e){t.saveLoading=!0;for(var s={},a=["SITEIDS","bwarns","bsmss","bstartfogs","troubles","trouble2s","trouble3s","pms","unwashs","washoffs","washoff2s","washoff3s","specs","tipovers","nospec1s","cable1s","unloads","fences","helmets","strangers","trespassers","fires","overloads","cameraoffs","invades","soils","airtights","vests"],r=0;r<a.length;r++)s[a[r]]=[];for(var i=0;i<t.tableForm.listData.length;i++)if(1==t.tableForm.listData[i].bchecked)for(var l=0;l<a.length;l++){var c="000";"bsmss"!=a[l]&&"bwarns"!=a[l]&&"bstartfogs"!=a[l]||(c=0),s[a[l]].push(t.isNull(t.tableForm.listData[i][a[l].substring(0,a[l].length-1)],c))}var u={};u.USERID=t.$route.params.id;for(var b=0;b<a.length;b++)u[a[b]]=s[a[b]].join(",");Object(o._240)(u).then(function(e){if(e.success&&e.data>0){t.$message({message:"保存成功",type:"success"});var s=JSON.parse(sessionStorage.getItem("Tags")).filter(function(e){return e.path!=t.$route.path});t.$store.commit("saveTagsData",n()(s)),t.closeUserSite()}else t.$message({message:e.msg,type:"error"});t.saveLoading=!1})}})},closeUserSite:function(){this.$router.replace("/frontuser/index")},isNull:function(t,e){var s=e;return t&&(s=t),s}}},i={render:function(){var t=this,e=t.$createElement,s=t._self._c||e;return s("section",[s("el-row",[s("el-col",{attrs:{span:20,offset:2}},[s("el-form",{ref:"userSiteForm",attrs:{model:t.tableForm}},[s("el-table",{directives:[{name:"loading",rawName:"v-loading",value:t.listLoading,expression:"listLoading"}],ref:"userSiteListTbl",staticStyle:{width:"100%"},attrs:{data:t.tableForm.listData,"tooltip-effect":"dark",height:"calc(95vh - 160px)"},on:{"selection-change":t.handleSelectionChange}},[s("el-table-column",{attrs:{width:"55",prop:"bchecked"},scopedSlots:t._u([{key:"default",fn:function(e){return[s("el-form-item",[s("el-checkbox",{attrs:{name:"bchecked",disabled:2!=e.row.usersitetype,checked:1==t.tableForm.listData[e.$index].bchecked},on:{change:function(s){return t.handleCheckChange(s,e.$index)}},model:{value:e.row.bchecked,callback:function(s){t.$set(e.row,"bchecked",s)},expression:"scope.row.bchecked"}})],1)]}}])}),t._v(" "),s("el-table-column",{attrs:{label:"No",width:"100"},scopedSlots:t._u([{key:"default",fn:function(e){return[t._v(t._s(e.$index+1))]}}])}),t._v(" "),s("el-table-column",{attrs:{prop:"siteshortname",label:"监测对象简称","min-width":"3",sortable:""}}),t._v(" "),s("el-table-column",{attrs:{prop:"sitename",label:"监测对象名","min-width":"4",sortable:"","show-overflow-tooltip":""}}),t._v(" "),s("el-table-column",{attrs:{prop:"bstartfog",label:"启动雾炮","min-width":"1"},scopedSlots:t._u([{key:"default",fn:function(e){return[s("el-form-item",{attrs:{prop:"listData["+e.$index+"].bstartfog"}},["1"==e.row.bstartfog?s("el-button",{attrs:{type:"success",icon:"el-icon-check",circle:"",disabled:"1"!=e.row.bchecked},on:{click:function(s){return t.changeFog(e.$index)}}}):s("el-button",{attrs:{icon:"el-icon-close",circle:"",disabled:"1"!=e.row.bchecked},on:{click:function(s){return t.changeFog(e.$index)}}})],1)]}}])}),t._v(" "),s("el-table-column",{attrs:{prop:"bwarn",label:"接收告警","min-width":"1"},scopedSlots:t._u([{key:"default",fn:function(e){return[s("el-form-item",{attrs:{prop:"listData["+e.$index+"].bwarn"}},["1"==e.row.bwarn?s("el-button",{attrs:{type:"success",icon:"el-icon-check",circle:"",disabled:"1"!=e.row.bchecked},on:{click:function(s){return t.changeWarn(e.$index)}}}):s("el-button",{attrs:{icon:"el-icon-close",circle:"",disabled:"1"!=e.row.bchecked},on:{click:function(s){return t.changeWarn(e.$index)}}})],1)]}}])}),t._v(" "),s("el-table-column",{attrs:{prop:"bsms",label:"微信+短信","min-width":"1"},scopedSlots:t._u([{key:"default",fn:function(e){return[s("el-form-item",{attrs:{prop:"listData["+e.$index+"].bsms"}},["1"==e.row.bsms?s("el-button",{attrs:{type:"success",icon:"el-icon-check",circle:"",disabled:"1"!=e.row.bchecked},on:{click:function(s){return t.changeSms(e.$index)}}}):s("el-button",{attrs:{icon:"el-icon-close",circle:"",disabled:"1"!=e.row.bchecked},on:{click:function(s){return t.changeSms(e.$index)}}})],1)]}}])})],1),t._v(" "),s("el-form-item",{staticClass:"form-button mt20 mr10"},[s("el-button",{on:{click:t.closeUserSite}},[t._v("取消")]),t._v(" "),s("el-button",{attrs:{type:"primary",loading:t.saveLoading},on:{click:t.saveUserSite}},[t._v("保存")])],1)],1)],1)],1)],1)},staticRenderFns:[]};var l=s("VU/8")(r,i,!1,function(t){s("FBoz")},"data-v-132946dd",null);e.default=l.exports},FBoz:function(t,e){}});
//# sourceMappingURL=53.248b269c47980be53302.js.map