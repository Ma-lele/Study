webpackJsonp([14],{FHZq:function(e,t){},nZix:function(e,t,a){"use strict";Object.defineProperty(t,"__esModule",{value:!0});var r=a("woOf"),l=a.n(r),o=a("P9l9"),s=a("4iyM"),d=a("JLHL"),i=a("KxDp"),n={components:{MarkMap:s.a},mixins:[d.a],data:function(){var e=this;return{groups:[],sitetypes:[{text:"建筑工地",value:172},{text:"市政工程",value:188}],citys:[],districts:[],addLoading:!1,addFormRules:{groupname:[{required:!0,message:"请输入分组名称",trigger:"blur"}],groupshortname:[{required:!0,message:"请输入分组简称",trigger:"blur"}],city:[{required:!0,message:"请选择城市",trigger:"blur"}],sitelng:[{required:!0,validator:function(t,a,r){var l=e.addForm.sitelat;return a&&l?r():r(new Error("请填写经纬度"))},trigger:"blur"}]},addForm:{},mprovider:i.c,sglx:[],pickerOptionsStart:{disabledDate:function(t){if(e.addForm.enddate)return t.getTime()>new Date(e.addForm.enddate).getTime()}},pickerOptionsEnd:{disabledDate:function(t){if(e.addForm.startdate)return t.getTime()<new Date(e.addForm.startdate).getTime()}}}},methods:{changeCity:function(e){var t=this.groups.filter(function(t){return t.GROUPID==e});t&&t.length>0&&(this.addForm.sitecity=t[0].city,this.districts=[],t[0].city>0&&this.getAreas(t[0].city,t[0].district))},callAddMap:function(e){this.$set(this.addForm,"sitelng",e.point.lng),this.$set(this.addForm,"sitelat",e.point.lat),this.$set(this.addForm,"siteaddr",e.address)},getCitys:function(e){var t=this,a={pid:e};Object(o.b)(a).then(function(e){e.success&&(t.citys=e.data)})},getAreas:function(e,t){var a=this,r={pid:e};Object(o.b)(r).then(function(e){e.success&&(a.districts=e.data,a.addForm.sitearea=t||"")})},cancelAdd:function(){this.$router.replace("/site/sites")},addSubmit:function(){var e=this;this.$refs.addForm.validate(function(t){if(t){e.addLoading=!0;var a=l()({},e.addForm);Object(o.z)(a).then(function(t){t.success?(e.$message({message:"新增成功",type:"success"}),e.cancelAdd()):e.$message({message:t.msg,type:"error"}),e.addLoading=!1})}})}},mounted:function(){this.getGroups(),this.getCitys(0),this.sglx=this.$store.state.options.sglx}},c={render:function(){var e=this,t=e.$createElement,a=e._self._c||t;return a("section",[a("el-row",{staticStyle:{"margin-left":"0","margin-right":"0"},attrs:{gutter:10}},[a("el-col",{attrs:{span:18}},[a("el-form",{ref:"addForm",attrs:{model:e.addForm,"label-width":"120px",rules:e.addFormRules}},[a("el-row",[a("el-col",{attrs:{span:8}},[a("el-form-item",{attrs:{label:"监测对象名称",prop:"sitename"}},[a("el-input",{attrs:{maxlength:"50",placeholder:"请输入监测对象名称","auto-complete":"off"},model:{value:e.addForm.sitename,callback:function(t){e.$set(e.addForm,"sitename",t)},expression:"addForm.sitename"}})],1)],1),e._v(" "),a("el-col",{attrs:{span:8}},[a("el-form-item",{attrs:{label:"监测对象简称",prop:"siteshortname"}},[a("el-input",{attrs:{placeholder:"请输入监测对象简称",maxlength:"12","show-word-limit":"","auto-complete":"off"},model:{value:e.addForm.siteshortname,callback:function(t){e.$set(e.addForm,"siteshortname",t)},expression:"addForm.siteshortname"}})],1)],1),e._v(" "),a("el-col",{attrs:{span:8}},[a("el-form-item",{attrs:{label:"监测类别",prop:"sitetype"}},[a("el-select",{attrs:{filterable:"",placeholder:"请选择"},model:{value:e.addForm.sitetype,callback:function(t){e.$set(e.addForm,"sitetype",t)},expression:"addForm.sitetype"}},e._l(e.sitetypes,function(e){return a("el-option",{key:e.value,attrs:{label:e.text,value:e.value}})}),1)],1)],1)],1),e._v(" "),a("el-row",[a("el-col",{attrs:{span:8}},[a("el-form-item",{attrs:{label:"所属分组",prop:"GROUPID"}},[a("el-select",{attrs:{filterable:"",placeholder:"请选择"},on:{change:e.changeCity},model:{value:e.addForm.GROUPID,callback:function(t){e.$set(e.addForm,"GROUPID",t)},expression:"addForm.GROUPID"}},e._l(e.groups,function(e){return a("el-option",{key:e.GROUPID,attrs:{label:e.groupshortname,value:e.GROUPID}})}),1)],1)],1),e._v(" "),a("el-col",{attrs:{span:8}},[a("el-form-item",{attrs:{label:"所属区",prop:"sitecity"}},[a("el-select",{attrs:{disabled:"",filterable:"",placeholder:"请选择"},model:{value:e.addForm.sitecity,callback:function(t){e.$set(e.addForm,"sitecity",t)},expression:"addForm.sitecity"}},e._l(e.citys,function(e){return a("el-option",{key:e.RegionId,attrs:{label:e.RegionName,value:e.RegionId}})}),1)],1)],1),e._v(" "),a("el-col",{attrs:{span:8}},[a("el-form-item",{attrs:{prop:"sitearea"}},[a("el-select",{attrs:{filterable:"",placeholder:"请选择"},model:{value:e.addForm.sitearea,callback:function(t){e.$set(e.addForm,"sitearea",t)},expression:"addForm.sitearea"}},e._l(e.districts,function(e){return a("el-option",{key:e.RegionId,attrs:{label:e.RegionName,value:e.RegionId}})}),1)],1)],1)],1),e._v(" "),a("el-row",[a("el-col",{attrs:{span:8}},[a("el-form-item",{attrs:{label:"监测对象地址",prop:"siteaddr"}},[a("el-input",{attrs:{"auto-complete":"off",maxlength:"50"},model:{value:e.addForm.siteaddr,callback:function(t){e.$set(e.addForm,"siteaddr",t)},expression:"addForm.siteaddr"}})],1)],1),e._v(" "),a("el-col",{attrs:{span:8}},[a("el-form-item",{attrs:{label:"经纬度",prop:"sitelng"}},[a("el-row",{attrs:{type:"flex",justify:"space-between"}},[a("el-col",{attrs:{span:11}},[a("el-input",{attrs:{"auto-complete":"off",type:"tel"},model:{value:e.addForm.sitelng,callback:function(t){e.$set(e.addForm,"sitelng",t)},expression:"addForm.sitelng"}})],1),e._v(" "),a("el-col",{attrs:{span:11}},[a("el-input",{attrs:{"auto-complete":"off",type:"tel"},model:{value:e.addForm.sitelat,callback:function(t){e.$set(e.addForm,"sitelat",t)},expression:"addForm.sitelat"}})],1)],1)],1)],1),e._v(" "),a("el-col",{attrs:{span:8}},[a("el-form-item",{attrs:{label:"项目编号",prop:"sitecode"}},[a("el-input",{attrs:{"auto-complete":"off",type:"tel",maxlength:"50"},model:{value:e.addForm.sitecode,callback:function(t){e.$set(e.addForm,"sitecode",t)},expression:"addForm.sitecode"}})],1)],1)],1),e._v(" "),a("el-row",[a("el-col",{attrs:{span:8}},[a("el-form-item",{attrs:{label:"开始日期",prop:"startdate"}},[a("el-date-picker",{attrs:{type:"date","picker-options":e.pickerOptionsStart,placeholder:"选择开始日期"},model:{value:e.addForm.startdate,callback:function(t){e.$set(e.addForm,"startdate",t)},expression:"addForm.startdate"}})],1)],1),e._v(" "),a("el-col",{attrs:{span:8}},[a("el-form-item",{attrs:{label:"结束日期",prop:"enddate"}},[a("el-date-picker",{attrs:{type:"date","picker-options":e.pickerOptionsEnd,placeholder:"选择结束日期"},model:{value:e.addForm.enddate,callback:function(t){e.$set(e.addForm,"enddate",t)},expression:"addForm.enddate"}})],1)],1),e._v(" "),a("el-col",{attrs:{span:8}},[a("el-form-item",{attrs:{label:"服务供应商",prop:"serviceprovider"}},[a("el-select",{attrs:{filterable:"",placeholder:"请选择"},model:{value:e.addForm.serviceprovider,callback:function(t){e.$set(e.addForm,"serviceprovider",t)},expression:"addForm.serviceprovider"}},e._l(e.mprovider,function(t){return a("el-option",{key:t,attrs:{label:e.mprovider[t],value:t}})}),1)],1)],1)],1),e._v(" "),a("el-row",[a("el-col",{attrs:{span:8}},[a("el-form-item",{attrs:{label:"项目经理",prop:"contact"}},[a("el-input",{attrs:{maxlength:"50","auto-complete":"off"},model:{value:e.addForm.contact,callback:function(t){e.$set(e.addForm,"contact",t)},expression:"addForm.contact"}})],1)],1),e._v(" "),a("el-col",{attrs:{span:8}},[a("el-form-item",{attrs:{label:"项目经理电话",prop:"tel"}},[a("el-input",{attrs:{maxlength:"50","auto-complete":"off"},model:{value:e.addForm.tel,callback:function(t){e.$set(e.addForm,"tel",t)},expression:"addForm.tel"}})],1)],1),e._v(" "),a("el-col",{attrs:{span:8}},[a("el-form-item",{attrs:{label:"项目经理身份证",prop:"contactidcard"}},[a("el-input",{attrs:{maxlength:"50","auto-complete":"off"},model:{value:e.addForm.contactidcard,callback:function(t){e.$set(e.addForm,"contactidcard",t)},expression:"addForm.contactidcard"}})],1)],1)],1),e._v(" "),a("el-row",[a("el-col",{attrs:{span:8}},[a("el-form-item",{attrs:{label:"建设单位",prop:"constructor1"}},[a("el-input",{attrs:{maxlength:"50","auto-complete":"off"},model:{value:e.addForm.constructor1,callback:function(t){e.$set(e.addForm,"constructor1",t)},expression:"addForm.constructor1"}})],1)],1),e._v(" "),a("el-col",{attrs:{span:8}},[a("el-form-item",{attrs:{label:"施工单位",prop:"builder"}},[a("el-input",{attrs:{maxlength:"50","auto-complete":"off"},model:{value:e.addForm.builder,callback:function(t){e.$set(e.addForm,"builder",t)},expression:"addForm.builder"}})],1)],1),e._v(" "),a("el-col",{attrs:{span:8}},[a("el-form-item",{attrs:{label:"施工类型",prop:"constructtype"}},[a("el-select",{attrs:{placeholder:"请选择"},model:{value:e.addForm.constructtype,callback:function(t){e.$set(e.addForm,"constructtype",t)},expression:"addForm.constructtype"}},e._l(e.sglx,function(e){return a("el-option",{key:e.DDID,attrs:{label:e.dataitem,value:e.DDID}})}),1)],1)],1)],1),e._v(" "),a("el-row",[a("el-col",{attrs:{span:8}},[a("el-form-item",{attrs:{label:"工程造价(万元)",prop:"sitecost"}},[a("el-input",{attrs:{maxlength:"50","auto-complete":"off"},model:{value:e.addForm.sitecost,callback:function(t){e.$set(e.addForm,"sitecost",t)},expression:"addForm.sitecost"}})],1)],1),e._v(" "),a("el-col",{attrs:{span:8}},[a("el-form-item",{attrs:{label:"占地面积",prop:"floorarea"}},[a("el-input",{attrs:{maxlength:"50","auto-complete":"off"},model:{value:e.addForm.floorarea,callback:function(t){e.$set(e.addForm,"floorarea",t)},expression:"addForm.floorarea"}})],1)],1),e._v(" "),a("el-col",{attrs:{span:8}},[a("el-form-item",{attrs:{label:"建筑面积",prop:"buildingarea"}},[a("el-input",{attrs:{maxlength:"50","auto-complete":"off"},model:{value:e.addForm.buildingarea,callback:function(t){e.$set(e.addForm,"buildingarea",t)},expression:"addForm.buildingarea"}})],1)],1)],1),e._v(" "),a("el-row",[a("el-col",{attrs:{span:8}},[a("el-form-item",{attrs:{label:"是否市管项目",prop:"bcityctrl"}},[a("el-select",{attrs:{filterable:"",placeholder:"请选择"},model:{value:e.addForm.bcityctrl,callback:function(t){e.$set(e.addForm,"bcityctrl",t)},expression:"addForm.bcityctrl"}},[a("el-option",{key:"1",attrs:{label:"是",value:"1"}}),e._v(" "),a("el-option",{key:"0",attrs:{label:"否",value:"0"}})],1)],1)],1),e._v(" "),a("el-col",{attrs:{span:8}},[a("el-form-item",{attrs:{label:"是否推送到市里",prop:"bpush"}},[a("el-select",{attrs:{filterable:"",placeholder:"请选择"},model:{value:e.addForm.bpush,callback:function(t){e.$set(e.addForm,"bpush",t)},expression:"addForm.bpush"}},[a("el-option",{key:"1",attrs:{label:"是",value:"1"}}),e._v(" "),a("el-option",{key:"0",attrs:{label:"否",value:"0"}})],1)],1)],1),e._v(" "),a("el-col",{attrs:{span:8}},[a("el-form-item",{attrs:{label:"信用等级",prop:"siterate"}},[a("el-input",{attrs:{maxlength:"50","auto-complete":"off"},model:{value:e.addForm.siterate,callback:function(t){e.$set(e.addForm,"siterate",t)},expression:"addForm.siterate"}})],1)],1)],1)],1)],1),e._v(" "),a("el-col",{attrs:{span:6}},[a("mark-map",{on:{callMap:e.callAddMap}})],1)],1),e._v(" "),a("div",{staticStyle:{"text-align":"center"}},[a("el-button",{nativeOn:{click:function(t){return e.cancelAdd(t)}}},[e._v("返回")]),e._v(" "),a("el-button",{attrs:{type:"primary",loading:e.addLoading},nativeOn:{click:function(t){return e.addSubmit(t)}}},[e._v("提交")])],1)],1)},staticRenderFns:[]};var m=a("VU/8")(n,c,!1,function(e){a("FHZq")},"data-v-e186a238",null);t.default=m.exports}});
//# sourceMappingURL=14.3e6017b43e29039d8dc6.js.map