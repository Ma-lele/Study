(window["webpackJsonp"]=window["webpackJsonp"]||[]).push([["chunk-477dba6d"],{"0abd":function(e,t,n){},2017:function(e,t,n){"use strict";var r=n("cafe"),i=n.n(r);i.a},"25f0":function(e,t,n){"use strict";var r=n("6eeb"),i=n("825a"),o=n("d039"),a=n("ad6d"),s="toString",l=RegExp.prototype,c=l[s],u=o((function(){return"/a/b"!=c.call({source:"a",flags:"b"})})),d=c.name!=s;(u||d)&&r(RegExp.prototype,s,(function(){var e=i(this),t=String(e.source),n=e.flags,r=String(void 0===n&&e instanceof RegExp&&!("flags"in l)?a.call(e):n);return"/"+t+"/"+r}),{unsafe:!0})},"7d0d":function(e,t,n){"use strict";var r=n("0abd"),i=n.n(r);i.a},"9ed6":function(e,t,n){"use strict";n.r(t);var r=function(){var e=this,t=e.$createElement,n=e._self._c||t;return n("div",{directives:[{name:"loading",rawName:"v-loading",value:e.singleLoading,expression:"singleLoading"}],staticClass:"login-container",attrs:{"element-loading-text":"登录中...","element-loading-background":"rgba(255, 255, 255, 0.7)"}},[n("el-form",{ref:"loginForm",staticClass:"login-form",attrs:{model:e.loginForm,rules:e.loginRules,"auto-complete":"on","label-position":"left"}},[n("div",{staticClass:"title-container"},[n("h3",{staticClass:"title"},[e._v(e._s(e.logintitle))])]),n("el-form-item",{staticClass:"input",attrs:{prop:"username"}},[n("span",{staticClass:"svg-container"},[e._v(" 账户 ")]),n("el-input",{ref:"username",attrs:{placeholder:"用户名",name:"username",type:"text",tabindex:"1","auto-complete":"on"},model:{value:e.loginForm.username,callback:function(t){e.$set(e.loginForm,"username",t)},expression:"loginForm.username"}})],1),n("el-form-item",{staticClass:"input",attrs:{prop:"password"}},[n("span",{staticClass:"svg-container"},[e._v(" 密码 ")]),n("el-input",{key:e.passwordType,ref:"password",attrs:{type:e.passwordType,placeholder:"密码",name:"password",tabindex:"2","auto-complete":"on"},nativeOn:{keyup:function(t){return!t.type.indexOf("key")&&e._k(t.keyCode,"enter",13,t.key,"Enter")?null:e.handleLogin(t)}},model:{value:e.loginForm.password,callback:function(t){e.$set(e.loginForm,"password",t)},expression:"loginForm.password"}}),n("span",{staticClass:"show-pwd",on:{click:e.showPwd}},[n("svg-icon",{attrs:{"icon-class":"password"===e.passwordType?"eye":"eye-open"}})],1)],1),0==e.noVerifyCode?n("el-form-item",{staticClass:"input",attrs:{prop:"regverifyCode"}},[n("span",{staticClass:"svg-container"},[e._v(" 验证码 ")]),n("span",{staticClass:"mobile-msg"},[e._v(e._s(e.mobileMsg))]),n("el-input",{ref:"regverifyCode",attrs:{placeholder:"短信验证码",name:"regverifyCode",type:"text",tabindex:"3","auto-complete":"on"},model:{value:e.loginForm.regverifyCode,callback:function(t){e.$set(e.loginForm,"regverifyCode",t)},expression:"loginForm.regverifyCode"}}),n("span",{class:"get-code "+(e.countdown>0?"disable":""),on:{click:e.getVerifyCode}},[e._v(" "+e._s(e.verifytext)+" ")])],1):e._e(),n("el-button",{staticClass:"btn",attrs:{loading:e.loading,type:"primary"},nativeOn:{click:function(t){return t.preventDefault(),e.handleLogin(t)}}},[e._v("登录")])],1)],1)},i=[],o=(n("b0c0"),n("d3b7"),n("25f0"),n("498a"),n("96cf"),n("1da1")),a=n("5530"),s=n("2f62"),l=n("365c"),c={name:"Login",data:function(){var e=function(e,t,n){t.length<6?n(new Error("密码不少于6位")):n()};return{singleLoading:!1,loginForm:{username:"",password:""},loginRules:{username:[{required:!0,trigger:"blur",message:"请输入用户名"}],password:[{required:!0,trigger:"blur",validator:e}]},loading:!1,passwordType:"password",redirect:void 0,logintitle:"  ",query:this.$route.query,countdown:-1,verifytext:"获取验证码",timer:null,mobileMsg:"",noVerifyCode:0}},watch:{$route:{handler:function(e){this.redirect=e.query&&e.query.redirect},immediate:!0}},created:function(){this.getLoginTitle(),"SingleSignOn"==this.$route.name&&(this.singleLoading=!0,this.handleSingleLogin())},methods:Object(a["a"])(Object(a["a"])({},Object(s["b"])(["login","singlelogin"])),{},{showPwd:function(){var e=this;"password"===this.passwordType?this.passwordType="":this.passwordType="password",this.$nextTick((function(){e.$refs.password.focus()}))},handleLogin:function(){var e=this;this.$refs.loginForm.validate(function(){var t=Object(o["a"])(regeneratorRuntime.mark((function t(n){var r,i,o;return regeneratorRuntime.wrap((function(t){while(1)switch(t.prev=t.next){case 0:if(!n){t.next=14;break}if(e.loginForm.regverifyCode||0!=e.noVerifyCode){t.next=4;break}return e.$message.error("请输入验证码"),t.abrupt("return",!1);case 4:return e.loading=!0,t.next=7,e.login(e.loginForm);case 7:r=t.sent,i=r.success,o=r.msg,i?e.$router.push({path:e.redirect||"/"}):e.$message.error(o),e.loading=!1,t.next=15;break;case 14:return t.abrupt("return",!1);case 15:case"end":return t.stop()}}),t)})));return function(e){return t.apply(this,arguments)}}())},handleSingleLogin:function(){var e=this;return Object(o["a"])(regeneratorRuntime.mark((function t(){var n,r,i;return regeneratorRuntime.wrap((function(t){while(1)switch(t.prev=t.next){case 0:return t.next=2,e.singlelogin(e.query);case 2:n=t.sent,r=n.success,i=n.msg,r?e.$router.push({path:e.redirect||"/"}):(e.singleLoading=!1,e.$message.error(i));case 6:case"end":return t.stop()}}),t)})))()},getLoginTitle:function(){var e=this;return Object(o["a"])(regeneratorRuntime.mark((function t(){var n,r,i;return regeneratorRuntime.wrap((function(t){while(1)switch(t.prev=t.next){case 0:return t.next=2,l["a"].getLoginTitle();case 2:n=t.sent,r=n.success,i=n.data,r?(e.logintitle=i.title,e.noVerifyCode=i.noverifycode):e.logintitle="智慧工地安全监管平台";case 6:case"end":return t.stop()}}),t)})))()},maskMobile:function(e){var t=e;return e&&e.trim().length>7&&(t=t.substr(0,2)+"***"+t.substr(t.length-4)),t},getVerifyCode:function(){var e=this;this.countdown>0||this.$refs.loginForm.validate(function(){var t=Object(o["a"])(regeneratorRuntime.mark((function t(n){var r,i,o,a,s;return regeneratorRuntime.wrap((function(t){while(1)switch(t.prev=t.next){case 0:if(!n){t.next=10;break}return t.next=3,l["a"].verifycode({loginName:e.loginForm.username,password:e.loginForm.password});case 3:r=t.sent,i=r.success,o=r.msg,a=r.data,i?(s=e.maskMobile(a.mobile),e.mobileMsg="已发送到"+s,e.countdown=66,e.timer=setInterval((function(){if(e.countdown--,e.countdown<=0)return e.verifytext="获取验证码",e.mobileMsg="",clearInterval(e.timer),void(e.timer=null);e.verifytext=e.countdown.toString()+"s 后重获验证码"}),1e3)):e.$message.error(o),t.next=11;break;case 10:return t.abrupt("return",!1);case 11:case"end":return t.stop()}}),t)})));return function(e){return t.apply(this,arguments)}}())}})},u=c,d=(n("2017"),n("7d0d"),n("2877")),g=Object(d["a"])(u,r,i,!1,null,"5ab20daf",null);t["default"]=g.exports},cafe:function(e,t,n){}}]);