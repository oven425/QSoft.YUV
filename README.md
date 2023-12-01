# QSoft.YUV
* yuv444p to rgb24
```c#
byte[] yuv444p_raw = File.ReadAllBytes("s1-yuv444p.yuv");
var yuv444p = new QSoft.YUV.YUV444P(yuv444p_raw, 6000,3376);
```

## refrence data
* https://paaatrick.com/2020-01-26-yuv-pixel-formats/
## ffmpeg support pixel format
```shell
ffmpeg -pix_fmts
```

* jpg to yuy2
```shell
ffmpeg -i 720-404-yuy2.jpg -pix_fmt yuyv422 .\a.yuv
```
* jpg to yuv444p
```shell
ffmpeg -i 720-404-yuy2.jpg -pix_fmt yuv444p 720-404-yuv444p.yuv
```

* jpg to yuv420p
```shell
ffmpeg -i 720-404-yuy2.jpg -pix_fmt yuv420p .\720-404-yuv420p.yuv
```

* jpg to nv12(yuv420sp)
```shell
ffmpeg -i 720-404-yuy2.jpg -pix_fmt nv12 720-404-nv12.yuv
```

* jpg to rgb24
```shell
ffmpeg -i 720-404-yuy2.jpg -pix_fmt rgb24 .\720-404-rgb24.rgb
```


https://codereview.stackexchange.com/questions/254255/sobel-operator-simd-x86-intrinsics-implementation