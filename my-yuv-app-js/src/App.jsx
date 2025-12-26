import { useEffect, useRef, useState } from 'react'
import './App.css'

function App() {
  const [fileName, setFileName] = useState('未選擇檔案');
  const canvasRef = useRef();
  useEffect(() => {
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
  }, [])

  const handleFileChange = async (event) => {
    const file = event.target.files?.[0];
    if (!file) return;

    setFileName(file.name);

    // 建立 FileReader
    const reader = new FileReader();

    // 設定讀取完成後的回呼函數
    reader.onload = async (e) => {
      const arrayBuffer = e.target?.result;

      if (arrayBuffer instanceof ArrayBuffer) {
        
 
        const data = new Uint8Array(arrayBuffer);
        console.log(data.slice(0, 10));

        // TODO: 呼叫你的 WebGPU 函式，把 data 傳進去
        // await uploadToGPU(data); 
      }
    };

    reader.readAsArrayBuffer(file);
    
    // 如果你是上傳圖片想直接顯示，可以使用:
    // reader.readAsDataURL(file);
  };
  return (
    <>
      <div className='h-screen grid grid-rows-[auto_1fr_auto]'>
        <header className=''>
          <h1 >Test WebGPU</h1>
        </header>

        <div className="w-full h-full overflow-auto bg-gray-50 relative">

          <canvas ref={canvasRef} width={100} height={200}
            style={{
              border: '1px solid black',
              display: 'block'
            }}/>

        </div>
        <footer className="bg-slate-700 p-4 border-t border-slate-600 flex items-center gap-4 text-white">
        

        <label className="cursor-pointer bg-blue-600 hover:bg-blue-500 text-white font-bold py-2 px-4 rounded transition duration-200 flex items-center gap-2">
          

          <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" strokeWidth={1.5} stroke="currentColor" className="w-5 h-5">
            <path strokeLinecap="round" strokeLinejoin="round" d="M3 16.5v2.25A2.25 2.25 0 005.25 21h13.5A2.25 2.25 0 0021 18.75V16.5m-13.5-9L12 3m0 0l4.5 4.5M12 3v13.5" />
          </svg>
          
          上傳 YUV/Image
          

          <input 
            type="file" 
            hidden 
            accept=".yuv,.bin,.raw,image/*" 
            onChange={handleFileChange} 
          />
        </label>


        <span className="text-sm text-gray-300">
          {fileName}
        </span>

      </footer>
      </div>
    </>
  )
}

export default App
