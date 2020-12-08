# ViewportedLayouts

提供一些组件，解决在有限大小的区域内，显示和布局大量的UI对象。

# Install

## Dependencies

- [UniPool](https://github.com/wlgys8/UniPool)
- [Log4Unity](https://github.com/wlgys8/Log4Unity)


install by Packages/manifest.json (you must add dependencies to project by yourself in this way)
```json
"com.ms.ugui.viewportedlayouts":"https://github.com/wlgys8/ViewportedLayouts.git"
```

or install by openupm (dependenceis will be resolved automaticly)

```
openupm install com.ms.ugui.viewportedlayouts
```

# 简单说明


以简单的一个例子来说明:

```
制作一个垂直线性布局、可滑动的列表。支持一万个以上的元素项。
```

如果按照Unity传统的方式，ScrollView + VerticalLayoutGroup来制作，性能将是十分低下的。特别是创建一万个GameObject对象的代价是无法接受的。

本项目提供的组件，利用对象池的方式，对布局元素实现循环使用，实现高性能显示与布局。 基本规则如下 ：

- 当元素项从非可见区域进入到可见区域时，才创建或从对象池中获取，并刷新显示状态。
- 当元素项从可见区域进入到非可见区域时，回收进对象池。

因此，至多只需要创建`同时显示在可见区域的元素项数量`

# 支持的布局类型

## 1. SimpleLinearLayout

线性布局

## 2. SimpleFlexGridLayout

可伸缩式的网格布局
