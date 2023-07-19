using System;
using System.Collections.Generic;
using System.Text;

namespace XHS.Build.Common
{
    /// <summary>
    /// 常量
    /// </summary>
    public partial class Const
    {
        /// <summary>
        /// 系统设置
        /// </summary>
        public class Setting
        {
            public const string YSToken = "YSToken";//萤石云Token
            public const string AQSToken = "AQSToken";//阿启视Token
            public const string S001 = "S001";//监管公司名称
            public const string S002 = "S002";//监管公司简称
            public const string S003 = "S003";//日间噪声报警值
            public const string S004 = "S004";//夜间噪声报警值
            public const string S005 = "S005";//联系邮箱
            public const string S006 = "S006";//初始密码
            public const string S007 = "S007";//网站名
            public const string S008 = "S008";//网站备案号
            public const string S009 = "S009";//管理员初始密码
            public const string S010 = "S010";//管理员角色ID
            public const string S011 = "S011";//考勤角色ID
            public const string S012 = "S012";//移动登录是否验证密码
            public const string S013 = "S013";//地图刷新间隔（分钟）
            public const string S014 = "S014";//存放巡查图片的文件夹
            public const string S015 = "S015";//存放巡查图片的网络路径
            public const string S016 = "S016";//存放巡查图片的物理路径
            public const string S017 = "S017";//word模板文件路径
            public const string S018 = "S018";//word文件导出临时文件夹路径
            public const string S019 = "S019";//需要获取实时AQI的城市
            public const string S020 = "S020";//车辆冲洗抓拍接口SecretKey
            public const string S021 = "S021";//获取实时AQI的appkey
            public const string S022 = "S022";//微信平台网站名
            public const string S023 = "S023";//存放平面设计图片的物理路径
            public const string S024 = "S024";//系统根目录
            public const string S025 = "S025";//实时数据保留时间(日)
            public const string S026 = "S026";//小时数据保留时间(月)
            public const string S027 = "S027";//萤石云appKey
            public const string S028 = "S028";//萤石云appSecret
            public const string S029 = "S029";//超标警报模板
            public const string S030 = "S030";//前台物理路径
            public const string S031 = "S031";//存放无人机照片的文件夹
            public const string S032 = "S032";//存放无人机视频的文件夹
            public const string S033 = "S033";//存放工程进度照片的文件夹
            public const string S034 = "S034";//存放无人机图片视频的网络路径
            public const string S035 = "S035";//和风天气API的key
            public const string S036 = "S036";//和风天气获取的城市列表
            public const string S037 = "S037";//中兴视频登录用户名
            public const string S038 = "S038";//中兴视频登录密码
            public const string S039 = "S039";//中兴视频服务器
            public const string S040 = "S040";//中兴视频服务器端口
            public const string S041 = "S041";//中兴视频客户端地址
            public const string S042 = "S042";//华为千里眼视频登录用户名
            public const string S043 = "S043";//华为千里眼视频登录密码
            public const string S044 = "S044";//华为千里眼视频服务器
            public const string S045 = "S045";//华为千里眼视频服务器端口
            public const string S046 = "S046";//海康7200视频注册服务器IP
            public const string S047 = "S047";//海康7200视频注册服务器端口
            public const string S048 = "S048";//海康7200视频流媒体服务器IP
            public const string S049 = "S049";//海康7200视频流媒体服务器端口
            public const string S050 = "S050";//海康7200视频码流类型0：主码流 1：子码流
            public const string S051 = "S051";//海康7200视频设备使用的服务线路
            public const string S052 = "S052";//海康7200视频登录用户名
            public const string S053 = "S053";//海康7200视频登录密码
            public const string S054 = "S054";//海康7200视频服务器地址
            public const string S055 = "S055";//后台个别画面特殊后缀
            public const string S056 = "S056";//存放特种设备照片的文件夹
            public const string S057 = "S057";//特种设备信息分类
            public const string S058 = "S058";//存放监测点相关照片的文件夹
            public const string S059 = "S059";//存放监测点相关照片分类
            public const string S060 = "S060";//车辆冲洗抓拍API
            public const string S061 = "S061";//阿启视视频服务器IP
            public const string S062 = "S062";//阿启视视频服务器端口
            public const string S063 = "S063";//阿启视视频用户
            public const string S064 = "S064";//海康8700流媒体服务器IP:端口
            public const string S065 = "S065";//特设实时数据保留件数
            public const string S066 = "S066";//特设报警延迟件数
            public const string S067 = "S067";//特设报警片段数据保留天数
            public const string S068 = "S068";//考勤接口API
            public const string S070 = "S070";//海康8200视频登录用户名
            public const string S071 = "S071";//海康8200视频Web登录密码
            public const string S072 = "S072";//海康8200视频服务器IP
            public const string S073 = "S073";//海康8200视频服务器Web端口
            public const string S074 = "S074";//海康8200视频服务器APP IP端口
            public const string S075 = "S075";//海康8200视频APP登录密码
            public const string S076 = "S076";//海康8200视频线路ID
            public const string S077 = "S077";//和风天气接口地址
            public const string S078 = "S078";//巡查问题类型
            public const string S079 = "S079";//大华8IP
            public const string S080 = "S080";//大华8端口
            public const string S081 = "S081";//大华8用户名
            public const string S082 = "S082";//大华8密码
            public const string S083 = "S083";//华为千里眼APP视频登录用户名
            public const string S084 = "S084";//华为千里眼APP视频登录密码
            public const string S085 = "S085";//华为千里眼APP视频服务器
            public const string S086 = "S086";//华为千里眼APP视频服务器端口
            public const string S087 = "S087";//全局文字替换From
            public const string S088 = "S088";//全局文字替换To
            public const string S089 = "S089";//南京城建Url
            public const string S090 = "S090";//南京城建Token
            public const string S091 = "S091";//南京城建salt
            public const string S092 = "S092";//金琥外接AppKey
            public const string S093 = "S093";//大华9IP
            public const string S094 = "S094";//大华9端口
            public const string S095 = "S095";//大华9用户名
            public const string S096 = "S096";//大华9密码
            public const string S097 = "S097";//郑州海康8200host
            public const string S098 = "S098";//郑州海康8200appKey
            public const string S099 = "S099";//郑州海康8200appSecret
            public const string S100 = "S100";//批处理报警周期(单位:小时)
            public const string S101 = "S101";//扬尘设备长时间离线小时定义2(单位:小时)
            public const string S102 = "S102";//扬尘设备长时间离线小时定义3(单位:小时)
            public const string S106 = "S106";//存放特种设备备案照片的文件夹
            public const string S107 = "S107";//吊钩视频地址端口
            public const string S108 = "S108";//吊钩视频用户名
            public const string S109 = "S109";//吊钩视频密码
            public const string S110 = "S110";//吊钩视频秘钥
            public const string S115 = "S115";//富友特种设备评估报告接口地址
            public const string S116 = "S116";//富友特种设备评估报告APPKEY
            public const string S117 = "S117";//富友特种设备评估报告APPSECRET
            public const string S118 = "S118";//大运考勤接口API
            public const string S119 = "S119";//防疫人员管理非主要字段json
            public const string S120 = "S120";//体温超标值
            public const string S121 = "S121";//冷丘钢丝绳API地址
            public const string S122 = "S122";//冷丘钢丝绳API用户名
            public const string S123 = "S123";//冷丘钢丝绳API密码
            public const string S124 = "S124";//宇科卸料平台API地址
            public const string S125 = "S125";//宇科卸料平台API用户名
            public const string S126 = "S126";//宇科卸料平台API密码
            public const string S127 = "S127";//安全帽平台API地址
            public const string S128 = "S128";//人车分流平台API地址
            public const string S129 = "S129";//陌生人识别平台API地址
            public const string S130 = "S130";//存放建邺工地进度excel的路径
            public const string S131 = "S131";//存放Notice素材路径
            public const string S132 = "S132";//存放江北工地进度安全交底图片和视频的路径
            public const string S133 = "S133";//视频AI解析获取token的地址
            public const string S134 = "S134";//火警分析平台API地址
            public const string S135 = "S135";//升降机人数识别平台API地址
            public const string S136 = "S136";//扬尘设备是否有主从关系
            public const string S137 = "S137";//升降机超员平台API地址
            public const string S138 = "S138";//存放告警闭环图片的文件夹
            public const string S139 = "S139";//存放考试题目图片
            public const string S140 = "S140";//本服务器login画面的样式css
            public const string S141 = "S141";//群耀考勤接口API
            public const string S142 = "S142";//群耀考勤接口API的pass
            public const string S143 = "S143";//群耀考勤接口API的城市代码
            public const string S144 = "S144";//平台地图坐标系(0：百度地图；1：天地图)
            public const string S145 = "S145";//傲途安全帽默认数据json
            public const string S146 = "S146";//傲途安全帽配置API地址
            public const string S147 = "S147";//晨会交底API地址
            public const string S148 = "S148";//是否即时开启雾泡
            public const string S149 = "S149";//签到照片存放相对路径
            public const string S150 = "S150";//签到是否上传照片
            public const string S151 = "S151";//考试试卷题数
            public const string S152 = "S152";//深圳安全帽定位长连接url
            public const string S153 = "S153";//深圳安全帽定位APIurl
            public const string S154 = "S154";//移动执法图片的文件夹
            public const string S155 = "S155";//移动执法整改单类型
            public const string S156 = "S156";//签到方式(1:打卡签到;2:扫码签到)
            public const string S157 = "S157";//海康8200OpenApihost
            public const string S158 = "S158";//海康8200OpenApiappKey
            public const string S159 = "S159";//海康8200OpenApiappSecret
            public const string S160 = "S160";//海康OpenApihost2
            public const string S161 = "S161";//海康OpenApiappKey2
            public const string S162 = "S162";//海康8200OpenApiappSecret2
            public const string S163 = "S163";//扫码巡查有效距离(米)
            public const string S164 = "S164";//安全隐患类型统计级别
            public const string S165 = "S165";//雄迈视频地址端口
            public const string S166 = "S166";//雄迈视频用户名
            public const string S167 = "S167";//雄迈视频密码（MD5）
            public const string S169 = "S169";//临时文件夹路径，用于存放验证码，前端显示的平面图等
            public const string S171 = "S171";//裸土覆盖百分率报警线
            public const string S172 = "S172";//新合盛实名制接口API
            public const string S174 = "S174";//示范片区动态考核url
            public const string S175 = "S175";//市平台对应的城市【不是市平台的话设置为空白！！！】
            public const string S176 = "S176";//市平台对应的实名制的用户【不是市平台的话设置为空白！！！】
            public const string S177 = "S177";//市平台对应的实名制的密码【不是市平台的话设置为空白！！！】
            public const string S178 = "S178";//傲途省，市平台对应的用户【不是市，区平台的话设置为空白！！！】
            public const string S179 = "S179";//傲途省，市平台对应的密码【不是市，区平台的话设置为空白！！！】
            public const string S180 = "S180";//市平台对应的组织机构编码【不是市平台的话设置为空白！！！】
            public const string S181 = "S181";//和项目申请平台对接哪些所属机构的数据（逗号分隔）
            public const string S182 = "S182";//市平台摄像头类型【不是市平台的话设置为空白！！！】
            public const string S183 = "S183";//傲途看板跳转域名
            public const string S184 = "S184";//新合盛申请平台设备在线统计url
            public const string S185 = "S185";//新合盛申请平台PlatformID
            public const string S186 = "S186";//新合盛实名制看板域名
            public const string S187 = "S187";//市区平台url
            public const string S188 = "S188";//市区平台单点登录用户名
            public const string S189 = "S189";//市区平台单点登录有效期（分钟）
            public const string S190 = "S190";//无锡风险数据推动地址
            public const string S191 = "S191";//无锡风险数据推送项目安监号（逗号分隔）
            public const string S192 = "S192";//市平台登录不进行短信验证
            public const string S199 = "S199";//华为千里眼实验室url
        }


        public class FileEx
        {
            public const string ALL = "*";
            public const string TMP = ".tmp";
            public const string TMP_ALL = "*.tmp";
            public const string DEL = ".del";
            public const string DEL_ALL = "*.del";
            public const string LOG = ".log";
            public const string LOG_ALL = "*.log";
            public const string XLS = ".xls";
            public const string XLS_ALL = "*.xls";
            public const string XLSX = ".xlsx";
            public const string XLSX_ALL = "*.xlsx";
            public const string CSV = ".csv";
            public const string CSV_ALL = "*.csv";
            public const string BAK = ".bak";
            public const string BAK_ALL = "*.bak";
            public const string JPG = ".jpg";
            public const string JPG_ALL = "*.jpg";
            public const string MP4 = ".mp4";
            public const string MP4_ALL = "*.mp4";
        }

        public class Symbol
        {
            public const string SLASH = "/";
            public const string COLON = ":";
            public const string COMMA = ",";
            public const string SEMICOLON = ";";
            public const string HYPHEN = "-";
            public const string EQUAL = "=";
            public const string BRACES_START = "{";
            public const string BRACES_END = "}";
            public const string DOUBLE_QUOTATION = "\"";
            public const string QUESTION = "?";
            public const string SPACE = " ";
        }

        public class Encryp
        {
            /// <summary>
            /// 公钥
            /// MIGfMA0GCSqGSIb3DQEBAQUAA4GNADCBiQKBgQDYUpck65xvmwTMx40qKgswFwHlSpJvSwaQBJtt9mDmO6p4YUtBrKcyMGvHwt8xwkaYfxTrFt0B6D3jeKKgnKGe94kCMhEKjnAA3KDXrkdi5mQ+meQa+FeG/o1uN8iuq2ZiviU9snff85UIH0M92HoDL9oHDb/7NhaZveELd+wAvQIDAQAB
            /// </summary>
            public const string PUBLIC_KEY = "<RSAKeyValue><Modulus>2FKXJOucb5sEzMeNKioLMBcB5UqSb0sGkASbbfZg5juqeGFLQaynMjBrx8LfMcJGmH8U6xbdAeg943iioJyhnveJAjIRCo5wANyg165HYuZkPpnkGvhXhv6NbjfIrqtmYr4lPbJ33/OVCB9DPdh6Ay/aBw2/+zYWmb3hC3fsAL0=</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";
            /// <summary>
            /// 私钥
            /// MIICdwIBADANBgkqhkiG9w0BAQEFAASCAmEwggJdAgEAAoGBANhSlyTrnG+bBMzHjSoqCzAXAeVKkm9LBpAEm232YOY7qnhhS0GspzIwa8fC3zHCRph/FOsW3QHoPeN4oqCcoZ73iQIyEQqOcADcoNeuR2LmZD6Z5Br4V4b+jW43yK6rZmK+JT2yd9/zlQgfQz3YegMv2gcNv/s2Fpm94Qt37AC9AgMBAAECgYEAvbOv2DDGfxjynKJiqRc1uHZ0sx8yS2b/2kwwAb6OO1kXpXmtBWrjNKBB5GyATqKQRisrrP3f0kxX+aNQ+ohhMem6YlWdpUpMEndYzzgxv4mziXdPPA6H9S4175eM9lvI5+cWknYKF0zapGH4EJJlBGP7K0O6DTE1jWSeGLVvkAECQQD2F5BK4g86sgoEETG9ZHFqEykTJ5Ptqsgp6u0oJzZ980JWckEqnbQibFLjdcLouGazS0GNcDdmBe4NWCe53WVpAkEA4Qgyi2N1cph9rC5oSdOV/vCkcdDTBmF349ZBbbjIrFALqmoJBJbVVhFltkJsEN8oa3R59eHvPKrncHvJtpuyNQJBAL+P0iNoWyB+jKtj0wsxj9NZfOTLLyyXf16Z3+gcth9O57mxEKciwaD2H4OuHH3ZZSB2GV4HFyiUvxymHa5h99kCQD5OqMATf9eFBXcBOnsGjMeUFdQ1v9hKcImzL1aUDWw4laJPzcNpiBRWqNT4OzfIskZeb853Cmi/4WkxvT5EiekCQFnCdAsovJDqXEaRUDSfluHCbiROxfjXJFpFAv6cEjc5WBkpg3+4Ek9eBRUTbqJcmpDTkZMWWeiM1IBVPL+xjNQ=
            /// </summary>
            public const string PRIVATE_KEY = "<RSAKeyValue><Modulus>2FKXJOucb5sEzMeNKioLMBcB5UqSb0sGkASbbfZg5juqeGFLQaynMjBrx8LfMcJGmH8U6xbdAeg943iioJyhnveJAjIRCo5wANyg165HYuZkPpnkGvhXhv6NbjfIrqtmYr4lPbJ33/OVCB9DPdh6Ay/aBw2/+zYWmb3hC3fsAL0=</Modulus><Exponent>AQAB</Exponent><P>9heQSuIPOrIKBBExvWRxahMpEyeT7arIKertKCc2ffNCVnJBKp20ImxS43XC6Lhms0tBjXA3ZgXuDVgnud1laQ==</P><Q>4Qgyi2N1cph9rC5oSdOV/vCkcdDTBmF349ZBbbjIrFALqmoJBJbVVhFltkJsEN8oa3R59eHvPKrncHvJtpuyNQ==</Q><DP>v4/SI2hbIH6Mq2PTCzGP01l85MsvLJd/Xpnf6By2H07nubEQpyLBoPYfg64cfdllIHYZXgcXKJS/HKYdrmH32Q==</DP><DQ>Pk6owBN/14UFdwE6ewaMx5QV1DW/2EpwibMvVpQNbDiVok/Nw2mIFFao1Pg7N8iyRl5vzncKaL/haTG9PkSJ6Q==</DQ><InverseQ>WcJ0Cyi8kOpcRpFQNJ+W4cJuJE7F+NckWkUC/pwSNzlYGSmDf7gST14FFRNuolyakNORkxZZ6IzUgFU8v7GM1A==</InverseQ><D>vbOv2DDGfxjynKJiqRc1uHZ0sx8yS2b/2kwwAb6OO1kXpXmtBWrjNKBB5GyATqKQRisrrP3f0kxX+aNQ+ohhMem6YlWdpUpMEndYzzgxv4mziXdPPA6H9S4175eM9lvI5+cWknYKF0zapGH4EJJlBGP7K0O6DTE1jWSeGLVvkAE=</D></RSAKeyValue>";

            /// <summary>
            /// 另一组公钥
            /// MIGfMA0GCSqGSIb3DQEBAQUAA4GNADCBiQKBgQCXvUXYpZJRvD69/fmYg8wZ3idRxgDSh61zyZ4/AkDQwqrmbT4q2CkFfHpARPffvZSnIj3vO6PJ1HxSvz3zjMy5odw7dif0z/iyU93Rfk/nGNyYTubQOHRA7cyHpwsO/34hUcMAyvySkFlV2MfY6JJ6NNZAjUTHlbXNNzq53zfzfwIDAQAB
            /// </summary>
            public const string PUBLIC_KEY_OTHER = "<RSAKeyValue><Modulus>l71F2KWSUbw+vf35mIPMGd4nUcYA0oetc8mePwJA0MKq5m0+KtgpBXx6QET3372UpyI97zujydR8Ur8984zMuaHcO3Yn9M/4slPd0X5P5xjcmE7m0Dh0QO3Mh6cLDv9+IVHDAMr8kpBZVdjH2OiSejTWQI1Ex5W1zTc6ud83838=</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";
            /// <summary>
            /// 另一组私钥
            /// MIICdwIBADANBgkqhkiG9w0BAQEFAASCAmEwggJdAgEAAoGBAJe9RdilklG8Pr39+ZiDzBneJ1HGANKHrXPJnj8CQNDCquZtPirYKQV8ekBE99+9lKciPe87o8nUfFK/PfOMzLmh3Dt2J/TP+LJT3dF+T+cY3JhO5tA4dEDtzIenCw7/fiFRwwDK/JKQWVXYx9jokno01kCNRMeVtc03OrnfN/N/AgMBAAECgYBD49E+T0YZ/8lqpBlqXX2SDU02TwrLJx058vk0Y8OwI4jnM/Veofwokjr4SmNE3cOGy9E+Gkb75HADbcFAGLz3mM+rFsNSkPKJ4mKDj+xlXJgcLfUBBbK9sfE9zq7WTEzev0BMT50ZLxgpnpyIt1+xuULI9AlDBYEfX1hDrfbgsQJBAMYY8GAmyXRW7H9lvdyIZwlIuNGUNDIIzMDBqxibjLZA1bid63XCT3F9OMGarN8p9APBdVQ+0LuYMtUnOPyGbFsCQQDEF34WLPNf6vIfBnlXFIuy6kg9PzOQxIL+n0RqtENPjNt184O5VpzD3+5x1YonZzjzvDpxqnU2cx7tfYT8p06tAkA6sDjGw7b7WKVIOQQ+ycp83aajsJymFiVTFg1yhOLzO9IVl/OiN6cBoG+oLAL7OpqoYjA4fUOp0DV/INepOMnxAkEAvjWXjcUukqUhFW0/OHPqirNtPVPYHh4wfvJY+DRBcqHMo56B8L8OM4Y9ElizuUMMYIj+HBHfvuBuaKov3LAhxQJBAKOKiUNH0eWF7+Rj9HdC1km/0edj9UP6aYp4+rV+YPC8Qe5oJ7VCdralleX3+zfOua3/d0p6j9ll7MjWPFQLq+M=
            /// </summary>
            public const string PRIVATE_KEY_OTHER = "<RSAKeyValue><Modulus>l71F2KWSUbw+vf35mIPMGd4nUcYA0oetc8mePwJA0MKq5m0+KtgpBXx6QET3372UpyI97zujydR8Ur8984zMuaHcO3Yn9M/4slPd0X5P5xjcmE7m0Dh0QO3Mh6cLDv9+IVHDAMr8kpBZVdjH2OiSejTWQI1Ex5W1zTc6ud83838=</Modulus><Exponent>AQAB</Exponent><P>xhjwYCbJdFbsf2W93IhnCUi40ZQ0MgjMwMGrGJuMtkDVuJ3rdcJPcX04wZqs3yn0A8F1VD7Qu5gy1Sc4/IZsWw==</P><Q>xBd+FizzX+ryHwZ5VxSLsupIPT8zkMSC/p9EarRDT4zbdfODuVacw9/ucdWKJ2c487w6cap1NnMe7X2E/KdOrQ==</Q><DP>OrA4xsO2+1ilSDkEPsnKfN2mo7CcphYlUxYNcoTi8zvSFZfzojenAaBvqCwC+zqaqGIwOH1DqdA1fyDXqTjJ8Q==</DP><DQ>vjWXjcUukqUhFW0/OHPqirNtPVPYHh4wfvJY+DRBcqHMo56B8L8OM4Y9ElizuUMMYIj+HBHfvuBuaKov3LAhxQ==</DQ><InverseQ>o4qJQ0fR5YXv5GP0d0LWSb/R52P1Q/ppinj6tX5g8LxB7mgntUJ2tqWV5ff7N865rf93SnqP2WXsyNY8VAur4w==</InverseQ><D>Q+PRPk9GGf/JaqQZal19kg1NNk8KyycdOfL5NGPDsCOI5zP1XqH8KJI6+EpjRN3DhsvRPhpG++RwA23BQBi895jPqxbDUpDyieJig4/sZVyYHC31AQWyvbHxPc6u1kxM3r9ATE+dGS8YKZ6ciLdfsblCyPQJQwWBH19YQ6324LE=</D></RSAKeyValue>";
        }
    }
}
