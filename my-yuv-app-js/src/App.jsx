import { useEffect,useRef } from 'react'
import './App.css'

function App() {
  const canvasRef = useRef();
  useEffect(()=>{
    const initWebGPU = async () => {
      // 1. 检查浏览器是否支持 WebGPU
      if (!navigator.gpu) {
        console.error("您的浏览器不支持 WebGPU");
        return;
      }

      // 2. 获取适配器 (Adapter)
      const adapter = await navigator.gpu.requestAdapter();
      if (!adapter) {
        console.error("无法获取 WebGPU 适配器");
        return;
      }

      // 3. 获取设备 (Device)
      const device = await adapter.requestDevice();

      // 4. 获取 Canvas 上下文并配置
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

      const renderPassDescriptor = {
        colorAttachments: [
          {
            view: textureView,

            loadOp: 'clear', 
            clearValue: { r: 0.73, g: 0.81, b: 0.94, a: 1.0 }, 
            
            storeOp: 'store',
          },
        ],
      };

      const passEncoder = commandEncoder.beginRenderPass(renderPassDescriptor);
      
      passEncoder.end();

      device.queue.submit([commandEncoder.finish()]);
    };

    initWebGPU();
  },[])
  return (
    <>
      <div>
        <canvas 
      ref={canvasRef} 
      width={800} 
      height={600} 
      style={{ border: '1px solid black' }}
    />
      </div>
    </>
  )
}

export default App
