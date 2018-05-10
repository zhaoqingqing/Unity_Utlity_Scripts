##说明
这个库包含两部分：

- Unity编辑器的扩展脚本(在每个插件的目录中都有对应的功能预览图片(preview结尾))
- 常用的一些工具类提取(具体解释请参考源码)

##AlignEditor

工作过程中写的一个对齐小插件，方便在开发中对Gameobject进行排列
![](https://github.com/zhaoqingqing/Unity_Editor_Scripts/blob/master/Assets/Editor/AlignEditor/AlignEditor_preview.jpg)
##UnityLock
AssetStore上的一个免费小插件，可以锁定Gameobject不可编辑
![](https://github.com/zhaoqingqing/Unity_Editor_Scripts/blob/master/Assets/Editor/UnityLock/UnityLock-preview.jpg)

##EditorStyleViewer
查看编辑器默认的样式
![](https://github.com/zhaoqingqing/Unity_Editor_Scripts/blob/master/Assets/Editor/Tools/TransformContextMenu-preview.jpg)


## Resource-Checker 

出处：[https://github.com/handcircus/Unity-Resource-Checker](https://github.com/handcircus/Unity-Resource-Checker)

需要Unity4.6及更高的版本

<hr>
##Dotween
Dotween:fork from  [https://github.com/Demigiant/dotween](https://github.com/Demigiant/dotween)

##SimpleJson
##运行环境
建议导入NGUI，DoTween；Unity5.x

##常用的工具类说明
### ColorHelper 
颜色辅助类，包括NGUI的十六进制颜色

### DatetimeHelper 
时间日期辅助类，包括常用的时间转换

### Extensions 
扩展方法，包括集合类，Transform的扩展

### Logger 
更加智能及好用的输出类，hook unity的方法：Application.RegisterLogCallbackThreaded(OnLogCallback);

### MathHelper 
数学辅助类，包括做战斗时需要的计算公式

### NumberHelper 
数字辅助类，包括人性化的显示数字

### StringHelper 
字符串辅助类，扩充字符串的功能，比如分隔，和数组整合

### UnityHelper
提供好多便捷的API，比如设置粒子缩放，更为强大transform的处理，对unity组件的处理；

编写好的通用动画(doTween)