# PixelViwer
## 參考資料
https://paaatrick.com/2020-01-26-yuv-pixel-formats/
## ffmpeg support pixel format
```shell
ffmpeg -pix_fmts
```

1. jpg to yuy2
```shell
ffmpeg -i 720-404-yuy2.jpg -pix_fmt yuyv422 .\a.yuv
```
2. jpg to yuv444p
```shell
ffmpeg -i 720-404-yuy2.jpg -pix_fmt yuv444p 720-404-yuv444p.yuv
```

3. jpg to yuv420p
```shell
ffmpeg -i 720-404-yuy2.jpg -pix_fmt yuv420p .\720-404-yuv420p.yuv
```

3. jpg to nv12(yuv420sp)
```shell
ffmpeg -i 720-404-yuy2.jpg -pix_fmt nv12 720-404-nv12.yuv
```

5. jpg to rgb24
```shell
ffmpeg -i 720-404-yuy2.jpg -pix_fmt rgb24 .\720-404-rgb24.rgb
```
