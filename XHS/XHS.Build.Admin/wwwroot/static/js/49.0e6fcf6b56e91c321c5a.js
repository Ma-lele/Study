webpackJsonp([49],{"Erx+":function(e,t){},Nx9f:function(e,t,s){"use strict";Object.defineProperty(t,"__esModule",{value:!0});var a=s("mvHQ"),n=s.n(a),i=(s("H84J"),s("P9l9")),r=1e3,l={data:function(){return{roles:[],roleid:0,data5:[],btns:[],assigns:[],checked1:!1,loadingSaveStr:"保存",loadingSave:!1,assignBtns:[],defaultProps:{children:"children",label:"label",btns:"btns"},selectedPermissions:[],currentRoleCode:"",stores:{role:{data:[]},permissionTree:{data:[]}},buttonProps:{type:"default",size:"small"},selectRole:{},menuData:[],menuSelections:[],menuLoading:!1,authLoading:!1,checkAll:!1,currentRoleMenus:[]}},computed:{filterRoles:function(){return this.roles.filter(function(e){return!e.issys})}},methods:{getRoles:function(){var e=this;Object(i._121)().then(function(t){e.roles=t.data,e.getPermissions()})},getPermissions:function(){var e=this,t=this;Object(i._118)({needbtn:!1}).then(function(s){t.loadingSave=!1,t.loadingSaveStr="保存",e.data=s.data.children,e.data5=JSON.parse(n()(e.data))})},getPermissionIds:function(e){var t=this,s=this;this.assigns=[],this.assignBtns=[];var a={rid:e};Object(i._117)(a).then(function(e){s.loadingSave=!1,s.loadingSaveStr="保存",t.$refs.tree.setCheckedKeys(e.data.permissionids),t.assignBtns=e.data.assignbtns})},operate:function(e){this.loadingSave=!0,this.loadingSaveStr="加载中...",this.roleid=e,this.getPermissionIds(e)},saveAssign:function(){var e=this,t=this;this.loadingSave=!0,this.loadingSaveStr="保存中...";var s=this.$refs.tree.getCheckedKeys();try{if(this.assignBtns.length>0)for(var a=0;a<this.assignBtns.length;a++){var n=this.assignBtns[a];n&&s.push(n)}}catch(e){}var r={pids:s,rid:this.roleid};r&&r.pids.length>0?Object(i.w)(r).then(function(s){if(t.loadingSave=!1,t.loadingSaveStr="保存",s.success){e.$message({message:"保存成功",type:"success"});var a={rid:e.roleid};Object(i._117)(a).then(function(t){e.$refs.tree.setCheckedKeys(t.data.permissionids),e.assignBtns=t.data.assignbtns,e.$message({message:"数据更新成功",type:"success"})})}else e.$message({message:s.msg,type:"error"})}):(this.loadingSaveStr="保存",this.loadingSave=!1,this.$message({message:"参数错误",type:"error"}))},append:function(e){var t={id:r++,label:"testtest",children:[]};e.children||this.$set(e,"children",[]),e.children.push(t)},remove:function(e,t){var s=e.parent,a=s.data.children||s.data,n=a.findIndex(function(e){return e.id===t.id});a.splice(n,1)},subButtonChange:function(e,t){e&&this.$refs.tree.setChecked(t,!0)},checkChange:function(e,t){if(!t&&e.btns)for(var s=0;s<e.btns.length;s++){var a=this.assignBtns.indexOf(e.btns[s].value);a>-1&&this.assignBtns.splice(a,1)}if(null!==e.Pid)if(!0===t)this.$refs.tree.setChecked(e.Pid,!0);else{var n=this.$refs.tree.getNode(e.Pid),i=!1;if(n)for(var r=0,l=n.childNodes.length;r<l;r++)if(!0===n.childNodes[r].checked){i=!0;break}i||this.$refs.tree.setChecked(e.Pid,!1)}if(null!=e.children&&!1===t)for(var o=0,d=e.children.length;o<d;o++){var c=this.$refs.tree.getNode(e.children[o].value);c&&!0===c.checked&&this.$refs.tree.setChecked(c.data.value,!1)}}},mounted:function(){this.loadingSave=!0,this.loadingSaveStr="加载中...",this.getRoles()}},o={render:function(){var e=this,t=e.$createElement,s=e._self._c||t;return s("section",[s("el-col",{staticClass:"toolbar roles",attrs:{span:8}},[s("el-card",{staticClass:"box-card left-aside"},[s("div",{staticClass:"clearfix",attrs:{slot:"header"},slot:"header"},[s("span",[e._v("权限")]),e._v(" "),s("el-button",{staticStyle:{float:"right",padding:"3px 0"},attrs:{type:"text"},on:{click:e.getRoles}},[e._v("刷新")])],1),e._v(" "),e._l(e.roles,function(t){return s("div",{key:t.Id,staticClass:"left-item",class:t.Id==e.roleid?"active":"",on:{click:function(s){return e.operate(t.Id)}}},[s("div",[s("i",{staticClass:"el-icon-price-tag"}),e._v("\n                    "+e._s(t.Name)+"\n                ")])])})],2)],1),e._v(" "),s("el-col",{staticClass:"toolbar perms morechildren",attrs:{span:16}},[s("el-card",{staticClass:"box-card"},[s("div",{staticClass:"clearfix",attrs:{slot:"header"},slot:"header"},[s("span",[e._v("菜单")]),e._v(" "),s("el-button",{staticStyle:{float:"right",padding:"3px 0"},attrs:{loading:e.loadingSave,type:"text"},on:{click:e.saveAssign}},[e._v(e._s(e.loadingSaveStr))])],1),e._v(" "),s("div",{staticClass:"block"},[s("el-tree",{ref:"tree",attrs:{data:e.data5,"show-checkbox":"","node-key":"value","default-expand-all":"","expand-on-click-node":!0,"check-strictly":!0},on:{"check-change":e.checkChange},scopedSlots:e._u([{key:"default",fn:function(t){var a=t.node,n=t.data;return s("span",{staticClass:"custom-tree-node"},[s("span",{staticClass:"node-label"},[e._v(e._s(a.label))]),e._v(" "),s("span",{staticClass:"node-btn"},[s("el-checkbox-group",{model:{value:e.assignBtns,callback:function(t){e.assignBtns=t},expression:"assignBtns"}},e._l(n.btns,function(t){return s("el-checkbox",{key:t.value,attrs:{label:t.value.toString(),disabled:!!t.disabled},on:{change:function(t){return e.subButtonChange(t,n)}}},[e._v("\n                                    "+e._s(t.label)+"\n                                ")])}),1)],1)])}}])})],1)])],1)],1)},staticRenderFns:[]};var d=s("VU/8")(l,o,!1,function(e){s("Erx+")},null,null);t.default=d.exports}});
//# sourceMappingURL=49.0e6fcf6b56e91c321c5a.js.map