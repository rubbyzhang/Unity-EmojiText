这个版本最初的源码：https://github.com/coding2233/TextInlineSprite

### 爬坑记录
[最初的源码](https://code.csdn.net/qq992817263/uguitextpro/tree/master)看似可用，但是在手机端ListView滚动情况下直接掉到20帧一下，即使在静态100个表情同时更新的境况下效率也很难令人满意。所以.................差不多用了一周时间爬各种坑，下面是一些主要的记录：

#### 3.1 优化内容
#####(1) GC
![mark](http://ohzzlljrf.bkt.clouddn.com/blog/20170320/234344431.png)
代码中在解析字符中基本每次都在new数据，包括解析字符、计算图片位置、更新图片Mesh等都存在很严重的GC，看上图就可以看到滚动中如果频繁创建的问题。

  **优化思路：**
* 对于每个Text，限制最大图片数量以及相关结构数量，只有在不够的时候再进行分配（不超过最大数量则），后续使用中不再进行分配，当然增加了数据有效性判断而不是是否为空。
* 对于图片管理Mesh，则管理器中图片总数量提前创建，只有再发生变化时才会重新进行内存分配。现在使用的策略还需优化。

##### (2) 图片信息查找
启动时读取配置信息，并简历sprite名字和信息的对应Dictionary，加快查找。当然也可以直接以Dictionary结构进行序列化，就可以节省这部分空间和时间，待优化。

#####(3)  有效图片更新方式
原始版本中有效Sprite 列表时通过List的形式进行管理，每次任一个Text的变化（enabled，posotion等）都会将这个列表清除并重新将有效Text中的有效Sprite添加到列表中来。这种方式如果在类似ListView等一直会变化的组件中就会产生不必要的CPU开销。

**优化思路：**
* 维护一个有效Text的Dictionary，保存Text中对应Sprite的Key值，在Text OnEnable/OnDisable中进行注册和注销操作
* 维护一个有效Sprite的Dictionary，保存Sprite string以及实际信息。
* 每次有Text改变时只修改Sprite 键值表中对应的部分，当然也考虑Text注销等情况。

这种方式避免在频繁更新中不必要的列表清除操作以及对SpriteManager lateUpdate的影响

#####(4) 图片Mesh数据更新过程时间
最初的版本采用对SpriteList遍历的形式逐个将triangles、uv、vertices 赋值到新创建的缓存中，再扔给iMesh去提交。在ListView快速移动时这部分的时间占用就很夸张了。
![mark](http://ohzzlljrf.bkt.clouddn.com/blog/20170320/234458649.png)

**优化思路：**
* 尽量减少无效sprite进入列表，限制每个Text中sprite的最大数量
* 采用Array.Copy的形式替代逐个赋值

#####(5) 占位符乱码清除方式
原始版本可能时作者计算错误了，清除乱码的UV位置其实只需要向后4个即可，但是也原始版本是按4 * Length（标签长度）来计算，这项的CPU占用率特别高。
![mark](http://ohzzlljrf.bkt.clouddn.com/blog/20170404/223954113.png)
##### (6) 动态表情更新方式
原始版本时在SpriteUpdate中每隔固定时间更新表情的索引（如果有）并重新更新Sprite Mesh内容。会产生一个问题：每种类型表情动画图片的数目不一样，那就很难保证每个动态表情都很自然的播放。提高更新的间隔意味着有些表情像发飙一样

**优化思路：**
每类型的表情中单独存放其时间间隔以及已经运行的时间，在Update中根据各自的情况进行更新。

#####(7)图片位置更新方式
原始代码中是在Text ：SetVerticesDirty()中进行ParseText的操作并依赖SpriteManager中LaterUpdate更新图片的Mesh数据，产生的问题：
* SetVerticesDirty 是Text 任何变化都会调用接口，意味着ParseText的操作在ListView滚动过程中一直在进行。
![mark](http://ohzzlljrf.bkt.clouddn.com/blog/20170320/234100606.png)
* SpriteManager中LaterUpdate更新与Text位置变化不同步，滚动时很明显的可以看到sprite的位置偏移

**优化思路：**
* ParseText只在text文本内容变化时进行更新，可通过重载Text的text属性实现
* 在ListView滚动过程中 sprite变化的只有位置信息，所以只更新位置即可，并且直接更新MESH,不等待SpriteManager。

#####(8)其他
对应的还有编辑器、数据结构、贴图资源管理等的优化

#### 3.2 新增功能
#####（1）支持简化标签
支持 "[xxxxx]"来替代<quad  xxxxx>冗长的设置
#####（2）图片层级管理
方便单个Canvas下多个层级，让Text 可以直接设置SpriteManager或者找最近的一个。
#####（3）增加文本与图片间隔设置
![mark](http://ohzzlljrf.bkt.clouddn.com/blog/20170404/230001583.png)

#### 3.3 待优化内容
(1)下划线解析和超链接解析都是基于字符位置对应实际字符顶点位置
(2)字符串解析
(3)图片Mesh
(4)多张sprite Asset


####3.4  优化效果
测试方式，屏幕中160个动画表情的情况，在ListView中快速滚动下进行测试的性能曲线（主要时CPU）;
**优化前**
![mark](http://ohzzlljrf.bkt.clouddn.com/blog/20170322/193412737.png)
**优化后**
![mark](http://ohzzlljrf.bkt.clouddn.com/blog/20170404/231315122.png)
**原生Text**, 有占位符，无表情
![mark](http://ohzzlljrf.bkt.clouddn.com/blog/20170404/230758701.png)
