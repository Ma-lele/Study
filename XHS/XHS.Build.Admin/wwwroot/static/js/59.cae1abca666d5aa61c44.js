webpackJsonp([59],{"49FN":function(t,e){},"7wHb":function(t,e,n){"use strict";Object.defineProperty(e,"__esModule",{value:!0});var i=n("P9l9"),r={data:function(){return{baseApiUrl:i.a+/log/}},mounted:function(){var t=document.getElementById("contentIframe"),e=document.documentElement.clientWidth,n=document.documentElement.clientHeight;t.style.width=Number(e)-250+"px",t.style.height=Number(n)-160+"px"},methods:{forward:function(){history.forward()},reverse:function(){history.back()}}},o={render:function(){var t=this.$createElement,e=this._self._c||t;return e("div",{staticStyle:{height:"auto"}},[e("div",[e("el-button",{attrs:{size:"mini",icon:"el-icon-back"},on:{click:this.reverse}}),e("el-button",{attrs:{size:"mini",icon:"el-icon-right"},on:{click:this.forward}})],1),this._v(" "),e("iframe",{staticClass:"logframe",staticStyle:{width:"100%",height:"100%"},attrs:{id:"contentIframe",src:this.baseApiUrl,frameborder:"0"}})])},staticRenderFns:[]};var c=n("VU/8")(r,o,!1,function(t){n("49FN")},null,null);e.default=c.exports}});
//# sourceMappingURL=59.cae1abca666d5aa61c44.js.map