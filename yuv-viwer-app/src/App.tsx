
import { useEffect, useRef, useState } from 'react';
import './App.css'

function App() {
  const canvasRef = useRef<HTMLCanvasElement>(null);
  const [yuvRaw, setYuvRaw] = useState<ArrayBuffer>(new ArrayBuffer(0));
  const [width, setWidth] = useState<number>(720);
  const [height, setHeight] = useState<number>(404);
  const yuv444 = (width: number, height: number, yuvRaw: ArrayBuffer) => {
    const y = yuvRaw.slice(0, width * height);
    console.log(y.byteLength);
  }

  useEffect(() => {
    yuv444(width, height, yuvRaw);
  }, [width, height, yuvRaw]);

  useEffect(() => {

    const aa = async () => {
      const adapter = await navigator.gpu.requestAdapter();
      if (!adapter) {
        throw new Error("No appropriate GPUAdapter found.");
      }
      const device = await adapter.requestDevice();

      const canvas = canvasRef.current;
      if (!canvas) return;

      const context = canvas.getContext('webgpu');

      if (!context) return;

      const format = navigator.gpu.getPreferredCanvasFormat();
      console.log("Preferred Canvas Format:", format);
      context.configure({
        device: device,
        format: format,
        alphaMode: 'premultiplied',
      });

      const commandEncoder = device.createCommandEncoder();

      const textureView = context.getCurrentTexture().createView();

      const renderPassDescriptor: GPURenderPassDescriptor = {
        colorAttachments: [
          {
            view: textureView,
            loadOp: 'clear',
            clearValue: { r: 0.73, g: 0.81, b: 0.94, a: 1.0 }, // GPUColor
            storeOp: 'store',
          },
        ],
      };


      const passEncoder: GPURenderPassEncoder = commandEncoder.beginRenderPass(renderPassDescriptor);

      // ... 這裡可以添加繪製指令 (draw calls) ...

      passEncoder.end();


      device.queue.submit([commandEncoder.finish()]);

    }
    aa();
  }, []);

  const openFile = (e: React.ChangeEvent<HTMLInputElement>) => {
    const file = e.target.files?.[0];
    if (!file) return;
    const reader = new FileReader();
    reader.onload = () => {
      const arrayBuffer = reader.result;
      setYuvRaw(arrayBuffer as ArrayBuffer);
      console.log("File loaded:", file.name, arrayBuffer);
      // 在這裡處理讀取到的檔案內容
    };
    reader.readAsArrayBuffer(file);
  }

  return (
    <>
      <div className='w-52 h-52 bg-red-200' >
        <canvas ref={canvasRef} width={200} height={200} />
        <input type="number" value={width} onChange={(e) => setWidth(Number(e.target.value))} />
        <input type="number" value={height} onChange={(e) => setHeight(Number(e.target.value))} />
        <input
          type="file"
          accept=".yuv,.bin,.raw,image/*"
          onChange={args => openFile(args)}
        />
      </div>

    </>
  )
}

export default App
