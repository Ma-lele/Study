webpackJsonp([47],{"/KQF":function(e,t){},"8cH8":function(e,t,s){"use strict";Object.defineProperty(t,"__esModule",{value:!0});var a=s("mvHQ"),n=s.n(a),o=s("P9l9"),r={components:{},data:function(){return{userSiteListData:[],multipleSelection:[],roles:[],companys:[],positions:[],tableForm:{},listLoading:!1,saveLoading:!1}},mounted:function(){this.getUserSiteList(this.$route.params.id)},methods:{getUserSiteList:function(e){var t=this;this.listLoading=!0,Object(o._150)({USERID:e}).then(function(e){e.success?(t.tableForm.listData=e.data,t.listLoading=!1):t.listLoading=!1})},handleSelectionChange:function(e){this.multipleSelection=e;for(var t=0;t<this.tableForm.listData.length;t++)if(e.length>0&&this.tableForm.listData[t].USERID==e[e.length-1]){this.$set(this.tableForm.listData[t],"bstartfog",0);break}},filterHandler:function(e,t,s){return t[s.property]===e},handleCheckChange:function(e,t){e||(this.tableForm.listData[t].bstartfog=0,this.tableForm.listData[t].bsms=0,this.tableForm.listData[t].bwarn=0)},changeFog:function(e){var t=this.tableForm.listData[e].bstartfog;this.tableForm.listData[e].bstartfog=1==t?0:1},changeSms:function(e){var t=this.tableForm.listData[e].bsms;this.tableForm.listData[e].bsms=1==t?0:1},changeWarn:function(e){var t=this.tableForm.listData[e].bwarn;this.tableForm.listData[e].bwarn=1==t?0:1},saveUserSite:function(){var e=this;this.$refs.userSiteForm.validate(function(t){if(t){e.saveLoading=!0;for(var s={},a=["SITEIDS","bwarns","bsmss","bstartfogs","troubles","trouble2s","trouble3s","pms","unwashs","washoffs","washoff2s","washoff3s","specs","tipovers","nospec1s","cable1s","unloads","fences","helmets","strangers","trespassers","fires","overloads","cameraoffs","invades","soils","airtights","vests"],r=0;r<a.length;r++)s[a[r]]=[];for(var i=0;i<e.tableForm.listData.length;i++)if(1==e.tableForm.listData[i].bchecked)for(var l=0;l<a.length;l++){var c="000";"bsmss"!=a[l]&&"bwarns"!=a[l]&&"bstartfogs"!=a[l]||(c=0),s[a[l]].push(e.isNull(e.tableForm.listData[i][a[l].substring(0,a[l].length-1)],c))}var u={};u.USERID=e.$route.params.id;for(var b=0;b<a.length;b++)u[a[b]]=s[a[b]].join(",");Object(o._236)(u).then(function(t){if(t.success&&t.data>0){e.$message({message:"保存成功",type:"success"});var s=JSON.parse(sessionStorage.getItem("Tags")).filter(function(t){return t.path!=e.$route.path});e.$store.commit("saveTagsData",n()(s)),e.closeUserSite()}else e.$message({message:t.msg,type:"error"});e.saveLoading=!1})}})},closeUserSite:function(){this.$router.replace("/frontuser/index")},isNull:function(e,t){var s=t;return e&&(s=e),s}}},i={render:function(){var e=this,t=e.$createElement,s=e._self._c||t;return s("section",[s("el-row",[s("el-col",{attrs:{span:20,offset:2}},[s("el-form",{ref:"userSiteForm",attrs:{model:e.tableForm}},[s("el-table",{directives:[{name:"loading",rawName:"v-loading",value:e.listLoading,expression:"listLoading"}],ref:"userSiteListTbl",staticStyle:{width:"100%"},attrs:{data:e.tableForm.listData,"tooltip-effect":"dark",height:"calc(95vh - 160px)"},on:{"selection-change":e.handleSelectionChange}},[s("el-table-column",{attrs:{width:"55",prop:"bchecked"},scopedSlots:e._u([{key:"default",fn:function(t){return[s("el-form-item",[s("el-checkbox",{attrs:{name:"bchecked",disabled:2!=t.row.usersitetype,checked:1==e.tableForm.listData[t.$index].bchecked},on:{change:function(s){return e.handleCheckChange(s,t.$index)}},model:{value:t.row.bchecked,callback:function(s){e.$set(t.row,"bchecked",s)},expression:"scope.row.bchecked"}})],1)]}}])}),e._v(" "),s("el-table-column",{attrs:{label:"No",width:"100"},scopedSlots:e._u([{key:"default",fn:function(t){return[e._v(e._s(t.$index+1))]}}])}),e._v(" "),s("el-table-column",{attrs:{prop:"siteshortname",label:"监测对象简称","min-width":"3",sortable:""}}),e._v(" "),s("el-table-column",{attrs:{prop:"sitename",label:"监测对象名","min-width":"4",sortable:"","show-overflow-tooltip":""}}),e._v(" "),s("el-table-column",{attrs:{prop:"bstartfog",label:"启动雾炮","min-width":"1"},scopedSlots:e._u([{key:"default",fn:function(t){return[s("el-form-item",{attrs:{prop:"listData["+t.$index+"].bstartfog"}},["1"==t.row.bstartfog?s("el-button",{attrs:{type:"success",icon:"el-icon-check",circle:"",disabled:"1"!=t.row.bchecked},on:{click:function(s){return e.changeFog(t.$index)}}}):s("el-button",{attrs:{icon:"el-icon-close",circle:"",disabled:"1"!=t.row.bchecked},on:{click:function(s){return e.changeFog(t.$index)}}})],1)]}}])}),e._v(" "),s("el-table-column",{attrs:{prop:"bwarn",label:"接收告警","min-width":"1"},scopedSlots:e._u([{key:"default",fn:function(t){return[s("el-form-item",{attrs:{prop:"listData["+t.$index+"].bwarn"}},["1"==t.row.bwarn?s("el-button",{attrs:{type:"success",icon:"el-icon-check",circle:"",disabled:"1"!=t.row.bchecked},on:{click:function(s){return e.changeWarn(t.$index)}}}):s("el-button",{attrs:{icon:"el-icon-close",circle:"",disabled:"1"!=t.row.bchecked},on:{click:function(s){return e.changeWarn(t.$index)}}})],1)]}}])}),e._v(" "),s("el-table-column",{attrs:{prop:"bsms",label:"微信+短信","min-width":"1"},scopedSlots:e._u([{key:"default",fn:function(t){return[s("el-form-item",{attrs:{prop:"listData["+t.$index+"].bsms"}},["1"==t.row.bsms?s("el-button",{attrs:{type:"success",icon:"el-icon-check",circle:"",disabled:"1"!=t.row.bchecked},on:{click:function(s){return e.changeSms(t.$index)}}}):s("el-button",{attrs:{icon:"el-icon-close",circle:"",disabled:"1"!=t.row.bchecked},on:{click:function(s){return e.changeSms(t.$index)}}})],1)]}}])})],1),e._v(" "),s("el-form-item",{staticClass:"form-button mt20 mr10"},[s("el-button",{on:{click:e.closeUserSite}},[e._v("取消")]),e._v(" "),s("el-button",{attrs:{type:"primary",loading:e.saveLoading},on:{click:e.saveUserSite}},[e._v("保存")])],1)],1)],1)],1)],1)},staticRenderFns:[]};var l=s("VU/8")(r,i,!1,function(e){s("/KQF")},"data-v-1e43ed69",null);t.default=l.exports}});
//# sourceMappingURL=47.ab62a4835193a7ada2b7.js.map